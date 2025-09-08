using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Qanat.EndpointTests.Services.APIHelper;
using Qanat.Models.DataTransferObjects;
using ThreadingTask = System.Threading.Tasks.Task;
using Newtonsoft.Json;

namespace Qanat.EndpointTests.Tests.Watershed
{
    public partial class WatershedBoundingBox
    {
        // MCS: The pattern established in other Qanat based apps it to just create a test for the null body case, since this check is done in the controller
        // The validation method is then tested by the integration tests

        [DataTestMethod]
        public async ThreadingTask CannotPostWithNullBody()
        {
            var route = "watersheds/getBoundingBox";
            var result = await APIHelper.Post<BoundingBoxDto>(_validTokenWithPermission, route, JsonConvert.SerializeObject(null));
            Assert.AreEqual(HttpStatusCode.BadRequest, result.HttpResponseMessage.StatusCode);
            Assert.IsNull(result.HttpResponseObject);
        }
    }
}

