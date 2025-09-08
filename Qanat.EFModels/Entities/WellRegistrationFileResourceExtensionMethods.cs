using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities;

public static partial class WellRegistrationFileResourceExtensionMethods
{
    public static WellRegistrationFileResourceDto AsWellRegistrationFileResourceDto(this WellRegistrationFileResource wellRegistrationFileResource)
    {
        return new WellRegistrationFileResourceDto()
        {
            WellRegistrationFileResourceID = wellRegistrationFileResource.WellRegistrationFileResourceID,
            WellRegistrationID = wellRegistrationFileResource.WellRegistrationID,
            FileResourceID = wellRegistrationFileResource.FileResourceID,
            FileDescription = wellRegistrationFileResource.FileDescription,
            FileResource = wellRegistrationFileResource.FileResource.AsSimpleDto()
        };
    }
}