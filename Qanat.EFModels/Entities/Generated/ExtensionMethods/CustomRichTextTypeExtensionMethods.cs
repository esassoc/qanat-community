//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[CustomRichTextType]

using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities
{
    public static partial class CustomRichTextTypeExtensionMethods
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