using System.Collections.Generic;
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Qanat.EndpointTests.Services.APIHelper;
using Qanat.Models.DataTransferObjects;
using ThreadingTask = System.Threading.Tasks.Task;

namespace Qanat.EndpointTests.Tests.Watershed
{
    public partial class WatershedListRead
    {
        [DataTestMethod]
        public async ThreadingTask CanReadListWithValidToken()
        {
            var route = "watersheds";
            var result = await APIHelper.Get<List<WatershedDto>>(_validTokenWithPermission, route);
            Assert.AreEqual(HttpStatusCode.OK, result.HttpResponseMessage.StatusCode);
            Assert.IsNotNull(result.HttpResponseObject);
        }
    }
}