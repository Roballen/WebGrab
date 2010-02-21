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
using ConsoleGrabit.Models;
using Db4objects.Db4o;
using Microsoft.VisualStudio.TestTools.WebTesting;
using Microsoft.VisualStudio.TestTools.WebTesting.Rules;
using Utilities;

namespace ConsoleGrabit
{
    class Program
    {
        static void Main(string[] args)
        {

//
//            IObjectContainer db = Db4oEmbedded.OpenFile(Db4oEmbedded.NewConfiguration(), Properties.Settings.Default.Db40Location);
//
//            try
//            {
//                Lead lead = new Lead();
//                lead.Address.Streetaddress1 = "9400 Owl ln";
//                lead.Debt = "24,000.00";
//                lead.Document.Pages = 1;
//                lead.Messages.Add(new Message() {Messagetype = MessageType.Information, Message1 = "Updated"});
//                lead.Document.Pages = 5;
//                db.Store(lead);
//
//                var test = new Lead();
//                test.Address.Streetaddress1 = "345 sumkner st";
//                db.Store(test);
//                
//                IObjectSet result = db.QueryByExample(test);
//                var objresult = (Lead)result.Next();
//                
//                Console.WriteLine(objresult.Address.Streetaddress1);
//
//                Console.ReadLine();
//
//
//            }
//            catch (Exception ex)
//            {
//               Console.WriteLine(ex.Message + "\n" + ex.InnerException);
//            }
//            finally
//            {
//                db.Commit();
//                db.Close();
//            }


            var configlocation = GetConfiglocation(args);

            //create a location for copying pdfs
            FileTools.SForceDirectories(Properties.Settings.Default.pdfstore);
            FileTools.SForceDirectories(Properties.Settings.Default.downloadpath);
           
            if (FileTools.SFileExists(configlocation))
                WebAutomaterFactory.GetConfigs(configlocation);

            var websites = WebAutomaterFactory.GetWebSiteList();

            //run automation on every website defined in config file
            foreach (var websiteAutomater in websites)
            {
                var results = ProcessManager.Process(websiteAutomater);
                foreach (var lead in results.Leads)
                {
                    Console.WriteLine(lead.Debt);
                }
                SaveCountyToDb(results);
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

        private static void SaveCountyToDb(CountyPull pull)
        {
            bool success = false;
            int retries = 5;
            int tries = 0;
            int wait = 750;
            while (!success && tries < retries)
            {
                IObjectContainer db = Db4oEmbedded.OpenFile(Db4oEmbedded.NewConfiguration(),
                                                            Properties.Settings.Default.Db40Location);
                try
                {
                    db.Store(pull);
                    db.Commit();
                    db.Close();
                    success = true;
                }
                catch (Exception e)
                {
                    Thread.Sleep(wait);
                }
            }
        }

    }
}
