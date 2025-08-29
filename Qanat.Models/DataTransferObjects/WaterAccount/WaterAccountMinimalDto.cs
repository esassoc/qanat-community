namespace Qanat.Models.DataTransferObjects;

public class WaterAccountMinimalDto
{
    public int WaterAccountID { get; set; }
    public int GeographyID { get; set; }
    public int WaterAccountNumber { get; set; }
    public string WaterAccountName { get; set; }
    public string Notes { get; set; }
    public DateTime? UpdateDate { get; set; }
    public string WaterAccountPIN { get; set; }
    public DateTime CreateDate { get; set; }

    public string WaterAccountNameAndNumber { get; set; }
    public GeographySimpleDto Geography { get; set; }
}