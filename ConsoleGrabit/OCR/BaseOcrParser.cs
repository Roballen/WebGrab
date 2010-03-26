using System;
using System.Collections.Generic;
using System.Linq;
using Utilities;
using ConsoleGrabit.Models;

namespace ConsoleGrabit.OCR
{
    public abstract class BaseOcrParser : ImageOCR
    {
        protected Lead _lead;
        protected Dictionary<string, int> _searches;
        protected IList<string> _pagelines;
        protected string[] _months = new[] { "january","february","march","april","may","june","july","august","september","october","november","december"};
        protected List<string> _noise = new List<string> { "judgment","creditor","debtor","warrant","county","state","section","law"};

        protected BaseOcrParser(string filename, string county) : base(filename)
        {
            _lead = new Lead();
            _lead.Address.County = county;
        }

        public Lead GetLeadFromOcr()
        {
            _pagelines = _lines.Last().Value;

            FindSearches();

                _lead.Address = GetAddress();
                GetPerson();
                GetAmount();
                GetRecordDate();

            return _lead;
        }
        /// <summary>
        /// Find all searches in one loop, no need to go through it multiple times
        /// </summary>
        /// <param name="lines"></param>
        public void FindSearches()
        {
            try
            {

                var searchlist = _searches.Keys.ToList();
                for (var i = 0; i < _pagelines.Count(); i++)
                {
                    foreach (var search in searchlist)
                    {
                        if (_pagelines[i].ToLower().Contains(search))
                            _searches[search] = i;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);   
            }
        }

        public static bool IsDate(string sdate)
        {
            var refn = new DateTime();
            return DateTime.TryParse(sdate, out refn);
        }

        protected string ReduceNoiseFromEnd(string text)
        {
            foreach (var variable in _noise)
            {
                if (text.ToLower().Contains(variable))
                    text = text.Substring(0, text.ToLower().IndexOf(variable));
            }
            return text;
        }

        protected string FixOcrnumbers(string text)
        {
            //handle the 4 problem
            var chars = new char[] { (char)39, (char)96, (char)180, (char)8216, (char)8217, (char)8218 };

                foreach (var c1 in chars)
                {
                    if (text.Contains(c1))
                    {
                        var i = text.IndexOf(c1);
                        var temp = text.Substring(i + 1, 1);
                        if (!temp.Replace("-", "").IsNumber())
                            temp = c1 + temp;
                        else
                            temp = c1.ToString();

                        text = text.FastReplace(temp, "4").Squeeze();
                    }
                }


            //handles the 4 problem
//            if (text.Contains("'"))
//            {
//                var i = text.IndexOf("'");
//                var temp = text.Substring(i + 1,1);
//                if (!temp.Replace("-","").IsNumber())
//                    temp = "'" + temp;
//                else
//                    temp = "'";
//
//                text = text.FastReplace(temp, "4").Squeeze();
//            }
            return text;
        }

        protected string ParseOutDebt(string amount)
        {

            foreach (var va in amount.Split(' '))
            {

                if (va.FastReplace("$", "").FastReplace(",", "").FastReplace(".","").IsNumber())
                {
                    return va;
                }
            }

            return "";
        }

        protected virtual void GetPerson(){}
        protected virtual void GetAmount() { }
        protected virtual Address GetAddress() { return null; }
        protected virtual void GetRecordDate() { }

    }
}
