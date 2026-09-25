param([string]$OutputDirectory = (Join-Path $PSScriptRoot 'dist'))
$ErrorActionPreference = 'Stop'
$compiler = Join-Path $env:WINDIR 'Microsoft.NET\Framework64\v4.0.30319\csc.exe'
if (-not (Test-Path -LiteralPath $compiler)) {
    $compiler = Join-Path $env:WINDIR 'Microsoft.NET\Framework\v4.0.30319\csc.exe'
}
if (-not (Test-Path -LiteralPath $compiler)) { throw '.NET Framework C# compiler was not found.' }
$license = Join-Path $PSScriptRoot 'LICENSE'
if (-not (Test-Path -LiteralPath $license)) { throw 'LICENSE is required before a distributable build.' }
New-Item -ItemType Directory -Force -Path $OutputDirectory | Out-Null
$output = Join-Path $OutputDirectory 'LiftPaper.exe'
$sources = @(Get-ChildItem -LiteralPath (Join-Path $PSScriptRoot 'src') -Filter '*.cs' | ForEach-Object FullName)
& $compiler /nologo /target:winexe /platform:anycpu /optimize+ /warnaserror+ /reference:System.Drawing.dll /reference:System.Windows.Forms.dll "/resource:$license,LiftPaper.LICENSE" "/out:$output" @sources
if ($LASTEXITCODE -ne 0) { throw 'Build failed.' }
$hash = (Get-FileHash -LiteralPath $output -Algorithm SHA256).Hash.ToLowerInvariant()
[IO.File]::WriteAllText((Join-Path $OutputDirectory 'SHA256SUMS.txt'), "$hash  LiftPaper.exe`n", (New-Object Text.UTF8Encoding($false)))
Write-Host "Built $output"
