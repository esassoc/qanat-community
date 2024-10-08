using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Qanat.EndpointTests.Services.APIHelper;
using Qanat.Models.DataTransferObjects;
using ThreadingTask = System.Threading.Tasks.Task;

namespace Qanat.EndpointTests.Tests.Impersonation
{
    public partial class ImpersonationImpersonate
    {
        [DataTestMethod]
        public async ThreadingTask CannotImpersonateWithInvalidUserID()
        {
            var route = $"impersonate/{int.MaxValue}";
            var result = await APIHelper.Post<UserDto>(_validTokenWithPermission, route, JsonConvert.SerializeObject(null));
            Assert.AreEqual(HttpStatusCode.NotFound, result.HttpResponseMessage.StatusCode);
            Assert.IsNull(result.HttpResponseObject);
        }
    }
}
