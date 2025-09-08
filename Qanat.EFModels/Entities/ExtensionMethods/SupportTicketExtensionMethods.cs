using Qanat.Models.DataTransferObjects.SupportTicket;

namespace Qanat.EFModels.Entities;

public static class SupportTicketExtensionMethods
{
    public static SupportTicketGridDto AsGridDto(this SupportTicket supportTicket)
    {
        var dto = new SupportTicketGridDto()
        {
            AssignedUserID = supportTicket.AssignedUser?.UserID,
            AssignedUserFullName = supportTicket.AssignedUser?.FullName,
            CreateUserFullName = supportTicket.CreateUser?.FullName,
            DateCreated = supportTicket.DateCreated,
            DateUpdated = supportTicket.DateUpdated,
            DateClosed = supportTicket.DateClosed,
            Description = supportTicket.Description,
            GeographyID = supportTicket.GeographyID,
            GeographyName = supportTicket.Geography.GeographyName,
            SupportTicketPriority = supportTicket.SupportTicketPriority.AsSimpleDto(),
            SupportTicketID = supportTicket.SupportTicketID,
            SupportTicketStatus = supportTicket.SupportTicketStatus.AsSimpleDto(),
            SupportTicketQuestionType = supportTicket.SupportTicketQuestionType.AsSimpleDto(),
            WaterAccountID = supportTicket.WaterAccountID,
            WaterAccountNumber = supportTicket.WaterAccount?.WaterAccountNumber,
            ContactFirstName = supportTicket.ContactFirstName,
            ContactLastName = supportTicket.ContactLastName,
            ContactEmail = supportTicket.ContactEmail,
            ContactPhoneNumber = supportTicket.ContactPhoneNumber,
            MostRecentNoteMessage = supportTicket.SupportTicketNotes.Any() ? supportTicket.SupportTicketNotes.MaxBy(x => x.CreateDate)?.Message : null
        };

        return dto;
    }
}