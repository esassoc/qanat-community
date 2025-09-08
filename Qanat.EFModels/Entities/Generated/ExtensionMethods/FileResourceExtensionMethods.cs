//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[FileResource]

using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities
{
    public static partial class FileResourceExtensionMethods
    {
        public static FileResourceSimpleDto AsSimpleDto(this FileResource fileResource)
        {
            var dto = new FileResourceSimpleDto()
            {
                FileResourceID = fileResource.FileResourceID,
                OriginalBaseFilename = fileResource.OriginalBaseFilename,
                OriginalFileExtension = fileResource.OriginalFileExtension,
                FileResourceGUID = fileResource.FileResourceGUID,
                FileResourceCanonicalName = fileResource.FileResourceCanonicalName,
                CreateUserID = fileResource.CreateUserID,
                CreateDate = fileResource.CreateDate
            };
            return dto;
        }
    }
}