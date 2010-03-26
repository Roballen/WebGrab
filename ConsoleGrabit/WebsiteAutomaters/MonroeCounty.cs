using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using ArtOfTest.WebAii.Core;
using ArtOfTest.WebAii.ObjectModel;
using ArtOfTest.WebAii.Silverlight.UI;
using ArtOfTest.WebAii.Win32.Dialogs;
using ConsoleGrabit.Interfaces;
using ConsoleGrabit.Models;
using ConsoleGrabit.OCR;
using Utilities;
using Message = ConsoleGrabit.Models.Message;

namespace ConsoleGrabit.WebsiteAutomaters
{
    class MonroeCounty : BaseWebAiiAutomater, IWebsiteAutomater
    {
        public MonroeCounty(WebconfigsConfig config) : base(config) { }

        public void NavigateToLeadList()
        {
            SetUp();
            _main.NavigateTo(@"https://gov.propertyinfo.com/wam3/loginForm.asp?iWAMid=30&a=true");

            SetTextByFieldName("txtUserName", _config.Username);
            SetTextByFieldName("txtPassword", _config.Password);

            var login = _find.ById("btn_login");
            _main.Actions.Click(login);

            var searchtype = _find.ByContent("Date/Doc Type");
            _main.Actions.Click(searchtype);

            var days = _config.Daysback;
            if (days < 1)
                days = 1;

            _main.RefreshDomTree();
            SetTextByFieldName("SearchbyDateFrom", DateTime.Now.AddDays(-days).ToString("MM/dd/yyyy"));
            
            var doctype = _find.ByName("availSearchDocType");
            _main.Actions.SelectDropDown(doctype, "FEDERAL TAX LIEN");
            _main.Actions.InvokeScript("addDocType()");
            _main.Actions.SelectDropDown(doctype, "STATE TAX WARRANT");
            _main.Actions.InvokeScript("addDocType()");

            var search = _find.ById("btn_go_search");
            _main.Actions.Click(search);

            var clientid = _main.ClientId;

            _manager.DialogMonitor.AddDialog(new AlertDialog(_main, DialogButton.OK));
            _manager.DialogMonitor.Start();

            //_main.Actions.InvokeScript("submitForm()");


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

            //there could be two results pages here
         
            
            if (_main.Url.Contains("SearchSummary")) //expect 2
            {
                var resultsets = _find.AllByAttributes("class=cls_button","id=~results");
                if (resultsets.Count > 1)
                {
                    //<A href="javascript:gotoResults('2001','22');"><IMG id=img_button_view_results_2001 title="View Land Records Result Set" alt="View Results Button" src="images/btn_view_results.gif"></A>

                    var select = GetUrlFromJavaPopHrefWithQuote(resultsets[0].InnerMarkup);
                    _main.Actions.InvokeScript(select);
                    //_main.Actions.Click(resultsets[0]);
                    ProcessResultsPage();

                    var nextpage = _find.ById("img_tab_search_summary");
                    if (nextpage != null)
                        _main.Actions.Click(nextpage);

                    var resultset2 = _find.AllByAttributes("class=cls_button", "id=~results");
                    if (resultset2.Count > 1)
                    {
                        //_main.Actions.Click(resultset2[1]);
                        var select2 = GetUrlFromJavaPopHrefWithQuote(resultsets[1].InnerMarkup);
                        _main.Actions.InvokeScript(select2);
                        ProcessResultsPage();
                    }
                }
            }
            else
            {
                //must be on the page already
                ProcessResultsPage();
            }

            foreach (var browse in _manager.Browsers)
            {
                browse.Close();
            }

            return _leads;
        }

        private void  ProcessResultsPage()
        {
            IList<Element> images = _find.AllByCustom(e => e.Content.Contains("viewImage"));

            //IList<Element> images = _find.AllByAttributes("tag=img", "title=View Image");
            var first = true;
            var type = LeadType.State;
            foreach (var element in images)
            {


                var lead = new Lead();
                //GetDocument(element, ref lead);

                try
                {
                    _main.Actions.Click(element);
                    if (first) //wait for image viewer to load, slow first time
                    {
                        type = RecursiveParentTextSearch("FEDERAL TAX", element, 5) ? LeadType.Federal : LeadType.State; 

                        Thread.Sleep(1000 * _config.Performancetweaks["imageviewerload"]);
                        first = false;
                    }
                    else
                        Thread.Sleep(1000 * _config.Performancetweaks["imagewaitafterload"]);
                    // wait for image
                    _main.WaitUntilReady();

                    var documentdound = false;

                    var tempx = _config.Positionals["SavePdf"].X;
                    Coordinate vord = new Coordinate();
                    _config.Positionals.TryGetValue("SavePdf", out vord);
                    var tempy = vord.X;


                    int x = _manager.ActiveBrowser.Window.Location.X + _config.Positionals["SavePdf"].X;
                    int y = _manager.ActiveBrowser.Window.Location.Y + _config.Positionals["SavePdf"].Y;

                    if (!_manager.ActiveBrowser.Window.IsVisible)
                    {
                        _manager.ActiveBrowser.Window.SetFocus();
                    }

                    _manager.Desktop.Mouse.Click(MouseClickType.LeftClick, x, y);
                    Thread.Sleep(200 );

                    _manager.Desktop.KeyBoard.KeyPress(Keys.Tab);
                    _manager.Desktop.KeyBoard.KeyPress(Keys.Tab);
                    _manager.Desktop.KeyBoard.KeyPress(Keys.Tab);
                    _manager.Desktop.KeyBoard.KeyPress(Keys.Tab);
                    _manager.Desktop.KeyBoard.KeyPress(Keys.Tab);


                    var rand = new Random();

                    

                    var path = System.IO.Path.Combine(Properties.Settings.Default.pdfstore, rand.Next(9999999) + "_monroe" + (type == LeadType.Federal ? "_federal" : "_state") +  ".pdf");
                    lead.Document.Disklocation = path;

                    _manager.Desktop.KeyBoard.KeyPress(Keys.Space);
                    Thread.Sleep(1000 * _config.Performancetweaks["textdialogueload"]);
                    _manager.Desktop.KeyBoard.TypeText(path, 10);
                    _manager.Desktop.KeyBoard.KeyPress(Keys.Tab);
                    _manager.Desktop.KeyBoard.KeyPress(Keys.Tab);
                    _manager.Desktop.KeyBoard.KeyPress(Keys.Enter);
                    //handle save pop up

                    documentdound = true;

                    //handle file download pop up
                    _manager.DialogMonitor.Stop();

//                    var detailbrowser = _manager.ActiveBrowser;
//                    if (detailbrowser == null) continue;
//
//                    try
//                    {
//                        detailbrowser.RefreshDomTree();
//                        detailbrowser.WaitUntilReady();
//                    }
//                    catch (Exception e) { }
//
//                    lead = ProcessDetailView(detailbrowser);
//
                    //we can leave open the details window for later use
//
//                    if (lead.IsValid())
//                    {
//                        lead.Status = LeadStatus.Complete;
//                        _leads.Add(lead);
//                        continue;
//                    }

                    if (!lead.Document.Disklocation.IsEmpty())
                    {
                        PerformOCR(ref lead, type);
                    }
                }
                catch (Exception ex)
                {
                    lead.Messages.Add(new Message() { Content = ex.Message, Messagetype = MessageType.Error });
                }
                _leads.Add(lead);

            }

        }

        protected override void PerformOCR(ref Lead lead, LeadType type)
        {
            if (type == LeadType.Federal)
            {
                var fed = new Federal(lead.Document.Disklocation, "Monroe");
                lead = fed.GetLeadFromOcr();
            }
            else
            {
                var state = new MonroeOcr(lead.Document.Disklocation,"Monroe");
                lead = state.GetLeadFromOcr();
            }
        }

        private bool GetDocument(Element element, ref Lead lead)
        {
            throw new NotImplementedException();
        }

        private Lead ProcessDetailView(Browser detailbrowser)
        {
            throw new NotImplementedException();
        }

        public bool AreRecordsToProcess()
        {
            return (_main.Url.Contains("SearchResults") || _main.Url.Contains("SearchSummary") )&& !_main.ContainsText("404") ;
        }

        public Config Config()
        {
            return _config;
        }
    }

}
