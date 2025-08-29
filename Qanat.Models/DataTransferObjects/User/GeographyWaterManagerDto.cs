namespace Qanat.Models.DataTransferObjects.User;

public class GeographyWaterManagerDto
{
    public int GeographyID { get; set; }
    public int UserID { get; set; }
    public string UserFullName { get; set; }
    public bool ReceivesNotifications { get; set; }
}