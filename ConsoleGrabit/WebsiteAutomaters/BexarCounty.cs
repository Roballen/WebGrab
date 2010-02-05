using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ArtOfTest.Common;
using ArtOfTest.WebAii.Controls.HtmlControls;
using ArtOfTest.WebAii.Core;
using ArtOfTest.WebAii.ObjectModel;
using Utilities;

using ConsoleGrabit.Interfaces;

namespace ConsoleGrabit.WebsiteAutomaters
{
    public class BexarCounty : BaseAutomater, IWebsiteAutomater
    {
        private WebconfigsConfig _config;

        public BexarCounty(WebconfigsConfig config) : base(config){}

        public IList<Lead> Automate()
        {
            return NeedtoCheck() ? Process() : _leads;
        }

        private IList<Lead> Process()
        {
            var settings = new Settings(BrowserType.InternetExplorer, @"c:\log\") {ClientReadyTimeout = 60*1000};
            var manager = new Manager(settings);
            manager.Start();
            manager.LaunchNewBrowser();
            manager.ActiveBrowser.AutoWaitUntilReady = true;

            var mainbrowser = manager.ActiveBrowser;

            mainbrowser.NavigateTo(@"https://www.landata-cc.com/WAM/loginForm.asp?iSiteID=3&iWAMid=3");
            var user = mainbrowser.Find.ByName("txtUserName");
            mainbrowser.Actions.SetText(user, "jsmithee");

            var pass = mainbrowser.Find.ByName("txtPassword");
            mainbrowser.Actions.SetText(pass, "2020");

            var login = mainbrowser.Find.ByAttributes("class=clsHotKeyLink");
            mainbrowser.Actions.Click(login);

            var select = mainbrowser.Find.ByName("Appls");
            mainbrowser.Actions.SelectDropDown(select, "56", true);
            mainbrowser.Actions.Click(select);

            var datebutton = mainbrowser.Find.ByCustom(e => e.TextContent.Contains("Document Type / Date"));
            mainbrowser.Actions.Click(datebutton);

            var from = mainbrowser.Find.ByName("SearchbyDateFrom");
            mainbrowser.Actions.SetText(from, DateTime.Now.AddDays(-10).ToString("MM/dd/yyyy"));

            var to = mainbrowser.Find.ByName("SearchbyDateTo");
            mainbrowser.Actions.SetText(to, DateTime.Now.ToString("MM/dd/yyyy"));

            var doctype = mainbrowser.Find.ByName("availSearchDocType");
            mainbrowser.Actions.SelectDropDown(doctype, "FEDERAL TAX LIEN");

            mainbrowser.Actions.InvokeScript("addDocType()");

            var pagesize = mainbrowser.Find.ByAttributes("value=200");
            if (pagesize != null)
                mainbrowser.Actions.Click(pagesize);

            var clientid = mainbrowser.ClientId;

            mainbrowser.Actions.InvokeScript("submitForm()");

            foreach (var browser in manager.Browsers.Where(browser => browser.ClientId != clientid))
                manager.RemoveBrowser(browser.ClientId);

            ProcessRecords(manager);

            return _leads;
        }

        private void ProcessRecords(Manager manager )
        {
            var mainbrowser = manager.ActiveBrowser;
            //first scroll through and try to get the data from the ViewDetails pop up
            ICollection<Element> details = mainbrowser.Find.AllByCustom(e => e.Content.Contains("Detail.asp"));
            foreach (var element in details)
            {
                var lead = new Lead();

                mainbrowser.Actions.Click(element);
                mainbrowser.WaitUntilReady();

                var detailbrowser = manager.ActiveBrowser;
                if (detailbrowser == null) continue;
              
                detailbrowser.RefreshDomTree();
                detailbrowser.WaitUntilReady();

                var date = detailbrowser.Find.ByCustom(e => e.TextContent.Contains("Filed Date"));
                lead.Recordeddate = date.GetNextSibling().TextContent;

                var amount = detailbrowser.Find.ByCustom(e => e.TextContent.Contains("Consideration Amt"));
                lead.Debt = amount.GetNextSibling().TextContent;

                var headers = detailbrowser.Find.AllByAttributes("class=clsDetailSubHead");
                foreach (var headertable in headers.Select(header => GetFirstParentByTagName(header,"table")))
                {

                    if (headertable.InnerText.Contains("Property Address"))
                    {
                        //haven't seen one with a property address yet.
                    }

                    else if (headertable.InnerText.IsEmpty())
                    {
                        var htmlnametable = new HtmlControl(headertable);
                        var namecells = htmlnametable.Find.AllByAttributes("class=clsdetaildata");
                        var name = "";
                        if (namecells.Count > 0)
                        {
                            name = namecells[0].InnerText;
                            if (name.Contains(","))
                            {
                                var names = name.Split(',');
                                if (names.Length > 1)
                                    lead.First = names[1];

                                lead.Last = names[0];
                            }
                            else
                                lead.Businessname = name;
                        }
                    }
                }

                //we can leave open the details window for later use

                if (lead.IsValid())
                {
                    _leads.Add(lead);
                    continue;
                }

                var detailrow = GetFirstParentByTagName(element, "tr");
                var tablerow = new HtmlTableRow(detailrow);
                var imageclick = tablerow.Find.ByCustom(e => e.Content.Contains("viewImageFrames.asp"));

                //add event listener
                AddListener();
                mainbrowser.Actions.Click(imageclick);
                WaitForJavaApplet();

                if ( _waitingforimage )
                {
                    //try once more
                    foreach (var browser in manager.Browsers.Where(browser => browser.ClientId != mainbrowser.ClientId))
                    {
                        browser.Close();
                        mainbrowser.Actions.Click(imageclick);
                        WaitForJavaApplet();
                    }
                }

                RemoveListener();
                foreach (var browser in manager.Browsers.Where(browser => browser.ClientId != mainbrowser.ClientId))
                {
                    browser.Close();
                }
            }           
        }
    }
}
