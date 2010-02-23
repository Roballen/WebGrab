using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using ArtOfTest.WebAii.Controls.HtmlControls;
using ArtOfTest.WebAii.Core;
using ArtOfTest.WebAii.ObjectModel;
using ConsoleGrabit.Interfaces;
using ConsoleGrabit.Models;
using Utilities;

namespace ConsoleGrabit.WebsiteAutomaters
{
    public class ColoradoSecretaryOfState : BaseWebAiiAutomater, IWebsiteAutomater
    {
        public ColoradoSecretaryOfState(WebconfigsConfig config) : base(config)
        {

        }

        public void NavigateToLeadList()
        {
            SetUp();
            _main.NavigateTo(@"http://www.sos.state.co.us/cgi-forte/fortecgi?serviceName=uccprodaccess&templateName=/sessauto/ucclogin_outer_form.forte");

            var user = _find.ByName("prop_userName");
            _main.Actions.SetText(user, _config.Username);

            var password = _find.ByName("prop_userPassword");
            _main.Actions.SetText(password, _config.Password);

            var login = _find.ById("btnLogin2");

            _main.Actions.ScrollToVisible(login);

            _main.Actions.Click(login);

            _main.Actions.InvokeScript("goNextSubmit('nextTemplateName', '/sessrequ/uccsecuredpartysearch_outer_form.forte')");

            var businessname = _find.ByName("prop_BusinessName");
            _main.Actions.SetText(businessname, "Internal Revenue Service");

            var from = _find.ByName("prop_StartDate");

            var days = _config.Daysback;
            if (days < 1)
                days = 1;
            _main.Actions.SetText(from, DateTime.Now.AddDays(-days).ToString("MM/dd/yyyy"));

            var searchtype = _find.ByName("prop_CollateralCode");
            _main.Actions.SelectDropDown(searchtype, "IRS LIEN");

            var maxresults = _find.ByName("prop_RowsPerPage");
            _main.Actions.SetText(maxresults, "699");

            var beginsearch = _find.ById("btnSearch2");
            _main.Actions.Click(beginsearch);

            _main.Actions.InvokeScript( @"goNextSubmit('cp',1,'sc',3,'nextTemplateName','/sessrequ/uccsecuredpartysearchresults_outer_form.forte')");

            if (! AreRecordsToProcess())
                throw new Exception("No Records to process");

        }

        public IList<Lead> ProcessMultiple()
        {
            IList<Element> details = _find.AllByAttributes("href=~javascript:goNextSubmit('sr"); // _find.AllByCustom(e => e.Content.Contains("INTERNAL REVENUE"));

            foreach (var element in details)
            {
                var lead = new Lead();
                try
                {
                    _main.Actions.Click(element);
                    //moves browser forward 1

                    _main.Actions.InvokeScript(@"goNextSubmit('sr',1)");
                    //moves broswer forward 1

                    ///html/body/table[2]/tbody/tr[2]/td/table/tbody/tr[2]/td/table/tbody/tr[4]/td[2]/form/div/table/tbody/tr[3]/td/table/tbody/tr[3]/td/table

                    var filingelement = _find.ByTagIndex("table", 7);
                    var htmlfilingelement = new HtmlControl(filingelement);

                    var fnumber = htmlfilingelement.Find.ByXPath("//tr[2]/td[1]");
                    if (fnumber != null)
                    {
                        lead.Id = fnumber.InnerText.Substring(0, fnumber.InnerMarkup.ToLower().IndexOf("<br>"));
                    }
                    var date = htmlfilingelement.Find.ByXPath("//tr[2]/td[3]");
                    if (date != null)
                        lead.Recordeddate = date.InnerText;

                    //var imagelink = htmlfilingelement.Find.ByXPath("//tr[2]/td[5]");
                    var imagelink = _find.ByAttributes("href=~javascript:goNextSubmit('sr");

                    //get name
                    // /html/body/table[2]/tbody/tr[2]/td/table/tbody/tr[2]/td/table/tbody/tr[4]/td[2]/form/div/table/tbody/tr[5]/td/table/tbody/tr[3]/td/table/tbody/tr[2]/td

                    var nameelement = _find.ByXPath("//html/body/table[2]/tbody/tr[2]/td/table/tbody/tr[2]/td/table/tbody/tr[4]/td[2]/form/div/table/tbody/tr[5]/td/table/tbody/tr[3]/td/table/tbody/tr[2]/td");
                    if (nameelement != null)
                    {
                        var a =new string[1] ;
                        a[0] = "<br>";
                        string[] split = nameelement.InnerMarkup.ToLower().Split(a, StringSplitOptions.None);

                        if (split.Length == 3)
                        {
                            var name = split[0];
                            if (name.Contains(","))
                            {
                                var names = name.Split(',');
                                if (names.Length > 1)
                                {
                                    if (names[0].Split(' ').Length > 1)
                                        lead.Businessname = name;
                                    else
                                    {
                                        lead.First = names[1];
                                        lead.Last = names[0];
                                    }
                                }
                                else
                                    lead.Businessname = name;
                            }
                            else
                                lead.Businessname = name;

                            lead.Address.Streetaddress1 = split[1];
                            
                            //city, co zip

                            var ad2 = split[2];

                            var s2 = ad2.Split(',');
                            if (s2.Length == 2)
                            {
                                lead.Address.City = s2[0];
                                var s3 = s2[1].Split(' ');

                                if (s3.Length == 2)
                                {
                                    lead.Address.State = s3[0];
                                    lead.Address.Zip = s3[1];
                                }
                            }
                        }
                    }


                    if (imagelink != null)
                    {
                        lead.Document.Url = imagelink.InnerText;
                        if (GetDocument(imagelink, ref lead))
                        {
                            lead.Document.Disklocation = _imagelocation;
                            PerformOCR(ref lead);
                        }
                    }

                    _main.GoBack();
                    _main.GoBack();

                }
                catch (Exception ex)
                {
                    lead.Messages.Add(new Message() { Content = ex.Message, Messagetype = MessageType.Error });
                }
                _leads.Add(lead);
            }
            return _leads;
        }

        private bool GetDocument(Element element, ref Lead lead)
        {
            try
            {
                _main.Actions.Click(element);
                //moves forward once
                var imageel =_find.ByAttributes("src=~https://www.sos.state.co.us/tmpdocs");
                var image = new HtmlImage(imageel);
                if (image != null)
                {
                    _main.Actions.ScrollToVisible(imageel);
                    _imagelocation = Path.Combine(Properties.Settings.Default.pdfstore, lead.GetHashCode() + Path.GetExtension(image.Src));

                    WebRequest req = WebRequest.Create(image.Src);
                    WebResponse response = req.GetResponse();
                    Image.FromStream(response.GetResponseStream()).Save(_imagelocation);
                    


                    return true;

                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                lead.Messages.Add(new Message() { Content = e.Message, Messagetype = MessageType.Error });
            }
            finally
            {
                _main.GoBack();
            }
            return false;
        }

        public bool AreRecordsToProcess()
        {
            return true;
        }

        public Config Config()
        {
            return _config;
        }
    }
}
