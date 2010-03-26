using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using ConsoleGrabit.Models;
using Utilities;

namespace ConsoleGrabit
{
    public class NameHelper
    {

        public static IList<string> businesstokens = new List<string>()
       {
           "INC","COMPANY","BUSINESS", "SERVICE", "LLC", "CORP", "CORPORATION", "INCORPORATED", "&", "HOME", "CONTRACTOR", "ROOFING",
           "PLUMBING", "HEATING", "COOLING", "LAWN", "CARE", "SON", "BROTHERS", "BROS", "IMPROVEMENT", "INSURANCE", "LLP",  "FLP",  "DBA", "CONTRUCTION", "FIRM", "LAW","ATTORNEY",
           "COUNSELOR", "ASSOCIATES",".COM", "LTD", "AGENCY", "SUPPORT","CO", "HOSPITAL", "SOCIETY"
       };

        public static IList<string> nameprefix = new List<string>()
       {
           "MR",
           "MR.",
           "MS.",
           "MS",
           "MISS",
           "MRS.",
           "MRS",
           "DR.",
           "DR",
           "PROF",
           "PROF.",
           "PH.D.",
           "PH.D",
           "PHD",
           "CAPT",
           "CAPT",
           "LT",
           "LT.",
           "D.D.S.",
           "D.D.S",
           "DDS",
           "REV",
           "REV."
       };

        public static IList<string> namesuffix = new List<string>()
        {
            "I","II","III","IV","V",
            "SR","JR","3RD","4TH","5TH"  ,  
            "1","2","3","4","5",
                        "MD","MD.","III","IV","V"      
        };

        public static void ParseName(ref Lead lead, string name, bool nameshavecommas)
        {
            name = name.UnEscapeXml().Replace("  ", " ").ToUpper();
            var isBusiness = false;
            var isPerson = false;
            var mastertokens = new List<string>();


            //if there is no comma then it is business

                if (name.Contains(","))
                {
                    var commasplit = name.Split(',');
                    var lastname = "";
                    foreach (var s in commasplit[0].Split(' '))
                    {
                        if (!nameprefix.Contains(s) && !namesuffix.Contains(s))
                            lastname += s + " ";
                    }
                    lastname = lastname.Trim();
                    if (lastname.Split(' ').Count() <= 1 && !businesstokens.Contains(lastname))
                    {
                        lead.Last = lastname.ToPascalCase();
                        isPerson = true;
                        foreach (var com in commasplit)
                        {
                            var t = com.Trim();
                            foreach (var VARIABLE in com.Split(' '))
                            {
                                if (!nameprefix.Contains(VARIABLE) && !namesuffix.Contains(VARIABLE) && !VARIABLE.IsEmpty() && VARIABLE != lastname)
                                                            mastertokens.Add(VARIABLE);
                            }
                        }
                    }
                }
                else if (nameshavecommas)
                    isBusiness = true;

            if (name.Contains('\''))
                isBusiness = true;

            //this means it is not standard and we are defaulting to a person and not business
            if (!isPerson && !isBusiness)
            {
                var commasplit = name.Split(',');

                foreach (var s in commasplit)
                {
                    var val = s.Trim();
                    var spacesplit = val.Split(' ');
                    foreach (var s1 in spacesplit)
                    {
                        if (!nameprefix.Contains(s1) && !namesuffix.Contains(s1))
                            mastertokens.Add(s1);
                        else
                        {
                            isPerson = true;
                        }
                    }

                    if (isPerson) continue;
                    if (!IsInBusinessList(spacesplit) && !MatchesBusinessLengths(spacesplit.Length, commasplit.Count() > 1)) continue;

                    isBusiness = true;
                    break;
                }
                if (!isPerson)
                {
                    if (MatchesBusinessLengths(mastertokens.Count, false)) isBusiness = true;
                }
            }

            if (isBusiness)
                lead.Businessname = name;
            else
            {
                if (!lead.Last.IsEmpty())
                {
                    foreach (var tok in mastertokens)
                    {
                        if (!nameprefix.Contains(tok) && !namesuffix.Contains(tok))
                        {
                            if (lead.First.IsEmpty())
                                lead.First = tok.ToPascalCase();
                            else if (lead.Middle.IsEmpty())
                                lead.Middle = tok.ToPascalCase();
                        }
                    }
                }
                else
                {
                    foreach (var tok in mastertokens)
                    {
                        if (!nameprefix.Contains(tok) && !namesuffix.Contains(tok))
                        {
                            if (lead.First.IsEmpty())
                                lead.First = tok.ToPascalCase();
                            else if (lead.Middle.IsEmpty() && mastertokens.Count > 2)
                                lead.Middle = tok.ToPascalCase();
                            else if (lead.Last.IsEmpty())
                                lead.Last = tok.ToPascalCase();
                        }
                    }
                }

            }
        }

        private static bool IsInBusinessList(IEnumerable<string> list)
        {
            return list.Any(s => businesstokens.Contains(s.Trim().ToUpper()));
        }

        private static bool MatchesBusinessLengths(int length, bool partial)
        {
            if (partial) return length > 2;
            return length > 6 || length == 1;
        }
    }
}
