using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Qanat.EndpointTests.Services.APIHelper;
using Qanat.Models.DataTransferObjects;
using ThreadingTask = System.Threading.Tasks.Task;

namespace Qanat.EndpointTests.Tests.FieldDefinition
{
    public partial class FieldDefinitionPut
    {
        [DataTestMethod]
        [DataRow(1)]
        public async ThreadingTask CannotPutWithNullBody(int fieldDefinitionTypeID)
        {
            var route = $"fieldDefinitions/{fieldDefinitionTypeID}";
            var putResult = await APIHelper.Put<FieldDefinitionDto>(_validTokenWithPermission, route, JsonConvert.SerializeObject(null));
            Assert.AreEqual(HttpStatusCode.BadRequest, putResult.HttpResponseMessage.StatusCode);
            Assert.IsNull(putResult.HttpResponseObject);
        }
    }
}
