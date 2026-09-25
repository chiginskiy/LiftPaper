# Contributing

Please describe the input type, app version, Windows version, expected result and actual result. Use synthetic or fully anonymised examples. Never post real signatures, financial records, customer documents or secrets.

## Local validation

Validate release-bound changes locally on Windows from the repository root:

```powershell
.\build.ps1
.\test.ps1
```

Both commands must complete successfully before a release-bound change is committed. When hosted CI is unavailable, the local Windows build and synthetic regression tests are the release validation source of truth. Keep the working tree and validation log available until the corresponding release is published.

Before submitting a pull request:

1. Run `build.ps1` and `test.ps1` on Windows.
2. Preserve the single-executable, offline operation model.
3. Document any change that can erase marks, alter colour, change page geometry or introduce network access.
4. Include focused regression tests for defects.

Contribute only material you have the right to submit. No open-source license has been selected yet: discuss contribution licensing with the maintainer before submitting code. Existing copyright notices must be preserved. No copyright assignment is implied.

The maintainer may reject changes that compromise document privacy or silently alter source files.
