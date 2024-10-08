using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Qanat.EndpointTests.Services.APIHelper;
using ThreadingTask = System.Threading.Tasks.Task;
using Qanat.Models.DataTransferObjects.User;

namespace Qanat.EndpointTests.Tests.User
{
    public partial class UserUnassigned
    {
        [DataTestMethod]
        public async ThreadingTask CanReadListWithValidToken()
        {
            var route = "users/unassigned-report";
            var result = await APIHelper.Get<UnassignedUserReportDto>(_validTokenWithPermission, route);
            Assert.AreEqual(HttpStatusCode.OK, result.HttpResponseMessage.StatusCode);
            Assert.IsNotNull(result.HttpResponseObject);
        }
    }
}