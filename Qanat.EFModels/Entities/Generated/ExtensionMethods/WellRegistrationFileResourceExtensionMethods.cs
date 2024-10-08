//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[WellRegistrationFileResource]

using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities
{
    public static partial class WellRegistrationFileResourceExtensionMethods
    {
        public static WellRegistrationFileResourceSimpleDto AsSimpleDto(this WellRegistrationFileResource wellRegistrationFileResource)
        {
            var dto = new WellRegistrationFileResourceSimpleDto()
            {
                WellRegistrationFileResourceID = wellRegistrationFileResource.WellRegistrationFileResourceID,
                WellRegistrationID = wellRegistrationFileResource.WellRegistrationID,
                FileResourceID = wellRegistrationFileResource.FileResourceID,
                FileDescription = wellRegistrationFileResource.FileDescription
            };
            return dto;
        }
    }
}