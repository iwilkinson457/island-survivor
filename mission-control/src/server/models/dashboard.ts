export type TimeRange = '24h' | '7d' | '30d';
export type RunStatus = 'success' | 'error' | 'rate-limited' | 'running' | 'unknown';
export type AgentStatus = 'idle' | 'busy' | 'error' | 'offline' | 'rate-limited';

export interface FilterOptions {
  agents: string[];
  providers: string[];
  models: string[];
  statuses: RunStatus[];
}

export interface TokenUsageRecord {
  id: string;
  agentId: string;
  sessionKey: string;
  startedAt: string;
  endedAt?: string;
  provider: string;
  model: string;
  status: RunStatus;
  inputTokens: number;
  outputTokens: number;
  cacheReadTokens?: number;
  totalTokens: number;
  latencyMs?: number;
  tokensPerSecond?: number;
  source: 'sessions.json' | 'session-jsonl';
}

export interface UsageBucket {
  label: string;
  start: string;
  end: string;
  totalsByModel: Record<string, number>;
  inputTokens: number;
  outputTokens: number;
  requests: number;
}

export interface UsageSummaryCard {
  label: string;
  value: string;
  helpText?: string;
}

export interface ModelBreakdownRow {
  model: string;
  provider: string;
  requests: number;
  inputTokens: number;
  outputTokens: number;
  avgTokensPerSecond?: number;
  avgLatencyMs?: number;
  errorCount: number;
}

export interface TrackedWindowUsage {
  window: string;
  trackedTokens: number;
  trackedRequests: number;
  limitTokens?: number;
  percentUsed?: number;
  resetAt?: string;
  live: boolean;
  note: string;
}

export interface UsageLimitsPanel {
  providerLimitsKnown: boolean;
  windows: TrackedWindowUsage[];
}

export interface TokenUsageResponse {
  generatedAt: string;
  refreshMs: number;
  liveSources: string[];
  filterOptions: FilterOptions;
  summaryCards: UsageSummaryCard[];
  buckets: UsageBucket[];
  performanceSeries: Array<{ timestamp: string; model: string; tokensPerSecond: number }>;
  breakdown: ModelBreakdownRow[];
  limits: UsageLimitsPanel;
  records: TokenUsageRecord[];
  notes: string[];
}

export interface AgentTask {
  id: string;
  title: string;
  status: 'outstanding' | 'current' | 'completed' | 'failed';
  startedAt?: string;
  endedAt?: string;
  durationMs?: number;
  model?: string;
  provider?: string;
  tokensIn?: number;
  tokensOut?: number;
  retryCount?: number;
  failureReason?: string;
  source: string;
}

export interface AgentCard {
  agentId: string;
  name: string;
  status: AgentStatus;
  queueCount: number;
  tasksInProgress: number;
  completedLast24h: number;
  lastActivityAt?: string;
  primaryModel?: string;
  fallbackModels: string[];
  outstandingTasks: AgentTask[];
  currentTasks: AgentTask[];
  completedTasks: AgentTask[];
  failuresLast24h: AgentTask[];
  tokenUsageSummary: {
    inputTokens: number;
    outputTokens: number;
    requests: number;
  };
  detailNotes: string[];
}

export interface AgentsResponse {
  generatedAt: string;
  refreshMs: number;
  liveSources: string[];
  filterOptions: {
    agents: string[];
    statuses: AgentStatus[];
  };
  agents: AgentCard[];
  notes: string[];
}

export interface DashboardSnapshot {
  tokenUsage: TokenUsageResponse;
  agents: AgentsResponse;
  dataSources: Array<{ name: string; path: string; live: boolean; purpose: string }>;
}