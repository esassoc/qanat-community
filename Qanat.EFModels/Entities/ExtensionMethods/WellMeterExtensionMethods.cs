using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities;

public static class WellMeterExtensionMethods
{
    public static WellMeterDto AsDto(this WellMeter wellMeter)
    {
        return new WellMeterDto()
        {
            GeographyID = wellMeter.Well.GeographyID,
            WellMeterID = wellMeter.WellMeterID,
            WellID = wellMeter.WellID,
            WellName = wellMeter.Well?.WellName,
            MeterID = wellMeter.MeterID,
            MeterSerialNumber = wellMeter.Meter?.SerialNumber,
            StartDate = wellMeter.StartDate,
            EndDate = wellMeter.EndDate,
            WaterAccountNumber = wellMeter.Well?.Parcel?.WaterAccount?.WaterAccountNumber
        };
    }
}