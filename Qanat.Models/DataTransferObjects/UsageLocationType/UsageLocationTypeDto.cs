using Qanat.Models.DataTransferObjects.Geography;

namespace Qanat.Models.DataTransferObjects;

public class UsageLocationTypeDto
{
    public int UsageLocationTypeID { get; set; }
    public GeographyDisplayDto Geography { get; set; }
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

    public DateTime CreateDate { get; set; }
    public UserDisplayDto CreateUser { get; set; }
    public DateTime? UpdateDate { get; set; }
    public UserDisplayDto UpdateUser { get; set; }
}