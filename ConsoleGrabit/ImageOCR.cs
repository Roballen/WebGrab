using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ConsoleGrabit.Models;
using MODI;

namespace ConsoleGrabit
{
    public class MyWord
    {
        public string Text { get; set; }
        public int Line { get; set; }
    }

    public class ImageOCR
    {
        private string _filename;
        protected IList<string> _words = new List<string>();
        protected Dictionary<string, IList<string>> _lines = new Dictionary<string, IList<string>>();
        private Document _modi;

        public IList<string> Words
        {
            get { return _words; }
            set { _words = value; }
        }

        public Dictionary<string, IList<string>> Lines
        {
            get { return _lines; }
            set { _lines = value; }
        }

        public ImageOCR(string filename)
        {
            _filename = filename;
            ConvertToTiffIfNeeded();
            _modi = new Document();

            try
            {
                _modi.Create(_filename);
                _modi.OCR(MiLANGUAGES.miLANG_SYSDEFAULT, true, true);
                GetWords();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                _modi.Close(false);
                _modi = null;
            }
        }

        private void GetLines(string page, string text)
        {
            var seps = new string[1]{Environment.NewLine};
            _lines.Add(page, text.Split(seps,StringSplitOptions.None));
        }

        /// <summary>
        /// Get the words and the lines, we may not need the words later
        /// </summary>
        public void GetWords()
        {
            foreach (IImage image in _modi.Images)
            {
                foreach (IWord word in image.Layout.Words)
                {
                    _words.Add(word.Text);
                }

                GetLines(image.GetHashCode().ToString(), image.Layout.Text );
            }
        }

        private void ConvertToTiffIfNeeded()
        {
            if (Path.GetExtension(_filename) == "pdf");
            {
                //save to same location with new extension
                var newfile = Path.ChangeExtension(_filename, "tif");
                PDFToTiff.ConvertPdfToTiff(_filename, Path.GetDirectoryName(_filename));
                _filename = newfile;
            }
        }
    }
}
