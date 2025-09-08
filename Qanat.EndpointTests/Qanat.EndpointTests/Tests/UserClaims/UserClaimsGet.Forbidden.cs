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
        public async ThreadingTask CannotGetWithInvalidGuid()
        {
            var route = $"user-claims/invalid-guid";
            var result = await APIHelper.Get<UserDto>(_validTokenWithPermission, route);
            Assert.AreEqual(HttpStatusCode.Forbidden, result.HttpResponseMessage.StatusCode);
            Assert.IsNull(result.HttpResponseObject);
        }

        [DataTestMethod]
        public async ThreadingTask CannotGetWithNonMatchingGuid()
        {
            var route = $"user-claims/{Guid.NewGuid()}";
            var result = await APIHelper.Get<UserDto>(_validTokenWithPermission, route);
            Assert.AreEqual(HttpStatusCode.Forbidden, result.HttpResponseMessage.StatusCode);
            Assert.IsNull(result.HttpResponseObject);
        }

        [DataTestMethod]
        public async ThreadingTask CannotGetWithSomeoneElsesGuid()
        {
            var users = await TestHelper.GetUsers(_validTokenWithPermission);
            Assert.IsNotNull(users);
            var user = users.First(x => x.FullName != "QanatAdmin User");

            var route = $"user-claims/{user.UserGuid}";
            var result = await APIHelper.Get<UserDto>(_validTokenWithPermission, route);
            Assert.AreEqual(HttpStatusCode.Forbidden, result.HttpResponseMessage.StatusCode);
            Assert.IsNull(result.HttpResponseObject);
        }
    }
}
