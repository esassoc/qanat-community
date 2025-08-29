using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities;

public static class MeterReadingExtensionMethods
{
    public static MeterReadingSimpleDto AsSimpleDto(this MeterReading meterReading)
    {
        var dto = new MeterReadingSimpleDto()
        {
            MeterReadingID = meterReading.MeterReadingID,
            GeographyID = meterReading.GeographyID,
            WellID = meterReading.WellID,
            MeterID = meterReading.MeterID,
            MeterReadingUnitTypeID = meterReading.MeterReadingUnitTypeID,
            ReadingDate = meterReading.ReadingDate,
            PreviousReading = meterReading.PreviousReading,
            CurrentReading = meterReading.CurrentReading,
            Volume = meterReading.Volume,
            VolumeInAcreFeet = meterReading.VolumeInAcreFeet,
            ReaderInitials = meterReading.ReaderInitials,
            Comment = meterReading.Comment
        };
        return dto;
    }

    public static MeterReadingDto AsDto(this MeterReading meterReading)
    {
        return new MeterReadingDto()
        {
            MeterReadingID = meterReading.MeterReadingID,
            MeterID = meterReading.MeterID,
            MeterSerialNumber = meterReading.Meter?.SerialNumber,
            WellID = meterReading.WellID,
            ReadingDate = meterReading.ReadingDate,
            PreviousReading = meterReading.PreviousReading,
            CurrentReading = meterReading.CurrentReading,
            Volume = meterReading.Volume,
            VolumeInAcreFeet = meterReading.VolumeInAcreFeet,
            MeterReadingUnitType = meterReading.MeterReadingUnitType.AsSimpleDto(),
            ReaderInitials = meterReading.ReaderInitials,
            Comment = meterReading.Comment
        };
    }
}