using System;
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
        public async ThreadingTask CannotPostClaimsWithoutToken()
        {
            var route = $"user-claims";
            var result = await APIHelper.Post<UserDto>(string.Empty, route, JsonConvert.SerializeObject(null));
            Assert.AreEqual(HttpStatusCode.Unauthorized, result.HttpResponseMessage.StatusCode);
            Assert.IsNull(result.HttpResponseObject);
        }

        [DataTestMethod]
        public async ThreadingTask CannotPostClaimsWithInvalidToken()
        {
            var route = $"user-claims";
            var result = await APIHelper.Post<UserDto>($"Bearer {Guid.NewGuid()}", route, JsonConvert.SerializeObject(null));
            Assert.AreEqual(HttpStatusCode.Unauthorized, result.HttpResponseMessage.StatusCode);
            Assert.IsNull(result.HttpResponseObject);
        }
    }
}
