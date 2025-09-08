using System;
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
        public async ThreadingTask CannotStopWithoutToken()
        {
            var route = $"/impersonate/stop-impersonation";
            var result = await APIHelper.Post<UserDto>(string.Empty, route, JsonConvert.SerializeObject(null));
            Assert.AreEqual(HttpStatusCode.Unauthorized, result.HttpResponseMessage.StatusCode);
            Assert.IsNull(result.HttpResponseObject);
        }

        [DataTestMethod]
        public async ThreadingTask CannotStopWithInvalidToken()
        {
            var route = $"/impersonate/stop-impersonation";
            var result = await APIHelper.Post<UserDto>($"Bearer {Guid.NewGuid()}", route, JsonConvert.SerializeObject(null));
            Assert.AreEqual(HttpStatusCode.Unauthorized, result.HttpResponseMessage.StatusCode);
            Assert.IsNull(result.HttpResponseObject);
        }
    }
}
