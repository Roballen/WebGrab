using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using ArtOfTest.WebAii.Controls.HtmlControls;
using ArtOfTest.WebAii.Core;
using ArtOfTest.WebAii.ObjectModel;
using ConsoleGrabit.Interfaces;
using Microsoft.VisualStudio.TestTools.WebTesting;
using Microsoft.VisualStudio.TestTools.WebTesting.Rules;
using Utilities;

namespace ConsoleGrabit
{
    class Program
    {
        static void Main(string[] args)
        {

            var configlocation = GetConfiglocation(args);

            //create a location for copying pdfs
            FileTools.SForceDirectories(Properties.Settings.Default.pdfstore);
           
            if (FileTools.SFileExists(configlocation))
                WebAutomaterFactory.GetConfigs(configlocation);

            var websites = WebAutomaterFactory.GetWebSiteList();


            //run automation on every website defined in config file
            foreach (var websiteAutomater in websites)
            {
                foreach (var lead in websiteAutomater.Automate())
                {
                    Console.WriteLine(lead.Streetaddress);
                }
            }
        }

        private static string GetConfiglocation(string[] args)
        {
            var configlocation = Properties.Settings.Default.configlocation;
            if (configlocation == null || configlocation.IsEmpty()) // use setting, then args, then something in the execution path
            {
                if (args.Length > 0)
                    configlocation = args[0];
                else
                {
                    var test = Path.Combine(Environment.CurrentDirectory, "Countyrules.xml");
                    if (FileTools.SFileExists(test))
                        configlocation = test;
                    else
                        Console.WriteLine("No configuration File specified, using defaults");
                }
            }
            return configlocation;
        }

    }
}
