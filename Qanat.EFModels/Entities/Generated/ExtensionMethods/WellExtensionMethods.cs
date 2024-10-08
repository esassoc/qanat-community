//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[Well]

using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities
{
    public static partial class WellExtensionMethods
    {
        public static WellSimpleDto AsSimpleDto(this Well well)
        {
            var dto = new WellSimpleDto()
            {
                WellID = well.WellID,
                GeographyID = well.GeographyID,
                ParcelID = well.ParcelID,
                WellName = well.WellName,
                StateWCRNumber = well.StateWCRNumber,
                CountyWellPermitNumber = well.CountyWellPermitNumber,
                DateDrilled = well.DateDrilled,
                WellDepth = well.WellDepth,
                CreateDate = well.CreateDate,
                WellStatusID = well.WellStatusID,
                Notes = well.Notes
            };
            return dto;
        }
    }
}