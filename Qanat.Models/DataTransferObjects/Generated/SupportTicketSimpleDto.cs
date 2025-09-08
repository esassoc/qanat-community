//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[SupportTicket]

namespace Qanat.Models.DataTransferObjects
{
    public partial class SupportTicketSimpleDto
    {
        public int SupportTicketID { get; set; }
        public int GeographyID { get; set; }
        public string Description { get; set; }
        public int SupportTicketStatusID { get; set; }
        public int SupportTicketPriorityID { get; set; }
        public int? SupportTicketQuestionTypeID { get; set; }
        public int? WaterAccountID { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
        public int? CreateUserID { get; set; }
        public int? AssignedUserID { get; set; }
        public string ContactFirstName { get; set; }
        public string ContactLastName { get; set; }
        public string ContactEmail { get; set; }
        public string ContactPhoneNumber { get; set; }
    }
}