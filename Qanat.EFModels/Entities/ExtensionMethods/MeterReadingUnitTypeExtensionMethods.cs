using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities
{
    public static class MeterReadingUnitTypeExtensionMethods
    {
        public static MeterReadingUnitTypeSimpleDto AsSimpleDto(this MeterReadingUnitType meterReadingUnitType)
        {
            var dto = new MeterReadingUnitTypeSimpleDto()
            {
                MeterReadingUnitTypeID = meterReadingUnitType.MeterReadingUnitTypeID,
                MeterReadingUnitTypeName = meterReadingUnitType.MeterReadingUnitTypeName,
                MeterReadingUnitTypeDisplayName = meterReadingUnitType.MeterReadingUnitTypeDisplayName,
                MeterReadingUnitTypeAbbreviation = meterReadingUnitType.MeterReadingUnitTypeAbbreviation
            };
            return dto;
        }
    }
}