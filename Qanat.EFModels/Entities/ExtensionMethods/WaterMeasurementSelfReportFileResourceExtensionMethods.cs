using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities;

public static class WaterMeasurementSelfReportFileResourceExtensionMethods
{
    public static WaterMeasurementSelfReportFileResourceDto AsDto(this WaterMeasurementSelfReportFileResource selfReportFileResource)
    {
        var dto = new WaterMeasurementSelfReportFileResourceDto
        {
            WaterMeasurementSelfReportFileResourceID = selfReportFileResource.WaterMeasurementSelfReportFileResourceID,
            WaterMeasurementSelfReportID = selfReportFileResource.WaterMeasurementSelfReportID,
            FileResource = selfReportFileResource.FileResource.AsSimpleDto(),
            FileDescription = selfReportFileResource.FileDescription,
        };

        return dto;
    }
}
