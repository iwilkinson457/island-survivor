# SCADA Parameter Compare

A small Blazor Server tool for comparing two SCADA parameter sets before preparing a PMCR (Parameter Management Change Request).

## What it does

- Paste two parameter lists (CSV or TSV, copied directly from Excel)
- Detects delimiter automatically (tab or comma)
- Matches rows by tag name — not row position, so reordering doesn't cause false diffs
- Shows three output sections:
  - **Added** — tags in the proposed set that don't exist in baseline
  - **Removed** — tags in baseline that are missing from proposed
  - **Changed** — tags present in both but with different values, units, or descriptions
- Old→New values shown with strikethrough on the before value
- Unchanged tags collapsible at the bottom
- Optional **OPC tag name validation** — flags reserved characters, bad start characters, length over 256
- Numeric normalisation: `1.0`, `1`, `1.000` all compare equal
- No file uploads, no persistence — paste and compare

## Accepted format

CSV or TSV, header row optional. Recognised column names:

| Column | Aliases |
|--------|---------|
| Tag name | `TagName`, `Tag`, `Name` |
| Value | `Value` |
| Unit | `Unit`, `Units` |
| Description | `Description`, `Desc` |

If no header is detected, columns are assumed in order: Tag, Value, Unit, Description.

## Run locally

```powershell
cd C:\Users\Ian\.openclaw\workspace\scada-param-compare
dotnet run
```

Default port: `http://localhost:5130`  
Or specify: `dotnet run --urls http://127.0.0.1:5010`

## Stack

- Blazor Server · .NET 9
- C# only — no JavaScript, no third-party packages
- Dark ops-console UI
