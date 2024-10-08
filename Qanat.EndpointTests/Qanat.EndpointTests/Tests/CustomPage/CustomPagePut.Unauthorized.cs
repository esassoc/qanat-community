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
        public async ThreadingTask CannotPutCustomPageWithoutToken()
        {
            // create page to later update
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

            // now update it
            var putRoute = $"customPages/{postResult.HttpResponseObject.CustomPageID}";

            var putResult = await APIHelper.Put<CustomPageDto>(string.Empty, putRoute, JsonConvert.SerializeObject(customPageUpsertDto));
            Assert.AreEqual(HttpStatusCode.Unauthorized, putResult.HttpResponseMessage.StatusCode);
            Assert.IsNull(putResult.HttpResponseObject);
        }

        [DataTestMethod]
        public async ThreadingTask CannotPutCustomPageWithInvalidToken()
        {
            // create page to later update
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

            // now update it
            var putRoute = $"customPages/{postResult.HttpResponseObject.CustomPageID}";

            var putResult = await APIHelper.Put<CustomPageDto>($"Bearer {Guid.NewGuid()}", putRoute, JsonConvert.SerializeObject(customPageUpsertDto));
            Assert.AreEqual(HttpStatusCode.Unauthorized, putResult.HttpResponseMessage.StatusCode);
            Assert.IsNull(putResult.HttpResponseObject);
        }
    }
}
