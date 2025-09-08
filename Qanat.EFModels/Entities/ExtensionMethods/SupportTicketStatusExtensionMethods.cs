using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities
{
    public static class SupportTicketStatusExtensionMethods
    {
        public static SupportTicketStatusSimpleDto AsSimpleDto(this SupportTicketStatus supportTicketStatus)
        {
            var dto = new SupportTicketStatusSimpleDto()
            {
                SupportTicketStatusID = supportTicketStatus.SupportTicketStatusID,
                SupportTicketStatusName = supportTicketStatus.SupportTicketStatusName,
                SupportTicketStatusDisplayName = supportTicketStatus.SupportTicketStatusDisplayName
            };
            return dto;
        }
    }
}