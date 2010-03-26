using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using ArtOfTest.WebAii.Controls.HtmlControls;
using ArtOfTest.WebAii.Core;
using ArtOfTest.WebAii.ObjectModel;
using ConsoleGrabit.Models;

namespace ConsoleGrabit
{
    public class BaseWebAiiAutomater : BaseAutomater
    {

        protected static Manager _manager = null;
        protected Browser _main;
        protected Browser _detail;
        protected Browser _documentbrowser;
        protected Find _find;

        public BaseWebAiiAutomater(WebconfigsConfig config) : base(config)
        {
            if (_manager != null)
            {
                foreach (var broswer in _manager.Browsers)
                {
                    broswer.Close();

                }
                return;
            }

            var settings = new Settings(BrowserType.InternetExplorer, @"c:\log\") { ClientReadyTimeout = 60 * 1000 };

            _manager = new Manager(settings);
            _manager.Start();
        }

        protected Element GetNextSibling(Browser browser, Predicate<Element> predicate)
        {
            Element firsttarget = null;
            try
            {
                firsttarget = browser.Find.ByCustom(predicate);
            }
            catch (Exception e1)
            {
                browser.RefreshDomTree();
                firsttarget = browser.Find.ByCustom(predicate);
            }

            return firsttarget.GetNextSibling();
        }

        protected string GetNextSiblingText(Browser browser, Predicate<Element> predicate)
        {
            var el = GetNextSibling(browser, predicate);
            return el != null ? el.TextContent : "";
        }

        protected Element GetFirstParentByTagName(Element header, string tagname)
        {
            header = header.Parent;
            return header.TagName.ToLower() == tagname.ToLower() ? header : GetFirstParentByTagName(header, tagname);
        }

        protected void SetUp()
        {
//            var settings = new Settings(BrowserType.InternetExplorer, @"c:\log\") { ClientReadyTimeout = 60 * 1000 };
//
//            _manager = new Manager(settings);
//            _manager.Start();
            _manager.LaunchNewBrowser();
            _manager.ActiveBrowser.AutoWaitUntilReady = true;

            _main = _manager.ActiveBrowser;
            _find = _main.Find;
        }

        protected string EscapeCharacters(string input)
        {
            return SecurityElement.Escape(input);
        }

        protected void SetTextByFieldName( string field, string value )
        {
            if (field.ToLower().Contains("password"))
                SetTextByFieldName(field, value, "password");
            else
                SetTextByFieldName(field, value, "text");
        }

        protected void SetTextByFieldName( string field, string value, string type )
        {
            var el = _find.ByAttributes("type=" + type,"name=" + field);
            _main.Actions.SetText(el, value);
        }

        protected bool RecursiveParentTextSearch(string text, Element e, int levels)
        {
            var found = false;

            if (levels == 0)
                return false;

            try
            {
                if (e.InnerText.ToLower().Contains(text.ToLower()))
                    found = true;
                
                if(e.Parent != null && !found)
                {
                    levels--;
                    found = RecursiveParentTextSearch(text, e.Parent, levels);
                }
            }
            catch (Exception error)
            {
                Console.WriteLine(error.Message);
            }

            return found;
        }

    }
}
