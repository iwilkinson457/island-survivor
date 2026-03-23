using System.Text.Json;
using System.Text.Json.Nodes;
using MissionControl.Models;

namespace MissionControl.Services;

public class OpenClawDataService
{
    private static readonly string HomeDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
    private static readonly string Root = Path.Combine(HomeDir, ".openclaw");
    private static readonly string AgentsRoot = Path.Combine(Root, "agents");
    private static readonly string ConfigPath = Path.Combine(Root, "openclaw.json");
    private static readonly string SubagentRunsPath = Path.Combine(Root, "subagents", "runs.json");

    private static readonly JsonSerializerOptions JsonOpts = new() { PropertyNameCaseInsensitive = true };

    // ─── helpers ────────────────────────────────────────────────────────────────

    private static JsonNode? ReadJson(string path)
    {
        try { return JsonNode.Parse(File.ReadAllText(path)); }
        catch { return null; }
    }

    private static List<JsonNode> ReadJsonLines(string path)
    {
        try
        {
            return File.ReadAllLines(path)
                .Where(l => !string.IsNullOrWhiteSpace(l))
                .Select(l => JsonNode.Parse(l))
                .Where(n => n is not null)
                .Select(n => n!)
                .ToList();
        }
        catch { return []; }
    }

    private static DateTime HoursAgo(int h) => DateTime.UtcNow.AddHours(-h);
    private static DateTime DaysAgo(int d) => DateTime.UtcNow.AddDays(-d);

    private static string ExtractAgentId(string sessionKey)
    {
        var parts = sessionKey.Split(':');
        return parts.Length > 1 ? parts[1] : sessionKey;
    }

    private static string NormalizeName(string? v) =>
        System.Text.RegularExpressions.Regex.Replace((v ?? "").ToLower(), "[^a-z0-9]+", "");

    private static RunStatus NormalizeStatus(string? stopReason, string? extra = null)
    {
        var text = $"{stopReason} {extra}".ToLower();
        if (text.Contains("rate")) return RunStatus.RateLimited;
        if (text.Contains("error") || text.Contains("fail")) return RunStatus.Error;
        if (text.Contains("tooluse")) return RunStatus.Running;
        if (text.Contains("stop")) return RunStatus.Success;
        return RunStatus.Unknown;
    }

    private static double? ParseDurationMs(string? start, string? end)
    {
        if (start is null || end is null) return null;
        if (!DateTime.TryParse(start, out var s) || !DateTime.TryParse(end, out var e)) return null;
        var ms = (e - s).TotalMilliseconds;
        return ms > 0 ? ms : null;
    }

    private static string? FirstLine(string? v) =>
        v?.Split('\n').Select(l => l.Trim()).FirstOrDefault(l => l.Length > 0);

    private static (string? provider, string? model) ParseBannerModel(List<JsonNode> lines)
    {
        var text = lines
            .Where(l => l["type"]?.GetValue<string>() == "message" && l["message"]?["role"]?.GetValue<string>() == "assistant")
            .SelectMany(l => l["message"]?["content"]?.AsArray() ?? new JsonArray())
            .Where(c => c?["type"]?.GetValue<string>() == "text")
            .Select(c => c?["text"]?.GetValue<string>() ?? "")
            .Aggregate("", (a, b) => a + "\n" + b);

        var m = System.Text.RegularExpressions.Regex.Match(text, @"Model:\s*([a-z0-9\-]+)/([^\s]+)", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        if (!m.Success) return (null, null);
        return (m.Groups[1].Value.ToLower(), m.Groups[2].Value);
    }

    private static string? InferAgentFromRun(JsonNode run, List<string> knownIds)
    {
        var haystacks = new[] { "label", "task", "requesterDisplayKey", "childSessionKey" }
            .Select(k => run[k]?.GetValue<string>() ?? "")
            .Where(v => v.Length > 0)
            .ToList();

        return knownIds.FirstOrDefault(id =>
            haystacks.Any(h => NormalizeName(h).Contains(NormalizeName(id))));
    }

    private static string ResolveAgentId(
        string sessionKey, List<JsonNode> lines,
        Dictionary<string, string> childMap, List<string> knownIds)
    {
        var direct = ExtractAgentId(sessionKey);
        if (direct != "main") return direct;
        if (childMap.TryGetValue(sessionKey, out var mapped)) return mapped;

        var text = JsonSerializer.Serialize(lines).ToLower();
        return knownIds.FirstOrDefault(id => id != "main" && text.Contains(id.ToLower())) ?? direct;
    }

    // ─── core load ──────────────────────────────────────────────────────────────

    private (
        JsonNode? Config,
        List<TokenUsageRecord> Records,
        Dictionary<string, List<AgentTask>> TaskMap,
        List<string> ConfiguredProviders,
        List<DataSource> DataSources
    ) LoadData()
    {
        var config = ReadJson(ConfigPath);
        var runs = ReadJson(SubagentRunsPath);
        var agentEntries = config?["agents"]?["list"]?.AsArray().Where(n => n is not null).Select(n => n!).ToList() ?? [];
        var knownIds = agentEntries.Select(a => a["id"]?.GetValue<string>() ?? "").Where(s => s.Length > 0).ToList();

        // build child-session → agent map from subagent runs
        var childMap = new Dictionary<string, string>();
        foreach (var run in (runs?["runs"]?.AsObject().Select(kv => kv.Value) ?? []).Where(r => r is not null).Select(r => r!))
        {
            var inferred = InferAgentFromRun(run, knownIds);
            var child = run["childSessionKey"]?.GetValue<string>();
            if (inferred is not null && child is not null)
                childMap[child] = inferred;
        }

        var records = new List<TokenUsageRecord>();
        var taskMap = new Dictionary<string, List<AgentTask>>();
        var configuredProviders = new HashSet<string>();

        foreach (var agent in agentEntries)
        {
            var agentId = agent["id"]?.GetValue<string>();
            if (string.IsNullOrEmpty(agentId)) continue;
            var sessionIndexPath = Path.Combine(AgentsRoot, agentId, "sessions", "sessions.json");
            var sessionIndex = ReadJson(sessionIndexPath)?.AsObject();
            if (sessionIndex is null) continue;

            foreach (var kv in sessionIndex)
            {
                var sessionKey = kv.Key;
                var meta = kv.Value;
                if (meta is null) continue;

                var sessionFile = meta["sessionFile"]?.GetValue<string>();
                var lines = sessionFile is not null && File.Exists(sessionFile)
                    ? ReadJsonLines(sessionFile)
                    : [];

                var resolvedId = ResolveAgentId(sessionKey, lines, childMap, knownIds);
                var (bannerProv, bannerModel) = ParseBannerModel(lines);
                var provider = meta["modelProvider"]?.GetValue<string>() ?? bannerProv ?? "unknown";
                var model = meta["model"]?.GetValue<string>() ?? bannerModel ?? "unknown";
                configuredProviders.Add(provider);

                var inp = meta["inputTokens"]?.GetValue<long>() ?? -1L;
                var outp = meta["outputTokens"]?.GetValue<long>() ?? -1L;
                if (inp >= 0 || outp >= 0)
                {
                    var updatedAt = meta["updatedAt"]?.GetValue<string>();
                    records.Add(new TokenUsageRecord(
                        $"{sessionKey}:summary",
                        resolvedId, sessionKey,
                        updatedAt is not null ? DateTime.Parse(updatedAt).ToUniversalTime() : DateTime.UtcNow,
                        null, provider, model,
                        meta["abortedLastRun"]?.GetValue<bool>() == true ? RunStatus.Error : RunStatus.Success,
                        Math.Max(0, inp), Math.Max(0, outp),
                        meta["cacheRead"]?.GetValue<long>() ?? 0,
                        meta["totalTokens"]?.GetValue<long>() ?? Math.Max(0, inp) + Math.Max(0, outp),
                        null, null, "sessions.json"));
                }

                // per-response records from JSONL
                var assistantLines = lines
                    .Where(l => l["type"]?.GetValue<string>() == "message"
                             && l["message"]?["role"]?.GetValue<string>() == "assistant"
                             && l["message"]?["usage"] is not null)
                    .ToList();

                var completedTasks = new List<AgentTask>();

                foreach (var line in assistantLines)
                {
                    var usage = line["message"]!["usage"]!;
                    var lineId = line["id"]?.GetValue<string>() ?? Guid.NewGuid().ToString();
                    var parentId = line["parentId"]?.GetValue<string>();
                    var endTs = line["timestamp"]?.GetValue<string>();
                    var startTs = lines.FirstOrDefault(l => l["id"]?.GetValue<string>() == parentId)?["timestamp"]?.GetValue<string>() ?? endTs;
                    var latency = ParseDurationMs(startTs, endTs);
                    var lineProv = line["message"]!["provider"]?.GetValue<string>() ?? "";
                    var lineModel = line["message"]!["model"]?.GetValue<string>() ?? "";
                    var isGw = lineProv == "openclaw" && lineModel == "gateway-injected";
                    var rProv = isGw ? (bannerProv ?? provider) : (lineProv.Length > 0 ? lineProv : provider);
                    var rModel = isGw ? (bannerModel ?? model) : (lineModel.Length > 0 ? lineModel : model);
                    configuredProviders.Add(rProv);

                    var inputT = usage["input"]?.GetValue<long>() ?? 0;
                    var outputT = usage["output"]?.GetValue<long>() ?? 0;
                    var total = usage["totalTokens"]?.GetValue<long>() ?? inputT + outputT;

                    records.Add(new TokenUsageRecord(
                        $"{sessionKey}:{lineId}",
                        resolvedId, sessionKey,
                        startTs is not null ? DateTime.Parse(startTs).ToUniversalTime() : DateTime.UtcNow,
                        endTs is not null ? DateTime.Parse(endTs).ToUniversalTime() : null,
                        rProv, rModel,
                        NormalizeStatus(line["message"]!["stopReason"]?.GetValue<string>()),
                        inputT, outputT,
                        usage["cacheRead"]?.GetValue<long>() ?? 0,
                        total, latency,
                        latency is > 0 && total > 0 ? total / (latency.Value / 1000.0) : null,
                        "session-jsonl"));

                    var parentNode = lines.FirstOrDefault(l => l["id"]?.GetValue<string>() == parentId);
                    var titleText = parentNode?["message"]?["content"]?.AsArray()
                        .FirstOrDefault(c => c?["type"]?.GetValue<string>() == "text")?["text"]?.GetValue<string>();
                    var title = FirstLine(titleText)?.Length > 0
                        ? (FirstLine(titleText)!.Length > 120 ? FirstLine(titleText)![..120] : FirstLine(titleText)!)
                        : $"Conversation response {lineId[..Math.Min(8, lineId.Length)]}";
                    var stopReason = line["message"]!["stopReason"]?.GetValue<string>();

                    completedTasks.Add(new AgentTask(
                        lineId, title,
                        NormalizeStatus(stopReason) == RunStatus.Error ? "failed" : "completed",
                        startTs is not null ? DateTime.Parse(startTs).ToUniversalTime() : null,
                        endTs is not null ? DateTime.Parse(endTs).ToUniversalTime() : null,
                        latency, rModel, rProv, inputT, outputT,
                        NormalizeStatus(stopReason) == RunStatus.Error ? stopReason : null,
                        "session-jsonl"));
                }

                if (!taskMap.ContainsKey(resolvedId)) taskMap[resolvedId] = [];
                taskMap[resolvedId].AddRange(completedTasks.TakeLast(8));
            }
        }

        // add tasks from subagent runs
        foreach (var run in (runs?["runs"]?.AsObject().Select(kv => kv.Value) ?? []).Where(r => r is not null).Select(r => r!))
        {
            var agId = InferAgentFromRun(run, knownIds)
                ?? ExtractAgentId(run["requesterSessionKey"]?.GetValue<string>() ?? "main");
            var startedAt = run["startedAt"]?.GetValue<double?>();
            var completedAt = run["completedAt"]?.GetValue<double?>();
            var startDt = startedAt.HasValue ? DateTimeOffset.FromUnixTimeMilliseconds((long)startedAt.Value).UtcDateTime : (DateTime?)null;
            var endDt = completedAt.HasValue ? DateTimeOffset.FromUnixTimeMilliseconds((long)completedAt.Value).UtcDateTime : (DateTime?)null;

            var task = new AgentTask(
                run["runId"]?.GetValue<string>() ?? Guid.NewGuid().ToString(),
                FirstLine(run["task"]?.GetValue<string>()) ?? "Subagent task",
                startDt.HasValue && !endDt.HasValue ? "current" : "completed",
                startDt, endDt,
                startDt.HasValue && endDt.HasValue ? (endDt.Value - startDt.Value).TotalMilliseconds : null,
                run["model"]?.GetValue<string>(), null, null, null, null,
                "subagents/runs.json");

            if (!taskMap.ContainsKey(agId)) taskMap[agId] = [];
            taskMap[agId].Add(task);
        }

        var sources = new List<DataSource>
        {
            new("OpenClaw config", ConfigPath, File.Exists(ConfigPath), "Agent definitions, model config, providers, gateway metadata"),
            new("Agent sessions index", Path.Combine(AgentsRoot, "<agent>", "sessions", "sessions.json"), true, "Current session totals and model/provider metadata"),
            new("Agent session transcripts", Path.Combine(AgentsRoot, "<agent>", "sessions", "*.jsonl"), true, "Per-response usage, timestamps, stop reasons, inferred latency"),
            new("Subagent runs", SubagentRunsPath, File.Exists(SubagentRunsPath), "Active/completed subagent tasks"),
        };

        return (config, records, taskMap, configuredProviders.OrderBy(p => p).ToList(), sources);
    }

    // ─── public API ─────────────────────────────────────────────────────────────

    public TokenUsageData GetTokenUsage(TimeRange range)
    {
        var (config, records, _, configuredProviders, sources) = LoadData();
        var start = range switch
        {
            TimeRange.D7 => DaysAgo(7),
            TimeRange.D30 => DaysAgo(30),
            _ => HoursAgo(24)
        };

        var relevant = records.Where(r => r.StartedAt >= start).ToList();
        var configuredAgentIds = config?["agents"]?["list"]?.AsArray()
            .Select(a => a?["id"]?.GetValue<string>())
            .Where(s => !string.IsNullOrEmpty(s))
            .Select(s => s!)
            .ToList() ?? [];

        var filterAgents = relevant.Select(r => r.AgentId).Concat(configuredAgentIds).Distinct().Order().ToList();
        var filterProviders = relevant.Select(r => r.Provider).Concat(configuredProviders).Distinct().Order().ToList();
        var filterModels = relevant.Select(r => r.Model).Distinct().Order().ToList();
        var filterStatuses = relevant.Select(r => r.Status.ToString()).Distinct().Order().ToList();

        // buckets
        var bucketCount = range == TimeRange.H24 ? 24 : range == TimeRange.D7 ? 7 : 30;
        var stepMs = range == TimeRange.H24 ? TimeSpan.FromHours(1) : TimeSpan.FromDays(1);
        var buckets = Enumerable.Range(0, bucketCount).Select(i =>
        {
            var bStart = start + i * stepMs;
            var bEnd = bStart + stepMs;
            var items = relevant.Where(r => r.StartedAt >= bStart && r.StartedAt < bEnd).ToList();
            var label = range == TimeRange.H24
                ? bStart.ToLocalTime().ToString("HH:mm")
                : bStart.ToLocalTime().ToString("MMM d");
            return new TokenBucket(
                label, bStart, bEnd,
                items.GroupBy(r => r.Model).ToDictionary(g => g.Key, g => g.Sum(r => r.TotalTokens)),
                items.Sum(r => r.InputTokens), items.Sum(r => r.OutputTokens), items.Count);
        }).ToList();

        // breakdown
        var breakdown = relevant
            .GroupBy(r => $"{r.Provider}::{r.Model}")
            .Select(g =>
            {
                var parts = g.Key.Split("::");
                var latencies = g.Where(r => r.LatencyMs.HasValue).Select(r => r.LatencyMs!.Value).ToList();
                var tps = g.Where(r => r.TokensPerSecond.HasValue).Select(r => r.TokensPerSecond!.Value).ToList();
                return new ModelBreakdownRow(
                    parts[1], parts[0], g.Count(),
                    g.Sum(r => r.InputTokens), g.Sum(r => r.OutputTokens),
                    tps.Count > 0 ? tps.Average() : null,
                    latencies.Count > 0 ? latencies.Average() : null,
                    g.Count(r => r.Status is RunStatus.Error or RunStatus.RateLimited));
            })
            .OrderByDescending(r => r.InputTokens + r.OutputTokens)
            .ToList();

        var totalInput = relevant.Sum(r => r.InputTokens);
        var totalOutput = relevant.Sum(r => r.OutputTokens);
        var totalTokens = totalInput + totalOutput;
        var totalRequests = relevant.Count;
        var cacheRead = relevant.Sum(r => r.CacheReadTokens);
        var avgPerReq = totalRequests > 0 ? (double)totalTokens / totalRequests : 0;
        var topModel = breakdown.FirstOrDefault()?.Model ?? "N/A";

        var five = records.Where(r => r.StartedAt >= HoursAgo(5)).ToList();
        var seven = records.Where(r => r.StartedAt >= DaysAgo(7)).ToList();

        var cards = new List<SummaryCard>
        {
            new("Total input tokens", totalInput.ToString("N0")),
            new("Total output tokens", totalOutput.ToString("N0")),
            new("Total requests", totalRequests.ToString("N0")),
            new("Avg tokens / req", avgPerReq.ToString("N0")),
            new("Cache read tokens", cacheRead.ToString("N0"), "Tracked when present in OpenClaw usage payloads"),
            new("Top model", topModel)
        };

        var windows = new List<TrackedWindow>
        {
            new("Last 5 hours", five.Sum(r => r.TotalTokens), five.Count, true, "Tracked only; provider quota ceiling not exposed."),
            new("Last 7 days", seven.Sum(r => r.TotalTokens), seven.Count, true, "Tracked only; weekly reset time unknown.")
        };

        var notes = new List<string>
        {
            "Provider quota ceilings (OpenAI, Anthropic) are not exposed in local OpenClaw data. Windows show tracked usage only.",
            "Latency and tokens/sec are inferred from session JSONL timestamps; unavailable for summary-only session rows."
        };

        return new TokenUsageData(
            DateTime.UtcNow, 30_000,
            new FilterOptions(filterAgents, filterProviders, filterModels, filterStatuses),
            cards, buckets, breakdown, windows,
            relevant.OrderByDescending(r => r.StartedAt).Take(150).ToList(),
            notes, sources);
    }

    public AgentsData GetAgents()
    {
        var (config, records, taskMap, _, sources) = LoadData();
        var agentEntries = config?["agents"]?["list"]?.AsArray()
            .Where(n => n is not null).Select(n => n!).ToList() ?? [];

        var cards = agentEntries.Select(agent =>
        {
            var agentId = agent["id"]?.GetValue<string>() ?? "unknown";
            var name = agent["name"]?.GetValue<string>() ?? agentId;
            var tasks = (taskMap.TryGetValue(agentId, out var t) ? t : [])
                .OrderByDescending(t => t.StartedAt ?? DateTime.MinValue)
                .ToList();

            var outstanding = tasks.Where(t => t.Status == "outstanding").Take(8).ToList();
            var current = tasks.Where(t => t.Status == "current").Take(8).ToList();
            var completed = tasks.Where(t => t.Status == "completed").Take(12).ToList();
            var failures = tasks.Where(t => t.Status == "failed").ToList();
            var recentUsage = records.Where(r => r.AgentId == agentId && r.StartedAt >= DaysAgo(1)).ToList();

            var status = AgentStatus.Idle;
            if (current.Count > 0) status = AgentStatus.Busy;
            if (failures.Count > 0) status = AgentStatus.Error;
            if (!tasks.Any() && !recentUsage.Any()) status = AgentStatus.Offline;
            if (recentUsage.Any(r => r.Status == RunStatus.RateLimited)) status = AgentStatus.RateLimited;

            var primaryModel = agent["model"]?["primary"]?.GetValue<string>()
                ?? config?["agents"]?["defaults"]?["model"]?["primary"]?.GetValue<string>();
            var fallbacks = agent["model"]?["fallbacks"]?.AsArray()
                .Select(f => f?.GetValue<string>() ?? "").Where(s => s.Length > 0).ToList()
                ?? [];

            return new AgentCard(
                agentId, name, status,
                outstanding.Count, current.Count,
                completed.Count(t => t.EndedAt.HasValue && t.EndedAt.Value >= DaysAgo(1)),
                recentUsage.FirstOrDefault()?.StartedAt ?? current.FirstOrDefault()?.StartedAt ?? completed.FirstOrDefault()?.EndedAt,
                primaryModel, fallbacks,
                outstanding, current, completed, failures,
                new TokenSummary(recentUsage.Sum(r => r.InputTokens), recentUsage.Sum(r => r.OutputTokens), recentUsage.Count),
                [
                    "Outstanding queue is live only if exposed via subagent run metadata.",
                    "Completed tasks come from assistant message runs and subagent run metadata."
                ]);
        }).ToList();

        return new AgentsData(
            DateTime.UtcNow, 15_000,
            new AgentFilterOptions(cards.Select(c => c.AgentId).ToList(), ["idle", "busy", "error", "offline", "rate-limited"]),
            cards, [
                "Agent statuses are inferred from recent activity, errors, and active subagent runs.",
                "A true pending task queue is not exposed by a dedicated OpenClaw API on this machine."
            ], sources);
    }
}
