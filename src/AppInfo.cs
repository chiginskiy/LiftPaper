// Copyright 2026 Dmitriy Chiginskiy
// SPDX-License-Identifier: Apache-2.0
using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

[assembly: AssemblyTitle("LiftPaper")]
[assembly: AssemblyDescription("Prepare scanned JPG/PNG pages for clean A4 printing")]
[assembly: AssemblyProduct("LiftPaper")]
[assembly: AssemblyCompany("Dmitriy Chiginskiy")]
[assembly: AssemblyCopyright("Copyright © 2026 Dmitriy Chiginskiy")]
[assembly: AssemblyVersion("0.1.0.0")]
[assembly: AssemblyFileVersion("0.1.0.0")]

internal static class AppInfo
{
    public const string Version = "0.1.0";
    public const string Copyright = "Copyright © 2026 Dmitriy Chiginskiy";
    public const string Author = "Дмитрий Сергеевич Чигинский (Dmitriy Chiginskiy)";
    public const string Website = "https://chiginskiy.ru/";
    public const string Repository = "https://github.com/chiginskiy/LiftPaper";
    public const string LicenseName = "Apache License 2.0";

    public static void ShowAbout(IWin32Window owner)
    {
        using (var dialog = new Form { Text = "LiftPaper — сведения и лицензия", Size = new Size(750, 550), StartPosition = FormStartPosition.CenterParent, Font = new Font("Segoe UI", 10) })
        {
            string license;
            string notice;
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("LiftPaper.LICENSE"))
            using (var reader = new StreamReader(stream)) license = reader.ReadToEnd();
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("LiftPaper.NOTICE"))
            using (var reader = new StreamReader(stream)) notice = reader.ReadToEnd();

            var text = new TextBox { Multiline = true, ReadOnly = true, ScrollBars = ScrollBars.Vertical, Dock = DockStyle.Fill, Text =
                "LiftPaper " + Version + Environment.NewLine +
                "Автор: " + Author + Environment.NewLine +
                "Сайт: " + Website + Environment.NewLine +
                "Лицензия: " + LicenseName + Environment.NewLine +
                "Репозиторий: " + Repository + Environment.NewLine + Environment.NewLine +
                "Подготавливает JPG/PNG-сканы к печати и сохраняет отдельные PDF A4." + Environment.NewLine +
                "Исходные изображения не изменяются; обработка выполняется локально." + Environment.NewLine +
                "Проверяйте готовый PDF перед отправкой или печатью." + Environment.NewLine +
                "Нет загрузки документов, телеметрии или автоматического обновления." + Environment.NewLine + Environment.NewLine +
                "NOTICE" + Environment.NewLine +
                notice.Replace("\r\n", "\n").Replace("\n", Environment.NewLine) + Environment.NewLine +
                "LICENSE" + Environment.NewLine +
                license.Replace("\r\n", "\n").Replace("\n", Environment.NewLine) };
            dialog.Controls.Add(text);
            dialog.ShowDialog(owner);
        }
    }
}
