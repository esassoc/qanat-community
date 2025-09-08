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
        public async ThreadingTask CanGetWithMatchingGuid()
        {
            var users = await TestHelper.GetUsers(_validTokenWithPermission);
            Assert.IsNotNull(users);
            var user = users.First(x => x.FullName == "QanatAdmin User");

            var route = $"user-claims/{user.UserGuid}";
            var result = await APIHelper.Get<UserDto>(_validTokenWithPermission, route);
            Assert.AreEqual(HttpStatusCode.OK, result.HttpResponseMessage.StatusCode);
            Assert.IsNotNull(result.HttpResponseObject);
        }
    }
}
