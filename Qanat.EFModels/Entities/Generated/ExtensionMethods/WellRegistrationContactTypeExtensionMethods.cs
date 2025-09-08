//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[WellRegistrationContactType]

using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities
{
    public static partial class WellRegistrationContactTypeExtensionMethods
    {
        public static WellRegistrationContactTypeSimpleDto AsSimpleDto(this WellRegistrationContactType wellRegistrationContactType)
        {
            var dto = new WellRegistrationContactTypeSimpleDto()
            {
                WellRegistrationContactTypeID = wellRegistrationContactType.WellRegistrationContactTypeID,
                WellRegistrationContactTypeName = wellRegistrationContactType.WellRegistrationContactTypeName,
                WellRegistrationContactTypeDisplayName = wellRegistrationContactType.WellRegistrationContactTypeDisplayName
            };
            return dto;
        }
    }
}