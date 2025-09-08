using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Qanat.EFModels.Entities;

[Table("SupportTicket")]
public partial class SupportTicket
{
    [Key]
    public int SupportTicketID { get; set; }

    public int GeographyID { get; set; }

    [Required]
    [Unicode(false)]
    public string Description { get; set; }

    public int SupportTicketStatusID { get; set; }

    public int SupportTicketPriorityID { get; set; }

    public int? SupportTicketQuestionTypeID { get; set; }

    public int? WaterAccountID { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime DateCreated { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? DateUpdated { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? DateClosed { get; set; }

    public int? CreateUserID { get; set; }

    public int? AssignedUserID { get; set; }

    [Required]
    [StringLength(50)]
    [Unicode(false)]
    public string ContactFirstName { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string ContactLastName { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string ContactEmail { get; set; }

    [StringLength(30)]
    [Unicode(false)]
    public string ContactPhoneNumber { get; set; }

    [ForeignKey("AssignedUserID")]
    [InverseProperty("SupportTicketAssignedUsers")]
    public virtual User AssignedUser { get; set; }

    [ForeignKey("CreateUserID")]
    [InverseProperty("SupportTicketCreateUsers")]
    public virtual User CreateUser { get; set; }

    [ForeignKey("GeographyID")]
    [InverseProperty("SupportTickets")]
    public virtual Geography Geography { get; set; }

    [InverseProperty("SupportTicket")]
    public virtual ICollection<SupportTicketNote> SupportTicketNotes { get; set; } = new List<SupportTicketNote>();

    [ForeignKey("WaterAccountID")]
    [InverseProperty("SupportTickets")]
    public virtual WaterAccount WaterAccount { get; set; }
}
