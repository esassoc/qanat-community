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
        public async ThreadingTask CannotPostCustomPageWithDuplicateDisplayName()
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
                ViewableRoleIDs = new List<int>() { 3 }
            };
            var postResult = await APIHelper.Post<CustomPageDto>(_validTokenWithPermission, postRoute, JsonConvert.SerializeObject(customPageUpsertDto));
            Assert.AreEqual(HttpStatusCode.OK, postResult.HttpResponseMessage.StatusCode);
            Assert.IsNotNull(postResult.HttpResponseObject);

            var guid2 = Guid.NewGuid().ToString();
            var customPageUpsertDto2 = new CustomPageUpsertDto()
            {
                CustomPageDisplayName = guid,  // same display name as above
                CustomPageVanityUrl = guid2,
                CustomPageContent = "Content2",
                MenuItemID = 2,
                SortOrder = 2,
                ViewableRoleIDs = new List<int>() { 3 }
            };
            var postResult2 = await APIHelper.Post<CustomPageDto>(_validTokenWithPermission, postRoute, JsonConvert.SerializeObject(customPageUpsertDto2));
            Assert.AreEqual(HttpStatusCode.BadRequest, postResult2.HttpResponseMessage.StatusCode);
            Assert.IsNull(postResult2.HttpResponseObject);
        }
    }

    public partial class CustomPagePost
    {
        [DataTestMethod]
        public async ThreadingTask CannotPostCustomPageWithDuplicateURL()
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
                ViewableRoleIDs = new List<int>() { 3 }
            };
            var postResult = await APIHelper.Post<CustomPageDto>(_validTokenWithPermission, postRoute, JsonConvert.SerializeObject(customPageUpsertDto));
            Assert.AreEqual(HttpStatusCode.OK, postResult.HttpResponseMessage.StatusCode);
            Assert.IsNotNull(postResult.HttpResponseObject);

            var guid2 = Guid.NewGuid().ToString();
            var customPageUpsertDto2 = new CustomPageUpsertDto()
            {
                CustomPageDisplayName = guid2,
                CustomPageVanityUrl = guid,  // same url as above
                CustomPageContent = "Content",
                MenuItemID = 1,
                SortOrder = 1,
                ViewableRoleIDs = new List<int>() { 3 }
            };
            var postResult2 = await APIHelper.Post<CustomPageDto>(_validTokenWithPermission, postRoute, JsonConvert.SerializeObject(customPageUpsertDto2));
            Assert.AreEqual(HttpStatusCode.BadRequest, postResult2.HttpResponseMessage.StatusCode);
            Assert.IsNull(postResult2.HttpResponseObject);
        }
    }
}
