namespace Qanat.Models.DataTransferObjects.SupportTicket;

public class SupportTicketNoteFeedDto
{
    public int SupportTicketNoteID { get; set; }
    public int SupportTicketID { get; set; }
    public string Message { get; set; }
    public bool InternalNote { get; set; }
    public int CreateUserID { get; set; }
    public string CreateUserFullName { get; set; }
    public DateTime CreateDate { get; set; }
}