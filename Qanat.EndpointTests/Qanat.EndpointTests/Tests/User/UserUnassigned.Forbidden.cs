using System.Collections.Generic;
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
        public async ThreadingTask CannotReadListWithoutPermission()
        {
            var route = "users/unassigned-report";
            var result = await APIHelper.Get<UnassignedUserReportDto>(_validTokenWithoutPermission, route);
            Assert.AreEqual(HttpStatusCode.Forbidden, result.HttpResponseMessage.StatusCode);
            Assert.IsNull(result.HttpResponseObject);
        }
    }
}