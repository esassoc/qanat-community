using System.Security.Cryptography.X509Certificates;
using Microsoft.EntityFrameworkCore;
using Qanat.Models.DataTransferObjects;
using Qanat.Models.DataTransferObjects.SupportTicket;

namespace Qanat.EFModels.Entities;

public class SupportTickets
{
    public static List<SupportTicketGridDto> GetSupportTicketsAsSimpleDto(QanatDbContext dbContext, int userID)
    {
        var userGeographies = GetUserGeographies(dbContext, userID);
        var supportTickets = dbContext.SupportTickets.AsNoTracking()
            .Include(x => x.CreateUser)
            .Include(x => x.AssignedUser)
            .Include(x => x.Geography)
            .Include(x => x.WaterAccount)
            .Where(x => userGeographies.Contains(x.GeographyID))
            .Select(x => new SupportTicketGridDto()
            {
                AssignedUserID = x.AssignedUser.UserID,
                AssignedUserFullName = x.AssignedUser.FullName,
                CreateUserFullName = x.CreateUser.FullName,
                DateCreated = x.DateCreated,
                DateUpdated = x.DateUpdated,
                Description = x.Description,
                GeographyID = x.GeographyID,
                GeographyName = x.Geography.GeographyName,
                SupportTicketPriority = x.SupportTicketPriority.AsSimpleDto(),
                SupportTicketID = x.SupportTicketID,
                SupportTicketStatus = x.SupportTicketStatus.AsSimpleDto(),
                SupportTicketQuestionType = x.SupportTicketQuestionType.AsSimpleDto(),
                WaterAccountID = x.WaterAccountID,
                WaterAccountNumber = x.WaterAccount.WaterAccountNumber,
                ContactFirstName = x.ContactFirstName,
                ContactLastName = x.ContactLastName,
                ContactEmail = x.ContactEmail,
                ContactPhoneNumber = x.ContactPhoneNumber,
            }).ToList();
        return supportTickets;
    }

    private static List<int> GetUserGeographies(QanatDbContext dbContext, int userID)
    {
        var isSysAdmin = dbContext.Users.Single(x => x.UserID == userID).Role == Role.SystemAdmin;
        List<int> userGeographies;
        if (isSysAdmin)
        {
            userGeographies = dbContext.Geographies.Select(x => x.GeographyID).ToList();
        }
        else
        {
            userGeographies = dbContext.GeographyUsers.AsNoTracking()
                .Where(x => x.UserID == userID && x.GeographyRoleID == GeographyRole.WaterManager.GeographyRoleID)
                .Select(x => x.GeographyID).ToList();
        }

        return userGeographies;
    }

    public static SupportTicketGridDto GetSupportTicketByID(QanatDbContext dbContext, int supportTicketID, int userID)
    {
        var userGeographies = GetUserGeographies(dbContext, userID);
        var supportTicket = dbContext.SupportTickets.AsNoTracking()
            .Include(x => x.CreateUser)
            .Include(x => x.AssignedUser)
            .Include(x => x.Geography)
            .Include(x => x.WaterAccount)
            .Single(x => x.SupportTicketID == supportTicketID);

        if (userGeographies.Contains(supportTicket.GeographyID))
        {
            return new SupportTicketGridDto()
            {
                AssignedUserID = supportTicket.AssignedUser?.UserID,
                AssignedUserFullName = supportTicket.AssignedUser?.FullName,
                CreateUserFullName = supportTicket.CreateUser?.FullName,
                DateCreated = supportTicket.DateCreated,
                DateUpdated = supportTicket.DateUpdated,
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
            };
        }

        return null;
    }

    public static async Task CreateSupportTicket(QanatDbContext dbContext, SupportTicketUpsertDto supportTicketUpsertDto, int? userID)
    {
        var supportTicket = new SupportTicket()
        {
            GeographyID = supportTicketUpsertDto.GeographyID,
            SupportTicketStatusID = supportTicketUpsertDto.AssignedUserID != null ? (int)SupportTicketStatusEnum.Assigned : (int)SupportTicketStatusEnum.Unassigned,
            SupportTicketPriorityID = supportTicketUpsertDto.SupportTicketPriorityID ?? (int)SupportTicketPriorityEnum.NotPrioritized,
            SupportTicketQuestionTypeID = supportTicketUpsertDto.SupportTicketQuestionTypeID,
            Description = supportTicketUpsertDto.Description,
            CreateUserID = userID,
            DateCreated = DateTime.UtcNow,
            AssignedUserID = supportTicketUpsertDto.AssignedUserID,
            WaterAccountID = supportTicketUpsertDto.WaterAccount?.WaterAccountID,
            ContactFirstName = supportTicketUpsertDto.ContactFirstName,
            ContactLastName = supportTicketUpsertDto.ContactLastName,
            ContactEmail = supportTicketUpsertDto.ContactEmail,
            ContactPhoneNumber = supportTicketUpsertDto.ContactPhoneNumber == "null" ? null : supportTicketUpsertDto.ContactPhoneNumber,
        };
        dbContext.AddRange(supportTicket);
        await dbContext.SaveChangesAsync();
    }

    public static async Task UpdateSupportTicket(QanatDbContext dbContext, SupportTicketUpsertDto supportTicketUpsertDto)
    {
        var currentSupportTicket =
            dbContext.SupportTickets.Single(x => x.SupportTicketID == supportTicketUpsertDto.SupportTicketID);
        currentSupportTicket.Description = supportTicketUpsertDto.Description;
        currentSupportTicket.SupportTicketStatusID = supportTicketUpsertDto.AssignedUserID != null ? (int)SupportTicketStatusEnum.Assigned : (int)SupportTicketStatusEnum.Unassigned;
        currentSupportTicket.AssignedUserID = supportTicketUpsertDto.AssignedUserID;
        currentSupportTicket.SupportTicketPriorityID = (int)supportTicketUpsertDto.SupportTicketPriorityID;
        currentSupportTicket.DateUpdated = DateTime.UtcNow;
        currentSupportTicket.ContactFirstName = supportTicketUpsertDto.ContactFirstName;
        currentSupportTicket.ContactLastName = supportTicketUpsertDto.ContactLastName;
        currentSupportTicket.ContactEmail = supportTicketUpsertDto.ContactEmail;
        currentSupportTicket.ContactPhoneNumber = supportTicketUpsertDto.ContactPhoneNumber;
        currentSupportTicket.WaterAccountID = supportTicketUpsertDto.WaterAccount != null && supportTicketUpsertDto.WaterAccount?.WaterAccountID != 0 ? supportTicketUpsertDto.WaterAccount.WaterAccountID : null;
        currentSupportTicket.GeographyID = supportTicketUpsertDto.GeographyID;

        await dbContext.SaveChangesAsync();
    }

    public static async Task CloseSupportTicket(QanatDbContext dbContext, int supportTicketID)
    {
        var currentSupportTicket = dbContext.SupportTickets.Single(x => x.SupportTicketID == supportTicketID);
        currentSupportTicket.DateUpdated = DateTime.UtcNow;
        currentSupportTicket.SupportTicketStatusID = (int)SupportTicketStatusEnum.Closed;
        await dbContext.SaveChangesAsync();
    }

    public static async Task ReopenSupportTicket(QanatDbContext dbContext, int supportTicketID)
    {
        var currentSupportTicket = dbContext.SupportTickets.Single(x => x.SupportTicketID == supportTicketID);
        currentSupportTicket.DateUpdated = DateTime.UtcNow;
        currentSupportTicket.SupportTicketStatusID = currentSupportTicket.AssignedUserID != null ? (int)SupportTicketStatusEnum.Assigned : (int)SupportTicketStatusEnum.Unassigned;
        await dbContext.SaveChangesAsync();
    }
}