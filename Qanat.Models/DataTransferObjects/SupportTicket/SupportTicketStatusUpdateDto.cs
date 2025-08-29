using System.ComponentModel.DataAnnotations;

namespace Qanat.Models.DataTransferObjects.SupportTicket;

public class SupportTicketStatusUpdateDto
{
    [Required]
    public int? SupportTicketStatusID { get; set; }

    [Required]
    public int? SupportTicketPriorityID { get; set; }
    public int? AssignedUserID { get; set; }
}