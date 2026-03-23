# Mission Control (Blazor)

Mission Control (Blazor) is a dark-theme operations dashboard for OpenClaw, rebuilt in C# with Blazor Server so it is easier to read and maintain in the preferred .NET stack.

## Stack
- **Blazor Server** (`InteractiveServer`) on **.NET 9**
- Pure **C#** data layer reading local OpenClaw files directly
- No React, no TypeScript, no client-side chart library
- SVG chart rendering built in Razor/C#

## Project location
`C:\Users\Ian\.openclaw\workspace\mission-control-blazor`

## Views
### Token Usage
- 24h / 7d / 30d range switching
- stats strip for core token metrics
- filter toolbar (agent, provider, model, outcome)
- stacked token-volume SVG chart
- model/provider breakdown table
- filtered totals and tracked windows
- recent request log
- collapsible source traceability

### Agents
- filterable agent roster
- selected-agent detail panel
- current / completed / failed task sections
- inferred status presentation
- notes and source traceability

## Real data sources used
1. `C:\Users\Ian\.openclaw\openclaw.json`
   - configured agents
   - primary/fallback model config
   - provider definitions
2. `C:\Users\Ian\.openclaw\agents\<agent>\sessions\sessions.json`
   - per-session token totals
   - model/provider metadata
3. `C:\Users\Ian\.openclaw\agents\<agent>\sessions\*.jsonl`
   - per-response usage entries
   - timestamps
   - stop reasons
4. `C:\Users\Ian\.openclaw\subagents\runs.json`
   - subagent task/run metadata

## Live vs inferred vs unavailable
### Live
- configured agent list
- provider and model labels
- session totals
- per-response token usage when present
- task/run metadata from subagent runs

### Inferred
- latency from timestamp deltas in session JSONL
- tokens/second from total tokens ÷ inferred duration
- agent status from activity + failures + active runs
- some task labeling from transcript structure

### Unavailable
- authoritative provider quota ceilings / reset times
- dedicated pending queue API
- formal per-agent health endpoint

The UI calls this out explicitly in notes and source panels.

## Run locally
```powershell
cd C:\Users\Ian\.openclaw\workspace\mission-control-blazor
dotnet run
```

Default local URLs from launch settings:
- `http://localhost:5004`
- `https://localhost:7292`

## Build
```powershell
dotnet build
dotnet publish -c Release
```

## Structure
- `Models/` - dashboard records / DTOs
- `Services/` - OpenClaw file reader and aggregation logic
- `Components/Layout/` - app shell
- `Components/Pages/` - Token Usage and Agents pages
- `Components/Shared/` - badges, chart, task blocks, source list
- `wwwroot/app.css` - dark ops-console styling
