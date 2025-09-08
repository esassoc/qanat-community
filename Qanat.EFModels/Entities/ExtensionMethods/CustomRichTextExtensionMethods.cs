using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities
{
    public static class CustomRichTextExtensionMethods
    {
        public static CustomRichTextDto AsCustomRichTextDto(this CustomRichText customRichText)
        {
            var customRichTextDto = new CustomRichTextDto()
            {
                CustomRichTextID = customRichText.CustomRichTextID,
                CustomRichTextType = customRichText.CustomRichTextType.AsSimpleDto(),
                CustomRichTextTitle = customRichText.CustomRichTextTitle,
                CustomRichTextContent = customRichText.CustomRichTextContent,
                Geography = customRichText.Geography?.AsSimpleDto(),
                IsEmptyContent = string.IsNullOrWhiteSpace(customRichText.CustomRichTextContent)
            };
            return customRichTextDto;
        }
    }
}