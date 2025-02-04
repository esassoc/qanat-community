//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[SupportTicketStatus]

using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities
{
    public static partial class SupportTicketStatusExtensionMethods
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