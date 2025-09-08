using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities
{
    public static class CustomRichTextTypeExtensionMethods
    {
        public static CustomRichTextTypeSimpleDto AsSimpleDto(this CustomRichTextType customRichTextType)
        {
            var dto = new CustomRichTextTypeSimpleDto()
            {
                CustomRichTextTypeID = customRichTextType.CustomRichTextTypeID,
                CustomRichTextTypeName = customRichTextType.CustomRichTextTypeName,
                CustomRichTextTypeDisplayName = customRichTextType.CustomRichTextTypeDisplayName,
                ContentTypeID = customRichTextType.ContentTypeID
            };
            return dto;
        }
    }
}