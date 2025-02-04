namespace Qanat.Models.DataTransferObjects;

public class WaterAccountParcelsRequestChangesDto
{
    public List<WaterAccountRequestChangesDto> WaterAccounts { get; set; }
    public List<WaterAccountRequestChangesParcelDto> ParcelsToRemove { get; set; }
}