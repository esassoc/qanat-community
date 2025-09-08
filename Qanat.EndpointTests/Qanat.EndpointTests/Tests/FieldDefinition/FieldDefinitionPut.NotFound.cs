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
        public async ThreadingTask CanPutWithInvalidFieldDefinitionTypeID(int fieldDefinitionTypeID)
        {
            var getRoute = $"fieldDefinitions/{fieldDefinitionTypeID}";
            var getResult = await APIHelper.Get<FieldDefinitionDto>(_validTokenWithPermission, getRoute);
            Assert.AreEqual(HttpStatusCode.OK, getResult.HttpResponseMessage.StatusCode);
            Assert.IsNotNull(getResult.HttpResponseObject);

            var fieldDefinition = getResult.HttpResponseObject;
            var fieldDefinitionDto = new FieldDefinitionDto()
            {
                FieldDefinitionID = fieldDefinition.FieldDefinitionID,
                FieldDefinitionType = fieldDefinition.FieldDefinitionType,
                FieldDefinitionValue = fieldDefinition.FieldDefinitionValue
            };

            var putRoute = $"fieldDefinitions/{int.MaxValue}";
            var putResult = await APIHelper.Put<FieldDefinitionDto>(_validTokenWithPermission, putRoute, JsonConvert.SerializeObject(fieldDefinitionDto));
            Assert.AreEqual(HttpStatusCode.NotFound, putResult.HttpResponseMessage.StatusCode);
            Assert.IsNull(putResult.HttpResponseObject);
        }
    }
}
