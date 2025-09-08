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
        public async ThreadingTask CannotPutWithoutPermission(int fieldDefinitionTypeID)
        {
            var route = $"fieldDefinitions/{fieldDefinitionTypeID}";
            var getResult = await APIHelper.Get<FieldDefinitionDto>(_validTokenWithPermission, route);
            Assert.AreEqual(HttpStatusCode.OK, getResult.HttpResponseMessage.StatusCode);
            Assert.IsNotNull(getResult.HttpResponseObject);

            var fieldDefinition = getResult.HttpResponseObject;
            var fieldDefinitionDto = new FieldDefinitionDto()
            {
                FieldDefinitionID = fieldDefinition.FieldDefinitionID,
                FieldDefinitionType = fieldDefinition.FieldDefinitionType,
                FieldDefinitionValue = fieldDefinition.FieldDefinitionValue
            };

            var putResult = await APIHelper.Put<FieldDefinitionDto>(_validTokenWithoutPermission, route, JsonConvert.SerializeObject(fieldDefinitionDto));
            Assert.AreEqual(HttpStatusCode.Forbidden, putResult.HttpResponseMessage.StatusCode);
            Assert.IsNull(putResult.HttpResponseObject);
        }
    }
}
