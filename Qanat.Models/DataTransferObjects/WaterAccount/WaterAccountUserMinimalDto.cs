namespace Qanat.Models.DataTransferObjects;

public class WaterAccountUserMinimalDto
{
    public int WaterAccountUserID { get; set; }
    public int UserID { get; set; }
    public string UserFullName { get; set; }
    public int WaterAccountID { get; set; }
    public int WaterAccountRoleID { get; set; }
    public DateTime ClaimDate { get; set; }
    public UserWithFullNameDto User { get; set; }
    public WaterAccountRoleSimpleDto WaterAccountRole { get; set; }
    public WaterAccountMinimalDto WaterAccount { get; set; }
    public string UserEmail { get; set; }
    public bool IsAccountHolder { get; set; }
}