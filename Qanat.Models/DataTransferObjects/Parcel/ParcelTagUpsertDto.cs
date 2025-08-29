namespace Qanat.Models.DataTransferObjects;

public class ParcelTagUpsertDto
{
    public int TagID { get; set; }
    public string TagName { get; set; }
    public List<int> ParcelIDs { get; set; }
}