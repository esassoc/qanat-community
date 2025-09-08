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
        public async ThreadingTask CannotImpersonateWithoutPermission()
        {
            var users = await TestHelper.GetUsers(_validTokenWithPermission);
            Assert.IsNotNull(users);
            var impersonatedUser = users.First();

            var route = $"impersonate/{impersonatedUser.UserID}";
            var result = await APIHelper.Post<UserDto>(_validTokenWithoutPermission, route, JsonConvert.SerializeObject(null));
            Assert.AreEqual(HttpStatusCode.Forbidden, result.HttpResponseMessage.StatusCode);
            Assert.IsNull(result.HttpResponseObject);
        }
    }
}
