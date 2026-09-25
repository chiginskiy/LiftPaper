// Copyright (c) 2026 chiginskiy. All rights reserved.
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

internal static class Tests
{
    private static int assertions;
    private static void Check(bool condition, string message) { assertions++; if (!condition) throw new Exception(message); }
    private static byte[] Hash(string path) { using (var sha = SHA256.Create()) return sha.ComputeHash(File.ReadAllBytes(path)); }

    public static int Main()
    {
        string dir = Path.Combine(Path.GetTempPath(), "LiftPaper-test-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(dir);
        try
        {
            string input = Path.Combine(dir, "Unicode-тест & spaces.png");
            using (var bitmap = new Bitmap(248, 350))
            using (var graphics = Graphics.FromImage(bitmap))
            {
                graphics.Clear(Color.FromArgb(250, 250, 250));
                graphics.FillRectangle(Brushes.Black, 40, 40, 50, 30);
                using (var ink = new SolidBrush(Color.FromArgb(120, 140, 210))) graphics.FillRectangle(ink, 60, 190, 60, 30);
                graphics.FillRectangle(Brushes.Black, 0, 0, 3, 3);
                bitmap.Save(input, ImageFormat.Png);
            }
            byte[] before = Hash(input);
            string first = Processor.Process(input, new Options());
            Check(Path.GetFileName(first) == "Unicode-тест & spaces – LiftPaper.pdf", "Output filename must use the locale-neutral LiftPaper suffix");
            byte[] firstHash = Hash(first);
            string second = Processor.Process(input, new Options());
            Check(Path.GetFileName(second) == "Unicode-тест & spaces – LiftPaper (2).pdf", "Collision numbering must follow the LiftPaper suffix");
            Check(first != second, "Existing PDFs must not be overwritten");
            Check(Hash(first).SequenceEqual(firstHash), "First output changed");
            Check(Hash(input).SequenceEqual(before), "Original changed");
            string brandedInput = Path.Combine(dir, "already LiftPaper.png");
            File.Copy(input, brandedInput);
            string branded = Processor.Process(brandedInput, new Options());
            Check(Path.GetFileName(branded) == "already LiftPaper – LiftPaper.pdf", "Source names containing LiftPaper must remain valid");
            byte[] pdf = File.ReadAllBytes(first);
            string ascii = Encoding.ASCII.GetString(pdf);
            Check(ascii.StartsWith("%PDF-1.4"), "PDF header");
            Check(ascii.Contains("595.275591 841.889764"), "Exact portrait A4 dimensions");
            Check(ascii.Contains("/FlateDecode") && !ascii.Contains("/DCTDecode"), "Default PDF is lossless");
            byte[] rgb = DecodeImage(pdf, 248, 350);
            Check(Pixel(rgb,248,1,1).SequenceEqual(new byte[]{255,255,255}), "Borders must be white");
            Check(Pixel(rgb,248,50,50).SequenceEqual(new byte[]{0,0,0}), "Dark text must remain dark");
            Check(Pixel(rgb,248,70,200).SequenceEqual(new byte[]{120,140,210}), "Colour ink must remain unchanged");
            Check(Pixel(rgb,248,120,120).SequenceEqual(new byte[]{255,255,255}), "Paper must become white");
            string noEdge = Processor.Process(input, new Options{EdgeMm=0});
            Check(Pixel(DecodeImage(File.ReadAllBytes(noEdge),248,350),248,1,1).SequenceEqual(new byte[]{0,0,0}), "Zero edge must preserve marks");
            string compact = Processor.Process(input,new Options{Compact=true});
            Check(Encoding.ASCII.GetString(File.ReadAllBytes(compact)).Contains("/DCTDecode"), "Compact JPEG filter");
            string transparent = Path.Combine(dir,"transparent.png");
            using(var bitmap = new Bitmap(300,150,PixelFormat.Format32bppArgb)) bitmap.Save(transparent,ImageFormat.Png);
            string alphaPdf = Processor.Process(transparent,new Options());
            Check(DecodeImage(File.ReadAllBytes(alphaPdf),300,150).All(x=>x==255), "Transparency composites to white");
            Check(Encoding.ASCII.GetString(File.ReadAllBytes(alphaPdf)).Contains("841.889764 595.275591"), "Landscape A4");
            string invalid = Path.Combine(dir,"invalid.jpg"); File.WriteAllText(invalid,"not an image");
            bool rejected=false;try{Processor.Process(invalid,new Options());}catch{rejected=true;}
            Check(rejected, "Invalid input must be rejected");
            Check(!File.Exists(Path.Combine(dir,"invalid – LiftPaper.pdf")), "Invalid input must not leave a PDF");
            Check(typeof(Processor).Assembly.GetReferencedAssemblies().All(x=>new[]{"mscorlib","System","System.Core","System.Drawing","System.Windows.Forms"}.Contains(x.Name)), "No bundled external dependencies");
            Console.WriteLine("PASS: " + assertions + " assertions; synthetic images only.");
            return 0;
        }
        catch(Exception ex) { Console.Error.WriteLine(ex); return 1; }
        finally { foreach(string file in Directory.GetFiles(dir)) File.Delete(file); Directory.Delete(dir); }
    }

    private static byte[] Pixel(byte[] rgb,int width,int x,int y){return rgb.Skip((y*width+x)*3).Take(3).ToArray();}

    // Independently read the PDF image's Flate/PNG predictor stream and verify
    // that its emitted pixels, checksum and PDF offsets are valid.
    private static byte[] DecodeImage(byte[] pdf,int width,int height)
    {
        string ascii=Encoding.ASCII.GetString(pdf);
        int obj=ascii.IndexOf("4 0 obj\n",StringComparison.Ordinal);
        int begin=ascii.IndexOf("stream\n",obj,StringComparison.Ordinal)+7;
        string header=ascii.Substring(obj,begin-obj);
        int length=int.Parse(Regex.Match(header,@"/Length (\d+)").Groups[1].Value);
        Check(pdf[begin]==0x78,"Zlib header");
        byte[] filtered;
        using(var compressed=new MemoryStream(pdf,begin+2,length-6))
        using(var deflate=new DeflateStream(compressed,CompressionMode.Decompress))
        using(var raw=new MemoryStream()){deflate.CopyTo(raw);filtered=raw.ToArray();}
        Check(filtered.Length==(width*3+1)*height,"Decoded dimensions");
        uint a=1,b=0;foreach(byte value in filtered){a=(a+value)%65521;b=(b+a)%65521;}
        uint expected=(b<<16)|a;
        uint actual=0;for(int i=begin+length-4;i<begin+length;i++)actual=(actual<<8)|pdf[i];
        Check(expected==actual,"Adler32 checksum");
        byte[] rgb=new byte[width*height*3];
        for(int y=0;y<height;y++)
        {
            int row=y*(width*3+1),dst=y*width*3;Check(filtered[row]==1,"PNG Sub predictor");
            for(int x=0;x<width*3;x++)rgb[dst+x]=unchecked((byte)(filtered[row+x+1]+(x>=3?rgb[dst+x-3]:0)));
        }
        long xref=long.Parse(Regex.Match(ascii,@"startxref\n(\d+)").Groups[1].Value);
        Check(ascii.Substring((int)xref).StartsWith("xref\n"),"Cross-reference offset");
        string[] entries=ascii.Substring((int)xref).Split('\n');
        for(int i=1;i<=5;i++){int offset=int.Parse(entries[i+2].Substring(0,10));Check(ascii.Substring(offset).StartsWith(i+" 0 obj\n"),"Object offset");}
        return rgb;
    }
}
