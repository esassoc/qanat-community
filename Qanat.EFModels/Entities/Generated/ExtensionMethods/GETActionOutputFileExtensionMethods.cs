//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[GETActionOutputFile]

using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities
{
    public static partial class GETActionOutputFileExtensionMethods
    {
        public static GETActionOutputFileSimpleDto AsSimpleDto(this GETActionOutputFile gETActionOutputFile)
        {
            var dto = new GETActionOutputFileSimpleDto()
            {
                GETActionOutputFileID = gETActionOutputFile.GETActionOutputFileID,
                GETActionOutputFileTypeID = gETActionOutputFile.GETActionOutputFileTypeID,
                GETActionID = gETActionOutputFile.GETActionID,
                FileResourceID = gETActionOutputFile.FileResourceID
            };
            return dto;
        }
    }
}