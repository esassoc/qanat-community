using System.Collections.Generic;
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Qanat.EndpointTests.Services.APIHelper;
using Qanat.Models.DataTransferObjects;
using ThreadingTask = System.Threading.Tasks.Task;
using System;
using Newtonsoft.Json;

namespace Qanat.EndpointTests.Tests.CustomPage
{
    public partial class CustomPageRoles
    {
        [DataTestMethod]
        public async ThreadingTask CannotReadListWithoutToken()
        {
            // need to create a custom page, since by default there are not any
            var postRoute = "customPages";

            var guid = Guid.NewGuid().ToString();
            var customPageUpsertDto = new CustomPageUpsertDto()
            {
                CustomPageDisplayName = guid,
                CustomPageVanityUrl = guid,
                CustomPageContent = "Content",
                MenuItemID = 1,
                SortOrder = 1,
                ViewableRoleIDs = new List<int>() { 1 }
            };
            var postResult = await APIHelper.Post<CustomPageDto>(_validTokenWithPermission, postRoute, JsonConvert.SerializeObject(customPageUpsertDto));
            Assert.AreEqual(HttpStatusCode.OK, postResult.HttpResponseMessage.StatusCode);
            Assert.IsNotNull(postResult.HttpResponseObject);

            // now get roles
            var route = $"customPages/getByURL/{postResult.HttpResponseObject.CustomPageVanityUrl}/roles";
            var result = await APIHelper.Get<List<CustomPageRoleSimpleDto>>(string.Empty, route);
            Assert.AreEqual(HttpStatusCode.Unauthorized, result.HttpResponseMessage.StatusCode);
            Assert.IsNull(result.HttpResponseObject);
        }

        [DataTestMethod]
        public async ThreadingTask CannotReadListWithInvalidToken()
        {
            // need to create a custom page, since by default there are not any
            var postRoute = "customPages";

            var guid = Guid.NewGuid().ToString();
            var customPageUpsertDto = new CustomPageUpsertDto()
            {
                CustomPageDisplayName = guid,
                CustomPageVanityUrl = guid,
                CustomPageContent = "Content",
                MenuItemID = 1,
                SortOrder = 1,
                ViewableRoleIDs = new List<int>() { 1 }
            };
            var postResult = await APIHelper.Post<CustomPageDto>(_validTokenWithPermission, postRoute, JsonConvert.SerializeObject(customPageUpsertDto));
            Assert.AreEqual(HttpStatusCode.OK, postResult.HttpResponseMessage.StatusCode);
            Assert.IsNotNull(postResult.HttpResponseObject);

            // now get roles
            var route = $"customPages/getByURL/{postResult.HttpResponseObject.CustomPageVanityUrl}/roles";
            var result = await APIHelper.Get<List<CustomPageRoleSimpleDto>>($"Bearer {Guid.NewGuid()}", route);
            Assert.AreEqual(HttpStatusCode.Unauthorized, result.HttpResponseMessage.StatusCode);
            Assert.IsNull(result.HttpResponseObject);
        }
    }
}