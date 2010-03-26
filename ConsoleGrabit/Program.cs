using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;
using ConsoleGrabit.Models;
using Db4objects.Db4o;
using StructureMap;
using Utilities;

namespace ConsoleGrabit
{
    class Program
    {
        static void Main(string[] args)
        {
            ConfigureStructureMap.BootStrapIt();

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

        private static void SaveLeadsToExcelFile( CountyPull pull)
        {
            var business = new Collection<string>();
            var residence = new Collection<string>();
            business.Add("\"name\",\"address\",\"city\",\"state\",\"zip\",\"four\",\"phone\",\"amount\",\"date\",\"type\",");
            residence.Add("\"first\",\"middle\"\"last\",\"address\",\"city\",\"state\",\"zip\",\"four\",\"type\",\"amount\",\"date\",");
            foreach (var lead in pull.Leads)
            {
                if (lead.Businessname.IsEmpty())
                {
                    residence.Add("\" " + lead.First + " \",\"" + lead.Middle + "\"\"" + lead.Last + "\",\"" + lead.Address.Streetaddress1 + "\",\"" + lead.Address.City + "\",\"" + lead.Address.State + "\",\"" + lead.Address.Zip + "\",\"" + lead.LeadType + "\",\"" + lead.Debt + "\",\"" + lead.Recordeddate + "\",");
                }
                else
                {
                    business.Add("\" " + lead.Businessname + " \",\" " + lead.Address.Streetaddress1 + " \",\" " + lead.Address.City + " \",\" " + lead.Address.State + " \",\" " + lead.Address.Zip + " \",\"\",\" " + "" + " \",\" " + lead.Debt + " \",\" " + lead.Recordeddate + " \",\" " + lead.LeadType+ " \",");
                }
            }

            FileTools.SSaveArrayToFile(Path.Combine(Properties.Settings.Default.exceloutput, pull.County + "_" + pull.Time + "_buiness.csv"  ), business);
            FileTools.SSaveArrayToFile(Path.Combine(Properties.Settings.Default.exceloutput, pull.County + "_" + pull.Time + "_residence.csv"), residence);
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
                var db = ObjectFactory.GetInstance<IObjectContainer>();
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
