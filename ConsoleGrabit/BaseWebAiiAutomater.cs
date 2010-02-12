using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ArtOfTest.WebAii.Core;
using ArtOfTest.WebAii.ObjectModel;

namespace ConsoleGrabit
{
    public class BaseWebAiiAutomater : BaseAutomater
    {

        protected Manager _manager;
        protected Browser _main;
        protected Browser _detail;
        protected Browser _documentbrowser;
        protected Find _find;

        public BaseWebAiiAutomater(WebconfigsConfig config) : base(config)
        {

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
            var settings = new Settings(BrowserType.InternetExplorer, @"c:\log\") { ClientReadyTimeout = 60 * 1000 };

            _manager = new Manager(settings);
            _manager.Start();
            _manager.LaunchNewBrowser();
            _manager.ActiveBrowser.AutoWaitUntilReady = true;

            _main = _manager.ActiveBrowser;
            _find = _main.Find;
        }

        protected void SetTextByFieldName( string field, string value )
        {
            var el = _find.ByName(field);
            _main.Actions.SetText(el, "value");
        }

    }
}
