using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ArtOfTest.Common;
using ArtOfTest.WebAii.Controls.HtmlControls;
using ArtOfTest.WebAii.Core;
using ArtOfTest.WebAii.ObjectModel;
using ArtOfTest.WebAii.Win32.Dialogs;
using ConsoleGrabit.Models;
using Utilities;

using ConsoleGrabit.Interfaces;

namespace ConsoleGrabit.WebsiteAutomaters
{
    public class BexarCounty : BaseWebAiiAutomater, IWebsiteAutomater
    {
        public BexarCounty(WebconfigsConfig config) : base(config) { }

        //        public CountyPull Automate()
        //        {
        //            return NeedtoCheck() ? Process() : _pull;
        //        }

        public bool AreRecordsToProcess()
        {
            return !_main.Url.Contains("SearchCriteria");
        }

        public string County()
        {
            return _config.type;
        }

        public void NavigateToLeadList()
        {
            SetUp();

            _main.NavigateTo(@"https://www.landata-cc.com/WAM/loginForm.asp?iSiteID=3&iWAMid=3");
            var user = _find.ByName("txtUserName");
            _main.Actions.SetText(user, _config.username);

            var pass = _find.ByName("txtPassword");
            _main.Actions.SetText(pass, _config.password);

            var login = _find.ByAttributes("class=clsHotKeyLink");
            _main.Actions.Click(login);

            var select = _find.ByName("Appls");
            _main.Actions.SelectDropDown(select, "56", true);
            _main.Actions.Click(select);

            var datebutton = _find.ByCustom(e => e.TextContent.Contains("Document Type / Date"));
            _main.Actions.Click(datebutton);

            var from = _find.ByName("SearchbyDateFrom");
            var days = _config.daysback;
            if (days < 1)
                days = 1;
            _main.Actions.SetText(from, DateTime.Now.AddDays(-days).ToString("MM/dd/yyyy"));

            var to = _find.ByName("SearchbyDateTo");
            _main.Actions.SetText(to, DateTime.Now.ToString("MM/dd/yyyy"));

            var doctype = _find.ByName("availSearchDocType");
            _main.Actions.SelectDropDown(doctype, "FEDERAL TAX LIEN");
            _main.Actions.InvokeScript("addDocType()");
            _main.Actions.SelectDropDown(doctype, "STATE TAX WARRANT");
            _main.Actions.InvokeScript("addDocType()");

            var pagesize = _find.ByAttributes("value=200");
            if (pagesize != null)
                _main.Actions.Click(pagesize);



            var clientid = _main.ClientId;

            _manager.DialogMonitor.AddDialog(new AlertDialog(_main,DialogButton.OK));
            _manager.DialogMonitor.Start();

            _main.Actions.InvokeScript("submitForm()");


            if (!AreRecordsToProcess())
            {
                throw new Exception("No records to process");
            }
            _manager.DialogMonitor.Stop();

            foreach (var browser in _manager.Browsers.Where(browser => browser.ClientId != clientid))
                _manager.RemoveBrowser(browser.ClientId);

        }

        public IList<Lead> ProcessMultiple()
        {
            _main = _manager.ActiveBrowser;
            //first scroll through and try to get the data from the ViewDetails pop up
            IList<Element> details = _find.AllByCustom(e => e.Content.Contains("Detail.asp"));

            foreach (var element in details)
            {
                var lead = new Lead();

                try
                {
                    _main.Actions.Click(element);
                    _main.WaitUntilReady();

                    var detailbrowser = _manager.ActiveBrowser;
                    if (detailbrowser == null) continue;

                    try
                    {
                        detailbrowser.RefreshDomTree();
                        detailbrowser.WaitUntilReady();
                    }
                    catch (Exception e) { }

                    lead = ProcessDetailView(detailbrowser);

                    //we can leave open the details window for later use

                    if (lead.IsValid())
                    {
                        lead.Status = LeadStatus.Complete;
                        _leads.Add(lead);
                        continue;
                    }

                    if (GetDocument(element, ref lead))
                    {
                        lead.Document.Disklocation = _imagelocation;
                        PerformOCR(ref lead);
                    }
                }
                catch (Exception ex)
                {
                    lead.Messages.Add(new Message() { Content = ex.Message, Messagetype = MessageType.Error });
                }
                    _leads.Add(lead);

            }

            foreach (var browse in _manager.Browsers)
            {
                browse.Close();
            }

            return _leads;
        }

        private Lead ProcessDetailView(Browser detailbrowser)
        {
            var lead = new Lead();

            try
            {
                lead.Recordeddate = GetNextSiblingText(detailbrowser, e => e.TextContent.Contains("Filed Date"));
                lead.Debt = GetNextSiblingText(detailbrowser, e => e.TextContent.Contains("Consideration Amt")); //  detailbrowser.Find.ByCustom(e => e.TextContent.Contains("Consideration Amt"));
                  lead.Id = GetNextSiblingText(detailbrowser, e => e.TextContent.Contains("Document Number"));
                  lead.Book = GetNextSiblingText(detailbrowser, e => e.TextContent.Contains("Book Number"));
                  lead.Page = GetNextSiblingText(detailbrowser, e => e.TextContent.Contains("Page Number"));
                
                //# of Pages
                var docpages = GetNextSibling(detailbrowser, e => e.TextContent.Contains("# of Pages"));// detailbrowser.Find.ByCustom(e => e.TextContent.Contains("# of Pages"));
                if (docpages != null)
                {
                    try
                    {
                        lead.Document.Pages = Convert.ToInt32(docpages.GetNextSibling().TextContent);
                    }
                    catch (Exception)
                    {
                    }
                }
                var headers = detailbrowser.Find.AllByAttributes("class=clsDetailSubHead");
                foreach (var headertable in headers.Select(header => GetFirstParentByTagName(header, "table")))
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
            }
            catch (Exception e)
            {
                lead.Status = LeadStatus.Error;
                lead.Messages.Add(new Message() { Content = e.Message, Messagetype = MessageType.Error });
            }

            return lead;
        }

        private bool GetDocument(Element element, ref Lead lead)
        {
            try
            {
                var detailrow = GetFirstParentByTagName(element, "tr");
                var tablerow = new HtmlTableRow(detailrow);
                var imageclick = tablerow.Find.ByCustom(e => e.Content.Contains("viewImageFrames.asp"));

                lead.Document.Url = GetUrlFromJavaPopHref(imageclick.Content);

                //add event listener
                AddListener();
                _main.Actions.Click(imageclick);
                var documentfound = WaitForJavaApplet();

                var test = _manager.ActiveBrowser.Url;
                Console.WriteLine(test);
                if (!documentfound)
                {
                    //try once more
                    foreach (var browser in _manager.Browsers.Where(browser => browser.ClientId != _main.ClientId))
                    {
                        browser.Close();
                    }
                    _main.Actions.Click(imageclick);
                    WaitForJavaApplet();
                }


                foreach (var browser in _manager.Browsers.Where(browser => browser.ClientId != _main.ClientId))
                {
                    browser.Close();
                }
                return documentfound;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                lead.Messages.Add(new Message() { Content = e.Message, Messagetype = MessageType.Error });
            }
            finally
            {
                RemoveListener();
            }

            return false;
        }
    }
}
