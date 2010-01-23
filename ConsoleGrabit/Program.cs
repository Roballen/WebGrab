using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using ArtOfTest.WebAii.Controls.HtmlControls;
using ArtOfTest.WebAii.Core;
using ArtOfTest.WebAii.ObjectModel;
using Microsoft.VisualStudio.TestTools.WebTesting;
using Microsoft.VisualStudio.TestTools.WebTesting.Rules;

namespace ConsoleGrabit
{
    class Program
    {
        static void Main(string[] args)
        {

//            WebPostRequest wpr = new WebPostRequest("http://74.8.243.133/ALIS/WW400R.HTM");
//            wpr.Add("W9FDTA", "01142010");
//            wpr.Add("W9TDTA", "01142010");
//            wpr.Add("W9ABR", "*ALL");
//            wpr.Add("W9TOWN", "*ALL");
//            var response = wpr.GetResponse();


            Settings settings = new Settings(BrowserType.InternetExplorer, @"c:\log\");
            Manager manager = new Manager(settings);

            manager.Start();
            manager.LaunchNewBrowser();
            manager.ActiveBrowser.AutoWaitUntilReady = true;

            manager.ActiveBrowser.NavigateTo(@"https://www.landata-cc.com/WAM/loginForm.asp?iSiteID=3&iWAMid=3");
            Element user = manager.ActiveBrowser.Find.ByName("txtUserName");
            manager.ActiveBrowser.Actions.SetText(user,"jsmithee");

            Element pass = manager.ActiveBrowser.Find.ByName("txtPassword");
            manager.ActiveBrowser.Actions.SetText(pass, "2020");

            Element login = manager.ActiveBrowser.Find.ByAttributes("class=clsHotKeyLink");
            manager.ActiveBrowser.Actions.Click(login);

            Element select = manager.ActiveBrowser.Find.ByName("Appls");
            //manager.ActiveBrowser.Actions.SelectDropDown(select,"Land Records");
            manager.ActiveBrowser.Actions.SelectDropDown(select,"56",true);
            manager.ActiveBrowser.Actions.Click(select);

            //Element datebtn = Find.ByCustom(e => e.TextContent.Contains("Document Type / Date"));

            Element datebutton = manager.ActiveBrowser.Find.ByCustom(e => e.TextContent.Contains("Document Type / Date"));
            manager.ActiveBrowser.Actions.Click(datebutton);

            Element from = manager.ActiveBrowser.Find.ByName("SearchbyDateFrom");
            manager.ActiveBrowser.Actions.SetText(from, DateTime.Now.AddDays(-10).ToString("MM/dd/yyyy") );

            Element to = manager.ActiveBrowser.Find.ByName("SearchbyDateTo");
            manager.ActiveBrowser.Actions.SetText(to, DateTime.Now.ToString("MM/dd/yyyy"));

            Element doctype = manager.ActiveBrowser.Find.ByName("availSearchDocType");
            manager.ActiveBrowser.Actions.SelectDropDown(doctype,"FEDERAL TAX LIEN");

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

//                string content = row.Content;
//                string url = content.Substring(content.IndexOf("'"));
                manager.ActiveBrowser.Actions.Click(row);

                manager.ActiveBrowser.RefreshDomTree();

                Browser detailbrowser = null;
                foreach (var browser in manager.Browsers)
                {
                    if (browser.ClientId != clientid)
                        detailbrowser = browser;
                }

                if (detailbrowser != null)
                {

                    var lead = new Lead();

                    Element date = detailbrowser.Find.ByCustom(e=> e.TextContent.Contains("Filed Date"));
                    lead.Recordeddate = date.GetNextSibling().TextContent;

                    Element amount = detailbrowser.Find.ByCustom(e => e.TextContent.Contains("Consideration Amt"));
                    lead.Debt = date.GetNextSibling().TextContent;

                    string blah = "";
                }



                
                //IList<Element> rows =  manager.ActiveBrowser.Find.AllByCustom( e => e.CssClassAttributeValue  == "clsdetaildata");

                //Element debt = manager.ActiveBrowser.Find
                int count = manager.Browsers.Count;


                manager.ActiveBrowser.GoBack();
            }



            //SearchbyDateFrom
            //MM/DD/YYYY
            //SearchbyDateTo


//            manager.ActiveBrowser.WaitUntilReady();
//            manager.ActiveBrowser.Actions.InvokeScript("onclick=\"beginSearch(296)\"");




            WebPostRequest wpr1 = new WebPostRequest("https://www.landata-cc.com/WAM/loginForm.asp?iSiteID=3&iWAMid=3");
            wpr1.Add("txtUserName", "jsmithee");
            wpr1.Add("txtPassword", "2020");
            var repo = wpr1.GetResponse();




            //rg413j();

            Console.Write(repo);


//            WebTestRequest request22 = new WebTestRequest("http://74.8.243.133/ALIS/WW400R.HTM");
//            request22.Method = "POST";
//            request22.Encoding = System.Text.Encoding.GetEncoding("iso-8859-1");
//            FormPostHttpBody request22Body = new FormPostHttpBody();
//            request22Body.FormPostParameters.Add("W9FDTA", "01142010");
//            request22Body.FormPostParameters.Add("W9TDTA", "01142010");
//            request22Body.FormPostParameters.Add("W9ABR", "*ALL");
//            request22Body.FormPostParameters.Add("W9TOWN", "*ALL");
//            request22Body.FormPostParameters.Add("W9FC$", "");
//            request22Body.FormPostParameters.Add("W9TC$", "");
//            request22Body.FormPostParameters.Add("WSHTNM", this.Context["$HIDDEN1.WSHTNM"].ToString());
//            request22Body.FormPostParameters.Add("WSIQTP", this.Context["$HIDDEN1.WSIQTP"].ToString());
//            request22Body.FormPostParameters.Add("WSKYCD", this.Context["$HIDDEN1.WSKYCD"].ToString());
//            request22.Body = request22Body;
//            yield return request22;
//
//            request22 = null;


        }
    }
}
