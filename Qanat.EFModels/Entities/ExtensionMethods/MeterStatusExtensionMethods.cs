using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities
{
    public static class MeterStatusExtensionMethods
    {
        public static MeterStatusSimpleDto AsSimpleDto(this MeterStatus meterStatus)
        {
            var dto = new MeterStatusSimpleDto()
            {
                MeterStatusID = meterStatus.MeterStatusID,
                MeterStatusName = meterStatus.MeterStatusName,
                MeterStatusDisplayName = meterStatus.MeterStatusDisplayName
            };
            return dto;
        }
    }
}