using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ArtOfTest.WebAii.Core;
using ArtOfTest.WebAii.ObjectModel;
using ConsoleGrabit.Interfaces;

namespace ConsoleGrabit.WebsiteAutomaters
{
    public class BexarCounty : BaseAutomater, IWebsiteAutomater
    {
        private WebconfigsConfig _config;

        public BexarCounty(WebconfigsConfig config) : base(config){}

        public IList<Lead> Automate()
        {
            return NeedtoCheck() ? Process() : new List<Lead>();
        }

        private IList<Lead> Process()
        {
            var leads = new List<Lead>();

            Settings settings = new Settings(BrowserType.InternetExplorer, @"c:\log\");
            settings.ClientReadyTimeout = 60*1000;

            Manager manager = new Manager(settings);

            manager.Start();
          
                manager.LaunchNewBrowser();
            manager.ActiveBrowser.AutoWaitUntilReady = true;

            manager.ActiveBrowser.NavigateTo(@"https://www.landata-cc.com/WAM/loginForm.asp?iSiteID=3&iWAMid=3");
            Element user = manager.ActiveBrowser.Find.ByName("txtUserName");
            manager.ActiveBrowser.Actions.SetText(user, "jsmithee");

            Element pass = manager.ActiveBrowser.Find.ByName("txtPassword");
            manager.ActiveBrowser.Actions.SetText(pass, "2020");

            Element login = manager.ActiveBrowser.Find.ByAttributes("class=clsHotKeyLink");
            manager.ActiveBrowser.Actions.Click(login);

            Element select = manager.ActiveBrowser.Find.ByName("Appls");
            //manager.ActiveBrowser.Actions.SelectDropDown(select,"Land Records");
            manager.ActiveBrowser.Actions.SelectDropDown(select, "56", true);
            manager.ActiveBrowser.Actions.Click(select);

            //Element datebtn = Find.ByCustom(e => e.TextContent.Contains("Document Type / Date"));

            Element datebutton = manager.ActiveBrowser.Find.ByCustom(e => e.TextContent.Contains("Document Type / Date"));
            manager.ActiveBrowser.Actions.Click(datebutton);

            Element from = manager.ActiveBrowser.Find.ByName("SearchbyDateFrom");
            manager.ActiveBrowser.Actions.SetText(from, DateTime.Now.AddDays(-10).ToString("MM/dd/yyyy"));

            Element to = manager.ActiveBrowser.Find.ByName("SearchbyDateTo");
            manager.ActiveBrowser.Actions.SetText(to, DateTime.Now.ToString("MM/dd/yyyy"));

            Element doctype = manager.ActiveBrowser.Find.ByName("availSearchDocType");
            manager.ActiveBrowser.Actions.SelectDropDown(doctype, "FEDERAL TAX LIEN");

            manager.ActiveBrowser.Actions.InvokeScript("addDocType()");

            Element pagesize = manager.ActiveBrowser.Find.ByAttributes("value=200");
            if (pagesize != null)
                manager.ActiveBrowser.Actions.Click(pagesize);


            string clientid = manager.ActiveBrowser.ClientId;

            manager.ActiveBrowser.Actions.InvokeScript("submitForm()");


            foreach (var browser in manager.Browsers)
            {
                if (browser.ClientId != clientid)
                    manager.RemoveBrowser(browser.ClientId);
            }

            ICollection<Element> details = manager.ActiveBrowser.Find.AllByCustom(e => e.Content.Contains("Detail.asp"));

            foreach (Element row in details)
            {
                var lead = new Lead();
                //                string content = row.Content;
                //                string url = content.Substring(content.IndexOf("'"));

                manager.ActiveBrowser.Actions.Click(row);
                manager.ActiveBrowser.WaitUntilReady();

                Browser detailbrowser = null;
                foreach (var browser in manager.Browsers)
                {
                    if (browser.ClientId != clientid)
                        detailbrowser = browser;
                }

                if (detailbrowser != null)
                {
                    detailbrowser.RefreshDomTree();
                    ICollection<Element> data = detailbrowser.Find.AllByAttributes("class=clsdetaildata");
                    foreach (var element in data)
                    {
                        Console.WriteLine( element.TagNameIndex + " : " + element.TextContent);
                    }

                    Element date = detailbrowser.Find.ByCustom(e => e.TextContent.Contains("Filed Date"));
                    lead.Recordeddate = date.GetNextSibling().TextContent;

                    Element amount = detailbrowser.Find.ByCustom(e => e.TextContent.Contains("Consideration Amt"));
                    lead.Debt = amount.GetNextSibling().TextContent;

                    Element generaltab = amount.Parent.Parent.Parent;
                    Element nametable = detailbrowser.Find.ByTagIndex("table", 5);
                    

                    leads.Add(lead);
                }




                //IList<Element> rows =  manager.ActiveBrowser.Find.AllByCustom( e => e.CssClassAttributeValue  == "clsdetaildata");

                //Element debt = manager.ActiveBrowser.Find
                //int count = manager.Browsers.Count;


                //manager.ActiveBrowser.GoBack();
            }

            return leads;
        }
    }
}
