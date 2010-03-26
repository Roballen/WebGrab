using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Utilities;

namespace ConsoleGrabit
{
    public class PDFToTiff
    {
        public static bool ConvertPdfToTiff(string pdffile, string savelocation)
        {
            try
            {
                var p =Process.Start(Path.Combine(Properties.Settings.Default.pdftotiff, "pdftotiff.exe"),
                              "-i \"" + pdffile + "\" -o \"" + savelocation + "\" -c lzw") ;// + " -r PFUFALHDHZOQBTXF");

                while (p!= null && !p.HasExited)
                {
                    Thread.Sleep(100);
                }
                
                FileTools.WaitForFile(Path.ChangeExtension(pdffile,".tif"),20);
                try
                {
                    if (p != null)
                        p.Kill();
                }
                catch (Exception)
                {
                }
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return false;
        }
    }
}
