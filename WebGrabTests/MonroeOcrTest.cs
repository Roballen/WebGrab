using System.IO;
using ConsoleGrabit;
using ConsoleGrabit.OCR;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace WebGrabTests
{
    
    
    /// <summary>
    ///This is a test class for MonroeOcrTest and is intended
    ///to contain all MonroeOcrTest Unit Tests
    ///</summary>
    [TestClass()]
    public class MonroeOcrTest
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
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for IsDate
        ///</summary>
        [TestMethod()]
        [DeploymentItem("ConsoleGrabit.exe")]
        public void IsDateTest()
        {
            string sdate = "MARCH 08, 2010";
            bool expected = true; 
            bool actual;
            actual = BaseOcrParser.IsDate(sdate);
            Assert.AreEqual(expected, actual, "we didn't parse the date correctly");

        }

        [TestMethod]
        [DeploymentItem("ConsoleGrabit.exe")]
        public void Monroe_Vertical_Ocr_Should_Return_Valid_Lead()
        {
            var filename = Path.Combine("c:\\pdf\\", "4588397_monroe_state.pdf");
            var target = new MonroeOcr(filename,"Monroe");
            var lead =target.GetLeadFromOcr();
            Assert.IsTrue(lead.IsValid(),"Lead is not valid");
            Assert.IsTrue(lead.Recordeddate != "", "didn't get the recorded date");
        }

        [TestMethod]
        [DeploymentItem("ConsoleGrabit.exe")]
        public void Monroe_Vertical_Ocr_Should_Return_Valid_Lead_2()
        {
            var filename = Path.Combine("c:\\pdf\\", "40477_monroe_state.pdf");
            var target = new MonroeOcr(filename,"monroe");
            var lead =target.GetLeadFromOcr();
            Assert.IsTrue(lead.IsValid(),"Lead is not valid");
            Assert.IsTrue(lead.Recordeddate != "", "didn't get the recorded date");
        }

        [TestMethod]
        [DeploymentItem("ConsoleGrabit.exe")]
        public void Monroe_Horizontal_Ocr_Should_Return_Valid_Lead()
        {
            var filename = Path.Combine("c:\\pdf\\", "3820316_monroe_state.pdf");
            var target = new MonroeOcr(filename,"Monroe");
            var lead = target.GetLeadFromOcr();
            Assert.IsTrue(lead.IsValid(), "Lead is not valid");
            Assert.IsTrue(lead.Recordeddate != "", "didn't get the recorded date");

        }

        [TestMethod]
        [DeploymentItem("ConsoleGrabit.exe")]
        public void Federal_Ocr_Should_Return_Valid_Lead()
        {
            var filename = Path.Combine("c:\\pdf\\", "3171370_monroe_federal.pdf");
            var target = new Federal(filename, "monroe");
            var lead = target.GetLeadFromOcr();
            Assert.IsTrue(lead.IsValid(), "Lead is not valid");
            Assert.IsTrue(lead.Recordeddate != "", "didn't get the recorded date");

        }


    }
}
