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
        public async ThreadingTask CannotReadSingleWithInvalidCustomRichTextTypeID()
        {
            var route = $"customRichText/{int.MaxValue}";
            var result = await APIHelper.Get<CustomRichTextDto>(_validTokenWithPermission, route);
            Assert.AreEqual(HttpStatusCode.NotFound, result.HttpResponseMessage.StatusCode);
            Assert.IsNull(result.HttpResponseObject);
        }
    }
}
