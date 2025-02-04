namespace Qanat.Models.DataTransferObjects;

public class WaterAccountParcelDto
{
    public int WaterAccountParcelID { get; set; }
    public int GeographyID { get; set; }
    public int WaterAccountID { get; set; }
    public int ParcelID { get; set; }
    public int EffectiveYear { get; set; }
    public int? EndYear { get; set; }
    public WaterAccountMinimalDto WaterAccount { get; set; }
}