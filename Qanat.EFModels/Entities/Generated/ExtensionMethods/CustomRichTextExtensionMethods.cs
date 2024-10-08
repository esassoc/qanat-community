//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[CustomRichText]

using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities
{
    public static partial class CustomRichTextExtensionMethods
    {
        public static CustomRichTextSimpleDto AsSimpleDto(this CustomRichText customRichText)
        {
            var dto = new CustomRichTextSimpleDto()
            {
                CustomRichTextID = customRichText.CustomRichTextID,
                CustomRichTextTypeID = customRichText.CustomRichTextTypeID,
                CustomRichTextTitle = customRichText.CustomRichTextTitle,
                CustomRichTextContent = customRichText.CustomRichTextContent,
                GeographyID = customRichText.GeographyID
            };
            return dto;
        }
    }
}