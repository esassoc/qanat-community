using System.Collections.Generic;
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Qanat.EndpointTests.Services.APIHelper;
using Qanat.Models.DataTransferObjects;
using ThreadingTask = System.Threading.Tasks.Task;

namespace Qanat.EndpointTests.Tests.User
{
    public partial class UserListRead
    {
        [DataTestMethod]
        public async ThreadingTask CannotReadListWithoutPermission()
        {
            var route = "users";
            var result = await APIHelper.Get<List<UserDto>>(_validTokenWithoutPermission, route);
            Assert.AreEqual(HttpStatusCode.Forbidden, result.HttpResponseMessage.StatusCode);
            Assert.IsNull(result.HttpResponseObject);
        }
    }
}