using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Qanat.EFModels.Entities;

[Table("User")]
[Index("Email", Name = "AK_User_Email", IsUnique = true)]
public partial class User
{
    [Key]
    public int UserID { get; set; }

    public Guid? UserGuid { get; set; }

    [Required]
    [StringLength(100)]
    [Unicode(false)]
    public string FirstName { get; set; }

    [Required]
    [StringLength(100)]
    [Unicode(false)]
    public string LastName { get; set; }

    [Required]
    [StringLength(255)]
    [Unicode(false)]
    public string Email { get; set; }

    [StringLength(30)]
    [Unicode(false)]
    public string Phone { get; set; }

    public int RoleID { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreateDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? UpdateDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? LastActivityDate { get; set; }

    public bool IsActive { get; set; }

    public bool ReceiveSupportEmails { get; set; }

    [StringLength(128)]
    [Unicode(false)]
    public string LoginName { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string Company { get; set; }

    public Guid? ImpersonatedUserGuid { get; set; }

    public bool IsClientUser { get; set; }

    public int ScenarioPlannerRoleID { get; set; }

    public int? GETRunCustomerID { get; set; }

    public int? GETRunUserID { get; set; }

    [InverseProperty("CreateUser")]
    public virtual ICollection<FileResource> FileResources { get; set; } = new List<FileResource>();

    [InverseProperty("User")]
    public virtual ICollection<GETAction> GETActions { get; set; } = new List<GETAction>();

    [InverseProperty("User")]
    public virtual ICollection<GeographyUser> GeographyUsers { get; set; } = new List<GeographyUser>();

    [InverseProperty("User")]
    public virtual ICollection<ModelUser> ModelUsers { get; set; } = new List<ModelUser>();

    [InverseProperty("UpdateUser")]
    public virtual ICollection<ParcelHistory> ParcelHistories { get; set; } = new List<ParcelHistory>();

    [InverseProperty("User")]
    public virtual ICollection<ParcelSupply> ParcelSupplies { get; set; } = new List<ParcelSupply>();

    [InverseProperty("CreateUser")]
    public virtual ICollection<ReportingPeriod> ReportingPeriodCreateUsers { get; set; } = new List<ReportingPeriod>();

    [InverseProperty("UpdateUser")]
    public virtual ICollection<ReportingPeriod> ReportingPeriodUpdateUsers { get; set; } = new List<ReportingPeriod>();

    [InverseProperty("User")]
    public virtual ICollection<ScenarioRun> ScenarioRuns { get; set; } = new List<ScenarioRun>();

    [InverseProperty("AssignedUser")]
    public virtual ICollection<SupportTicket> SupportTicketAssignedUsers { get; set; } = new List<SupportTicket>();

    [InverseProperty("CreateUser")]
    public virtual ICollection<SupportTicket> SupportTicketCreateUsers { get; set; } = new List<SupportTicket>();

    [InverseProperty("CreateUser")]
    public virtual ICollection<SupportTicketNote> SupportTicketNotes { get; set; } = new List<SupportTicketNote>();

    [InverseProperty("User")]
    public virtual ICollection<UploadedGdb> UploadedGdbs { get; set; } = new List<UploadedGdb>();

    [InverseProperty("User")]
    public virtual ICollection<UploadedWellGdb> UploadedWellGdbs { get; set; } = new List<UploadedWellGdb>();

    [InverseProperty("User")]
    public virtual ICollection<WaterAccountUserStaging> WaterAccountUserStagings { get; set; } = new List<WaterAccountUserStaging>();

    [InverseProperty("User")]
    public virtual ICollection<WaterAccountUser> WaterAccountUsers { get; set; } = new List<WaterAccountUser>();

    [InverseProperty("CreateUser")]
    public virtual ICollection<WaterMeasurementSelfReport> WaterMeasurementSelfReportCreateUsers { get; set; } = new List<WaterMeasurementSelfReport>();

    [InverseProperty("CreateUser")]
    public virtual ICollection<WaterMeasurementSelfReportLineItem> WaterMeasurementSelfReportLineItemCreateUsers { get; set; } = new List<WaterMeasurementSelfReportLineItem>();

    [InverseProperty("UpdateUser")]
    public virtual ICollection<WaterMeasurementSelfReportLineItem> WaterMeasurementSelfReportLineItemUpdateUsers { get; set; } = new List<WaterMeasurementSelfReportLineItem>();

    [InverseProperty("UpdateUser")]
    public virtual ICollection<WaterMeasurementSelfReport> WaterMeasurementSelfReportUpdateUsers { get; set; } = new List<WaterMeasurementSelfReport>();

    [InverseProperty("CreateUser")]
    public virtual ICollection<WellRegistration> WellRegistrations { get; set; } = new List<WellRegistration>();
}
