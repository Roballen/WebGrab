using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using ConsoleGrabit;
using ConsoleGrabit.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Utilities;

namespace WebGrabTests
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class NameParsingTests
    {
      

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void Should_Parse_Business_Names()
        {

            var businessnames = new List<string>();
            businessnames.Add("Larry's Garage Service");
            businessnames.Add("Henry Plumbing");
            businessnames.Add("Holland & Hart");
            businessnames.Add("Patent Offices of Rick Sanchez, P.C");
            businessnames.Add("Steve Wall, Attorney");
            businessnames.Add("FRVP");
            businessnames.Add("Dave's beef");

            foreach (var businessname in businessnames)
            {
                var lead = new Lead();
                NameHelper.ParseName(ref lead, businessname, true);
                Assert.IsFalse(lead.Businessname.IsEmpty(), businessname + " didn't parse to business name");
                Assert.IsTrue(lead.Last.IsEmpty() && lead.First.IsEmpty(), businessname + " parsed to incorrect place");
            }
        }

        [TestMethod]
        public void Should_Parse_2_token_name()
        {
            var lead = new Lead();
            NameHelper.ParseName(ref lead,"Rob Allen", false);
            Assert.IsTrue(lead.Businessname.IsEmpty() && lead.First == "Rob" && lead.Last == "Allen");
        }
        [TestMethod]
        public void Should_Parse_3_Token_Name()
        {
            var lead = new Lead();
            NameHelper.ParseName(ref lead, "Robert Milton Allen", false);
            Assert.IsTrue(lead.Businessname.IsEmpty() && lead.First == "Robert" && lead.Last == "Allen" && lead.Middle == "Milton");
        }
        [TestMethod]
        public void Should_Parse_Upper_Case_Name_ToProper()
        {
            var lead = new Lead();
            NameHelper.ParseName(ref lead, "ROBERT MILTON ALLEN", false);
            Assert.IsTrue(lead.Businessname.IsEmpty() && lead.First == "Robert" && lead.Last == "Allen" && lead.Middle == "Milton");
        }
        [TestMethod]
        public void Should_Last_Name_First_With_Comma_And_Just_First()
        {
            var lead = new Lead();
            NameHelper.ParseName(ref lead, "Allen, Rob", false);
            Assert.IsTrue(lead.Businessname.IsEmpty() && lead.First == "Rob" && lead.Last == "Allen");
        }
        [TestMethod]
        public void Should_Parse_Comma_Style_With_Full_Name()
        {
            var lead = new Lead();
            NameHelper.ParseName(ref lead, "Allen, Rob Milton", false);
            Assert.IsTrue(lead.Businessname.IsEmpty() && lead.First == "Rob" && lead.Last == "Allen" && lead.Middle == "Milton");
        }
        [TestMethod]
        public void Should_Parse_Comma_Style_With_Full_Name_With_Suffix()
        {
            var lead = new Lead();
            NameHelper.ParseName(ref lead, "Allen, Rob Milton III", false);
            Assert.IsTrue(lead.Businessname.IsEmpty() && lead.First == "Rob" && lead.Last == "Allen" && lead.Middle == "Milton");
        }
        [TestMethod]
        public void Should_Parse_Comma_Style_With_Full_Name_With_Suffix_And_Prefix()
        {
            var lead = new Lead();
            NameHelper.ParseName(ref lead, "Allen III, Mr Rob Milton", false);
            Assert.IsTrue(lead.Businessname.IsEmpty() && lead.First == "Rob" && lead.Last == "Allen" && lead.Middle == "Milton");
        }
    }
}
