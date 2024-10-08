using System;
using System.Linq;
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Qanat.EndpointTests.Helpers;
using Qanat.EndpointTests.Services.APIHelper;
using Qanat.Models.DataTransferObjects;
using ThreadingTask = System.Threading.Tasks.Task;

namespace Qanat.EndpointTests.Tests.UserClaims
{
    public partial class UserClaimsGet
    {
        [DataTestMethod]
        public async ThreadingTask CannotGetWithoutToken()
        {
            var users = await TestHelper.GetUsers(_validTokenWithPermission);
            Assert.IsNotNull(users);
            var user = users.First(x => x.FullName == "QanatAdmin User");

            var route = $"user-claims/{user.UserGuid}";
            var result = await APIHelper.Get<UserDto>(string.Empty, route);
            Assert.AreEqual(HttpStatusCode.Unauthorized, result.HttpResponseMessage.StatusCode);
            Assert.IsNull(result.HttpResponseObject);
        }

        [DataTestMethod]
        public async ThreadingTask CannotGetWithInvalidToken()
        {
            var users = await TestHelper.GetUsers(_validTokenWithPermission);
            Assert.IsNotNull(users);
            var user = users.First(x => x.FullName == "QanatAdmin User");

            var route = $"user-claims/{user.UserGuid}";
            var result = await APIHelper.Get<UserDto>($"Bearer {Guid.NewGuid()}", route);
            Assert.AreEqual(HttpStatusCode.Unauthorized, result.HttpResponseMessage.StatusCode);
            Assert.IsNull(result.HttpResponseObject);
        }
    }
}
