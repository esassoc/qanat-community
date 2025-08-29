namespace Qanat.Models.DataTransferObjects;

public class UsageLocationTypeSimpleDto
{
    public int UsageLocationTypeID { get; set; }
    public string Name { get; set; }
    public string Definition { get; set; }
    public bool CanBeRemoteSensed { get; set; }
    public bool IsIncludedInUsageCalculation { get; set; }
    public bool IsDefault { get; set; }
    public string ColorHex { get; set; }
    public int SortOrder { get; set; }


    public bool CanBeSelectedInCoverCropForm { get; set; }
    public bool CountsAsCoverCropped { get; set; }
    public bool CanBeSelectedInFallowForm { get; set; }
    public bool CountsAsFallowed { get; set; }
}