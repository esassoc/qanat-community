namespace Qanat.Models.DataTransferObjects;

public class OnboardingWaterAccountDto
{
    public int WaterAccountID { get; set; }
    public string WaterAccountName { get; set; }
    public int WaterAccountNumber { get; set; }
    public string WaterAccountPIN { get; set; }
    public List<string> ParcelNumbers { get; set; }
    public List<string> ParcelGeoJson { get; set; }
    public bool? IsClaimed { get; set; }
}