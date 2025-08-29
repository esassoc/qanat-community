using NetTopologySuite.Features;

namespace Qanat.Models.DataTransferObjects;

public class WaterMeasurementDto
{
    public int WaterMeasurementID { get; set; }
    public int GeographyID { get; set; }
    public DateTime ReportedDate { get; set; }
    public decimal? ReportedValueInNativeUnits { get; set; }
    public decimal ReportedValueInFeet { get; set; }
    public decimal ReportedValueInAcreFeet { get; set; }
    public DateTime LastUpdateDate { get; set; }
    public bool FromManualUpload { get; set; }

    public int? UnitTypeID { get; set; }
    public string UnitTypeName { get; set; }

    public int? WaterMeasurementTypeID { get; set; }
    public string WaterMeasurementTypeName { get; set; }
    public string WaterMeasurementCategoryTypeName { get; set; }

    public int? UsageLocationID { get; set; }
    public string UsageLocationName { get; set; }
    public decimal? UsageLocationArea { get; set; }

    public int ParcelID { get; set; }
    public string ParcelNumber { get; set; }

    public int? WaterAccountID { get; set; }
    public string WaterAccountNumberAndName { get; set; }
}