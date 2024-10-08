using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Qanat.EndpointTests.Services.APIHelper;
using Qanat.Models.DataTransferObjects;
using ThreadingTask = System.Threading.Tasks.Task;

namespace Qanat.EndpointTests.Tests.Watershed
{
    public partial class WatershedSingleRead
    {
        [DataTestMethod]
        public async ThreadingTask CannotReadSingleWithInvalidWatersheID()
        {
            var route = $"watersheds/{int.MaxValue}";
            var result = await APIHelper.Get<WatershedDto>(_validTokenWithPermission, route);
            Assert.AreEqual(HttpStatusCode.NotFound, result.HttpResponseMessage.StatusCode);
            Assert.IsNull(result.HttpResponseObject);
        }
    }
}
