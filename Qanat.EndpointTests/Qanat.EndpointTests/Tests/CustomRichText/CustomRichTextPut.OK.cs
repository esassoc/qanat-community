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
        public async ThreadingTask CanPutWithValidToken(int customRichTextTypeID)
        {
            var route = $"customRichText/{customRichTextTypeID}";
            var getResult = await APIHelper.Get<CustomRichTextDto>(_validTokenWithPermission, route);
            Assert.AreEqual(HttpStatusCode.OK, getResult.HttpResponseMessage.StatusCode);
            Assert.IsNotNull(getResult.HttpResponseObject);

            var customRichText = getResult.HttpResponseObject;
            var customRichTextDto = new CustomRichTextDto()
            {
                CustomRichTextID = customRichText.CustomRichTextID,
                CustomRichTextType = customRichText.CustomRichTextType,
                CustomRichTextContent = customRichText.CustomRichTextContent,
                IsEmptyContent = customRichText.IsEmptyContent
            };

            var putResult = await APIHelper.Put<CustomRichTextDto>(_validTokenWithPermission, route, JsonConvert.SerializeObject(customRichTextDto));
            Assert.AreEqual(HttpStatusCode.OK, putResult.HttpResponseMessage.StatusCode);
            Assert.IsNotNull(putResult.HttpResponseObject);
        }
    }
}
