//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[GETActionOutputFileType]

using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities
{
    public static partial class GETActionOutputFileTypeExtensionMethods
    {
        public static GETActionOutputFileTypeSimpleDto AsSimpleDto(this GETActionOutputFileType gETActionOutputFileType)
        {
            var dto = new GETActionOutputFileTypeSimpleDto()
            {
                GETActionOutputFileTypeID = gETActionOutputFileType.GETActionOutputFileTypeID,
                GETActionOutputFileTypeName = gETActionOutputFileType.GETActionOutputFileTypeName,
                GETActionOutputFileTypeExtension = gETActionOutputFileType.GETActionOutputFileTypeExtension
            };
            return dto;
        }
    }
}