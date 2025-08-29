using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities;

public static class WellFileResourceExtensionMethods
{
    public static WellFileResourceDto AsWellFileResourceDto(this WellFileResource wellRegistrationFileResource)
    {
        return new WellFileResourceDto()
        {
            WellFileResourceID = wellRegistrationFileResource.WellFileResourceID,
            WellID = wellRegistrationFileResource.WellID,
            FileResourceID = wellRegistrationFileResource.FileResourceID,
            FileDescription = wellRegistrationFileResource.FileDescription,
            FileResource = wellRegistrationFileResource.FileResource.AsSimpleDto()
        };
    }
}