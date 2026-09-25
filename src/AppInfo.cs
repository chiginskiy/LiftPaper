// Copyright (c) 2026 chiginskiy. All rights reserved.
using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

[assembly: AssemblyTitle("LiftPaper")]
[assembly: AssemblyDescription("Prepare scanned JPG/PNG pages for clean A4 printing")]
[assembly: AssemblyProduct("LiftPaper")]
[assembly: AssemblyCompany("chiginskiy")]
[assembly: AssemblyCopyright("Copyright (c) 2026 chiginskiy. All rights reserved.")]
[assembly: AssemblyVersion("0.1.0.0")]
[assembly: AssemblyFileVersion("0.1.0.0")]

internal static class AppInfo
{
    public const string Version = "0.1.0";
    public const string Copyright = "Copyright (c) 2026 chiginskiy";
    public const string Repository = "https://github.com/chiginskiy/LiftPaper";

    public static void ShowAbout(IWin32Window owner)
    {
        using (var dialog = new Form { Text = "LiftPaper — сведения и лицензия", Size = new Size(750, 550), StartPosition = FormStartPosition.CenterParent, Font = new Font("Segoe UI", 10) })
        {
            string license;
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("LiftPaper.LICENSE"))
            using (var reader = new StreamReader(stream)) license = reader.ReadToEnd();
            var text = new TextBox { Multiline = true, ReadOnly = true, ScrollBars = ScrollBars.Vertical, Dock = DockStyle.Fill, Text =
                "LiftPaper " + Version + Environment.NewLine + Copyright + Environment.NewLine +
                "Подготавливает JPG/PNG-сканы к печати и сохраняет отдельные PDF A4." + Environment.NewLine +
                "Исходные изображения не изменяются; обработка выполняется локально." + Environment.NewLine +
                "Проверяйте готовый PDF перед отправкой или печатью." + Environment.NewLine +
                "Нет загрузки документов, телеметрии или автоматического обновления." + Environment.NewLine +
                "Исходники и условия распространения: " + Repository + Environment.NewLine + Environment.NewLine +
                license.Replace("\r\n", "\n").Replace("\n", Environment.NewLine) };
            dialog.Controls.Add(text);
            dialog.ShowDialog(owner);
        }
    }
}
