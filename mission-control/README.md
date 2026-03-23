# Mission Control

Mission Control is a dark-theme operations dashboard for OpenClaw telemetry.

This version keeps the same underlying real data work intact, but redesigns the client into a denser control-room UI with clearer information hierarchy and better use of screen space.

## UX structure
- **Top navigation shell** instead of a fixed sidebar
- **Token Usage** redesigned as an ops console:
  - compact page header with range control and live status
  - horizontal stats strip for core telemetry
  - cohesive filter toolbar
  - large token volume chart beside a persistent model breakdown table
  - secondary performance / totals / tracked windows row
  - dense recent request log table
  - collapsible data-source traceability section
- **Agents** redesigned as a roster + detail view:
  - filterable agent roster on the left
  - selected-agent detail panel on the right
  - current/completed/failure task sections
  - notes and collapsible source traceability section

## Real data sources discovered on this machine
1. `C:\Users\Ian\.openclaw\openclaw.json`
   - live
   - configured agents, model primary/fallbacks, provider definitions, Ollama endpoint
2. `C:\Users\Ian\.openclaw\agents\<agent>\sessions\sessions.json`
   - live
   - per-session totals including provider/model/input/output/cache/total tokens
3. `C:\Users\Ian\.openclaw\agents\<agent>\sessions\*.jsonl`
   - live
   - per-response usage objects, timestamps, stop reasons, transcript structure
4. `C:\Users\Ian\.openclaw\subagents\runs.json`
   - live
   - subagent run metadata used for mission board current/completed tasks
5. `C:\Users\Ian\.openclaw\logs\config-audit.jsonl`
   - live but currently documented only
   - can support future operational event timelines
6. `C:\Users\Ian\.openclaw\lcm.db`
   - live and schema inspected
   - available for future deeper message/context analytics; current version does not depend on it directly

## Live vs inferred vs unavailable
### Live now
- agent list
- primary/fallback model config
- provider/model labels
- session token totals
- per-response token usage from JSONL
- recent task/runs from subagent metadata
- timestamps for activity and response completion

### Inferred now
- response latency from parent/child timestamps in session JSONL
- tokens/sec from total tokens divided by inferred duration
- agent status from recent errors, current runs, and last activity
- completed task cards from recent assistant run entries

### Fallback / approximate now
- outstanding queue counts: only live when present via current inferred task sources
- usage windows: tracked usage only, not hard provider quota ceilings

### Unavailable on this machine today
- exact provider quota limits / reset times for OpenAI or Anthropic
- dedicated OpenClaw queue API for pending tasks
- authoritative rate-limit budget objects
- formal per-agent health endpoint

## Architecture
- `src/server/models` - normalized dashboard contracts
- `src/server/providers` - OpenClaw file adapters and normalization
- `src/server/index.ts` - API server
- `src/client/components` - reusable UI pieces
- `src/client/pages` - Token Usage and Agents pages
- `src/client/lib` - API client and polling hook

## Run locally
```powershell
cd C:\Users\Ian\.openclaw\workspace\mission-control
npm install
npm run dev
```
- Vite UI: `http://127.0.0.1:4173`
- API server: `http://127.0.0.1:4187`

## Build
```powershell
npm run build
npm run start
```

## Extending later
- Add a provider-specific quota adapter under `src/server/providers`
- Add new pages under `src/client/pages`
- Add another API route in `src/server/index.ts`
- Expand the data model in `src/server/models/dashboard.ts` without changing UI primitives

## TODO markers
- integrate `lcm.db` message_part metrics where useful
- surface `config-audit.jsonl` as an event timeline page
- add true queue integration if OpenClaw exposes it later
- add provider quota adapters when local APIs become available
