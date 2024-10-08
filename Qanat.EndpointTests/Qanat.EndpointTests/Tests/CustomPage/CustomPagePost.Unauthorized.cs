using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Qanat.EndpointTests.Services.APIHelper;
using Qanat.Models.DataTransferObjects;
using ThreadingTask = System.Threading.Tasks.Task;

namespace Qanat.EndpointTests.Tests.CustomPage
{
    public partial class CustomPagePost
    {
        [DataTestMethod]
        public async ThreadingTask CannotPostCustomPageWithoutToken()
        {
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
            var postResult = await APIHelper.Post<CustomPageDto>(string.Empty, postRoute, JsonConvert.SerializeObject(customPageUpsertDto));
            Assert.AreEqual(HttpStatusCode.Unauthorized, postResult.HttpResponseMessage.StatusCode);
            Assert.IsNull(postResult.HttpResponseObject);
        }

        [DataTestMethod]
        public async ThreadingTask CannotPostCustomPageWithInvalidToken()
        {
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
            var postResult = await APIHelper.Post<CustomPageDto>($"Bearer {Guid.NewGuid()}", postRoute, JsonConvert.SerializeObject(customPageUpsertDto));
            Assert.AreEqual(HttpStatusCode.Unauthorized, postResult.HttpResponseMessage.StatusCode);
            Assert.IsNull(postResult.HttpResponseObject);
        }
    }
}
