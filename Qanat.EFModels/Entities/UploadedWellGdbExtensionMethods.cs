using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities;

public static partial class UploadedWellGdbExtensionMethods
{
    public static UploadedWellGdbDto AsUploadedWellGdbDto(this UploadedWellGdb uploadedWellGdb)
    {
        var uploadedWellGdbDto = new UploadedWellGdbDto()
        {
            UploadedWellGdbID = uploadedWellGdb.UploadedWellGdbID,
            User = uploadedWellGdb.User.AsUserDto(),
            Geography = uploadedWellGdb.Geography.AsSimpleDto(),
            CanonicalName = uploadedWellGdb.CanonicalName,
            UploadDate = uploadedWellGdb.UploadDate,
            EffectiveDate = uploadedWellGdb.EffectiveDate,
            Finalized = uploadedWellGdb.Finalized,
            SRID = uploadedWellGdb.SRID
        };
        return uploadedWellGdbDto;
    }
}