//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[OpenETSync]

using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities
{
    public static partial class OpenETSyncExtensionMethods
    {
        public static OpenETSyncSimpleDto AsSimpleDto(this OpenETSync openETSync)
        {
            var dto = new OpenETSyncSimpleDto()
            {
                OpenETSyncID = openETSync.OpenETSyncID,
                GeographyID = openETSync.GeographyID,
                OpenETDataTypeID = openETSync.OpenETDataTypeID,
                Year = openETSync.Year,
                Month = openETSync.Month,
                FinalizeDate = openETSync.FinalizeDate
            };
            return dto;
        }
    }
}