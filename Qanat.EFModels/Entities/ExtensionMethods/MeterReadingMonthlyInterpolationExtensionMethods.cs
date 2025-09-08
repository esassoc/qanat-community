using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities
{
    public static class MeterReadingMonthlyInterpolationExtensionMethods
    {
        public static MeterReadingMonthlyInterpolationSimpleDto AsSimpleDto(this MeterReadingMonthlyInterpolation meterReadingMonthlyInterpolation)
        {
            var dto = new MeterReadingMonthlyInterpolationSimpleDto()
            {
                MeterReadingMonthlyInterpolationID = meterReadingMonthlyInterpolation.MeterReadingMonthlyInterpolationID,
                GeographyID = meterReadingMonthlyInterpolation.GeographyID,
                WellID = meterReadingMonthlyInterpolation.WellID,
                MeterID = meterReadingMonthlyInterpolation.MeterID,
                MeterReadingUnitTypeID = meterReadingMonthlyInterpolation.MeterReadingUnitTypeID,
                Date = meterReadingMonthlyInterpolation.Date,
                InterpolatedVolume = meterReadingMonthlyInterpolation.InterpolatedVolume,
                InterpolatedVolumeInAcreFeet = meterReadingMonthlyInterpolation.InterpolatedVolumeInAcreFeet
            };
            return dto;
        }
    }
}