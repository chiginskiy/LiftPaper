# Contributing

Please describe the input type, app version, Windows version, expected result and actual result. Use synthetic or fully anonymised examples. Never post real signatures, financial records, customer documents or secrets.

## License of contributions

LiftPaper is licensed under the [Apache License 2.0](LICENSE).

Unless you explicitly state otherwise, a contribution intentionally submitted for inclusion in LiftPaper is provided under Apache-2.0, consistent with Section 5 of the license. Submit only material you have the right to contribute.

Preserve applicable copyright and attribution notices. Do not remove or obscure [NOTICE](NOTICE). No copyright assignment is required merely to submit a contribution.

## Local validation

Validate release-bound changes locally on Windows from the repository root:

```powershell
.\build.ps1
.\test.ps1
```

Both commands must complete successfully before a release-bound change is committed. The current release process is local-first; GitHub-hosted Actions are not required to publish a release.

Before submitting a pull request:

1. Run `build.ps1` and `test.ps1` on Windows.
2. Preserve the single-executable, offline operation model unless a change is explicitly discussed.
3. Document any change that can erase marks, alter colour, change page geometry, or introduce network access.
4. Include focused regression tests for defects.
5. Keep user documents and real personal data out of tests and Issues.

The maintainer may reject changes that compromise document privacy, silently alter source files, or materially broaden the product scope without prior discussion.
