using System.ComponentModel.DataAnnotations;

namespace Qanat.Models.DataTransferObjects.User;

public class UserUpsertDto
{
    [Required]
    public int? RoleID { get; set; }

    [Required]
    public bool ReceiveSupportEmails { get; set; }

    #region GET supporting fields

    [Required]
    public int? ScenarioPlannerRoleID { get; set; }

    public List<int> ModelIDs { get; set; } = new();

    public int? GETRunCustomerID { get; set; }

    public int? GETRunUserID { get; set; }

    #endregion
}