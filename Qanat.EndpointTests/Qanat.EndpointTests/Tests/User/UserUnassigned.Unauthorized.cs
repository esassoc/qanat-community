using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Qanat.EndpointTests.Services.APIHelper;
using ThreadingTask = System.Threading.Tasks.Task;
using Qanat.Models.DataTransferObjects.User;
using System;

namespace Qanat.EndpointTests.Tests.User
{
    public partial class UserUnassigned
    {
        [DataTestMethod]
        public async ThreadingTask CannotReadListWithoutToken()
        {
            var route = "users/unassigned-report";
            var result = await APIHelper.Get<UnassignedUserReportDto>(string.Empty, route);
            Assert.AreEqual(HttpStatusCode.Unauthorized, result.HttpResponseMessage.StatusCode);
            Assert.IsNull(result.HttpResponseObject);
        }

        [DataTestMethod]
        public async ThreadingTask CannotReadListWithInvalidToken()
        {
            var route = "users/unassigned-report";
            var result = await APIHelper.Get<UnassignedUserReportDto>($"Bearer {Guid.NewGuid()}", route);
            Assert.AreEqual(HttpStatusCode.Unauthorized, result.HttpResponseMessage.StatusCode);
            Assert.IsNull(result.HttpResponseObject);
        }
    }
}