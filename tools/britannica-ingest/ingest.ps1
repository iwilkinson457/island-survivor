#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Britannica Markdown Ingestion Dispatcher (PowerShell wrapper)

.DESCRIPTION
    Routes a file to the appropriate Python wrapper, validates the result,
    and writes a cleaned Markdown note to notes\cleaned\.

.PARAMETER File
    Path to the source file (DOCX, PDF, XLSX, XLSM, XLS, PPTX).

.PARAMETER Out
    Output directory. Defaults to notes\cleaned\ relative to this script.

.PARAMETER DryRun
    Extract and validate but do not write output.

.EXAMPLE
    .\ingest.ps1 "C:\Docs\SomeProcedure.docx"
    .\ingest.ps1 "C:\Docs\Report.pdf" --Out "D:\Britannica\notes\cleaned"
    .\ingest.ps1 "C:\Docs\Params.xlsx" --DryRun
#>
param(
    [Parameter(Mandatory=$true, Position=0)]
    [string]$File,

    [string]$Out = "",

    [switch]$DryRun
)

$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$IngestPy = Join-Path $ScriptDir "ingest.py"

if (-not (Test-Path $IngestPy)) {
    Write-Error "ingest.py not found at $IngestPy"
    exit 2
}

if (-not (Test-Path $File)) {
    Write-Error "Source file not found: $File"
    exit 2
}

$args_list = @($IngestPy, $File)

if ($Out -ne "") {
    $args_list += "--out"
    $args_list += $Out
}

if ($DryRun) {
    $args_list += "--dry-run"
}

Write-Host "[ingest] Dispatching: $File" -ForegroundColor Cyan
python @args_list
$exitCode = $LASTEXITCODE

switch ($exitCode) {
    0 { Write-Host "[ingest] Done — grade A/B (trusted or usable)." -ForegroundColor Green }
    1 { Write-Host "[ingest] Done — grade C/D (weak extraction). Review the note before use." -ForegroundColor Yellow }
    2 { Write-Host "[ingest] Failed — unsupported type or hard error." -ForegroundColor Red }
}

exit $exitCode
