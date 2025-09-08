using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Qanat.EndpointTests.Services.APIHelper;
using ThreadingTask = System.Threading.Tasks.Task;

namespace Qanat.EndpointTests.Tests.FileResource
{
    public partial class FileResourceDisplay
    {
        [DataTestMethod]
        public async ThreadingTask CannotGetFileResourceWithInvalidGuid()
        {
            var getRoute = "FileResource/invalid-file-resource-guid";

            var getResult = await APIHelper.Get<object>(_validTokenWithPermission, getRoute);
            Assert.AreEqual(HttpStatusCode.NotFound, getResult.HttpResponseMessage.StatusCode);
            Assert.IsNull(getResult.HttpResponseObject);
        }
    }
}
