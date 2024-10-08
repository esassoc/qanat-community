//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[GETActionFileResource]

using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities
{
    public static partial class GETActionFileResourceExtensionMethods
    {
        public static GETActionFileResourceSimpleDto AsSimpleDto(this GETActionFileResource gETActionFileResource)
        {
            var dto = new GETActionFileResourceSimpleDto()
            {
                GETActionFileResourceID = gETActionFileResource.GETActionFileResourceID,
                GETActionID = gETActionFileResource.GETActionID,
                FileResourceID = gETActionFileResource.FileResourceID
            };
            return dto;
        }
    }
}