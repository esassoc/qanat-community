using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Qanat.EndpointTests.Services.APIHelper;
using Qanat.Models.DataTransferObjects;
using ThreadingTask = System.Threading.Tasks.Task;
using System;

namespace Qanat.EndpointTests.Tests.User
{
    public partial class UserDisclaimer
    {
        [DataTestMethod]
        public async ThreadingTask CannotPutWithoutToken()
        {
            var route = "users/set-disclaimer-acknowledged-date";
            var result = await APIHelper.Put<UserDto>(string.Empty, route, JsonConvert.SerializeObject(null));
            Assert.AreEqual(HttpStatusCode.Unauthorized, result.HttpResponseMessage.StatusCode);
            Assert.IsNull(result.HttpResponseObject);
        }

        [DataTestMethod]
        public async ThreadingTask CannotPutWithInvalidToken()
        {
            var route = "users/set-disclaimer-acknowledged-date";
            var result = await APIHelper.Put<UserDto>($"Bearer {Guid.NewGuid()}", route, JsonConvert.SerializeObject(null));
            Assert.AreEqual(HttpStatusCode.Unauthorized, result.HttpResponseMessage.StatusCode);
            Assert.IsNull(result.HttpResponseObject);
        }

    }
}
