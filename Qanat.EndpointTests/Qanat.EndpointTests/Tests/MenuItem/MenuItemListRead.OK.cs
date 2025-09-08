using System.Collections.Generic;
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Qanat.EndpointTests.Services.APIHelper;
using Qanat.Models.DataTransferObjects;
using ThreadingTask = System.Threading.Tasks.Task;

namespace Qanat.EndpointTests.Tests.MenuItem
{
    public partial class MenuItemListRead
    {
        [DataTestMethod]
        public async ThreadingTask CanReadListWithValidToken()
        {
            var route = "menuItems";
            var result = await APIHelper.Get<List<MenuItemDto>>(_validTokenWithPermission, route);
            Assert.AreEqual(HttpStatusCode.OK, result.HttpResponseMessage.StatusCode);
            Assert.IsNotNull(result.HttpResponseObject);
        }
    }
}