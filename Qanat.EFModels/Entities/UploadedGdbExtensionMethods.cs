using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities;

public static partial class UploadedGdbExtensionMethods
{
    public static UploadedGdbDto AsDto(this UploadedGdb uploadedGdb)
    {
        var uploadedGdbDto = new UploadedGdbDto()
        {
            UploadedGdbID = uploadedGdb.UploadedGdbID,
            User = uploadedGdb.User.AsUserDto(),
            Geography = uploadedGdb.Geography.AsSimpleDto(),
            CanonicalName = uploadedGdb.CanonicalName,
            UploadDate = uploadedGdb.UploadDate,
            EffectiveYear = uploadedGdb.EffectiveYear,
            Finalized = uploadedGdb.Finalized,
            SRID = uploadedGdb.SRID
        };
        return uploadedGdbDto;
    }

}