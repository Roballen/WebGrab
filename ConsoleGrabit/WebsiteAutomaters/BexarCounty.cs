using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using ArtOfTest.Common;
using ArtOfTest.Common.Win32;
using ArtOfTest.WebAii.Controls.HtmlControls;
using ArtOfTest.WebAii.Core;
using ArtOfTest.WebAii.ObjectModel;
using ArtOfTest.WebAii.Win32.Dialogs;
using ConsoleGrabit.Models;
using Utilities;

using ConsoleGrabit.Interfaces;
using Message = ConsoleGrabit.Models.Message;

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

        public void NavigateToLeadList()
        {

                SetUp();

                _main.NavigateTo(@"https://www.landata-cc.com/WAM/loginForm.asp?iSiteID=3&iWAMid=3");
                var user = _find.ByName("txtUserName");
                _main.Actions.SetText(user, _config.Username);

                var pass = _find.ByName("txtPassword");
                _main.Actions.SetText(pass, _config.Password);

                var login = _find.ByAttributes("class=clsHotKeyLink");
                _main.Actions.Click(login);

                var select = _find.ByName("Appls");
                _main.Actions.SelectDropDown(select, "56", true);
                _main.Actions.Click(select);

                var datebutton = _find.ByCustom(e => e.TextContent.Contains("Document Type / Date"));
                _main.Actions.Click(datebutton);

                var from = _find.ByName("SearchbyDateFrom");
                var days = _config.Daysback;
                if (days < 1)
                    days = 1;
                _main.Actions.SetText(from, DateTime.Now.AddDays(-days).ToString("MM/dd/yyyy"));

                var to = _find.ByName("SearchbyDateTo");
                _main.Actions.SetText(to, DateTime.Now.ToString("MM/dd/yyyy"));

                var doctype = _find.ByName("availSearchDocType");
                _main.Actions.SelectDropDown(doctype, "FEDERAL TAX LIEN");
                _main.Actions.InvokeScript("addDocType()");
                _main.Actions.SelectDropDown(doctype, "STATE TAX LIEN");
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
            //_main = _manager.ActiveBrowser;
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
                        lead.Document.Pages = Convert.ToInt32(docpages.TextContent);
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
                            name = namecells[0].InnerText.UnEscapeXml();
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

        private bool GetDocument(Element element, ref Lead lead, bool viewerloaded)
        {
            var documentdound = false;
            try
            {
                var detailrow = GetFirstParentByTagName(element, "tr");
                var tablerow = new HtmlTableRow(detailrow);
                var imageclick = tablerow.Find.ByCustom(e => e.Content.Contains("viewImageFrames.asp"));

                lead.Document.Url = GetUrlFromJavaPopHref(imageclick.Content);

                //add event listener
                //AddListener();
                _main.Actions.Click(imageclick);
                //just wait for it to finish loading
                if (viewerloaded)
                    Thread.Sleep(1000 * _config.Performancetweaks["imageviewerload"]);
                else
                    Thread.Sleep(1000 * _config.Performancetweaks["imagewaitafterload"]);
                //documentdound = WaitForJavaApplet(););
                
                if (lead.Document.Pages > 0)
                {
                    //RemoveListener();
                    //try and use the save button
                    //AddListener(Properties.Settings.Default.downloadpath);
                    int x = _manager.ActiveBrowser.Window.Location.X + _config.Positionals["SavePdf"].X;
                    int y = _manager.ActiveBrowser.Window.Location.Y + _config.Positionals["SavePdf"].Y;

                    if (!_manager.ActiveBrowser.Window.IsVisible)
                    {
                        _manager.ActiveBrowser.Window.SetFocus();
                    }

                    _manager.Desktop.Mouse.Click(MouseClickType.LeftClick, x, y );
                    Thread.Sleep(200);
                        _manager.Desktop.KeyBoard.KeyPress(Keys.Tab);
                    _manager.Desktop.KeyBoard.KeyPress(Keys.Tab);
                    _manager.Desktop.KeyBoard.KeyPress(Keys.Tab);
                    _manager.Desktop.KeyBoard.KeyPress(Keys.Tab);
                    _manager.Desktop.KeyBoard.KeyPress(Keys.Tab);

                    var path = Path.Combine(Properties.Settings.Default.pdfstore, lead.GetHashCode() + ".tif");
//                    var save = new SaveAsDialog(_manager.ActiveBrowser, DialogButton.SAVE, path, _manager.Desktop);
//                    _manager.DialogMonitor.AddDialog(save);
//                    _manager.DialogMonitor.Start();

                    _manager.Desktop.KeyBoard.KeyPress(Keys.Space);
                    Thread.Sleep(_config.Performancetweaks["textdialogueload"] * 1000);
                    _manager.Desktop.KeyBoard.TypeText(path, 100 );
                    _manager.Desktop.KeyBoard.KeyPress(Keys.Tab);
                    _manager.Desktop.KeyBoard.KeyPress(Keys.Tab);
                    _manager.Desktop.KeyBoard.KeyPress(Keys.Enter);
                    //handle save pop up

                    documentdound = true;

                    //handle file download pop up
                    _manager.DialogMonitor.Stop();

                }


                var test = _manager.ActiveBrowser.Url;
                Console.WriteLine(test);
//                if (!documentfound)
//                {
                    //try once more
//                    foreach (var browser in _manager.Browsers.Where(browser => browser.ClientId != _main.ClientId))
//                    {
//                        browser.Close();
//                    }
//                    _main.Actions.Click(imageclick);
//                    WaitForJavaApplet();
//                }


                foreach (var browser in _manager.Browsers.Where(browser => browser.ClientId != _main.ClientId))
                {
                    browser.Close();
                }
                return documentdound;
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

        #region IWebsiteAutomater Members


        public Config Config()
        {
            return _config;
        }

        #endregion
    }
}
