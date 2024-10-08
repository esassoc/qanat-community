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
        public async ThreadingTask CannotPutWithInvalidCustomRichTextTypeID(int customRichTextTypeID)
        {
            var getRoute = $"customRichText/{customRichTextTypeID}";
            var getResult = await APIHelper.Get<CustomRichTextDto>(_validTokenWithPermission, getRoute);
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

            var putRoute = $"customRichText/{int.MaxValue}";
            var putResult = await APIHelper.Put<CustomRichTextDto>(_validTokenWithPermission, putRoute, JsonConvert.SerializeObject(customRichTextDto));
            Assert.AreEqual(HttpStatusCode.NotFound, putResult.HttpResponseMessage.StatusCode);
            Assert.IsNull(putResult.HttpResponseObject);
        }
    }
}
