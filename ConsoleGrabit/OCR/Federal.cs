using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConsoleGrabit.Models;
using Utilities;

namespace ConsoleGrabit.OCR
{
    public class Federal : BaseOcrParser
    {
        public Federal(string filename, string county) : base(filename, county)
        {
            _searches = new Dictionary<string, int> { { "taxpayer", 0 }, { "residence", 0 }, { "total", 0 }, { "day of", 0 } };
        }

        protected override Address GetAddress()
        {
            var addr = new Address();
            var street1 = "";
            var street2 = "";

            var position = _searches["residence"];

            if (position == 0)
                throw new ArgumentException("Unable to find an address");

            street1 = _pagelines[position + 1];
            street2 = _pagelines[position + 2];

            street1 = AddressParsing.AddressParsing.ParseStreetAddress(street1);

            street2 = FixOcrnumbers(street2); // street2.Replace("'.", "4").Replace("  ", " ");
            var splits = street2.Split(' ');

            addr.Streetaddress1 = street1;

            //need to start with the last split, get zip , state, rest is city

            if (splits.Count() > 0)
            {

                var city = "";
                for (var i = splits.Count() - 1; i >= 0; i--)
                {
                    var temp = FixOcrnumbers(splits[i]);
                    if (temp.Replace("-", "").IsNumber())
                        addr.Zip = temp;
                    else if (temp.Length == 2)
                        addr.State = temp;
                    else
                    {
                        city = temp + " " + city;
                    }

                }
                addr.City = city.Trim();
            }
            return addr;
        }

        protected override void GetPerson()
        {

                var position = _searches["taxpayer"];

                if (position == 0)
                throw new ArgumentException("Unable to find an address");

                var name = _pagelines[position + 1];

            name = ReduceNoiseFromEnd(name);

            //            if (name.Contains("warrant id"))
            //                name = name.Substring(0, name.ToLower().IndexOf("warrant id"));

            NameHelper.ParseName(ref _lead, name, false);
        }

        protected override void GetRecordDate()
        {
            var position = _searches["day of"];

            if (position == 0)
                throw new ArgumentException("Unable to find an amount");

            var str = _pagelines[position];

            var splitz = str.Split(' ');
            var year = "";
            var month = "";
            var day = "";

            for (int i = splitz.Length -1 ; i >=0; i--)
            {
                var temp = FixOcrnumbers(splitz[i]).ToLower().FastReplace(".","");
                if (temp.IsNumber() && temp.Length == 4)
                    year = temp;
                else if (_months.Contains(temp))
                    month = temp;
                else if (temp != "day" && temp != "of" && temp != "this" && temp != "on" && temp != "the")
                {
                    temp = temp.FastReplace("rd", "").FastReplace("th", "").FastReplace("st", "").FastReplace("nd", "");
                    if (temp.IsNumber())
                        day = temp;
                }

            }

            _lead.Recordeddate = month + " " + day + ", " + year;
        }

        protected override void GetAmount()
        {
            var position = _searches["total"];

            if (position == 0)
                throw new ArgumentException("Unable to find an amount");

            var amount = _pagelines[position - 1].Squeeze().FastReplace(" ","");

            //amount = amount.Substring(amount.IndexOf("total") + "total".Length);

            _lead.Debt = ParseOutDebt(amount);
        }
    }
}
