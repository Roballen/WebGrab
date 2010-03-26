using System;
using System.Collections.Generic;
using System.IO;
using ConsoleGrabit;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ConsoleGrabit.Models;

namespace WebGrabTests
{
    
    [TestClass()]
    public class ImageOCRTest
    {
        private TestContext testContextInstance;

        public TestContext TestContext
        {
            get{ return testContextInstance;}
            set{ testContextInstance = value; }
        }

        [TestMethod()]
        public void OCR_Should_Get_Some_Words()
        {
            var filename = Path.Combine("c:\\pdf\\", "4588397_monroe_state.pdf");
            var target = new ImageOCR(filename);
            target.GetWords();
            Assert.IsTrue(target.Words.Count > 0, "Failed to get any words");
        }

        [TestMethod()]
        public void OCR_Should_Get_Some_Lines()
        {
            var filename = Path.Combine("c:\\pdf\\", "4588397_monroe_state.pdf");
            var target = new ImageOCR(filename);
            Assert.IsTrue(target.Lines.Count > 0, "Failed to get any lines");
        }

        [TestMethod()]
        public void What_Is_The_Encoding()
        {
            var chars = new char[] { (char)39, (char)96, (char)180, (char)8216, (char)8217, (char)8218 };


            var str = "1’i450-8791";
            foreach (var c in str)
            {
                var b = false;
                foreach (var c1 in chars)
                {
                    if (c.Equals(c1))
                        b = true;

                }

            }
        }

    }
}
