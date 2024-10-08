using System;
using System.Linq;
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Qanat.EndpointTests.Helpers;
using Qanat.EndpointTests.Services.APIHelper;
using Qanat.Models.DataTransferObjects;
using ThreadingTask = System.Threading.Tasks.Task;

namespace Qanat.EndpointTests.Tests.Impersonation
{
    public partial class ImpersonationImpersonate
    {
        [DataTestMethod]
        public async ThreadingTask CannotImpersonateWithoutToken()
        {
            var users = await TestHelper.GetUsers(_validTokenWithPermission);
            Assert.IsNotNull(users);
            var impersonatedUser = users.First();

            var route = $"impersonate/{impersonatedUser.UserID}";
            var result = await APIHelper.Post<UserDto>(string.Empty, route, JsonConvert.SerializeObject(null));
            Assert.AreEqual(HttpStatusCode.Unauthorized, result.HttpResponseMessage.StatusCode);
            Assert.IsNull(result.HttpResponseObject);
        }

        [DataTestMethod]
        public async ThreadingTask CannotImpersonateWithInvalidToken()
        {
            var users = await TestHelper.GetUsers(_validTokenWithPermission);
            Assert.IsNotNull(users);
            var impersonatedUser = users.First();

            var route = $"impersonate/{impersonatedUser.UserID}";
            var result = await APIHelper.Post<UserDto>($"Bearer {Guid.NewGuid()}", route, JsonConvert.SerializeObject(null));
            Assert.AreEqual(HttpStatusCode.Unauthorized, result.HttpResponseMessage.StatusCode);
            Assert.IsNull(result.HttpResponseObject);
        }
    }
}
