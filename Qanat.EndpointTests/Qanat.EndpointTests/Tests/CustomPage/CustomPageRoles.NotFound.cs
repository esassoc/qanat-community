using System.Collections.Generic;
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Qanat.EndpointTests.Services.APIHelper;
using Qanat.Models.DataTransferObjects;
using ThreadingTask = System.Threading.Tasks.Task;

namespace Qanat.EndpointTests.Tests.CustomPage
{
    public partial class CustomPageRoles
    {
        [DataTestMethod]
        public async ThreadingTask CannotReadListWithInvalidUrl()
        {
            var route = $"customPages/getByURL/invalid-vanity-url/roles";
            var result = await APIHelper.Get<List<CustomPageRoleSimpleDto>>(_validTokenWithPermission, route);
            Assert.AreEqual(HttpStatusCode.NotFound, result.HttpResponseMessage.StatusCode);
            Assert.IsNull(result.HttpResponseObject);
        }
    }
}