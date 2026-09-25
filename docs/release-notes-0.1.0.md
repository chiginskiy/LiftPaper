# LiftPaper 0.1.0 — первый предварительный выпуск

**Из скана — к чистой печати.**

Автор: **[Дмитрий Сергеевич Чигинский / Dmitriy Chiginskiy](https://chiginskiy.ru/)**. LiftPaper распространяется по Apache License 2.0.

LiftPaper 0.1.0 — локальная Windows-утилита для подготовки JPG/PNG-сканов к печати. Каждый входной файл преобразуется в отдельный PDF A4: нейтральный фон осветляется, тёмный нейтральный текст становится контрастнее, а выраженные цветные штрихи при включённой защите стараются сохраниться.

## Скачать

- `LiftPaper.exe` — один исполняемый файл; отдельная установка не требуется.
- `SHA256SUMS.txt` — контрольная сумма EXE.
- `LICENSE` — Apache License 2.0.
- `NOTICE` — уведомление об авторстве и происхождении проекта.
- Исходники и инструкции сборки находятся в репозитории и архиве исходников соответствующего тега.

## Как использовать

1. Откройте `LiftPaper.exe`.
2. Выберите JPG/PNG или перетащите файлы в окно.
3. Проверьте ширину очищаемых краёв и другие параметры.
4. Для каждого исходника рядом будет создан отдельный `имя – LiftPaper.pdf`.
5. Просмотрите результат.
6. Для ожидаемого размера страницы печатайте PDF как A4 в режиме **«Фактический размер / 100%»**.

Исходные изображения не изменяются.

## Важно: очищаемые края

По умолчанию полностью отбеливается полоса, соответствующая **8,5 мм по периметру страницы A4**.

**Все отметки внутри этой зоны удаляются из результата**, включая текст, подписи и печати. Если полезное содержимое расположено близко к краю, сначала откройте программу без файлов и установите `0` или меньшее значение.

## Цветные отметки

Защита цвета основана на порогах пикселей. Она помогает сохранить выраженные цветные штрихи за пределами очищаемых краёв, но не распознаёт подписи или печати. Бледные, серые и слабо окрашенные отметки могут изменяться.

Всегда проверяйте PDF перед отправкой или печатью и храните исходные файлы.

## Что входит в 0.1.0

- JPG/JPEG и PNG → отдельный PDF A4.
- Осветление нейтрального фона и усиление контраста нейтрального текста.
- Опциональная защита выраженных цветных штрихов.
- Полное отбеливание настраиваемых краевых полос.
- Учёт EXIF-ориентации.
- Книжная или альбомная A4-страница в зависимости от изображения.
- Сжатие без потерь по умолчанию или компактный JPEG 95.
- Пакетная обработка.
- Полностью локальная работа без сетевых запросов, телеметрии и автоматического обновления.

## Ограничения

Версия 0.1.0 не выполняет OCR, deskew, исправление перспективы, импорт PDF, объединение страниц и электронное подписание. Фактический размер исходной бумаги не определяется: любой вход вписывается в A4.

Профиль предназначен для светлой бумаги с тёмным текстом, а не для фотографий, цветных иллюстраций, чертежей или документов, где критична точная цветопередача.

Максимальный размер входного изображения — 100 млн пикселей.

## Система

- Windows 10/11 — целевые версии.
- .NET Framework 4.5 или новее.
- Интерфейс 0.1.0 — русский.
- Бинарник пока не подписан сертификатом издателя.

Windows Defender SmartScreen может показать предупреждение о неизвестном приложении. Не отключайте SmartScreen: скачивайте `LiftPaper.exe` только из официального Release и при необходимости сверяйте SHA-256 из `SHA256SUMS.txt`.


---

## English

**From scan to clean print.**

Author: **[Dmitriy Chiginskiy / Дмитрий Сергеевич Чигинский](https://chiginskiy.ru/)**. LiftPaper is licensed under the Apache License 2.0.

LiftPaper 0.1.0 is a local Windows utility for preparing scanned JPG/PNG pages for printing. Each input image becomes a separate A4 PDF: neutral paper is brightened, dark neutral text gains contrast, and pronounced coloured marks can be preserved when colour protection is enabled.

### How to use it

1. Open `LiftPaper.exe`.
2. Select JPG/PNG files or drag them into the window.
3. Review the cleaned-edge width and other settings.
4. LiftPaper creates a separate `name – LiftPaper.pdf` beside each source image.
5. Review the result.
6. For the intended page size, print the PDF as A4 using **Actual size / 100%**.

Source images are never modified.

### Important: cleaned edges

By default, LiftPaper completely whitens a band corresponding to **8.5 mm around the A4 output page**.

**Everything inside this band is removed from the output**, including text, signatures, stamps, and other marks. If important content is close to an edge, open the application without input files first and set the value to `0` or a smaller width.

Colour protection is pixel-based. It can preserve pronounced coloured marks outside the cleaned edge bands, but it does not recognise signatures or stamps. Faint, grey, or weakly coloured marks can change.

Always review the PDF before sharing or printing it, and keep the source files.

### Included in 0.1.0

- JPG/JPEG and PNG to separate A4 PDFs.
- Neutral-paper brightening and stronger neutral-text contrast.
- Optional preservation of pronounced coloured marks.
- Configurable complete whitening of edge bands.
- EXIF orientation handling.
- Portrait or landscape A4 placement according to the image.
- Lossless compression by default or optional JPEG quality 95 compact mode.
- Batch processing.
- Fully local operation with no network requests, telemetry, ads, or automatic updater.

Version 0.1.0 does not provide OCR, deskew, perspective correction, PDF input, page merging, or digital signing. The physical size of the source sheet is not detected; every input is fitted to A4.

Windows 10/11 are the target platforms. .NET Framework 4.5 or newer is required. The 0.1.0 user interface is in Russian. The executable is not yet publisher-code-signed.

Windows Defender SmartScreen may warn that the application is unrecognized. Do not disable SmartScreen: download `LiftPaper.exe` only from the official Release and verify its SHA-256 against `SHA256SUMS.txt` when appropriate.
