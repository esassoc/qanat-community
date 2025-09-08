using System.ComponentModel.DataAnnotations;

namespace Qanat.Models.DataTransferObjects.SupportTicket;

public class SupportTicketUpsertDto
{
    [Required]
    public int GeographyID { get; set; }

    [Required]
    public int? SupportTicketQuestionTypeID { get; set; }

    [Required]
    public string Description { get; set; }

    [Required]
    public string ContactFirstName { get; set; }
    public string ContactLastName { get; set; }
    public string ContactEmail { get; set; }
    public string ContactPhoneNumber { get; set; }

    public int? WaterAccountID { get; set; }
    public int? AssignedUserID { get; set; }
    public int? SupportTicketPriorityID { get; set; }
        
    public string RecaptchaToken { get; set; }
}