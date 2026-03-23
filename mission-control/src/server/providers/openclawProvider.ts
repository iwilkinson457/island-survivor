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

function normalizeName(value?: string): string {
  return (value ?? '').toLowerCase().replace(/[^a-z0-9]+/g, '');
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

function parseConfiguredModelFromBanner(lines: any[]): { provider?: string; model?: string } {
  const text = lines
    .filter((line) => line.type === 'message' && line.message?.role === 'assistant')
    .flatMap((line) => safeArray<any>(line.message?.content))
    .filter((item) => item?.type === 'text' && typeof item.text === 'string')
    .map((item) => item.text)
    .join('\n');

  const match = text.match(/Model:\s*([a-z0-9-]+)\/([^\s]+)/i);
  if (!match) return {};
  return { provider: match[1].toLowerCase(), model: match[2] };
}

function inferAgentIdFromRun(run: any, knownAgentIds: string[]): string | undefined {
  const haystacks = [run?.label, run?.task, run?.requesterDisplayKey, run?.childSessionKey]
    .filter((value): value is string => typeof value === 'string');

  for (const agentId of knownAgentIds) {
    const normalizedAgentId = normalizeName(agentId);
    if (haystacks.some((value) => normalizeName(value).includes(normalizedAgentId))) return agentId;
  }

  return undefined;
}

function resolveAgentId(sessionKey: string, lines: any[], childSessionAgentMap: Map<string, string>, knownAgentIds: string[]): string {
  const directAgentId = extractAgentId(sessionKey);
  if (directAgentId !== 'main') return directAgentId;

  const mappedChildAgent = childSessionAgentMap.get(sessionKey);
  if (mappedChildAgent) return mappedChildAgent;

  const text = JSON.stringify(lines).toLowerCase();
  for (const agentId of knownAgentIds) {
    if (agentId !== 'main' && text.includes(agentId.toLowerCase())) return agentId;
  }

  return directAgentId;
}

export function loadOpenClawData() {
  const config = readJson<any>(openClawConfigPath, {});
  const runs = readJson<any>(subagentRunsPath, { runs: {} });
  const agentEntries = safeArray<any>(config?.agents?.list);
  const knownAgentIds = agentEntries.map((agent) => agent.id);
  const childSessionAgentMap = new Map<string, string>();

  for (const run of Object.values<any>(runs.runs ?? {})) {
    const inferredAgentId = inferAgentIdFromRun(run, knownAgentIds);
    if (inferredAgentId && typeof run?.childSessionKey === 'string') {
      childSessionAgentMap.set(run.childSessionKey, inferredAgentId);
    }
  }

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
  const configuredProviders = new Set<string>();

  for (const entry of sessions) {
    const meta = entry.meta;
    const lines = entry.filePath && fs.existsSync(entry.filePath) ? readJsonLines(entry.filePath) : [];
    const agentId = resolveAgentId(entry.sessionKey, lines, childSessionAgentMap, knownAgentIds);
    const parsedBanner = parseConfiguredModelFromBanner(lines);
    const provider = meta.modelProvider ?? parsedBanner.provider ?? 'unknown';
    const model = meta.model ?? parsedBanner.model ?? 'unknown';
    configuredProviders.add(provider);

    if (typeof meta?.inputTokens === 'number' || typeof meta?.outputTokens === 'number') {
      records.push({
        id: `${entry.sessionKey}:summary`,
        agentId,
        sessionKey: entry.sessionKey,
        startedAt: new Date(meta.updatedAt ?? Date.now()).toISOString(),
        provider,
        model,
        status: meta.abortedLastRun ? 'error' : 'success',
        inputTokens: meta.inputTokens ?? 0,
        outputTokens: meta.outputTokens ?? 0,
        cacheReadTokens: meta.cacheRead ?? 0,
        totalTokens: meta.totalTokens ?? (meta.inputTokens ?? 0) + (meta.outputTokens ?? 0),
        source: 'sessions.json'
      });
    }

    const assistantMessages = lines.filter((line) => line.type === 'message' && line.message?.role === 'assistant' && line.message?.usage);
    for (const line of assistantMessages) {
      const usage = line.message.usage ?? {};
      const endedAt = line.timestamp;
      const startedAt = lines.find((candidate) => candidate.id === line.parentId)?.timestamp ?? endedAt;
      const latencyMs = parseDurationMs(startedAt, endedAt);
      const total = (usage.input ?? 0) + (usage.output ?? 0);
      const resolvedProvider = line.message.provider === 'openclaw' && line.message.model === 'gateway-injected'
        ? parsedBanner.provider ?? provider
        : line.message.provider ?? provider;
      const resolvedModel = line.message.provider === 'openclaw' && line.message.model === 'gateway-injected'
        ? parsedBanner.model ?? model
        : line.message.model ?? model;
      configuredProviders.add(resolvedProvider);

      records.push({
        id: `${entry.sessionKey}:${line.id}`,
        agentId,
        sessionKey: entry.sessionKey,
        startedAt,
        endedAt,
        provider: resolvedProvider,
        model: resolvedModel,
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
      model: line.message?.model ?? model,
      provider: line.message?.provider ?? provider,
      tokensIn: line.message?.usage?.input,
      tokensOut: line.message?.usage?.output,
      source: 'session-jsonl',
      failureReason: normalizeStatus(line.message?.stopReason) === 'error' ? line.message?.stopReason : undefined
    }));
    taskMap.set(agentId, [...(taskMap.get(agentId) ?? []), ...completedTasks]);
  }

  for (const run of Object.values<any>(runs.runs ?? {})) {
    const agentId = inferAgentIdFromRun(run, knownAgentIds) ?? extractAgentId(run.requesterSessionKey ?? 'main');
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
    configuredProviders: [...configuredProviders].sort(),
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
  const trackedProviders = [...new Set(relevant.map((item) => item.provider))].sort();
  const filters = {
    agents: [...new Set(relevant.map((item) => item.agentId))].sort(),
    providers: [...new Set([...trackedProviders, ...data.configuredProviders])].sort(),
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
  const totalCacheRead = relevant.reduce((sum, item) => sum + (item.cacheReadTokens ?? 0), 0);
  const anthropicTracked = relevant.filter((item) => item.provider === 'anthropic' && item.totalTokens > 0);
  const anthropicSeenButZero = data.records.some((item) => item.provider === 'anthropic');

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
      { label: 'Cache read tokens', value: totalCacheRead.toLocaleString(), helpText: 'Tracked when present in OpenClaw usage payloads' },
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
      'Latency and tokens/sec are inferred from session JSONL timestamps and may be unavailable for summary-only session rows.',
      anthropicTracked.length
        ? `Anthropic tracked usage is included (${anthropicTracked.length} request${anthropicTracked.length === 1 ? '' : 's'} in the selected range).`
        : anthropicSeenButZero
          ? 'Anthropic is configured/detected, but there are no tracked Anthropic token events in the available OpenClaw data for the selected range.'
          : 'No Anthropic provider records were found in the available OpenClaw data for the selected range.'
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
