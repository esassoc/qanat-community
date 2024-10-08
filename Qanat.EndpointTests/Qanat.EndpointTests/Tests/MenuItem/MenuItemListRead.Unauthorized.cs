using System.Collections.Generic;
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Qanat.EndpointTests.Services.APIHelper;
using Qanat.Models.DataTransferObjects;
using ThreadingTask = System.Threading.Tasks.Task;
using System;

namespace Qanat.EndpointTests.Tests.MenuItem
{
    public partial class MenuItemListRead
    {
        [DataTestMethod]
        public async ThreadingTask CannotReadListWithoutToken()
        {
            var route = "menuItems";
            var result = await APIHelper.Get<List<MenuItemDto>>(string.Empty, route);
            Assert.AreEqual(HttpStatusCode.Unauthorized, result.HttpResponseMessage.StatusCode);
            Assert.IsNull(result.HttpResponseObject);
        }

        [DataTestMethod]
        public async ThreadingTask CannotReadListWithInvalidToken()
        {
            var route = "menuItems";
            var result = await APIHelper.Get<List<MenuItemDto>>($"Bearer {Guid.NewGuid()}", route);
            Assert.AreEqual(HttpStatusCode.Unauthorized, result.HttpResponseMessage.StatusCode);
            Assert.IsNull(result.HttpResponseObject);
        }

    }
}