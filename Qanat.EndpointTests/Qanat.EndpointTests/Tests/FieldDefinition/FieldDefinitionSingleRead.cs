using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Qanat.EndpointTests.Tests.FieldDefinition
{
    [TestClass]
    public partial class FieldDefinitionSingleRead
    {
        private static string _validTokenWithPermission = "";
        private static string _validTokenWithoutPermission = "";

        public FieldDefinitionSingleRead()
        {
        }

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            _validTokenWithPermission = AssemblySteps.TokenDictionary["Admin"];
            _validTokenWithoutPermission = AssemblySteps.TokenDictionary["Inactive"];
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
        }

        [TestInitialize]
        public void TestInitialize()
        {
        }

        [TestCleanup]
        public void TestCleanup()
        {
        }
    }
}