# LiftPaper

Offline scan cleanup and A4 PDF export in a single Windows executable.

[Русский](README.md) · [Releases](https://github.com/chiginskiy/LiftPaper/releases)

**License status:** all rights reserved; no open-source license has been selected. Public visibility alone does not permit unrestricted use or redistribution. See [LICENSE](LICENSE).

## Use

Download `LiftPaper.exe` from Releases. Open it to select JPG/PNG files, or drag files onto its icon/window. Each image produces a separate `name — чистый.pdf` beside the original. Existing results receive numbered names and are never overwritten.

**The default 8.5 mm border is completely whitened, including any writing inside it.** Open the app first and set the border to zero or a smaller value if there is important content near the edges. Dropping files on the executable starts processing with default settings.

Windows with .NET Framework 4.5 or newer is required; Windows 10/11 are the target platforms. No Python, GIMP, account, installer, or network connection is needed. The initial user interface is in Russian.

The tool brightens neutral paper, deepens neutral text, optionally preserves pronounced coloured pixels outside the border, respects EXIF orientation, and composites PNG transparency onto white. It fits the image proportionally onto portrait or landscape A4 without resampling. Default PDF compression is lossless; the compact option performs one JPEG encoding at quality 95.

Review every output before sharing or printing. Colour protection is a pixel heuristic, not signature recognition: faint/grey strokes can change. There is no OCR, deskew, PDF input, page merging, or digital signing. A3 is also fitted onto A4; original paper size is not detected. This preset is intended for dark text on light paper, not photos or colour drawings. Keep originals. The maximum input size is 100 million pixels; large images require substantial memory.

## Privacy and licensing

The app has no network requests, telemetry, ads or auto-updater. Cloud folder clients can still sync files independently. Never upload private scans or signatures to public issues.

Maintained by **chiginskiy**. Copyright (c) 2026 chiginskiy. No open-source license has been selected. Permission for use and redistribution must be obtained separately, except as provided by applicable law or platform terms. See `LICENSE`; the notice is also embedded in the executable's About dialog. It does not claim ownership of, or impose licensing terms on, your documents.

Only system Windows/.NET components are required; no third-party libraries are bundled. See [THIRD_PARTY.md](THIRD_PARTY.md).

Version 0.1.0 is an initial pre-release. Executables are not publisher-code-signed. SHA-256 checksums detect altered downloads when checked against a trusted release, but are not publisher signatures.

## Build and test

Run `./build.ps1`, then `./test.ps1` in Windows PowerShell. The build uses the .NET Framework C# compiler and does not download packages. Outputs are in `dist`. The scripts do not change system execution policies.

`LiftPaper.exe --batch <files...>` processes files without a GUI using defaults. Exit status is 0 for success, 1 for one or more file errors, or 2 for an empty batch. A failed input does not prevent subsequent inputs from being processed.

See [CONTRIBUTING.md](CONTRIBUTING.md) and [SECURITY.md](SECURITY.md) for contributions and reporting.
