using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Qanat.EndpointTests.Services.APIHelper;
using Qanat.Models.DataTransferObjects;
using ThreadingTask = System.Threading.Tasks.Task;

namespace Qanat.EndpointTests.Tests.UserClaims
{
    public partial class UserClaimsPost
    {
        [DataTestMethod]
        public async ThreadingTask CanPostClaims()
        {
            var route = $"user-claims";
            var result = await APIHelper.Post<UserDto>(_validTokenWithPermission, route, JsonConvert.SerializeObject(null));
            Assert.AreEqual(HttpStatusCode.OK, result.HttpResponseMessage.StatusCode);
            Assert.IsNotNull(result.HttpResponseObject);
        }
    }
}
