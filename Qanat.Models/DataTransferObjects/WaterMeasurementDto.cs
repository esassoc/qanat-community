using CsvHelper.Configuration;
using System;

namespace Qanat.Models.DataTransferObjects;

public class WaterMeasurementDto
{
    public int WaterMeasurementID { get; set; }
    public int GeographyID { get; set; }
    public int? WaterMeasurementTypeID { get; set; }
    public int? UnitTypeID { get; set; }
    public string UsageEntityName { get; set; }
    public DateTime ReportedDate { get; set; }
    public decimal ReportedValue { get; set; }
    public decimal? ReportedValueInAcreFeet { get; set; }
    public decimal? UsageEntityArea { get; set; }
    public DateTime LastUpdateDate { get; set; }
    public bool FromManualUpload { get; set; }

    public string UnitTypeName { get; set; }
    public string WaterMeasurementTypeName { get; set; }
    public string WaterMeasurementCategoryTypeName { get; set; }
}


public sealed class WaterMeasurementMap : ClassMap<WaterMeasurementDto>
{
    public WaterMeasurementMap()
    {
        Map(m => m.UsageEntityName).Name("Parcel Number");
        Map(m => m.UnitTypeName).Name("Water Usage Type Name");
        Map(m => m.ReportedDate).Name("Reported Date");
        Map(m => m.ReportedValue).Name("Reported Value");
        Map(m => m.LastUpdateDate).Name("Last Update Date");
    }
}