namespace Qanat.Models.DataTransferObjects;

public class WaterAccountSearchDto
{
    public int? GeographyID { get; set; }
    public int? WaterAccountID { get; set; }
    public string SearchString { get; set; }
}