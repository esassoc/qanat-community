namespace Qanat.Models.DataTransferObjects.SupportTicket;

public class SupportTicketUpsertDto
{
    public int? SupportTicketID { get; set; }
    public int GeographyID { get; set; }
    public string Description { get; set; }
    public int? SupportTicketStatusID { get; set; }
    public int? SupportTicketPriorityID { get; set; }
    public int? SupportTicketQuestionTypeID { get; set; }
    public WaterAccountSimpleDto? WaterAccount { get; set; }
    public int? AssignedUserID { get; set; }
    public string ContactFirstName { get; set; }
    public string ContactLastName { get; set; }
    public string ContactEmail { get; set; }
    public string ContactPhoneNumber { get; set; }
}