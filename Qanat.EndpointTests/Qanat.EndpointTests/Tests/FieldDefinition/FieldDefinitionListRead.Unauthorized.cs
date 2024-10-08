using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Qanat.EndpointTests.Services.APIHelper;
using Qanat.Models.DataTransferObjects;
using ThreadingTask = System.Threading.Tasks.Task;

namespace Qanat.EndpointTests.Tests.FieldDefinition
{
    public partial class FieldDefinitionListRead
    {
        [DataTestMethod]
        public async ThreadingTask CannotReadListWithoutToken()
        {
            var route = "fieldDefinitions";
            var result = await APIHelper.Get<List<FieldDefinitionDto>>(string.Empty, route);
            Assert.AreEqual(HttpStatusCode.Unauthorized, result.HttpResponseMessage.StatusCode);
            Assert.IsNull(result.HttpResponseObject);
        }

        [DataTestMethod]
        public async ThreadingTask CannotReadListWithInvalidToken()
        {
            var route = "fieldDefinitions";
            var result = await APIHelper.Get<List<FieldDefinitionDto>>($"Bearer {Guid.NewGuid()}", route);
            Assert.AreEqual(HttpStatusCode.Unauthorized, result.HttpResponseMessage.StatusCode);
            Assert.IsNull(result.HttpResponseObject);
        }
    }
}