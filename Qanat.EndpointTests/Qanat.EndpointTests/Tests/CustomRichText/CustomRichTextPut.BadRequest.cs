using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Qanat.EndpointTests.Services.APIHelper;
using Qanat.Models.DataTransferObjects;
using ThreadingTask = System.Threading.Tasks.Task;

namespace Qanat.EndpointTests.Tests.CustomRichText
{
    public partial class CustomRichTextPut
    {
        [DataTestMethod]
        [DataRow(1)]
        public async ThreadingTask CannotPutWithNullBody(int customRichTextTypeID)
        {
            var route = $"customRichText/{customRichTextTypeID}";
            var putResult = await APIHelper.Put<CustomRichTextDto>(_validTokenWithPermission, route, JsonConvert.SerializeObject(null));
            Assert.AreEqual(HttpStatusCode.BadRequest, putResult.HttpResponseMessage.StatusCode);
            Assert.IsNull(putResult.HttpResponseObject);
        }
    }
}
