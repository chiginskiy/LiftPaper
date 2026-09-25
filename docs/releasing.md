# Releasing LiftPaper

LiftPaper uses a **local-first release process on Windows**. GitHub Actions are not required for publishing releases.

The GitHub build workflow is intentionally manual-only (`workflow_dispatch`). There is no automatic release workflow while hosted Actions are unavailable or unnecessary.

## Prerequisites

- Windows PowerShell 5.1 or newer;
- Git;
- GitHub CLI (`gh`);
- the .NET Framework compiler required by `build.ps1`;
- a clean local clone of `chiginskiy/LiftPaper`.

If GitHub CLI is not authenticated, `release.ps1` starts the browser/device flow and shows a one-time code. Use `-Reauthenticate` to force a fresh device-code login.

## Validate without releasing

```powershell
.\build.ps1
.\test.ps1
```

## Publish a release

Pre-release:

```powershell
.\release.ps1 -Version 0.1.0 -Prerelease -Reauthenticate
```

Stable release:

```powershell
.\release.ps1 -Version 1.0.0 -Reauthenticate
```

The script starts a transcript in the user's Downloads directory, checks `main`/`origin/main`, validates the version and release notes, runs build/tests, verifies SHA-256, publishes `LiftPaper.exe`, `SHA256SUMS.txt`, `LICENSE`, and `NOTICE`, and reads the Release back from GitHub.

## Code signing

Current releases are unsigned.

When Authenticode signing is introduced, the release sequence must become:

`build → tests → sign → verify signature → generate SHA-256 → publish`

Signing changes the executable bytes, so `SHA256SUMS.txt` must always be generated **after** the final signature is applied.
