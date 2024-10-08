using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Qanat.EndpointTests.Services.APIHelper;
using Qanat.Models.DataTransferObjects;
using ThreadingTask = System.Threading.Tasks.Task;
using Qanat.Models.DataTransferObjects.Watershed;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Qanat.EndpointTests.Tests.Watershed
{
    public partial class WatershedBoundingBox
    {
        [DataTestMethod]
        public async ThreadingTask CanPostWithValidToken()
        {
            var route = "watersheds/getBoundingBox";
            var watershedIDListDto = new WatershedIDListDto()
            {
                WatershedIDs = new List<int>() { 2, 3, 4 }
            };
            var result = await APIHelper.Post<BoundingBoxDto>(_validTokenWithPermission, route, JsonConvert.SerializeObject(watershedIDListDto));
            Assert.AreEqual(HttpStatusCode.OK, result.HttpResponseMessage.StatusCode);
            Assert.IsNotNull(result.HttpResponseObject);
        }
    }
}
