namespace Qanat.Models.DataTransferObjects.SupportTicket;

public class SupportTicketGridDto
{
    public int SupportTicketID { get; set; }
    public int GeographyID { get; set; }
    public string GeographyName { get; set; }
    public string Description { get; set; }
    public SupportTicketStatusSimpleDto SupportTicketStatus { get; set; }
    public SupportTicketPrioritySimpleDto SupportTicketPriority { get; set; }
    public SupportTicketQuestionTypeSimpleDto SupportTicketQuestionType { get; set; }
    public int? WaterAccountID { get; set; }
    public int? WaterAccountNumber { get; set; }
    public DateTime DateCreated { get; set; }
    public DateTime? DateUpdated { get; set; }
    public string CreateUserFullName { get; set; }
    public int? AssignedUserID { get; set; }
    public string? AssignedUserFullName { get; set; }
    public string ContactFirstName { get; set; }
    public string ContactLastName { get; set; }
    public string ContactEmail{ get; set; }
    public string ContactPhoneNumber{ get; set; }
}