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
        public async ThreadingTask CannotReadListWithoutPermission()
        {
            var route = "fieldDefinitions";
            var result = await APIHelper.Get<List<FieldDefinitionDto>>(_validTokenWithoutPermission, route);
            Assert.AreEqual(HttpStatusCode.Forbidden, result.HttpResponseMessage.StatusCode);
            Assert.IsNull(result.HttpResponseObject);
        }
    }
}