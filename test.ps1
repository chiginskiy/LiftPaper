$ErrorActionPreference = 'Stop'
$compiler = Join-Path $env:WINDIR 'Microsoft.NET\Framework64\v4.0.30319\csc.exe'
if (-not (Test-Path -LiteralPath $compiler)) { $compiler = Join-Path $env:WINDIR 'Microsoft.NET\Framework\v4.0.30319\csc.exe' }
$testDirectory = Join-Path $PSScriptRoot 'test-output'
New-Item -ItemType Directory -Force -Path $testDirectory | Out-Null
$testExe = Join-Path $testDirectory 'Tests.exe'
$sources = @(Get-ChildItem -LiteralPath (Join-Path $PSScriptRoot 'src') -Filter '*.cs' | ForEach-Object FullName)
$sources += Join-Path $PSScriptRoot 'tests\Tests.cs'
& $compiler /nologo /target:exe /main:Tests /optimize+ /warnaserror+ /reference:System.Drawing.dll /reference:System.Windows.Forms.dll "/out:$testExe" @sources
if ($LASTEXITCODE -ne 0) { throw 'Test compilation failed.' }
& $testExe
if ($LASTEXITCODE -ne 0) { throw 'Regression tests failed.' }
