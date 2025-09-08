using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities
{
    public static class WellRegistrationStatusExtensionMethods
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