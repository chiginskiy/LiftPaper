// Copyright (c) 2026 chiginskiy. All rights reserved.
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

// Build with the Windows .NET Framework C# compiler. No third-party packages.
internal static class Program
{
    [STAThread]
    private static int Main(string[] args)
    {
        if (args.Contains("--batch"))
        {
            if (args.Length == 1) return 2;
            int failures = 0;
            foreach (string path in args.Where(x => x != "--batch"))
                try { Processor.Process(path, new Options()); }
                catch (Exception ex) { Console.Error.WriteLine(ex.Message); failures++; }
            return failures == 0 ? 0 : 1;
        }
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new MainForm(args));
        return 0;
    }
}

internal sealed class Options
{
    public double EdgeMm = 8.5;
    public bool ProtectInk = true;
    public bool Compact = false;
}

internal sealed class MainForm : Form
{
    private readonly NumericUpDown edges = new NumericUpDown();
    private readonly CheckBox protect = new CheckBox();
    private readonly CheckBox compact = new CheckBox();
    private readonly Button choose = new Button();
    private readonly Label status = new Label();
    private readonly TextBox results = new TextBox();
    private readonly Button open = new Button();
    private string lastOutput;
    private bool busy;

    public MainForm(string[] initialFiles)
    {
        Text = "LiftPaper " + AppInfo.Version;
        Font = new Font("Segoe UI", 10);
        ClientSize = new Size(670, 505);
        MinimumSize = new Size(686, 544);
        StartPosition = FormStartPosition.CenterScreen;
        AutoScaleMode = AutoScaleMode.Dpi;
        AllowDrop = true;
        var title = new Label { Text = "Из скана — в PDF для печати", Font = new Font("Segoe UI", 17, FontStyle.Bold), AutoSize = true, Location = new Point(22, 18) };
        var hint = new Label { Text = "Выберите JPG/PNG или перетащите файлы в окно.", AutoSize = true, Location = new Point(24, 62) };
        var edgeLabel = new Label { Text = "Очистить края до белого, мм:", AutoSize = true, Location = new Point(24, 106) };
        edges.Location = new Point(278, 102); edges.Size = new Size(80, 28);
        edges.DecimalPlaces = 1; edges.Increment = 0.5m; edges.Minimum = 0; edges.Maximum = 15; edges.Value = 8.5m;
        var edgeNote = new Label { Text = "Полоса считается по A4; все отметки внутри неё удаляются из результата.", AutoSize = true, Location = new Point(24, 139), ForeColor = Color.FromArgb(85, 85, 85) };
        protect.Text = "Сохранять выраженные цветные штрихи (подписи, печати)";
        protect.Checked = true; protect.AutoSize = true; protect.Location = new Point(24, 173);
        compact.Text = "Уменьшить размер PDF (JPEG 95; иначе — без потерь)";
        compact.AutoSize = true; compact.Location = new Point(24, 206);
        choose.Text = "Выбрать JPG/PNG…"; choose.Location = new Point(24, 245); choose.Size = new Size(175, 36);
        choose.Click += async delegate {
            using (var dialog = new OpenFileDialog { Filter = "Изображения JPG и PNG|*.jpg;*.jpeg;*.png", Multiselect = true, Title = "Выберите исходные изображения" })
                if (dialog.ShowDialog(this) == DialogResult.OK) await RunFiles(dialog.FileNames);
        };
        open.Text = "Открыть PDF"; open.Location = new Point(213, 245); open.Size = new Size(145, 36); open.Enabled = false;
        open.Click += delegate { if (lastOutput != null) System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(lastOutput) { UseShellExecute = true }); };
        var about = new Button { Text = "О программе", Location = new Point(372, 245), Size = new Size(140, 36) };
        about.Click += delegate { AppInfo.ShowAbout(this); };
        status.Text = "Исходники не изменяются. Для каждого файла создаётся отдельный PDF A4 рядом с ним.";
        status.AutoSize = true; status.Location = new Point(24, 299);
        results.Location = new Point(24, 328); results.Size = new Size(622, 115);
        results.Multiline = true; results.ReadOnly = true; results.ScrollBars = ScrollBars.Vertical;
        results.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        var copyright = new Label { Text = AppInfo.Copyright, AutoSize = true, Location = new Point(24, 473), Anchor = AnchorStyles.Bottom | AnchorStyles.Left };
        Controls.AddRange(new Control[] { title, hint, edgeLabel, edges, edgeNote, protect, compact, choose, open, about, status, results, copyright });
        DragEnter += delegate(object sender, DragEventArgs e) { if (!busy && e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy; };
        DragDrop += async delegate(object sender, DragEventArgs e) { if (!busy) await RunFiles((string[])e.Data.GetData(DataFormats.FileDrop)); };
        Shown += async delegate { if (initialFiles.Length > 0) await RunFiles(initialFiles); };
        FormClosing += delegate(object sender, FormClosingEventArgs e) { if (busy) e.Cancel = true; };
    }

    private async Task RunFiles(string[] files)
    {
        if (busy) return;
        busy = true; choose.Enabled = edges.Enabled = protect.Enabled = compact.Enabled = open.Enabled = false;
        var opts = new Options { EdgeMm = (double)edges.Value, ProtectInk = protect.Checked, Compact = compact.Checked };
        int passed = 0, failed = 0;
        try
        {
            for (int i = 0; i < files.Length; i++)
            {
                string path = files[i]; status.Text = "Обработка " + (i + 1) + " из " + files.Length + "…";
                try
                {
                    string output = await Task.Run(() => Processor.Process(path, opts));
                    lastOutput = output; passed++;
                    results.AppendText("Готово: " + output + Environment.NewLine);
                }
                catch (Exception ex) { failed++; results.AppendText("Ошибка: " + Path.GetFileName(path) + " — " + ex.Message + Environment.NewLine); }
            }
        }
        finally
        {
            busy = false; choose.Enabled = edges.Enabled = protect.Enabled = compact.Enabled = true;
            open.Enabled = lastOutput != null;
            status.Text = "Готово: " + passed + ". Ошибок: " + failed + ". Печатайте как A4 в фактическом размере (100%).";
        }
    }
}

internal static class Processor
{
    // Empirical document tone curve. It deepens dark
    // text and clips paper highlights, while the colour-ink mask bypasses it.
    private static readonly int[] CurveX = { 0, 45, 60, 80, 100, 120, 160, 200, 220, 238, 255 };
    private static readonly int[] CurveY = { 0, 0, 15, 58, 90, 117, 169, 218, 243, 255, 255 };
    private static readonly byte[] Lut = MakeLut();

    private static byte[] MakeLut()
    {
        byte[] lut = new byte[256]; int segment = 0;
        for (int i = 0; i < 256; i++)
        {
            while (segment < CurveX.Length - 2 && i > CurveX[segment + 1]) segment++;
            lut[i] = (byte)Math.Round(CurveY[segment] + (i - CurveX[segment]) * (double)(CurveY[segment + 1] - CurveY[segment]) / (CurveX[segment + 1] - CurveX[segment]));
        }
        return lut;
    }

    public static string Process(string source, Options opts)
    {
        source = Path.GetFullPath(source);
        string ext = Path.GetExtension(source).ToLowerInvariant();
        if (ext != ".jpg" && ext != ".jpeg" && ext != ".png") throw new Exception("Поддерживаются JPG и PNG.");
        if (!File.Exists(source)) throw new Exception("Файл не найден.");
        if (opts.EdgeMm < 0 || opts.EdgeMm > 15) throw new Exception("Ширина полей должна быть от 0 до 15 мм.");
        using (var input = Image.FromFile(source, true))
        {
            if ((long)input.Width * input.Height > 100000000) throw new Exception("Слишком большое изображение: предел 100 млн пикселей.");
            if (input.FrameDimensionsList.Length > 0 && input.GetFrameCount(new FrameDimension(input.FrameDimensionsList[0])) > 1)
                throw new Exception("Используйте обычное одностраничное изображение.");
            if (input.PropertyIdList.Contains(0x112))
            {
                int orientation = BitConverter.ToUInt16(input.GetPropertyItem(0x112).Value, 0);
                RotateFlipType[] rotations = { RotateFlipType.RotateNoneFlipNone, RotateFlipType.RotateNoneFlipNone, RotateFlipType.RotateNoneFlipX, RotateFlipType.Rotate180FlipNone, RotateFlipType.Rotate180FlipX, RotateFlipType.Rotate90FlipX, RotateFlipType.Rotate90FlipNone, RotateFlipType.Rotate270FlipX, RotateFlipType.Rotate270FlipNone };
                if (orientation >= 1 && orientation <= 8) input.RotateFlip(rotations[orientation]);
            }
            using (var bitmap = new Bitmap(input.Width, input.Height, PixelFormat.Format24bppRgb))
            {
                using (Graphics g = Graphics.FromImage(bitmap)) { g.Clear(Color.White); g.DrawImage(input, new Rectangle(0, 0, bitmap.Width, bitmap.Height), 0, 0, input.Width, input.Height, GraphicsUnit.Pixel); }
                byte[] rgb = Clean(bitmap, opts);
                byte[] data;
                if (opts.Compact)
                {
                    PutRgb(bitmap, rgb);
                    using (var buffer = new MemoryStream())
                    using (var parameters = new EncoderParameters(1))
                    {
                        parameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 95L);
                        bitmap.Save(buffer, ImageCodecInfo.GetImageEncoders().First(x => x.MimeType == "image/jpeg"), parameters);
                        data = buffer.ToArray();
                    }
                }
                else data = CompressRgb(rgb, bitmap.Width, bitmap.Height);
                return SavePdf(source, bitmap.Width, bitmap.Height, data, opts.Compact);
            }
        }
    }

    private static byte[] Clean(Bitmap bitmap, Options opts)
    {
        int width = bitmap.Width, height = bitmap.Height;
        double pw = width > height ? 297 : 210, ph = width > height ? 210 : 297;
        int edge = (int)Math.Round(opts.EdgeMm / Math.Min(pw / width, ph / height));
        byte[] rgb = new byte[checked(width * height * 3)];
        var locked = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
        try
        {
            byte[] row = new byte[Math.Abs(locked.Stride)];
            for (int y = 0; y < height; y++)
            {
                Marshal.Copy(IntPtr.Add(locked.Scan0, y * locked.Stride), row, 0, row.Length);
                for (int x = 0; x < width; x++)
                {
                    int k = x * 3, dst = (y * width + x) * 3;
                    int r = row[k + 2], g = row[k + 1], b = row[k];
                    if (x < edge || y < edge || x >= width - edge || y >= height - edge)
                        r = g = b = 255;
                    else
                    {
                        int max = Math.Max(r, Math.Max(g, b)), min = Math.Min(r, Math.Min(g, b));
                        bool blue = b - r > 15 && b - g > 8;
                        bool red = r - g > 15 && r - b > 8;
                        bool green = g - r > 15 && g - b > 8;
                        bool ink = opts.ProtectInk && (blue || red || green || max - min > 35);
                        if (!ink)
                        {
                            // Neutral paper and text are made truly neutral, so scanner
                            // colour fringes do not survive as dirty paper or coloured text.
                            int grey = (299 * r + 587 * g + 114 * b + 500) / 1000;
                            r = g = b = Lut[grey];
                        }
                    }
                    rgb[dst] = (byte)r; rgb[dst + 1] = (byte)g; rgb[dst + 2] = (byte)b;
                }
            }
        }
        finally { bitmap.UnlockBits(locked); }
        return rgb;
    }

    private static void PutRgb(Bitmap bitmap, byte[] rgb)
    {
        var locked = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
        try
        {
            byte[] row = new byte[Math.Abs(locked.Stride)];
            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    int src = (y * bitmap.Width + x) * 3, dst = x * 3;
                    row[dst] = rgb[src + 2]; row[dst + 1] = rgb[src + 1]; row[dst + 2] = rgb[src];
                }
                Marshal.Copy(row, 0, IntPtr.Add(locked.Scan0, y * locked.Stride), row.Length);
            }
        }
        finally { bitmap.UnlockBits(locked); }
    }

    private static byte[] CompressRgb(byte[] rgb, int width, int height)
    {
        using (var output = new MemoryStream())
        {
            // PDF Flate requires a zlib header and Adler32 around raw Deflate.
            output.WriteByte(0x78); output.WriteByte(0x9C);
            uint a = 1, b = 0;
            using (var deflate = new DeflateStream(output, CompressionLevel.Optimal, true))
            {
                byte[] row = new byte[width * 3 + 1];
                for (int y = 0; y < height; y++)
                {
                    row[0] = 1; // PNG Sub predictor, /Predictor 15 in the PDF.
                    int start = y * width * 3;
                    for (int x = 0; x < width * 3; x++) row[x + 1] = unchecked((byte)(rgb[start + x] - (x >= 3 ? rgb[start + x - 3] : 0)));
                    for (int i = 0; i < row.Length; i++) { a = (a + row[i]) % 65521; b = (b + a) % 65521; }
                    deflate.Write(row, 0, row.Length);
                }
            }
            uint checksum = (b << 16) | a;
            for (int shift = 24; shift >= 0; shift -= 8) output.WriteByte((byte)(checksum >> shift));
            return output.ToArray();
        }
    }

    private static string F(double value) { return value.ToString("0.######", CultureInfo.InvariantCulture); }
    private static void Text(Stream stream, string text) { byte[] bytes = Encoding.ASCII.GetBytes(text); stream.Write(bytes, 0, bytes.Length); }

    private static string SavePdf(string source, int width, int height, byte[] data, bool jpeg)
    {
        string stem = Path.Combine(Path.GetDirectoryName(source), Path.GetFileNameWithoutExtension(source) + " — чистый");
        string target = null; FileStream file = null;
        for (int i = 1; i < 10000; i++)
        {
            string candidate = stem + (i == 1 ? "" : " (" + i + ")") + ".pdf";
            try { file = new FileStream(candidate, FileMode.CreateNew, FileAccess.Write, FileShare.None); target = candidate; break; }
            catch (IOException) { if (!File.Exists(candidate)) throw; }
        }
        if (file == null) throw new Exception("Слишком много файлов с одинаковыми именами.");
        try
        {
            using (file)
            {
                double pw = (width > height ? 297 : 210) * 72.0 / 25.4, ph = (width > height ? 210 : 297) * 72.0 / 25.4;
                double scale = Math.Min(pw / width, ph / height), w = width * scale, h = height * scale;
                long[] offsets = new long[6];
                Text(file, "%PDF-1.4\n%"); file.Write(new byte[] { 0xE2, 0xE3, 0xCF, 0xD3, 10 }, 0, 5);
                offsets[1] = file.Position; Text(file, "1 0 obj\n<< /Type /Catalog /Pages 2 0 R >>\nendobj\n");
                offsets[2] = file.Position; Text(file, "2 0 obj\n<< /Type /Pages /Kids [3 0 R] /Count 1 >>\nendobj\n");
                offsets[3] = file.Position; Text(file, "3 0 obj\n<< /Type /Page /Parent 2 0 R /MediaBox [0 0 " + F(pw) + " " + F(ph) + "] /Resources << /XObject << /Im0 4 0 R >> >> /Contents 5 0 R >>\nendobj\n");
                offsets[4] = file.Position;
                string filter = jpeg ? "/Filter /DCTDecode" : "/Filter /FlateDecode /DecodeParms << /Predictor 15 /Colors 3 /BitsPerComponent 8 /Columns " + width + " >>";
                Text(file, "4 0 obj\n<< /Type /XObject /Subtype /Image /Width " + width + " /Height " + height + " /ColorSpace /DeviceRGB /BitsPerComponent 8 /Interpolate false " + filter + " /Length " + data.Length + " >>\nstream\n");
                file.Write(data, 0, data.Length); Text(file, "\nendstream\nendobj\n");
                string content = "q\n" + F(w) + " 0 0 " + F(h) + " " + F((pw - w) / 2) + " " + F((ph - h) / 2) + " cm\n/Im0 Do\nQ\n";
                offsets[5] = file.Position; Text(file, "5 0 obj\n<< /Length " + Encoding.ASCII.GetByteCount(content) + " >>\nstream\n" + content + "endstream\nendobj\n");
                long xref = file.Position; Text(file, "xref\n0 6\n0000000000 65535 f \n");
                for (int i = 1; i <= 5; i++) Text(file, offsets[i].ToString("0000000000", CultureInfo.InvariantCulture) + " 00000 n \n");
                Text(file, "trailer\n<< /Size 6 /Root 1 0 R >>\nstartxref\n" + xref.ToString(CultureInfo.InvariantCulture) + "\n%%EOF\n");
            }
            return target;
        }
        catch { file.Dispose(); File.Delete(target); throw; }
    }
}
