using System;
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
        public async ThreadingTask CannotReadSingleWithoutToken(int fieldDefinitionTypeID)  // field definitions are weird; the route parameter is a fieldDefinitionTypeID...  so just hard code
        {
            var route = $"fieldDefinitions/{fieldDefinitionTypeID}";
            var result = await APIHelper.Get<FieldDefinitionDto>(string.Empty, route);
            Assert.AreEqual(HttpStatusCode.Unauthorized, result.HttpResponseMessage.StatusCode);
            Assert.IsNull(result.HttpResponseObject);
        }

        [DataTestMethod]
        [DataRow(1)]
        public async ThreadingTask CannotReadSingleWithInvalidToken(int fieldDefinitionTypeID)  // field definitions are weird; the route parameter is a fieldDefinitionTypeID...  so just hard code
        {
            var route = $"fieldDefinitions/{fieldDefinitionTypeID}";
            var result = await APIHelper.Get<FieldDefinitionDto>($"Bearer {Guid.NewGuid()}", route);
            Assert.AreEqual(HttpStatusCode.Unauthorized, result.HttpResponseMessage.StatusCode);
            Assert.IsNull(result.HttpResponseObject);
        }
    }
}
