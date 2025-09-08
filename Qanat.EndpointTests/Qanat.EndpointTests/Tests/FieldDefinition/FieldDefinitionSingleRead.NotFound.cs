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
        public async ThreadingTask CannotReadSingleWithInvalidFieldDefinitionTypeID()  // field definitions are weird; the route parameter is a fieldDefinitionTypeID...  so just hard code
        {
            var route = $"fieldDefinitions/{int.MaxValue}";
            var result = await APIHelper.Get<FieldDefinitionDto>(_validTokenWithPermission, route);
            Assert.AreEqual(HttpStatusCode.NotFound, result.HttpResponseMessage.StatusCode);
            Assert.IsNull(result.HttpResponseObject);
        }
    }
}
