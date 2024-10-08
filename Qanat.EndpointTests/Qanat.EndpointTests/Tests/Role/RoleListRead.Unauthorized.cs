using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Qanat.EndpointTests.Services.APIHelper;
using Qanat.Models.DataTransferObjects;
using ThreadingTask = System.Threading.Tasks.Task;

namespace Qanat.EndpointTests.Tests.Role
{
    public partial class RoleListRead
    {
        [DataTestMethod]
        public async ThreadingTask CannotReadListWithoutToken()
        {
            var route = "roles";
            var result = await APIHelper.Get<List<RoleDto>>(string.Empty, route);
            Assert.AreEqual(HttpStatusCode.Unauthorized, result.HttpResponseMessage.StatusCode);
            Assert.IsNull(result.HttpResponseObject);
        }

        [DataTestMethod]
        public async ThreadingTask CannotReadListWithInvalidToken()
        {
            var route = "roles";
            var result = await APIHelper.Get<List<RoleDto>>($"Bearer {Guid.NewGuid()}", route);
            Assert.AreEqual(HttpStatusCode.Unauthorized, result.HttpResponseMessage.StatusCode);
            Assert.IsNull(result.HttpResponseObject);
        }
    }
}