using System.Text.Json.Serialization;

namespace Qanat.Models.DataTransferObjects;

public class UsageLocationDto
{
    public int UsageLocationID { get; set; }
    public GeographySimpleDto Geography { get; set; }
    public UsageLocationTypeSimpleDto UsageLocationType { get; set; }
    public WaterAccountSimpleDto WaterAccount { get; set; }
    public ParcelSimpleDto Parcel { get; set; }
    public ReportingPeriodSimpleDto ReportingPeriod { get; set; }

    public string Name { get; set; }
    public double Area { get; set; }

    public string FallowStatus { get; set; }
    public bool FallowSelfReportApproved { get; set; }
    public string FallowNote { get; set; }

    public string CoverCropStatus { get; set; }
    public bool CoverCropSelfReportApproved { get; set; }
    public string CoverCropNote { get; set; }

    public List<UsageLocationCropSimpleDto> Crops { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string SourceOfRecordWaterMeasurementTypeName { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? SourceOfRecordValueInAcreFeet { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? SourceOfRecordValueInFeet { get; set; }


    #region Create and Update 

    public DateTime CreateDate { get; set; }
    public UserSimpleDto CreateUser { get; set; }
    public DateTime? UpdateDate { get; set; }
    public UserSimpleDto UpdateUser { get; set; }

    #endregion
}