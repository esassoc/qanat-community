namespace Qanat.Models.DataTransferObjects;

public class WellDisplayDto
{
    public int WellID { get; set; }
    public string WellName { get; set; }
    public string StateWCRNumber { get; set; }
    public int GeographyID { get; set; }
    public int? ParcelID { get; set; }
}