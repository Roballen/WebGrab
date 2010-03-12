using System;
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
        private Stream _filestream;

        public IList<MyWord> Words
        {
            get { return _words; }
            set { _words = value; }
        }

        private IList<MyWord> _words = new List<MyWord>();



        private Document _modi;

        public ImageOCR(string filename)
        {
            _modi = new Document();
            _filename = filename;

            var lead = new Lead();

            _modi.Create(_filename);
            _modi.OCR(MiLANGUAGES.miLANG_SYSDEFAULT, true, true);

        }

        /// <summary>
        /// Right now, just getting all the words
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public void GetWords()
        {
            foreach (IImage image in _modi.Images)
            {
                

                foreach (IWord word in image.Layout.Words)
                {
                    _words.Add(new MyWord(){Text = word.Text, Line = word.LineId});
                }
            }
        }

        private void ConvertToTiffIfNeeded(string filename)
        {
            if (Path.GetExtension(filename) == "pdf");
            {
                //save to same location with new extension
                var newfile = Path.ChangeExtension(filename, "tif");
                PDFToTiff.ConvertPdfToTiff(filename, newfile);
                _filename = newfile;
            }
        }

//        public bool MatchAddress()
//        {
//
//            int resipos = _words["RESIDENCE"];
//
//            var addressPattern = new Regex(@"(?<city>[A-Za-z',.\s]+) (?<state>([A-Za-z]{2}|[A-Za-z]{2},))\s*(?<zip>\d{5}(-\d{4})|\d{5})");
//
//            var search = new MiDocSearch();
//            search.Initialize(_modi, addressPattern.ToString(), 1, );
//
//
//        }


    }
}
