namespace Qanat.Models.DataTransferObjects;

public class UserGeographySummaryDto
{
    public int GeographyID { get; set; }
    public string GeographyName { get; set; }
    public string GeographyDisplayName { get; set; }
    public List<WaterAccountSummaryDto> WaterAccounts { get; set; }
    public int ParcelsCount => WaterAccounts?.Sum(x => x.NumOfParcels) ?? 0;
    public int? WellsCount { get; set; }
    public bool AllowWaterMeasurementSelfReporting { get; set; }
    public bool AllowFallowSelfReporting { get; set; }
    public bool AllowCoverCropSelfReporting { get; set; }
    public bool WellRegistryEnabled { get; set; }
    public bool AllowLandownersToRequestAccountChanges { get; set; }
    public bool IsGeographyWaterManager { get; set; }
}