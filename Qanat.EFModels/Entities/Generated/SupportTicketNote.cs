using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Qanat.EFModels.Entities;

[Table("SupportTicketNote")]
public partial class SupportTicketNote
{
    [Key]
    public int SupportTicketNoteID { get; set; }

    public int SupportTicketID { get; set; }

    [Required]
    [Unicode(false)]
    public string Message { get; set; }

    public bool InternalNote { get; set; }

    public int CreateUserID { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreateDate { get; set; }

    [ForeignKey("CreateUserID")]
    [InverseProperty("SupportTicketNotes")]
    public virtual User CreateUser { get; set; }

    [ForeignKey("SupportTicketID")]
    [InverseProperty("SupportTicketNotes")]
    public virtual SupportTicket SupportTicket { get; set; }
}
