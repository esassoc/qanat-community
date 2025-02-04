//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[SupportTicketNote]

namespace Qanat.Models.DataTransferObjects
{
    public partial class SupportTicketNoteSimpleDto
    {
        public int SupportTicketNoteID { get; set; }
        public int SupportTicketID { get; set; }
        public string Message { get; set; }
        public bool InternalNote { get; set; }
        public int CreateUserID { get; set; }
        public DateTime CreateDate { get; set; }
    }
}