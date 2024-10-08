using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Qanat.EndpointTests.Services.APIHelper;
using Qanat.Models.DataTransferObjects;
using ThreadingTask = System.Threading.Tasks.Task;

namespace Qanat.EndpointTests.Tests.FieldDefinition
{
    public partial class FieldDefinitionSingleRead
    {
        [DataTestMethod]
        [DataRow(1)]
        public async ThreadingTask CanReadSingleWithValidToken(int fieldDefinitionTypeID)  // field definitions are weird; the route parameter is a fieldDefinitionTypeID...  so just hard code
        {
            var route = $"fieldDefinitions/{fieldDefinitionTypeID}";
            var result = await APIHelper.Get<FieldDefinitionDto>(_validTokenWithPermission, route);
            Assert.AreEqual(HttpStatusCode.OK, result.HttpResponseMessage.StatusCode);
            Assert.IsNotNull(result.HttpResponseObject);
        }
    }
}
