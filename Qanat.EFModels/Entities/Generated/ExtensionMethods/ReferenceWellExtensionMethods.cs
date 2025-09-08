//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ReferenceWell]

using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities
{
    public static partial class ReferenceWellExtensionMethods
    {
        public static ReferenceWellSimpleDto AsSimpleDto(this ReferenceWell referenceWell)
        {
            var dto = new ReferenceWellSimpleDto()
            {
                ReferenceWellID = referenceWell.ReferenceWellID,
                GeographyID = referenceWell.GeographyID,
                WellName = referenceWell.WellName,
                CountyWellPermitNo = referenceWell.CountyWellPermitNo,
                WellDepth = referenceWell.WellDepth,
                StateWCRNumber = referenceWell.StateWCRNumber,
                DateDrilled = referenceWell.DateDrilled
            };
            return dto;
        }
    }
}