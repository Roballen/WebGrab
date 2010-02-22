using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConsoleGrabit.Models;

namespace ConsoleGrabit
{
    public class NameHelper
    {

        public static IList<string> businesstokens = new List<string>()
       {
           "INC","COMPANY","BUSINESS", "SERVICE", "LLC", "CORP", "CORPORATION", "INCORPORATED", "&", "HOME", "CONTRACTOR", "ROOFING",
           "PLUMBING", "HEATING", "COOLING", "LAWN", "CARE", "SON", "BROTHERS", "BROS", "IMPROVEMENT", "INSURANCE", "LLP",  "FLP",  "DBA", "CONTRUCTION"                                
       };

        public static void ParseName(ref Lead lead, string name)
        {
            //if there is no comma then it is business
            if (name.Contains(","))
            {
                var names = name.Split(',');

                if (names.Length > 1)
                {

                    var tokensInFirstSplit = names[0].Split(' ');

                    var tokfound = false;
                    foreach (var s in tokensInFirstSplit.Where(s => businesstokens.Contains(s)))
                    {
                        tokfound = true;
                    }

                    if (tokensInFirstSplit.Length < 3 && !tokfound)
                    {

                    }

                    lead.First = names[1];
                }


                lead.Last = names[0];
            }

            lead.Businessname = name;

        }


    }
}
