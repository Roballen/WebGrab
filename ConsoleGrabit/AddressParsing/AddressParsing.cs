using System;
using System.Collections.Generic;
using System.IO;
using Utilities;

namespace ConsoleGrabit.AddressParsing
{
    public class AddressParsing
    {
        private static List<string> _suffixes = null;
        private static List<string> _secondaryunit = null;
        private static List<string> _requiresunit = null;

        public static bool RequiresUnit(string text)
        {
            if (_requiresunit == null)
                InitializerequiredUnits();

            return _requiresunit.Contains(text.ToUpper());
        }

        public static bool IsStreetSuffix(string text)
        {
            if (_suffixes == null)
                InitializeSuffixes();

            return _suffixes.Contains(text.ToUpper());
        }

        public static bool IsSecondaryUnit(string text)
        {
            if (_secondaryunit == null)
                InitializeSecondary();

            return _secondaryunit.Contains(text.ToUpper());
        }

        private static void InitializeSuffixes()
        {
            _suffixes = new List<string>();
            Utilities.FileTools.SLoadFileToArray(FindFile("AddressSuffixes.txt"), ref _suffixes);

            if (_suffixes == null)
                throw new ApplicationException("Unable to Initialize Address Suffix Parsing");
        }

        private static void InitializeSecondary()
        {
            _secondaryunit = new List<string>();

            Utilities.FileTools.SLoadFileToArray(FindFile("SecondaryUnits.txt"), ref _secondaryunit);

            if (_secondaryunit == null)
                throw new ApplicationException("Unable to Initialize Address Secondary Unit Parsing");
        }

        private static void InitializerequiredUnits()
        {
            _requiresunit = new List<string>();
            Utilities.FileTools.SLoadFileToArray(FindFile("RequiresUnit.txt"), ref _requiresunit);

            if (_requiresunit == null)
                throw new ApplicationException("Unable to Initialize Address Secondary Unit Parsing");
        }

        private static string FindFile(string filename)
        {
            var di = new DirectoryInfo(Environment.CurrentDirectory);
            return di.GetFiles(filename, SearchOption.AllDirectories)[0].FullName;
        }

        public static string ParseStreetAddress(string address)
        {
            var temp = "";

            //fix smashed address
            address = FixSmashedAddress(address);

            var splitz = address.Squeeze().Split(' ');
            var last = splitz.Length - 1;
            for (var i = splitz.Length -1; i >= 0; i-- )
            {
                if ( IsSecondaryUnit(splitz[i]))
                {
                    if (RequiresUnit(splitz[i]) && (i + 1 != splitz.Length) && splitz[i + 1].IsNumeric())
                    {
                        last = i + 1;
                        break;
                    }
                    last = i;  
                }

                if (IsStreetSuffix(splitz[i]))
                {
                    last = i;
                    break;
                }
            }

            for (int j =0; j <= last; j++)
            {
                temp += " " + splitz[j];
            }
            return temp.Trim();
        }

        private static string FixSmashedAddress(string address)
        {
            var tempaddr = "";
            foreach (var chr in address)
            {
                if (chr.ToString().IsInteger())
                    tempaddr += chr;
            }

            address = tempaddr + " " + address.Substring(tempaddr.Length).Squeeze();
            return address;
        }
    }
}
