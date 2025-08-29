namespace Qanat.Models.DataTransferObjects;

public class WaterMeasurementTypeSimpleDto
{
    public int WaterMeasurementTypeID { get; set; }
    public int GeographyID { get; set; }
    public int WaterMeasurementCategoryTypeID { get; set; }
    public bool IsActive { get; set; }
    public string WaterMeasurementTypeName { get; set; }
    public string ShortName { get; set; }
    public int SortOrder { get; set; }
    public bool IsUserEditable { get; set; }
    public bool IsSelfReportable { get; set; }
    public bool ShowToLandowner { get; set; }
    public int? WaterMeasurementCalculationTypeID { get; set; }
    public string CalculationJSON { get; set; }
    public string WaterMeasurementCategoryName { get; set; }
    public string WaterMeasurementCalculationName { get; set; }
    public bool IsSourceOfRecord { get; set; }
}
