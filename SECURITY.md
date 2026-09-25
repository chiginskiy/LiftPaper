# Security and privacy

Treat unknown image files as untrusted input and keep Windows/.NET patched. LiftPaper relies on Windows image decoders. It performs no network requests and never intentionally changes source files.

## Reporting a vulnerability

Do not post real documents, signatures, secrets, or executable exploit payloads in a public Issue.

If GitHub **Private Vulnerability Reporting** is enabled for this repository, use **Report a vulnerability**. Otherwise, open an Issue containing only a high-level description and ask the maintainer for a private reporting channel before sharing sensitive technical details.

Only the latest release is maintained. There is no guaranteed response or remediation deadline.

## Release authenticity

Official binaries are distributed through the repository's **Releases** page with `SHA256SUMS.txt`.

Release binaries are currently unsigned. Windows Defender SmartScreen may therefore warn that LiftPaper is an unrecognized application. A checksum verifies file integrity against the published release but is not proof of publisher identity.

Do not disable SmartScreen or other Windows security protections to run LiftPaper. Publisher code signing is planned for future official releases.
