//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[WellRegistrationStatus]

using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities
{
    public static partial class WellRegistrationStatusExtensionMethods
    {
        public static WellRegistrationStatusSimpleDto AsSimpleDto(this WellRegistrationStatus wellRegistrationStatus)
        {
            var dto = new WellRegistrationStatusSimpleDto()
            {
                WellRegistrationStatusID = wellRegistrationStatus.WellRegistrationStatusID,
                WellRegistrationStatusName = wellRegistrationStatus.WellRegistrationStatusName,
                WellRegistrationStatusDisplayName = wellRegistrationStatus.WellRegistrationStatusDisplayName
            };
            return dto;
        }
    }
}