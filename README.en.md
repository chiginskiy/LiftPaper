# LiftPaper

**From scan to clean print.**

LiftPaper is a local Windows utility that turns scanned JPG/PNG pages into separate A4 PDFs prepared for printing. It brightens neutral paper, increases the contrast of neutral text, can preserve pronounced coloured marks, and places the result on an A4 page without modifying the source image.

[Русский](README.md) · [Releases](https://github.com/chiginskiy/LiftPaper/releases)

**Author:** [Dmitriy Chiginskiy / Дмитрий Сергеевич Чигинский](https://chiginskiy.ru/)  
**License:** [Apache License 2.0](LICENSE). LiftPaper may be used, studied, modified, and redistributed, including commercially, subject to the license terms and applicable attribution notices in [NOTICE](NOTICE).

## What LiftPaper is for

A scanned document may look acceptable on screen yet print with a grey paper background, scanner colour cast, or weak text contrast. LiftPaper has a deliberately narrow purpose: **prepare an existing image of a paper page for printing again**.

It is not a scanner, OCR system, or document-reconstruction tool. It processes the pixels of an existing scan and creates a new A4 PDF intended primarily for light paper with dark text.

Typical workflow:

**scanned JPG/PNG → LiftPaper → separate A4 PDF → review → print at 100%.**

## Quick start

1. Download `LiftPaper.exe` from **Releases**.
2. Open the application.
3. Select JPG/PNG files or drag them into the window.
4. Review the settings, especially the edge-cleaning width.
5. LiftPaper creates a separate `name – LiftPaper.pdf` beside each source image.
6. Review the output before sharing or printing it.
7. For the intended page size, print as A4 using **Actual size / 100%**.

Source JPG/PNG files are never overwritten. If an output filename already exists, LiftPaper adds a number and creates a new PDF.

### Dropping files onto the executable

Dropping images directly onto `LiftPaper.exe` starts processing immediately with the default settings.

If you need to change edge cleaning, colour-mark protection, or compression, open the application first, adjust the settings, and then add the scans.

## Settings

### Clean white edges

By default, LiftPaper completely whitens an edge band corresponding to **8.5 mm on the A4 output page**.

**Everything inside that band is removed from the output, including text, signatures, stamps, and other marks.**

If important content is close to an edge, set the value to `0` or to a smaller width before processing.

### Preserve coloured marks

When enabled, colour protection attempts to preserve strongly coloured pixels—such as blue or red handwritten marks or stamps—outside the cleaned edge bands.

This is a colour heuristic, not signature or stamp recognition. Faint, grey, or weakly coloured marks can still change. Always review the resulting PDF.

### PDF size

By default, image data is stored in the PDF losslessly.

The **Compact PDF** option performs one JPEG encode at quality 95. This usually reduces file size, but the image is no longer strictly lossless.

## What happens during processing

For each input image, LiftPaper:

- accepts JPG/JPEG or a regular PNG;
- respects EXIF orientation;
- composites PNG transparency onto white;
- neutralises nominally grey pixels, reducing scanner colour cast in paper and text;
- applies a tonal curve so light neutral paper becomes whiter and dark neutral text becomes stronger;
- when colour protection is enabled, leaves pronounced coloured pixels outside the cleaned edges out of that neutral processing;
- completely whitens the configured edge bands;
- preserves the image aspect ratio;
- automatically chooses portrait or landscape A4;
- places the source pixels on the PDF page without an additional raster resampling step;
- writes a separate PDF beside the source image.

## Best suited for

LiftPaper is intended primarily for:

- scans or photos of ordinary paper documents;
- white or light paper;
- dark text;
- documents that need to be printed again on A4;
- isolated coloured signatures, stamps, or marks when colour protection is enabled.

## Limitations

LiftPaper changes the document image. Keep the source files.

Version 0.1.0 does not:

- perform OCR;
- deskew pages;
- correct perspective;
- combine multiple images into one multi-page PDF;
- accept PDF as an input format;
- recognise signatures, stamps, or document types;
- digitally sign documents;
- detect the physical size of the original sheet.

Every supported input size is fitted to A4. For example, an A3 scan is also placed on an A4 page.

The processing profile is not intended for photographs, colour artwork, technical drawings, or documents where exact grey levels or colour fidelity matter.

The maximum input size is **100 million pixels**. Very large images can require substantial memory.

## Privacy

Processing happens locally on the computer.

LiftPaper contains no:

- document uploads;
- network requests;
- telemetry;
- advertising;
- automatic updater.

If source files are stored in a folder synchronised by OneDrive, Dropbox, or another cloud client, that client may upload them independently of LiftPaper.

Do not attach real contracts, signatures, financial records, personal data, or other confidential material to public GitHub Issues.

## Download trust and SmartScreen

Official LiftPaper binaries are published in **Releases** together with `SHA256SUMS.txt`. Early public builds are not yet publisher-code-signed, so Windows Defender SmartScreen may warn that the app is unrecognized.

Do not disable SmartScreen or other Windows security protections to install LiftPaper. Download the EXE only from the official repository and verify SHA-256 when appropriate. Authenticode signing is planned for official releases; once signing is introduced, checksums will be generated from the already signed binary.

## Requirements

- Windows 10/11 are the target platforms;
- .NET Framework 4.5 or newer;
- a single `LiftPaper.exe`;
- no Python, GIMP, extra packages, account, installer, or Internet connection is required.

The 0.1.0 user interface is in Russian.

## Build and test

From the project directory in Windows PowerShell:

```powershell
.\build.ps1
.\test.ps1
```

The build uses the system .NET Framework C# compiler and downloads no packages. The EXE and checksum are written to `dist`. The scripts do not modify the system PowerShell execution policy.

Headless batch processing with default settings:

```powershell
$p = Start-Process .\LiftPaper.exe -ArgumentList '--batch', '"C:\Scans\page 1.png"' -Wait -PassThru
$p.ExitCode
```

Exit codes:

- `0` — all files were processed;
- `1` — at least one file failed;
- `2` — batch mode was started without input files.

A failed input does not stop subsequent files from being processed.

## Copyright and licensing

The original author and maintainer is **[Dmitriy Chiginskiy (Дмитрий Сергеевич Чигинский)](https://chiginskiy.ru/)**.

Copyright © 2026 Dmitriy Chiginskiy.

LiftPaper is licensed under the **Apache License 2.0**. See [LICENSE](LICENSE), [NOTICE](NOTICE), and [AUTHORS.md](AUTHORS.md). The attribution notice and license text are also embedded in the executable and available from the About dialog.

Apache-2.0 does not transfer ownership of your source documents or generated PDFs to the developer and does not impose licensing terms on user documents.

Only system Windows/.NET components are required; no third-party libraries are bundled. See [THIRD_PARTY.md](THIRD_PARTY.md).

Version 0.1.0 is a pre-release. Public executables are not yet publisher-code-signed.

## Contributing

Use Issues for bugs and suggestions. See [CONTRIBUTING.md](CONTRIBUTING.md), [SECURITY.md](SECURITY.md), and [CHANGELOG.md](CHANGELOG.md).
