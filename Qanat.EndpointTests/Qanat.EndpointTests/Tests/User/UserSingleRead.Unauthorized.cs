using System;
using System.Linq;
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Qanat.EndpointTests.Helpers;
using Qanat.EndpointTests.Services.APIHelper;
using Qanat.Models.DataTransferObjects;
using ThreadingTask = System.Threading.Tasks.Task;

namespace Qanat.EndpointTests.Tests.User
{
    public partial class UserSingleRead
    {
        [DataTestMethod]
        public async ThreadingTask CannotReadSingleWithoutToken()
        {
            var users = await TestHelper.GetUsers(_validTokenWithPermission);
            Assert.IsNotNull(users);
            var userID = users.First().UserID;

            var route = $"users/{userID}";
            var result = await APIHelper.Get<UserDto>(string.Empty, route);
            Assert.AreEqual(HttpStatusCode.Unauthorized, result.HttpResponseMessage.StatusCode);
            Assert.IsNull(result.HttpResponseObject);
        }

        [DataTestMethod]
        public async ThreadingTask CannotReadSingleWithInvalidToken()
        {
            var users = await TestHelper.GetUsers(_validTokenWithPermission);
            Assert.IsNotNull(users);
            var userID = users.First().UserID;

            var route = $"users/{userID}";
            var result = await APIHelper.Get<UserDto>($"Bearer {Guid.NewGuid()}", route);
            Assert.AreEqual(HttpStatusCode.Unauthorized, result.HttpResponseMessage.StatusCode);
            Assert.IsNull(result.HttpResponseObject);
        }
    }
}
