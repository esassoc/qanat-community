using System;
using System.Collections.Generic;

namespace Qanat.EFModels.Entities;

public partial class WaterMeasurement
{
    public WaterMeasurement(int geographyID, int waterMeasurementTypeID, UnitTypeEnum unitTypeEnum, string parcelNumber, DateTime reportedDate,
        decimal reportedValue, decimal reportedValueInAcreFeet, decimal? usageEntityArea, DateTime lastUpdateDate, bool fromManualUpload)
    {
        GeographyID = geographyID;
        UnitTypeID = (int) unitTypeEnum;
        WaterMeasurementTypeID = waterMeasurementTypeID;
        UsageEntityName = parcelNumber; // todo: match the usage entity name
        ReportedDate = reportedDate;
        ReportedValue = reportedValue;
        ReportedValueInAcreFeet = reportedValueInAcreFeet;
        UsageEntityArea = usageEntityArea;
        LastUpdateDate = lastUpdateDate;
        FromManualUpload = fromManualUpload;
    }

    public WaterMeasurement()
    {
    }
}