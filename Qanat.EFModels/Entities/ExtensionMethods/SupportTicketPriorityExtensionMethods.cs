using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities
{
    public static class SupportTicketPriorityExtensionMethods
    {
        public static SupportTicketPrioritySimpleDto AsSimpleDto(this SupportTicketPriority supportTicketPriority)
        {
            var dto = new SupportTicketPrioritySimpleDto()
            {
                SupportTicketPriorityID = supportTicketPriority.SupportTicketPriorityID,
                SupportTicketPriorityName = supportTicketPriority.SupportTicketPriorityName,
                SupportTicketPriorityDisplayName = supportTicketPriority.SupportTicketPriorityDisplayName
            };
            return dto;
        }
    }
}