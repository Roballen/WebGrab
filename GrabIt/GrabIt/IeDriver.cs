﻿using System;
using System.Collections;
using System.Diagnostics;
using System.Threading;
using mshtml;
using SHDocVw;

namespace IEAutomation
{
    /// <summary>
    /// Summary description for IEDriver.
    /// </summary>
    public class IEDriver
    {
        public int TimeoutSeconds
        {
            get { return _timeoutSeconds; }
            set { _timeoutSeconds = value; }
        }

        private InternetExplorer IE
        {
            get { return _IE; }
        }

        private bool isDocumentComplete = false;

        public string CurrentURL
        {
            get { return IE.LocationURL; }
        }

        private InternetExplorer _IE;
        private int _timeoutSeconds = 60;

        public IEDriver()
        {
            Process m_Proc = Process.Start("IExplore.exe");

            Thread.Sleep(3000);
            _IE = null;
            ShellWindows m_IEFoundBrowsers = new ShellWindowsClass();
            foreach (InternetExplorer Browser in m_IEFoundBrowsers)
            {
                if (m_Proc != null && m_Proc.MainWindowHandle != null)
                {
                    if (m_Proc.MainWindowHandle == (IntPtr) Browser.HWND)
                    {
                        _IE = Browser;
                        break;
                    }
                }
            }

            IE.Visible = true;
            IE.DocumentComplete += new DWebBrowserEvents2_DocumentCompleteEventHandler(IE_DocumentComplete);
        }

        public void ClickAnchor(string anchorId)
        {
            isDocumentComplete = false;
            HTMLAnchorElement input = GetAnchorElement(anchorId);
            input.click();
            WaitForComplete();
        }

        public void ClickAnchorWithParent(string parentControlId, string anchorId)
        {
            isDocumentComplete = false;
            anchorId = parentControlId + anchorId;
            HTMLAnchorElement input = GetAnchorElement(anchorId);
            input.click();
            WaitForComplete();
        }

        public void ClickAnchorWithValue(string anchorValue)
        {
            HTMLAnchorElement anchor = (HTMLAnchorElement)GetElementByValue("A", anchorValue);
            anchor.click();
            WaitForComplete();
        }

        public void ClickButton(string buttonId)
        {
            isDocumentComplete = false;
            HTMLInputElementClass input = GetInputElement(buttonId);
            input.click();
            WaitForComplete();
        }

        public void ClickCheckbox(string anchorId)
        {
            isDocumentComplete = false;
            HTMLInputElement input = GetCheckboxElement(anchorId);

            input.click();
        }

        public bool DoesElementExist(string elementId)
        {
            IHTMLElement input = GetElementById(elementId);
            return input != null;
        }

        public object GetElementAttribute(string elementId, string attributeName)
        {
            IHTMLElement element = GetElementById(elementId);
            if (element == null)
            {
                return null;
            }
            return element.getAttribute(attributeName, 0);
        }

        public IHTMLElement GetElementById(string elementId)
        {
            HTMLDocument document = ((HTMLDocument)IE.Document);
            IHTMLElement element = document.getElementById(elementId);

            int nullElementCount = 0;
            // The following loop is to account for any latency that IE
            // might experience.  Tweak the number of times to attempt
            // to continue checking the document before giving up.
            while (element == null && nullElementCount < 10)
            {
                Thread.Sleep(500);
                element = document.getElementById(elementId);
                nullElementCount++;
            }

            return element;
        }

        public bool doesTextExist(string text)
        {
            try
            {
                HTMLDocument document = ((HTMLDocument)IE.Document);
                return (document.body.innerText.IndexOf(text) > -1);
            }
            catch (Exception e)
            {
                return false;
            }

        }

        public string GetInputValue(string inputId)
        {
            HTMLInputElementClass input = GetInputElement(inputId);

            if (input == null)
            {
                return null;
            }

            return input.value;
        }

        public void Navigate(string url)
        {
            isDocumentComplete = false;
            object empty = "";
            IE.Navigate(url, ref empty, ref empty, ref empty, ref empty);
            WaitForComplete();
        }

        public void SetCheckboxValue(string checkboxId, bool isChecked, bool failIfNotExist)
        {
            HTMLInputElementClass input = GetInputElement(checkboxId);

            if (input == null && failIfNotExist)
            {
                throw new ApplicationException("CheckBox ID: " + checkboxId + " was not found.");
            }
            if (input != null)
            {
                input.@checked = isChecked;
            }
        }

        public void SetInputStringValue(string inputId, string elementValue)
        {
            HTMLInputElementClass input = GetInputElement(inputId);
            input.value = elementValue;
        }

        public void SetInputIntValue(string inputId, int elementValue)
        {
            HTMLInputElementClass input = GetInputElement(inputId);
            input.value = elementValue.ToString();
        }

        public void SelectValueByIndex(string inputId, int index)
        {
            HTMLSelectElementClass input = (HTMLSelectElementClass)GetSelectElement(inputId);
            input.selectedIndex = index;
        }

        public void SelectValueByValue(string inputId, string value)
        {
            bool found = false;
            int pass = 0;
            HTMLSelectElementClass input = (HTMLSelectElementClass)GetSelectElement(inputId);
            string teststring = input.IHTMLElement_innerHTML;

            value = "<OPTION>" + value + "</OPTION>";
            while (!found)
            {

                int index = teststring.IndexOf(value);
                if (index > -1 && index < 12)
                {
                    found = true;
                    input.selectedIndex = pass;
                    return;
                }
                else
                {
                    teststring = teststring.Substring(teststring.IndexOf("</OPTION>") + 9);
                    pass += 1;
                }
            }
        }

        public void ClickDefaultSubmit(string inputId)
        {

        }

        protected IHTMLElement GetElementByValue(string tagName, string elementValue)
        {
            int nullElementCount = 0;
            IHTMLElement element = GetElementByValueOnce(tagName, elementValue);

            // The following loop is to account for any latency that IE
            // might experience.  Tweak the number of times to attempt
            // to continue checking the document before giving up.
            while (element == null && nullElementCount < 10)
            {
                Thread.Sleep(500);
                element = GetElementByValueOnce(tagName, elementValue);
                nullElementCount++;
            }

            return element;
        }

        private HTMLAnchorElement GetAnchorElement(string inputId)
        {
            return (HTMLAnchorElement)GetElementById(inputId);
        }

        private HTMLInputElement GetCheckboxElement(string inputId)
        {
            return (HTMLInputElement)GetElementById(inputId);
        }

        private IHTMLElement GetElementByValueOnce(string tagName, string elementValue)
        {
            HTMLDocument document = ((HTMLDocument)IE.Document);
            IHTMLElementCollection tags = document.getElementsByTagName(tagName);

            IEnumerator enumerator = tags.GetEnumerator();

            while (enumerator.MoveNext())
            {
                IHTMLElement element = (IHTMLElement)enumerator.Current;
                if (element.innerText == elementValue)
                {
                    return element;
                }
            }

            return null;
        }

        private HTMLInputElementClass GetInputElement(string inputId)
        {
            return (HTMLInputElementClass)GetElementById(inputId);
        }

        private HTMLSelectElement GetSelectElement(string inputId)
        {
            return (HTMLSelectElement)GetElementById(inputId);
        }

        private void WaitForComplete()
        {
            int elapsedSeconds = 0;
            while (!isDocumentComplete && elapsedSeconds != TimeoutSeconds)
            {
                Thread.Sleep(1000);
                elapsedSeconds++;
            }
        }

        private void IE_DocumentComplete(object pDisp, ref object URL)
        {
            isDocumentComplete = true;
        }

        public void Quit()
        {
            IE.Quit();
        }
    }
}