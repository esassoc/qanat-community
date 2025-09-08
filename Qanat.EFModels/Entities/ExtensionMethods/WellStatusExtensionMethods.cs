using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities
{
    public static class WellStatusExtensionMethods
    {
        public static WellStatusSimpleDto AsSimpleDto(this WellStatus wellStatus)
        {
            var dto = new WellStatusSimpleDto()
            {
                WellStatusID = wellStatus.WellStatusID,
                WellStatusName = wellStatus.WellStatusName,
                WellStatusDisplayName = wellStatus.WellStatusDisplayName
            };
            return dto;
        }
    }
}