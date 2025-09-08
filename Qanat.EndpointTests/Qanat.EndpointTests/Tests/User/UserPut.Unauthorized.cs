using System;
using System.Linq;
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Qanat.EndpointTests.Helpers;
using Qanat.EndpointTests.Services.APIHelper;
using Qanat.Models.DataTransferObjects;
using Qanat.Models.DataTransferObjects.User;
using ThreadingTask = System.Threading.Tasks.Task;

namespace Qanat.EndpointTests.Tests.User
{
    public partial class UserPut
    {
        [DataTestMethod]
        public async ThreadingTask CannotPutWithoutToken()
        {
            var users = await TestHelper.GetUsers(_validTokenWithPermission);
            Assert.IsNotNull(users);
            var user = users.First();

            var route = $"users/{user.UserID}";
            var userUpsertDto = new UserUpsertDto()  // we are updating an actual user, so don't actually change anything
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                OrganizationName = user.Company,
                Email = user.Email,
                PhoneNumber = user.Phone,
                RoleID = user.Role.RoleID,
                ReceiveSupportEmails = user.ReceiveSupportEmails
            };

            var result = await APIHelper.Put<UserDto>(string.Empty, route, JsonConvert.SerializeObject(userUpsertDto));
            Assert.AreEqual(HttpStatusCode.Unauthorized, result.HttpResponseMessage.StatusCode);
            Assert.IsNull(result.HttpResponseObject);
        }

        [DataTestMethod]
        public async ThreadingTask CannotPutWithInvalidToken()
        {
            var users = await TestHelper.GetUsers(_validTokenWithPermission);
            Assert.IsNotNull(users);
            var user = users.First();

            var route = $"users/{user.UserID}";
            var userUpsertDto = new UserUpsertDto()  // we are updating an actual user, so don't actually change anything
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                OrganizationName = user.Company,
                Email = user.Email,
                PhoneNumber = user.Phone,
                RoleID = user.Role.RoleID,
                ReceiveSupportEmails = user.ReceiveSupportEmails
            };

            var result = await APIHelper.Put<UserDto>($"Bearer {Guid.NewGuid()}", route, JsonConvert.SerializeObject(userUpsertDto));
            Assert.AreEqual(HttpStatusCode.Unauthorized, result.HttpResponseMessage.StatusCode);
            Assert.IsNull(result.HttpResponseObject);
        }
    }
}
