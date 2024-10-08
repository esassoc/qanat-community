//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[UploadedWellGdb]

using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities
{
    public static partial class UploadedWellGdbExtensionMethods
    {
        public static UploadedWellGdbSimpleDto AsSimpleDto(this UploadedWellGdb uploadedWellGdb)
        {
            var dto = new UploadedWellGdbSimpleDto()
            {
                UploadedWellGdbID = uploadedWellGdb.UploadedWellGdbID,
                UserID = uploadedWellGdb.UserID,
                GeographyID = uploadedWellGdb.GeographyID,
                CanonicalName = uploadedWellGdb.CanonicalName,
                UploadDate = uploadedWellGdb.UploadDate,
                EffectiveDate = uploadedWellGdb.EffectiveDate,
                Finalized = uploadedWellGdb.Finalized,
                SRID = uploadedWellGdb.SRID
            };
            return dto;
        }
    }
}