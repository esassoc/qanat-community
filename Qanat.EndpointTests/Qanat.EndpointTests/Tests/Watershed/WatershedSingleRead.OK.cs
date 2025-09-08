using System.Linq;
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Qanat.EndpointTests.Helpers;
using Qanat.EndpointTests.Services.APIHelper;
using Qanat.Models.DataTransferObjects;
using ThreadingTask = System.Threading.Tasks.Task;

namespace Qanat.EndpointTests.Tests.Watershed
{
    public partial class WatershedSingleRead
    {
        [DataTestMethod]
        public async ThreadingTask CanReadSingleWithValidToken()
        {
            // get a watershed to read
            var watersheds = await TestHelper.GetWatersheds(_validTokenWithPermission);
            Assert.IsNotNull(watersheds);
            var watershedID = watersheds.First().WatershedID;

            // read single watershed
            var route = $"watersheds/{watershedID}";
            var result = await APIHelper.Get<WatershedDto>(_validTokenWithPermission, route);
            Assert.AreEqual(HttpStatusCode.OK, result.HttpResponseMessage.StatusCode);
            Assert.IsNotNull(result.HttpResponseObject);
        }
    }
}
