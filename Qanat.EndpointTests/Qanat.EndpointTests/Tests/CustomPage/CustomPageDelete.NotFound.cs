using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Qanat.EndpointTests.Services.APIHelper;
using Qanat.Models.DataTransferObjects;
using ThreadingTask = System.Threading.Tasks.Task;

namespace Qanat.EndpointTests.Tests.CustomPage
{
    public partial class CustomPageDelete
    {
        [DataTestMethod]
        public async ThreadingTask CannotDeleteWithInvalidCustomPageID()
        {
            var route = $"customPages/{int.MaxValue}";
            var result = await APIHelper.Delete<CustomPageDto>(_validTokenWithPermission, route);
            Assert.AreEqual(HttpStatusCode.NotFound, result.HttpResponseMessage.StatusCode);
            Assert.IsNull(result.HttpResponseObject);
        }
    }
}