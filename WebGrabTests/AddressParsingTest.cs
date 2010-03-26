using Microsoft.VisualStudio.TestTools.UnitTesting;
using ConsoleGrabit.AddressParsing;
namespace WebGrabTests
{
    
    
    /// <summary>
    ///This is a test class for AddressParsingTest and is intended
    ///to contain all AddressParsingTest Unit Tests
    ///</summary>
    [TestClass()]
    public class AddressParsingTest
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
        ///A test for RequiresUnit
        ///</summary>
        [TestMethod()]
        public void RequiresUnitTest()
        {
            Assert.IsTrue(AddressParsing.RequiresUnit("Unit"), "Unit requires a numbered unit");
            Assert.IsFalse(AddressParsing.RequiresUnit("BASEMENT"), "Unit requires a numbered unit");
        }

        /// <summary>
        ///A test for IsSecondaryUnit
        ///</summary>
        [TestMethod()]
        public void IsSecondaryUnitTest()
        {
            Assert.IsTrue(AddressParsing.IsSecondaryUnit("APT"), "Apartment is a secondary unit");
            Assert.IsFalse(AddressParsing.IsSecondaryUnit("Backyard"), "back yard is not a secondary unit");
        }

        [TestMethod()]
        public void SuffixesTest()
        {

            //            var path = @"C:\git\dev\WebGrab\ConsoleGrabit\AddressParsing\SecondaryUnits.txt";
            //            var list = new List<string>();
            //
            //            FileTools.SLoadFileToArray(path, ref list);
            //            var newlist = new Collection<string>();
            //            foreach (var suffix in list)
            //            {
            //                newlist.Add(suffix.Trim());
            //            }
            //
            //            FileTools.SSaveArrayToFile(@"C:\git\dev\WebGrab\ConsoleGrabit\AddressParsing\SecondaryUnits.txt", newlist);

            Assert.IsTrue(AddressParsing.IsStreetSuffix("Road"), "failed to load the address suffix list");
            Assert.IsTrue(AddressParsing.IsStreetSuffix("Rd"), "failed to load the address suffix list");
            Assert.IsTrue(AddressParsing.IsStreetSuffix("BLVD"), "failed to load the address suffix list");
            Assert.IsTrue(AddressParsing.IsStreetSuffix("Alley"), "failed to load the address suffix list");
            Assert.IsTrue(AddressParsing.IsStreetSuffix("St"), "failed to load the address suffix list");
            Assert.IsTrue(AddressParsing.IsStreetSuffix("STREET"), "failed to load the address suffix list");
            Assert.IsTrue(AddressParsing.IsStreetSuffix("CIR"), "failed to load the address suffix list");

        }
    }
}
