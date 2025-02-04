//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[SupportTicket]
namespace Qanat.EFModels.Entities
{
    public partial class SupportTicket
    {
        public int PrimaryKey => SupportTicketID;
        public SupportTicketStatus SupportTicketStatus => SupportTicketStatus.AllLookupDictionary[SupportTicketStatusID];
        public SupportTicketPriority SupportTicketPriority => SupportTicketPriority.AllLookupDictionary[SupportTicketPriorityID];
        public SupportTicketQuestionType? SupportTicketQuestionType => SupportTicketQuestionTypeID.HasValue ? SupportTicketQuestionType.AllLookupDictionary[SupportTicketQuestionTypeID.Value] : null;

        public static class FieldLengths
        {
            public const int ContactFirstName = 50;
            public const int ContactLastName = 50;
            public const int ContactEmail = 50;
            public const int ContactPhoneNumber = 10;
        }
    }
}