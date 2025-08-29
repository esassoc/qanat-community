namespace Qanat.Models.DataTransferObjects;

public class WaterAccountDisplayDto
{
    public WaterAccountDisplayDto()
    {
    }

    public int WaterAccountID { get; set; }
    public string WaterAccountName { get; set; }
    public int WaterAccountNumber { get; set; }
    public string WaterAccountPIN { get; set; }
    public int GeographyID { get; set; }

    public WaterAccountDisplayDto(int waterAccountID, string waterAccountName, int waterAccountNumber, string waterAccountPIN, int geographyID)
    {
        WaterAccountID = waterAccountID;
        WaterAccountName = waterAccountName;
        WaterAccountNumber = waterAccountNumber;
        WaterAccountPIN = waterAccountPIN;
        GeographyID = geographyID;
    }
}