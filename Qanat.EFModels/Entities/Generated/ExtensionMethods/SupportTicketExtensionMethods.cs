//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[SupportTicket]

using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities
{
    public static partial class SupportTicketExtensionMethods
    {
        public static SupportTicketSimpleDto AsSimpleDto(this SupportTicket supportTicket)
        {
            var dto = new SupportTicketSimpleDto()
            {
                SupportTicketID = supportTicket.SupportTicketID,
                GeographyID = supportTicket.GeographyID,
                Description = supportTicket.Description,
                SupportTicketStatusID = supportTicket.SupportTicketStatusID,
                SupportTicketPriorityID = supportTicket.SupportTicketPriorityID,
                SupportTicketQuestionTypeID = supportTicket.SupportTicketQuestionTypeID,
                WaterAccountID = supportTicket.WaterAccountID,
                DateCreated = supportTicket.DateCreated,
                DateUpdated = supportTicket.DateUpdated,
                CreateUserID = supportTicket.CreateUserID,
                AssignedUserID = supportTicket.AssignedUserID,
                ContactFirstName = supportTicket.ContactFirstName,
                ContactLastName = supportTicket.ContactLastName,
                ContactEmail = supportTicket.ContactEmail,
                ContactPhoneNumber = supportTicket.ContactPhoneNumber
            };
            return dto;
        }
    }
}