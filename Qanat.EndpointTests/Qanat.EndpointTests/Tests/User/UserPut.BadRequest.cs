using System.Linq;
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Qanat.EndpointTests.Helpers;
using Qanat.EndpointTests.Services.APIHelper;
using Qanat.Models.DataTransferObjects;
using ThreadingTask = System.Threading.Tasks.Task;

namespace Qanat.EndpointTests.Tests.User
{
    public partial class UserPut
    {
        [DataTestMethod]
        public async ThreadingTask CannotPutWithNullBody()
        {
            var users = await TestHelper.GetUsers(_validTokenWithPermission);
            Assert.IsNotNull(users);
            var user = users.First();

            var route = $"users/{user.UserID}";

            var result = await APIHelper.Put<UserDto>(_validTokenWithPermission, route, JsonConvert.SerializeObject(null));
            Assert.AreEqual(HttpStatusCode.BadRequest, result.HttpResponseMessage.StatusCode);
            Assert.IsNull(result.HttpResponseObject);
        }
    }
}
