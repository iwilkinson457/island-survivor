#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Britannica Markdown Ingestion Dispatcher (PowerShell wrapper)

.DESCRIPTION
    Routes a file to the appropriate Python wrapper, validates the result,
    and writes a cleaned Markdown note to the Britannica cleaned notes folder.

.PARAMETER File
    Path to the source file (DOCX, PDF, XLSX, XLSM, XLS, PPTX).

.PARAMETER Out
    Output directory for the cleaned note.
    Defaults to: C:\Users\<you>\.openclaw\agents\britannica\workspace\notes\cleaned

.PARAMETER DryRun
    Extract and validate but do not write output.

.EXAMPLE
    .\ingest.ps1 "C:\Docs\SomeProcedure.docx"
    .\ingest.ps1 "C:\Docs\Report.pdf" -Out "D:\Britannica\notes\cleaned"
    .\ingest.ps1 "C:\Docs\Params.xlsx" -DryRun
#>
param(
    [Parameter(Mandatory=$true, Position=0)]
    [string]$File,

    [string]$Out = "",

    [switch]$DryRun
)

$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$IngestPy  = Join-Path $ScriptDir "ingest.py"

if (-not (Test-Path $IngestPy)) {
    Write-Error "ingest.py not found at $IngestPy"
    exit 2
}

if (-not (Test-Path $File)) {
    Write-Error "Source file not found: $File"
    exit 2
}

$pyArgs = @($IngestPy, $File)

if ($Out -ne "") {
    $pyArgs += "--out"
    $pyArgs += $Out
}

if ($DryRun) {
    $pyArgs += "--dry-run"
}

Write-Host "[ingest] Dispatching: $File" -ForegroundColor Cyan
python @pyArgs
$exitCode = $LASTEXITCODE

if ($exitCode -eq 0) {
    Write-Host "[ingest] Done - grade A/B (trusted or usable)." -ForegroundColor Green
} elseif ($exitCode -eq 1) {
    Write-Host "[ingest] Done - grade C/D (weak). Review the note before use." -ForegroundColor Yellow
} else {
    Write-Host "[ingest] Failed - unsupported file type or hard error." -ForegroundColor Red
}

exit $exitCode
