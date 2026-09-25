# Copyright 2026 Dmitriy Chiginskiy
# SPDX-License-Identifier: Apache-2.0
$ErrorActionPreference = 'Stop'
$compiler = Join-Path $env:WINDIR 'Microsoft.NET\Framework64\v4.0.30319\csc.exe'
if (-not (Test-Path -LiteralPath $compiler)) { $compiler = Join-Path $env:WINDIR 'Microsoft.NET\Framework\v4.0.30319\csc.exe' }
$testDirectory = Join-Path $PSScriptRoot 'test-output'
New-Item -ItemType Directory -Force -Path $testDirectory | Out-Null
$testExe = Join-Path $testDirectory 'Tests.exe'
$sources = @(Get-ChildItem -LiteralPath (Join-Path $PSScriptRoot 'src') -Filter '*.cs' | ForEach-Object FullName)
$sources += Join-Path $PSScriptRoot 'tests\Tests.cs'
$license = Join-Path $PSScriptRoot 'LICENSE'
$notice = Join-Path $PSScriptRoot 'NOTICE'
if (-not (Test-Path -LiteralPath $license)) { throw 'LICENSE is required for tests.' }
if (-not (Test-Path -LiteralPath $notice)) { throw 'NOTICE is required for tests.' }
& $compiler /nologo /target:exe /main:Tests /optimize+ /warnaserror+ /reference:System.Drawing.dll /reference:System.Windows.Forms.dll "/resource:$license,LiftPaper.LICENSE" "/resource:$notice,LiftPaper.NOTICE" "/out:$testExe" @sources
if ($LASTEXITCODE -ne 0) { throw 'Test compilation failed.' }
& $testExe
if ($LASTEXITCODE -ne 0) { throw 'Regression tests failed.' }
