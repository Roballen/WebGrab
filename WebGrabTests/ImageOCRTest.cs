using System;
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
            var filename = Path.Combine(Environment.CurrentDirectory, "test_doc1.tif");
            var target = new ImageOCR(filename);
            target.GetWords();
            Assert.IsTrue(target.Words.Count > 0, "Failed to get any words");
        }
    }
}
