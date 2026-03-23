import fs from 'node:fs';
import path from 'node:path';
import os from 'node:os';
import { AgentTask, DashboardSnapshot, TimeRange, TokenUsageRecord, RunStatus, AgentStatus } from '../models/dashboard.js';

const home = os.homedir();
const openClawRoot = path.join(home, '.openclaw');
const agentsRoot = path.join(openClawRoot, 'agents');
const sessionsRoot = path.join(agentsRoot);
const openClawConfigPath = path.join(openClawRoot, 'openclaw.json');
const subagentRunsPath = path.join(openClawRoot, 'subagents', 'runs.json');
const configAuditPath = path.join(openClawRoot, 'logs', 'config-audit.jsonl');

function readJson<T>(filePath: string, fallback: T): T {
  try { return JSON.parse(fs.readFileSync(filePath, 'utf-8')) as T; } catch { return fallback; }
}

function readJsonLines(filePath: string): any[] {
  try {
    return fs.readFileSync(filePath, 'utf-8').split(/\r?\n/).filter(Boolean).map((line) => JSON.parse(line));
  } catch {
    return [];
  }
}

function safeArray<T>(value: T[] | undefined): T[] { return Array.isArray(value) ? value : []; }
function hoursAgo(hours: number): Date { return new Date(Date.now() - hours * 60 * 60 * 1000); }
function daysAgo(days: number): Date { return new Date(Date.now() - days * 24 * 60 * 60 * 1000); }

function extractAgentId(sessionKey: string): string {
  const parts = sessionKey.split(':');
  return parts.length > 1 ? parts[1] : sessionKey;
}

function normalizeStatus(stopReason?: string, outputText?: string): RunStatus {
  const text = `${stopReason ?? ''} ${outputText ?? ''}`.toLowerCase();
  if (text.includes('rate')) return 'rate-limited';
  if (text.includes('error') || text.includes('failed')) return 'error';
  if (text.includes('tooluse')) return 'running';
  if (text.includes('stop')) return 'success';
  return 'unknown';
}

function parseDurationMs(start?: string, end?: string): number | undefined {
  if (!start || !end) return undefined;
  const ms = new Date(end).getTime() - new Date(start).getTime();
  return Number.isFinite(ms) && ms > 0 ? ms : undefined;
}

export function loadOpenClawData() {
  const config = readJson<any>(openClawConfigPath, {});
  const runs = readJson<any>(subagentRunsPath, { runs: {} });
  const agentEntries = safeArray<any>(config?.agents?.list);
  const sessions: Array<{ sessionKey: string; meta: any; filePath?: string }> = [];

  for (const agent of agentEntries) {
    const sessionIndexPath = path.join(sessionsRoot, agent.id, 'sessions', 'sessions.json');
    const sessionIndex = readJson<Record<string, any>>(sessionIndexPath, {});
    for (const [sessionKey, meta] of Object.entries(sessionIndex)) {
      sessions.push({ sessionKey, meta, filePath: (meta as any).sessionFile });
    }
  }

  const records: TokenUsageRecord[] = [];
  const taskMap = new Map<string, AgentTask[]>();

  for (const entry of sessions) {
    const agentId = extractAgentId(entry.sessionKey);
    const meta = entry.meta;
    if (typeof meta?.inputTokens === 'number' || typeof meta?.outputTokens === 'number') {
      records.push({
        id: `${entry.sessionKey}:summary`,
        agentId,
        sessionKey: entry.sessionKey,
        startedAt: new Date(meta.updatedAt ?? Date.now()).toISOString(),
        provider: meta.modelProvider ?? 'unknown',
        model: meta.model ?? 'unknown',
        status: meta.abortedLastRun ? 'error' : 'success',
        inputTokens: meta.inputTokens ?? 0,
        outputTokens: meta.outputTokens ?? 0,
        cacheReadTokens: meta.cacheRead ?? 0,
        totalTokens: meta.totalTokens ?? (meta.inputTokens ?? 0) + (meta.outputTokens ?? 0),
        source: 'sessions.json'
      });
    }

    if (entry.filePath && fs.existsSync(entry.filePath)) {
      const lines = readJsonLines(entry.filePath);
      const assistantMessages = lines.filter((line) => line.type === 'message' && line.message?.role === 'assistant' && line.message?.usage);
      for (const line of assistantMessages) {
        const usage = line.message.usage ?? {};
        const endedAt = line.timestamp;
        const startedAt = lines.find((candidate) => candidate.id === line.parentId)?.timestamp ?? endedAt;
        const latencyMs = parseDurationMs(startedAt, endedAt);
        const total = (usage.input ?? 0) + (usage.output ?? 0);
        records.push({
          id: `${entry.sessionKey}:${line.id}`,
          agentId,
          sessionKey: entry.sessionKey,
          startedAt,
          endedAt,
          provider: line.message.provider ?? meta.modelProvider ?? 'unknown',
          model: line.message.model ?? meta.model ?? 'unknown',
          status: normalizeStatus(line.message.stopReason, JSON.stringify(line.message.content ?? '')),
          inputTokens: usage.input ?? 0,
          outputTokens: usage.output ?? 0,
          cacheReadTokens: usage.cacheRead ?? 0,
          totalTokens: usage.totalTokens ?? total,
          latencyMs,
          tokensPerSecond: latencyMs && total > 0 ? total / (latencyMs / 1000) : undefined,
          source: 'session-jsonl'
        });
      }

      const completedTasks: AgentTask[] = assistantMessages.slice(-8).map((line) => ({
        id: line.id,
        title: extractTaskTitle(lines, line.parentId) ?? `Conversation response ${line.id.slice(0, 8)}`,
        status: normalizeStatus(line.message?.stopReason) === 'error' ? 'failed' : 'completed',
        startedAt: lines.find((candidate) => candidate.id === line.parentId)?.timestamp,
        endedAt: line.timestamp,
        durationMs: parseDurationMs(lines.find((candidate) => candidate.id === line.parentId)?.timestamp, line.timestamp),
        model: line.message?.model ?? meta.model,
        provider: line.message?.provider ?? meta.modelProvider,
        tokensIn: line.message?.usage?.input,
        tokensOut: line.message?.usage?.output,
        source: 'session-jsonl',
        failureReason: normalizeStatus(line.message?.stopReason) === 'error' ? line.message?.stopReason : undefined
      }));
      taskMap.set(agentId, [...(taskMap.get(agentId) ?? []), ...completedTasks]);
    }
  }

  for (const run of Object.values<any>(runs.runs ?? {})) {
    const agentId = run.label?.includes('codefather') ? 'codefather' : extractAgentId(run.requesterSessionKey ?? 'main');
    const task: AgentTask = {
      id: run.runId,
      title: firstLine(run.task) ?? 'Subagent task',
      status: run.startedAt && !run.completedAt ? 'current' : 'completed',
      startedAt: run.startedAt ? new Date(run.startedAt).toISOString() : undefined,
      endedAt: run.completedAt ? new Date(run.completedAt).toISOString() : undefined,
      durationMs: parseDurationMs(run.startedAt ? new Date(run.startedAt).toISOString() : undefined, run.completedAt ? new Date(run.completedAt).toISOString() : undefined),
      model: run.model,
      source: 'subagents/runs.json'
    };
    taskMap.set(agentId, [...(taskMap.get(agentId) ?? []), task]);
  }

  return {
    config,
    records,
    taskMap,
    dataSources: [
      { name: 'OpenClaw config', path: openClawConfigPath, live: fs.existsSync(openClawConfigPath), purpose: 'Agent definitions, model config, providers, gateway metadata' },
      { name: 'Agent sessions index', path: path.join(agentsRoot, '<agent>', 'sessions', 'sessions.json'), live: true, purpose: 'Current session totals and model/provider metadata' },
      { name: 'Agent session transcripts', path: path.join(agentsRoot, '<agent>', 'sessions', '*.jsonl'), live: true, purpose: 'Per-response usage, timestamps, stop reasons, inferred latency' },
      { name: 'Subagent runs', path: subagentRunsPath, live: fs.existsSync(subagentRunsPath), purpose: 'Active/completed subagent tasks for mission board' },
      { name: 'Config audit log', path: configAuditPath, live: fs.existsSync(configAuditPath), purpose: 'Operational timeline for config changes (documented, not charted yet)' }
    ]
  };
}

function extractTaskTitle(lines: any[], parentId?: string): string | undefined {
  if (!parentId) return undefined;
  const parent = lines.find((line) => line.id === parentId);
  const text = parent?.message?.content?.find((item: any) => item.type === 'text')?.text;
  return firstLine(text)?.slice(0, 120);
}

function firstLine(value?: string): string | undefined {
  return value?.split(/\r?\n/).find(Boolean)?.trim();
}

export function buildTokenUsage(range: TimeRange) {
  const data = loadOpenClawData();
  const start = range === '24h' ? hoursAgo(24) : range === '7d' ? daysAgo(7) : daysAgo(30);
  const relevant = data.records.filter((record) => new Date(record.startedAt) >= start);
  const filters = {
    agents: [...new Set(relevant.map((item) => item.agentId))].sort(),
    providers: [...new Set(relevant.map((item) => item.provider))].sort(),
    models: [...new Set(relevant.map((item) => item.model))].sort(),
    statuses: [...new Set(relevant.map((item) => item.status))].sort() as RunStatus[]
  };

  const bucketCount = range === '24h' ? 24 : range === '7d' ? 7 : 30;
  const stepMs = range === '24h' ? 60 * 60 * 1000 : 24 * 60 * 60 * 1000;
  const buckets = Array.from({ length: bucketCount }, (_, index) => {
    const bucketStart = new Date(start.getTime() + index * stepMs);
    const bucketEnd = new Date(bucketStart.getTime() + stepMs);
    const bucketItems = relevant.filter((item) => {
      const t = new Date(item.startedAt).getTime();
      return t >= bucketStart.getTime() && t < bucketEnd.getTime();
    });
    return {
      label: range === '24h' ? bucketStart.toLocaleTimeString([], { hour: '2-digit' }) : bucketStart.toLocaleDateString([], { month: 'short', day: 'numeric' }),
      start: bucketStart.toISOString(),
      end: bucketEnd.toISOString(),
      totalsByModel: bucketItems.reduce<Record<string, number>>((acc, item) => {
        acc[item.model] = (acc[item.model] ?? 0) + item.totalTokens;
        return acc;
      }, {}),
      inputTokens: bucketItems.reduce((sum, item) => sum + item.inputTokens, 0),
      outputTokens: bucketItems.reduce((sum, item) => sum + item.outputTokens, 0),
      requests: bucketItems.length
    };
  });

  const grouped = new Map<string, TokenUsageRecord[]>();
  for (const record of relevant) {
    const key = `${record.provider}::${record.model}`;
    grouped.set(key, [...(grouped.get(key) ?? []), record]);
  }

  const breakdown = [...grouped.entries()].map(([key, items]) => {
    const [provider, model] = key.split('::');
    const avgLatency = average(items.map((item) => item.latencyMs).filter((x): x is number => typeof x === 'number'));
    const avgTps = average(items.map((item) => item.tokensPerSecond).filter((x): x is number => typeof x === 'number'));
    return {
      provider,
      model,
      requests: items.length,
      inputTokens: items.reduce((sum, item) => sum + item.inputTokens, 0),
      outputTokens: items.reduce((sum, item) => sum + item.outputTokens, 0),
      avgLatencyMs: avgLatency,
      avgTokensPerSecond: avgTps,
      errorCount: items.filter((item) => item.status === 'error' || item.status === 'rate-limited').length
    };
  }).sort((a, b) => (b.inputTokens + b.outputTokens) - (a.inputTokens + a.outputTokens));

  const perf = relevant.filter((item) => item.tokensPerSecond).map((item) => ({ timestamp: item.startedAt, model: item.model, tokensPerSecond: item.tokensPerSecond! }));
  const mostUsedModel = breakdown[0]?.model ?? 'N/A';
  const totalRequests = relevant.length;
  const totalInput = relevant.reduce((sum, item) => sum + item.inputTokens, 0);
  const totalOutput = relevant.reduce((sum, item) => sum + item.outputTokens, 0);
  const totalTokens = totalInput + totalOutput;
  const avgTokens = totalRequests ? totalTokens / totalRequests : 0;
  const avgSpeed = average(perf.map((item) => item.tokensPerSecond));

  const fiveHours = data.records.filter((item) => new Date(item.startedAt) >= hoursAgo(5));
  const sevenDays = data.records.filter((item) => new Date(item.startedAt) >= daysAgo(7));

  return {
    generatedAt: new Date().toISOString(),
    refreshMs: 30000,
    liveSources: ['sessions.json', 'session-jsonl', 'subagents/runs.json', 'openclaw.json'],
    filterOptions: filters,
    summaryCards: [
      { label: 'Total input tokens', value: totalInput.toLocaleString() },
      { label: 'Total output tokens', value: totalOutput.toLocaleString() },
      { label: 'Total requests', value: totalRequests.toLocaleString() },
      { label: 'Avg tokens / request', value: avgTokens.toFixed(0) },
      { label: 'Avg response speed', value: avgSpeed ? `${avgSpeed.toFixed(1)} tok/s` : 'Unavailable', helpText: 'Inferred from token count and JSONL timestamps when present' },
      { label: 'Most used model', value: mostUsedModel }
    ],
    buckets,
    performanceSeries: perf,
    breakdown,
    limits: {
      providerLimitsKnown: false,
      windows: [
        { window: 'Last 5 hours', trackedTokens: fiveHours.reduce((s, i) => s + i.totalTokens, 0), trackedRequests: fiveHours.length, live: true, note: 'Tracked usage only; provider quota limit not exposed by OpenClaw on this machine.' },
        { window: 'Last 7 days', trackedTokens: sevenDays.reduce((s, i) => s + i.totalTokens, 0), trackedRequests: sevenDays.length, live: true, note: 'Tracked usage only; weekly reset time unknown.' }
      ]
    },
    records: relevant.sort((a, b) => +new Date(b.startedAt) - +new Date(a.startedAt)).slice(0, 150),
    notes: [
      'OpenAI/Anthropic/Ollama provider quota ceilings are not exposed in local OpenClaw data, so the limits panel shows tracked usage only.',
      'Latency and tokens/sec are inferred from session JSONL timestamps and may be unavailable for summary-only session rows.'
    ]
  };
}

export function buildAgents() {
  const data = loadOpenClawData();
  const configuredAgents = safeArray<any>(data.config?.agents?.list);
  const agentCards = configuredAgents.map((agent) => {
    const agentId = agent.id;
    const tasks = (data.taskMap.get(agentId) ?? []).sort((a, b) => +new Date(b.startedAt ?? 0) - +new Date(a.startedAt ?? 0));
    const outstanding = tasks.filter((task) => task.status === 'outstanding').slice(0, 8);
    const current = tasks.filter((task) => task.status === 'current').slice(0, 8);
    const completed = tasks.filter((task) => task.status === 'completed').slice(0, 12);
    const failures = tasks.filter((task) => task.status === 'failed');
    const recentUsage = data.records.filter((record) => record.agentId === agentId && new Date(record.startedAt) >= daysAgo(1));
    let status: AgentStatus = 'idle';
    if (current.length > 0) status = 'busy';
    if (failures.length > 0) status = 'error';
    if (!tasks.length && !recentUsage.length) status = 'offline';
    if (recentUsage.some((item) => item.status === 'rate-limited')) status = 'rate-limited';

    return {
      agentId,
      name: agent.name ?? agent.id,
      status,
      queueCount: outstanding.length,
      tasksInProgress: current.length,
      completedLast24h: completed.filter((task) => task.endedAt && new Date(task.endedAt) >= daysAgo(1)).length,
      lastActivityAt: recentUsage[0]?.startedAt ?? current[0]?.startedAt ?? completed[0]?.endedAt,
      primaryModel: agent.model?.primary ?? data.config?.agents?.defaults?.model?.primary,
      fallbackModels: safeArray<string>(agent.model?.fallbacks ?? data.config?.agents?.defaults?.model?.fallbacks),
      outstandingTasks: outstanding,
      currentTasks: current,
      completedTasks: completed,
      failuresLast24h: failures.filter((task) => task.endedAt && new Date(task.endedAt) >= daysAgo(1)).slice(0, 6),
      tokenUsageSummary: {
        inputTokens: recentUsage.reduce((sum, item) => sum + item.inputTokens, 0),
        outputTokens: recentUsage.reduce((sum, item) => sum + item.outputTokens, 0),
        requests: recentUsage.length
      },
      detailNotes: [
        'Outstanding queue is live only if OpenClaw exposes it through subagent runs or future queue APIs.',
        'Completed tasks mostly come from assistant message runs and subagent run metadata.'
      ]
    };
  });

  return {
    generatedAt: new Date().toISOString(),
    refreshMs: 15000,
    liveSources: ['openclaw.json', 'sessions.json', 'session-jsonl', 'subagents/runs.json'],
    filterOptions: {
      agents: agentCards.map((item) => item.agentId),
      statuses: ['idle', 'busy', 'error', 'offline', 'rate-limited'] as AgentStatus[]
    },
    agents: agentCards,
    notes: [
      'Agent statuses are inferred from recent activity, errors, and active subagent runs.',
      'A true pending task queue is not currently exposed by a dedicated OpenClaw API on this machine, so mission board columns blend live run metadata with inferred recent work.'
    ]
  };
}

export function buildSnapshot(range: TimeRange = '24h'): DashboardSnapshot {
  const data = loadOpenClawData();
  return {
    tokenUsage: buildTokenUsage(range),
    agents: buildAgents(),
    dataSources: data.dataSources
  };
}

function average(values: number[]): number | undefined {
  if (!values.length) return undefined;
  return values.reduce((sum, item) => sum + item, 0) / values.length;
}