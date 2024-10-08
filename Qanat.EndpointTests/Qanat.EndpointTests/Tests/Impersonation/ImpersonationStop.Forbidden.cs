using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Qanat.EndpointTests.Services.APIHelper;
using Qanat.Models.DataTransferObjects;
using ThreadingTask = System.Threading.Tasks.Task;

namespace Qanat.EndpointTests.Tests.Impersonation
{
    public partial class ImpersonationStop
    {
        [DataTestMethod]
        public async ThreadingTask CannotStopWithoutPermission()
        {
            var route = $"/impersonate/stop-impersonation";
            var result = await APIHelper.Post<UserDto>(_validTokenWithoutPermission, route, JsonConvert.SerializeObject(null));
            Assert.AreEqual(HttpStatusCode.Forbidden, result.HttpResponseMessage.StatusCode);
            Assert.IsNull(result.HttpResponseObject);
        }

        [DataTestMethod]
        public async ThreadingTask CannotStopIfNotImpersonating()
        {
            var route = $"/impersonate/stop-impersonation";
            var result = await APIHelper.Post<UserDto>(_validTokenWithPermission, route, JsonConvert.SerializeObject(null));
            Assert.AreEqual(HttpStatusCode.Forbidden, result.HttpResponseMessage.StatusCode);
            Assert.IsNull(result.HttpResponseObject);
        }
    }
}
