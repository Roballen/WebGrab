using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ArtOfTest.WebAii.Core;
using ArtOfTest.WebAii.ObjectModel;
using ArtOfTest.WebAii.Win32.Dialogs;
using ConsoleGrabit.Interfaces;
using ConsoleGrabit.Models;

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
            IList<Element> details = _find.AllByCustom(e => e.Content.Contains("viewIndex"));

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
            return !_main.Url.Contains("SearchCriteria");
        }

        public Config Config()
        {
            return _config;
        }
    }

}
