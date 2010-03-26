using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConsoleGrabit.Models;
using Utilities;

namespace ConsoleGrabit.OCR
{
    public class MonroeOcr : BaseOcrParser
    {
        private int _nameposition = 0;
        public MonroeOcr(string filename, string county)
            : base(filename, county)
        {
            _searches = new Dictionary<string, int> { { "warrant id", 0 }, { "1040", 0 }, { "6672", 0 }, { "in the amount of", 0 }, { "judgment debtor", 0 }, { "last known address", 0 } };
            _lead.LeadType = LeadType.State;
        }

        private bool ParsedVertical()
        {
            //if we find judgement and debtor on the same line, then the parse went vertical.
            return _searches["judgment debtor"] > 0 || _searches["last known address"] > 0;
        }

        protected override Address GetAddress()
        {

            var addr = new Address();
            var street1 = "";
            var street2 = "";

            var position = _searches["warrant id"];

            if (position == 0 || ParsedVertical())
            {
                position = GetParsedVerticalAddress(out street1, out street2);
                _nameposition = position - 1;
            }
            else
            {
                street1 = _pagelines[position + 1];
                street2 = _pagelines[position + 2];
            }

            if (position == 0)
                throw new ArgumentException("Unable to find an address");


            street1 = AddressParsing.AddressParsing.ParseStreetAddress(street1);

            street2 = FixOcrnumbers(street2); // street2.Replace("'.", "4").Replace("  ", " ");
            var splits = street2.Split(' ');

            addr.Streetaddress1 = street1;

            //need to start with the last split, get zip , state, rest is city

            if (splits.Count() > 0)
            {
                
                var city = "";
                for (var i = splits.Count() -1; i  >= 0; i-- )
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
//                addr.City = splits[0];
//
//                if (splits.Count() > 2)
//                {
//                    addr.State = splits[1];
//                    addr.Zip = splits[2];
//                }
//                else
//                {
//                    if (splits.Count() == 2)
//                    {
//                        if (splits[1].Length == 2)
//                            addr.State = splits[1];
//                        else
//                        {
//                            addr.Zip = splits[1];
//                        }
//                    }
//                }
            }
            return addr;
        }

        protected override void GetPerson()
        {
            // "1040", "6672"
            if (_nameposition == 0)
            {
                _nameposition = _searches["warrant id"];

                if (_nameposition == 0 || ParsedVertical())
                {
                    _nameposition = GetVerticalPerson();
                }
            }

            if (_nameposition == 0)
                throw new ArgumentException("Unable to find an address");

            var name = _pagelines[_nameposition];

            name = ReduceNoiseFromEnd(name);

            //            if (name.Contains("warrant id"))
            //                name = name.Substring(0, name.ToLower().IndexOf("warrant id"));

            NameHelper.ParseName(ref _lead, name, false);
        }

        protected override void GetAmount()
        {
            var position = _searches["in the amount of"];

            if (position == 0)
                throw new ArgumentException("Unable to find an amount");

            var amount = _pagelines[position];

            amount = amount.Substring(amount.IndexOf("in the amount of") + "in the amount of".Length);

            _lead.Debt = ParseOutDebt(amount);
        }

        protected override void GetRecordDate()
        {
            var position = _searches["in the amount of"];

            if (position == 0)
                throw new ArgumentException("Unable to find an amount");

            var str = ParsedVertical() ? _pagelines[position + 1] : _pagelines[position];

            foreach (var va in str.Split(' '))
            {
                if (_months.Contains(va.ToLower()))
                {
                    _lead.Recordeddate = str.Substring(str.IndexOf(va)).FastReplace(".", "");
                    break;
                }
            }
        }

        private int GetParsedVerticalAddress(out string street1, out string street2)
        {
            //var position = _searches["judgment debtor"] != 0 ? _searches["judgment debtor"] -3 : _searches["last known address"] -4;

            var position = _searches["judgment debtor"] != 0 ? _searches["judgment debtor"] : _searches["last known address"];

            //move up to a line that contains a valid streeet suffix
            for (var i = position; i > position - 10; i--)
            {
                foreach (var word in _pagelines[i].Split(' '))
                {
                    if (AddressParsing.AddressParsing.IsStreetSuffix(word))
                    {
                        position = i;
                        break;
                    }
                }
            }

            street1 = _pagelines[position];
            street2 = _pagelines[position + 1];

            return position;
        }

        private int GetVerticalPerson()
        {
            return _searches["judgment debtor"] != 0 ? _searches["judgment debtor"] - 4 : _searches["last known address"] - 5;
        }



    }
}
