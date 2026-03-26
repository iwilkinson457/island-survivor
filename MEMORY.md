# MEMORY

This file is Clare's curated long-term memory.
Only stable, durable, high-value information should live here.

## Identity
- Clare is the front desk, coordinator, and orchestration agent
- Clare should prefer routing specialist work instead of keeping everything herself
- Clare should keep the conversation coherent while delegating internally

## User
- Primary user: Wilko
- Formal name: Ian Wilkinson
- Email: iwilkinson@testtech.com.au
- Wilko prefers Windows-first instructions
- Wilko prefers PowerShell and Windows paths
- Wilko likes one step at a time
- Wilko strongly prefers C#/Blazor over React/TypeScript for maintainable internal tools like Mission Control

## Agent map
- Clare = orchestrator / front desk
- Britannica = memory / knowledge
- The Codefather = software implementation
- Gideon = code reviewer
- Roxy = Unity specialist
- Control Father = industrial control systems specialist (Aveva Plant SCADA, ControlLogix PLCs)

## Routing defaults
- Send durable knowledge, retrieval, and document-ingestion-to-knowledge tasks to Britannica
- Send Unity engine implementation, Unity debugging, Unity C# scripting, and Unity build/performance work to Roxy
- Send software implementation, coding, patching, refactoring, Blazor, .NET MAUI, API, service, script, and SQL build tasks outside Unity to The Codefather
- Send code review, bug finding, maintainability review, architecture critique, and technical risk review tasks to Gideon

## Stack awareness
- The Codefather prefers C#
- The Codefather prefers Blazor for websites
- The Codefather prefers .NET MAUI for mobile apps
- Gideon is strong in C#, Blazor, .NET MAUI, SQL, and service/API review
- Unity should normally be treated as a separate specialist area, not owned by Clare, The Codefather, or Gideon by default

## Coordination rules
- Clare should remain the default user-facing entry point
- Clare should combine specialist outputs into a coherent response when useful
- Clare should avoid becoming the long-term knowledge owner
- Clare should avoid becoming the primary implementation agent
- Clare should avoid becoming the primary code review agent
- When specialist agents already exist as running sessions, Clare should send work to those real persistent agent sessions instead of spawning generic main subagents
- This persistent-session routing rule applies to Codefather, Britannica, Gideon, Roxy, and Nena

## Nena
- Nena is the persistent document writing and formatting specialist agent
- Nena should own writing, rewriting, proofreading, formatting, and polished document-output tasks

## Roxy
- Roxy is the persistent Unity specialist agent
- Roxy should own Unity-specific implementation, debugging, tooling, integration, build, and performance tasks
- Roxy should use the installed Unity skill when it clearly applies

## Control Father
- Control Father is the persistent industrial control systems specialist agent
- Control Father should own Aveva Plant SCADA configuration, ControlLogix PLC programming, HMI design, and industrial automation tasks
- Control Father uses local Ollama model (`ollama/qwen3.5:35b`) to keep costs at zero
- Control Father has a safety-critical mindset and always considers failure modes

## Current durable operational notes
- Mission Control's intended deliverable is the parallel Blazor app at `C:\Users\Ian\.openclaw\workspace\mission-control-blazor`, not the older React/TypeScript app in `mission-control`
- Mission Control has been unstable when launched from an interactive shell; a detached `dotnet run` launch with logs written to `mission-control.stdout.log` and `mission-control.stderr.log` in the app folder worked and listened on `http://localhost:5004`
- OpenClaw was on version `2026.3.13` during this session, with update `2026.3.23-2` available on the stable channel via `openclaw update`
- Britannica should be treated as partially untrusted for ingestion status: require exact paths and verify outputs on disk before accepting completion claims
- **Codefather now uses local Ollama model (`ollama/qwen3.5:35b`) instead of paid Anthropic Claude to conserve subscription tokens**
- **Gideon also switched to local Ollama model (`ollama/qwen3.5:35b`) for reviews**
- **Longwall Parameter Comparator tool built by Codefather**: `C:\Users\Ian\.openclaw\workspace\tools\param-comparator\compare_params.py`
  - Python CLI tool that diffs two Excel parameter sheets (PMCR revisions)
  - Flags changed, missing, and new parameters
  - Outputs human-readable tables and Markdown reports
  - Successfully tested with sample files showing changed/missing/new params
- **MineM8 proposal work completed**: `C:\ClareDocuments\MineM8\MineM8 - Narrabri Foundational Asset Starter Proposal.docx` ($10k starter for shearer model + coal textures)
  - Email sent to iwilkinson@testtech.com.au with attachment
  - Proposal submitted for approval
- **Contractor briefs reviewed by Roxy and Nena**:
  - `C:\ClareDocuments\MineM8\CGTrader Project Coal Material - Nena Final.docx` — needs client confirmation on texture specs (resolution, URP workflow, tile scale, format)
  - `C:\ClareDocuments\MineM8\ShearerModelRequirements - Nena Final.docx` — needs client confirmation on cylinder scope and drawing pack delivery method
- **Britannica ingestion still requires external wrappers** for DOCX/PDF/XLSX; she cannot directly use installed skills in runtime. Use `word-docx`, `pdf-text-extractor`, and Excel skills via external Python scripts, not Britannica's direct parsing.
