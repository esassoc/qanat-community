using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Qanat.EndpointTests.Services.APIHelper;
using Qanat.Models.DataTransferObjects;
using ThreadingTask = System.Threading.Tasks.Task;
using Qanat.Models.DataTransferObjects.Watershed;
using System.Collections.Generic;
using Newtonsoft.Json;
using System;

namespace Qanat.EndpointTests.Tests.Watershed
{
    public partial class WatershedBoundingBox
    {
        [DataTestMethod]
        public async ThreadingTask CannotPostWithoutToken()
        {
            var route = "watersheds/getBoundingBox";
            var watershedIDListDto = new WatershedIDListDto()
            {
                WatershedIDs = new List<int>() { 2, 3, 4 }
            };
            var result = await APIHelper.Post<BoundingBoxDto>(string.Empty, route, JsonConvert.SerializeObject(watershedIDListDto));
            Assert.AreEqual(HttpStatusCode.Unauthorized, result.HttpResponseMessage.StatusCode);
            Assert.IsNull(result.HttpResponseObject);
        }

        [DataTestMethod]
        public async ThreadingTask CannotPostWithInvalidToken()
        {
            var route = "watersheds/getBoundingBox";
            var watershedIDListDto = new WatershedIDListDto()
            {
                WatershedIDs = new List<int>() { 2, 3, 4 }
            };
            var result = await APIHelper.Post<BoundingBoxDto>($"Bearer {Guid.NewGuid()}", route, JsonConvert.SerializeObject(watershedIDListDto));
            Assert.AreEqual(HttpStatusCode.Unauthorized, result.HttpResponseMessage.StatusCode);
            Assert.IsNull(result.HttpResponseObject);
        }
    }
}
