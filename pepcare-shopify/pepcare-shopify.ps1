param(
    [Parameter(ValueFromRemainingArguments = $true)]
    [string[]]$ArgsFromCaller
)

$root = Split-Path -Parent $MyInvocation.MyCommand.Path
$project = Join-Path $root 'PepCare.Shopify\PepCare.Shopify.csproj'

Push-Location $root
try {
    dotnet run --project $project -- @ArgsFromCaller
}
finally {
    Pop-Location
}
