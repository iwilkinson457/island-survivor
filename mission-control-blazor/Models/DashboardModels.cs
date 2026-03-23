namespace MissionControl.Models;

public enum TimeRange { H24, D7, D30 }

public enum RunStatus { Success, Error, RateLimited, Running, Unknown }

public enum AgentStatus { Idle, Busy, Error, Offline, RateLimited }

public record DataSource(string Name, string Path, bool Live, string Purpose);

public record SummaryCard(string Label, string Value, string? HelpText = null);

public record TokenUsageRecord(
    string Id,
    string AgentId,
    string SessionKey,
    DateTime StartedAt,
    DateTime? EndedAt,
    string Provider,
    string Model,
    RunStatus Status,
    long InputTokens,
    long OutputTokens,
    long CacheReadTokens,
    long TotalTokens,
    double? LatencyMs,
    double? TokensPerSecond,
    string Source);

public record TokenBucket(
    string Label,
    DateTime Start,
    DateTime End,
    Dictionary<string, long> TotalsByModel,
    long InputTokens,
    long OutputTokens,
    int Requests);

public record ModelBreakdownRow(
    string Model,
    string Provider,
    int Requests,
    long InputTokens,
    long OutputTokens,
    double? AvgTokensPerSecond,
    double? AvgLatencyMs,
    int ErrorCount);

public record TrackedWindow(
    string Window,
    long TrackedTokens,
    int TrackedRequests,
    bool Live,
    string Note);

public record FilterOptions(
    List<string> Agents,
    List<string> Providers,
    List<string> Models,
    List<string> Statuses);

public record TokenUsageData(
    DateTime GeneratedAt,
    int RefreshMs,
    FilterOptions FilterOptions,
    List<SummaryCard> SummaryCards,
    List<TokenBucket> Buckets,
    List<ModelBreakdownRow> Breakdown,
    List<TrackedWindow> Windows,
    List<TokenUsageRecord> Records,
    List<string> Notes,
    List<DataSource> DataSources);

public record AgentTask(
    string Id,
    string Title,
    string Status,
    DateTime? StartedAt,
    DateTime? EndedAt,
    double? DurationMs,
    string? Model,
    string? Provider,
    long? TokensIn,
    long? TokensOut,
    string? FailureReason,
    string Source);

public record TokenSummary(long InputTokens, long OutputTokens, int Requests);

public record AgentCard(
    string AgentId,
    string Name,
    AgentStatus Status,
    int QueueCount,
    int TasksInProgress,
    int CompletedLast24h,
    DateTime? LastActivityAt,
    string? PrimaryModel,
    List<string> FallbackModels,
    List<AgentTask> OutstandingTasks,
    List<AgentTask> CurrentTasks,
    List<AgentTask> CompletedTasks,
    List<AgentTask> FailuresLast24h,
    TokenSummary TokenUsageSummary,
    List<string> DetailNotes);

public record AgentFilterOptions(List<string> Agents, List<string> Statuses);

public record AgentsData(
    DateTime GeneratedAt,
    int RefreshMs,
    AgentFilterOptions FilterOptions,
    List<AgentCard> Agents,
    List<string> Notes,
    List<DataSource> DataSources);
