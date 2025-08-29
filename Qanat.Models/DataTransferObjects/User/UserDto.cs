using Qanat.Models.Security;

namespace Qanat.Models.DataTransferObjects;

public class UserDto
{
    public UserDto()
    {
    }

    public int UserID { get; set; }
    public Guid? UserGuid { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public int RoleID { get; set; }
    public string RoleDisplayName { get; set; }
    public DateTime CreateDate { get; set; }
    public DateTime? UpdateDate { get; set; }
    public DateTime? LastActivityDate { get; set; }
    public DateTime? DisclaimerAcknowledgedDate { get; set; }
    public bool IsActive { get; set; }
    public bool ReceiveSupportEmails { get; set; }
    public string LoginName { get; set; }
    public string Company { get; set; }
    public Guid? ImpersonatedUserGuid { get; set; }
    public bool IsClientUser { get; set; }
    public string FullName {get; set;}
    public Dictionary<string, Rights> Rights { get; set; }
    public Dictionary<string, bool> Flags { get; set; }

    public Dictionary<int, Dictionary<string, Rights>> GeographyRights { get; set; } 
    public Dictionary<int, Dictionary<string, bool>> GeographyFlags { get; set; }

    public Dictionary<int, Dictionary<string, Rights>> WaterAccountRights { get; set; }
    public Dictionary<int, Dictionary<string, bool>> WaterAccountFlags { get; set; }

    public int NumberOfWaterAccounts { get; set; }
    public List<GeographyUserSimpleDto> GeographyUser { get; set; }


    public int ScenarioPlannerRoleID { get; set; }
    public string ScenarioPlannerRoleDisplayName { get; set; }
    public List<ModelSimpleDto> Models { get; set; }
    public Dictionary<string, Rights> ScenarioPlannerRights { get; set; }
    public int? GETRunCustomerID { get; set; }
    public int? GETRunUserID { get; set; }
}
