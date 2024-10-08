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
        public async ThreadingTask CanReadSingleWithValidToken()
        {
            var users = await TestHelper.GetUsers(_validTokenWithPermission);
            Assert.IsNotNull(users);
            var userID = users.First().UserID;

            var route = $"users/{userID}";
            var result = await APIHelper.Get<UserDto>(_validTokenWithPermission, route);
            Assert.AreEqual(HttpStatusCode.OK, result.HttpResponseMessage.StatusCode);
            Assert.IsNotNull(result.HttpResponseObject);
        }
    }
}
