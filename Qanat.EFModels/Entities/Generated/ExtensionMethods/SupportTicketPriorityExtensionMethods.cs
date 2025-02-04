//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[SupportTicketPriority]

using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities
{
    public static partial class SupportTicketPriorityExtensionMethods
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