# Copyright 2026 Dmitriy Chiginskiy
# SPDX-License-Identifier: Apache-2.0
param(
    [Parameter(Mandatory = $true)]
    [ValidatePattern('^\d+\.\d+\.\d+$')]
    [string]$Version,

    [switch]$Prerelease,
    [switch]$Reauthenticate
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$RepoSlug = 'chiginskiy/LiftPaper'
$Tag = 'v' + $Version
$Downloads = Join-Path $env:USERPROFILE 'Downloads'
$Timestamp = Get-Date -Format 'yyyyMMdd-HHmmss'
$LogPath = Join-Path $Downloads ("LiftPaper-$Version-release-$Timestamp.log.txt")
$ExitCode = 1
$TranscriptStarted = $false

function Invoke-Native {
    param(
        [Parameter(Mandatory = $true)][string]$FilePath,
        [Parameter(ValueFromRemainingArguments = $true)][string[]]$Arguments
    )

    $oldPreference = $ErrorActionPreference
    try {
        $ErrorActionPreference = 'Continue'
        & $FilePath @Arguments
        $code = $LASTEXITCODE
    }
    finally {
        $ErrorActionPreference = $oldPreference
    }

    if ($code -ne 0) {
        throw "$FilePath exited with code $code."
    }
}

function Invoke-NativeCode {
    param(
        [Parameter(Mandatory = $true)][string]$FilePath,
        [Parameter(ValueFromRemainingArguments = $true)][string[]]$Arguments
    )

    $oldPreference = $ErrorActionPreference
    try {
        $ErrorActionPreference = 'Continue'
        & $FilePath @Arguments
        return $LASTEXITCODE
    }
    finally {
        $ErrorActionPreference = $oldPreference
    }
}

try {
    if (-not (Test-Path -LiteralPath $Downloads)) {
        throw "Downloads directory not found: $Downloads"
    }

    Start-Transcript -LiteralPath $LogPath -Force | Out-Null
    $TranscriptStarted = $true
    Set-Location -LiteralPath $PSScriptRoot

    Write-Host "=== LiftPaper $Version release ==="
    Write-Host "Repository: $PSScriptRoot"
    Write-Host "Transcript: $LogPath"

    foreach ($command in @('git', 'gh')) {
        if (-not (Get-Command $command -ErrorAction SilentlyContinue)) {
            throw "Required command '$command' was not found in PATH."
        }
    }

    if ($Reauthenticate) {
        [void](Invoke-NativeCode gh auth logout --hostname github.com --user chiginskiy)
    }

    $authCode = Invoke-NativeCode gh auth status --hostname github.com
    if ($authCode -ne 0) {
        Write-Host ""
        Write-Host "GitHub authentication is required."
        Invoke-Native gh auth login --hostname github.com --git-protocol https --web --clipboard --scopes 'repo,workflow,read:org'
    }
    Invoke-Native gh auth setup-git --hostname github.com

    $inside = [string](& git rev-parse --show-toplevel)
    if ($LASTEXITCODE -ne 0 -or -not $inside) {
        throw 'release.ps1 must be run from a Git clone.'
    }
    $repoRoot = $inside.Trim()
    if ($repoRoot -ne (Resolve-Path -LiteralPath $PSScriptRoot).Path) {
        throw "Unexpected repository root: $repoRoot"
    }

    $origin = [string](& git remote get-url origin)
    if ($LASTEXITCODE -ne 0 -or $origin.Trim() -notmatch 'chiginskiy/LiftPaper(?:\.git)?$') {
        throw "Unexpected origin remote: $origin"
    }

    $branch = ([string](& git branch --show-current)).Trim()
    if ($branch -ne 'main') {
        throw "Releases must be created from main; current branch is '$branch'."
    }

    $dirty = @(& git status --porcelain)
    if ($LASTEXITCODE -ne 0) { throw 'git status failed.' }
    if ($dirty.Count -ne 0) {
        $dirty | ForEach-Object { Write-Host $_ }
        throw 'Working tree must be clean before releasing.'
    }

    Invoke-Native git fetch origin main --tags
    $head = ([string](& git rev-parse HEAD)).Trim()
    $originMain = ([string](& git rev-parse origin/main)).Trim()
    if ($head -ne $originMain) {
        throw "Local main ($head) does not match origin/main ($originMain)."
    }

    $appInfoPath = Join-Path $PSScriptRoot 'src\AppInfo.cs'
    $appInfo = Get-Content -LiteralPath $appInfoPath -Raw
    $escapedVersion = [Regex]::Escape($Version)
    if ($appInfo -notmatch ('public const string Version = "' + $escapedVersion + '";')) {
        throw "src/AppInfo.cs does not declare Version = $Version."
    }
    if ($appInfo -notmatch ('AssemblyVersion\("' + $escapedVersion + '\.0"\)')) {
        throw "AssemblyVersion does not match $Version.0."
    }
    if ($appInfo -notmatch ('AssemblyFileVersion\("' + $escapedVersion + '\.0"\)')) {
        throw "AssemblyFileVersion does not match $Version.0."
    }

    $notes = Join-Path $PSScriptRoot ("docs\release-notes-$Version.md")
    if (-not (Test-Path -LiteralPath $notes)) {
        throw "Release notes not found: $notes"
    }

    $remoteTag = @(& git ls-remote --tags origin "refs/tags/$Tag")
    if ($LASTEXITCODE -ne 0) { throw 'Could not check remote tags.' }
    if ($remoteTag.Count -gt 0) {
        throw "Tag $Tag already exists."
    }

    $oldPreference = $ErrorActionPreference
    try {
        $ErrorActionPreference = 'Continue'
        $releaseTags = @(& gh release list --repo $RepoSlug --limit 100 --json tagName --jq '.[].tagName')
        $releaseListExit = $LASTEXITCODE
    }
    finally {
        $ErrorActionPreference = $oldPreference
    }
    if ($releaseListExit -ne 0) { throw 'Could not list GitHub releases.' }
    if ($releaseTags -contains $Tag) {
        throw "GitHub Release $Tag already exists."
    }

    Write-Host ""
    Write-Host "=== Build ==="
    & (Join-Path $PSScriptRoot 'build.ps1')
    if ($LASTEXITCODE -ne 0) { throw "build.ps1 failed with exit code $LASTEXITCODE." }

    Write-Host ""
    Write-Host "=== Tests ==="
    & (Join-Path $PSScriptRoot 'test.ps1')
    if ($LASTEXITCODE -ne 0) { throw "test.ps1 failed with exit code $LASTEXITCODE." }

    $exe = Join-Path $PSScriptRoot 'dist\LiftPaper.exe'
    $sums = Join-Path $PSScriptRoot 'dist\SHA256SUMS.txt'
    $license = Join-Path $PSScriptRoot 'LICENSE'
    $notice = Join-Path $PSScriptRoot 'NOTICE'

    foreach ($path in @($exe, $sums, $license, $notice)) {
        if (-not (Test-Path -LiteralPath $path)) {
            throw "Required release file not found: $path"
        }
    }

    $signature = Get-AuthenticodeSignature -LiteralPath $exe
    if ($signature.Status -eq 'NotSigned') {
        Write-Warning 'LiftPaper.exe is not publisher-code-signed. SmartScreen may warn users.'
    }
    elseif ($signature.Status -ne 'Valid') {
        throw "LiftPaper.exe has an invalid Authenticode status: $($signature.Status)."
    }
    else {
        Write-Host "Authenticode signature: Valid ($($signature.SignerCertificate.Subject))"
    }

    $hash = (Get-FileHash -LiteralPath $exe -Algorithm SHA256).Hash.ToLowerInvariant()
    $checksumText = (Get-Content -LiteralPath $sums -Raw).Trim()
    if ($checksumText -notmatch ('^' + [Regex]::Escape($hash) + '\s+LiftPaper\.exe$')) {
        throw 'SHA256SUMS.txt does not match LiftPaper.exe.'
    }
    Write-Host "LiftPaper.exe SHA-256: $hash"

    $args = @(
        'release', 'create', $Tag,
        $exe, $sums, $license, $notice,
        '--repo', $RepoSlug,
        '--target', $head,
        '--title', "LiftPaper $Version",
        '--notes-file', $notes
    )
    if ($Prerelease) {
        $args += '--prerelease'
    }

    Write-Host ""
    Write-Host "=== Publish $Tag ==="
    Invoke-Native gh @args

    Write-Host ""
    Write-Host "=== Verify release ==="
    Invoke-Native gh release view $Tag --repo $RepoSlug

    Write-Host ""
    Write-Host "SUCCESS: LiftPaper $Version published."
    $ExitCode = 0
}
catch {
    Write-Host ""
    Write-Host "ERROR: $($_.Exception.Message)"
    $ExitCode = 1
}
finally {
    if ($TranscriptStarted) {
        Write-Host "Finished: $(Get-Date -Format o)"
        Write-Host "Transcript: $LogPath"
        Stop-Transcript | Out-Null
    }
}

exit $ExitCode
