namespace Qanat.Models.DataTransferObjects;

public class ParcelBulkUpdateParcelStatusDto
{
    public List<int> ParcelIDs { get; set; }
    public int ParcelStatusID { get; set; }
}