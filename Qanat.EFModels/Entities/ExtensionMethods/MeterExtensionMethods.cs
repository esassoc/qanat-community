using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities;

public static class MeterExtensionMethods
{
    public static MeterGridDto AsGridDto(this Meter meter)
    {
        var dto = new MeterGridDto()
        {
            MeterID = meter.MeterID,
            SerialNumber = meter.SerialNumber,
            DeviceName = meter.DeviceName,
            Make = meter.Make,
            ModelNumber = meter.ModelNumber,
            GeographyID = meter.GeographyID,
            MeterStatusID = meter.MeterStatusID,
            MeterStatus = meter.MeterStatus.AsSimpleDto(),
            WellID = meter.WellMeters.SingleOrDefault(x => !x.EndDate.HasValue)?.WellID
        };
        return dto;
    }

    public static MeterLinkDisplayDto AsLinkDisplayDto(this Meter meter)
    {
        return new MeterLinkDisplayDto()
        {
            MeterID = meter.MeterID,
            LinkDisplay = meter.DeviceName == null ? meter.SerialNumber : $"{meter.SerialNumber} ({meter.DeviceName})"
        };
    }
}