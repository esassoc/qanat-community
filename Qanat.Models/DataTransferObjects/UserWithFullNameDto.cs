using System;

namespace Qanat.Models.DataTransferObjects;

public class UserWithFullNameDto
{
    public int UserID { get; set; }
    public Guid? UserGuid { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public int RoleID { get; set; }
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
    public string FullName { get; set; }
}