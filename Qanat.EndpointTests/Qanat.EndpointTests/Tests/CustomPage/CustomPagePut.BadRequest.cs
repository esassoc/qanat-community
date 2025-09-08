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
    public partial class CustomPagePut
    {
        [DataTestMethod]
        public async ThreadingTask CannotPutCustomPageWithDuplicateDisplayNameOrUrl()
        {
            var postRoute = "customPages";

            // custom page 1
            var guid1 = Guid.NewGuid().ToString();
            var customPageUpsertDto1 = new CustomPageUpsertDto()
            {
                CustomPageDisplayName = guid1,
                CustomPageVanityUrl = guid1,
                CustomPageContent = "Content",
                MenuItemID = 1,
                SortOrder = 1,
                ViewableRoleIDs = new List<int>() { 3 }
            };
            var postResult1 = await APIHelper.Post<CustomPageDto>(_validTokenWithPermission, postRoute, JsonConvert.SerializeObject(customPageUpsertDto1));
            Assert.AreEqual(HttpStatusCode.OK, postResult1.HttpResponseMessage.StatusCode);
            Assert.IsNotNull(postResult1.HttpResponseObject);

            // custom page 2
            var guid2 = Guid.NewGuid().ToString();
            var customPageUpsertDto2 = new CustomPageUpsertDto()
            {
                CustomPageDisplayName = guid2,
                CustomPageVanityUrl = guid2,
                CustomPageContent = "Content",
                MenuItemID = 2,
                SortOrder = 2,
                ViewableRoleIDs = new List<int>() { 3 }
            };
            var postResult2 = await APIHelper.Post<CustomPageDto>(_validTokenWithPermission, postRoute, JsonConvert.SerializeObject(customPageUpsertDto2));
            Assert.AreEqual(HttpStatusCode.OK, postResult2.HttpResponseMessage.StatusCode);
            Assert.IsNotNull(postResult2.HttpResponseObject);

            // try to update page 2 so it has the same name as page 1
            var putRoute = $"customPages/{postResult2.HttpResponseObject.CustomPageID}";

            customPageUpsertDto2.CustomPageDisplayName = guid1;
            var putResult = await APIHelper.Put<CustomPageDto>(_validTokenWithPermission, putRoute, JsonConvert.SerializeObject(customPageUpsertDto2));
            Assert.AreEqual(HttpStatusCode.BadRequest, putResult.HttpResponseMessage.StatusCode);
            Assert.IsNull(putResult.HttpResponseObject);

            // try to update page 2 so it has the same url as page 1
            customPageUpsertDto2.CustomPageDisplayName = guid2;
            customPageUpsertDto2.CustomPageVanityUrl = guid1;
            putResult = await APIHelper.Put<CustomPageDto>(_validTokenWithPermission, putRoute, JsonConvert.SerializeObject(customPageUpsertDto2));
            Assert.AreEqual(HttpStatusCode.BadRequest, putResult.HttpResponseMessage.StatusCode);
            Assert.IsNull(putResult.HttpResponseObject);
        }

    }
}
