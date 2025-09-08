//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[UploadedGdb]

using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities
{
    public static partial class UploadedGdbExtensionMethods
    {
        public static UploadedGdbSimpleDto AsSimpleDto(this UploadedGdb uploadedGdb)
        {
            var dto = new UploadedGdbSimpleDto()
            {
                UploadedGdbID = uploadedGdb.UploadedGdbID,
                UserID = uploadedGdb.UserID,
                GeographyID = uploadedGdb.GeographyID,
                CanonicalName = uploadedGdb.CanonicalName,
                UploadDate = uploadedGdb.UploadDate,
                EffectiveYear = uploadedGdb.EffectiveYear,
                Finalized = uploadedGdb.Finalized,
                SRID = uploadedGdb.SRID
            };
            return dto;
        }
    }
}