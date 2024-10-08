using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Qanat.EndpointTests.Services.APIHelper;
using Qanat.Models.DataTransferObjects;
using ThreadingTask = System.Threading.Tasks.Task;

namespace Qanat.EndpointTests.Tests.CustomRichText
{
    public partial class CustomRichTextSingleRead
    {
        [DataTestMethod]
        [DataRow(1)]
        public async ThreadingTask CanReadSingleWithValidToken(int customRichTextTypeID)  // custom rich texts are weird; there is no Post or list route and the route parameter is a customRichTextTypeID...  so just hard code
        {
            var route = $"customRichText/{customRichTextTypeID}";
            var result = await APIHelper.Get<CustomRichTextDto>(_validTokenWithPermission, route);
            Assert.AreEqual(HttpStatusCode.OK, result.HttpResponseMessage.StatusCode);
            Assert.IsNotNull(result.HttpResponseObject);
        }
    }
}
