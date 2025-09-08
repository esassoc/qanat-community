using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Qanat.EndpointTests.Services.APIHelper;
using Qanat.Models.DataTransferObjects;
using ThreadingTask = System.Threading.Tasks.Task;

namespace Qanat.EndpointTests.Tests.CustomPage
{
    public partial class CustomPageSingleRead
    {
        [DataTestMethod]
        public async ThreadingTask CannotReadSingleWithInvalidUrl()
        {
            var route = "customPages/getByURL/invalid-vanity-url";
            var result = await APIHelper.Get<CustomPageDto>(_validTokenWithPermission, route);
            Assert.AreEqual(HttpStatusCode.NotFound, result.HttpResponseMessage.StatusCode);
            Assert.IsNull(result.HttpResponseObject);
        }
    }
}
