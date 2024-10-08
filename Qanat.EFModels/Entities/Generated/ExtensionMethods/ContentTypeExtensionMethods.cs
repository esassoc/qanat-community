//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ContentType]

using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities
{
    public static partial class ContentTypeExtensionMethods
    {
        public static ContentTypeSimpleDto AsSimpleDto(this ContentType contentType)
        {
            var dto = new ContentTypeSimpleDto()
            {
                ContentTypeID = contentType.ContentTypeID,
                ContentTypeName = contentType.ContentTypeName,
                ContentTypeDisplayName = contentType.ContentTypeDisplayName
            };
            return dto;
        }
    }
}