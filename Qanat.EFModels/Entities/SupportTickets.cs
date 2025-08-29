using Microsoft.EntityFrameworkCore;
using Qanat.Models.DataTransferObjects;
using Qanat.Models.DataTransferObjects.SupportTicket;
using System.Linq;

namespace Qanat.EFModels.Entities;

public class SupportTickets
{
    public static List<SupportTicketGridDto> GetSupportTicketsAsSimpleDto(QanatDbContext dbContext, int userID)
    {
        var userGeographies = ListManagedGeographyIDsByUser(dbContext, userID);
        var supportTickets = dbContext.SupportTickets.AsNoTracking()
            .Include(x => x.CreateUser)
            .Include(x => x.AssignedUser)
            .Include(x => x.Geography)
            .Include(x => x.WaterAccount)
            .Include(x => x.SupportTicketNotes)
            .Where(x => userGeographies.Contains(x.GeographyID))
            .Select(x => x.AsGridDto()).ToList();

        return supportTickets;
    }

    private static List<int> ListManagedGeographyIDsByUser(QanatDbContext dbContext, int userID)
    {
        var userDto = dbContext.Users.AsNoTracking()
            .Include(x => x.GeographyUsers)
            .Single(x => x.UserID == userID).AsUserDto();

        var isSystemAdmin = userDto.Flags.ContainsKey(FlagEnum.IsSystemAdmin.ToString()) && userDto.Flags[FlagEnum.IsSystemAdmin.ToString()];

        List<int> userGeographies;
        if (isSystemAdmin)
        {
            userGeographies = dbContext.Geographies.Select(x => x.GeographyID).ToList();
        }
        else
        {
            var userGeographiesImpl = dbContext.GeographyUsers.AsNoTracking()
                .Where(x => x.UserID == userDto.UserID).AsEnumerable();

            userGeographies = userGeographiesImpl
                .Where(x => userDto.GeographyFlags.ContainsKey(x.GeographyID) 
                            && userDto.GeographyFlags[x.GeographyID].ContainsKey(FlagEnum.HasManagerDashboard.ToString()) 
                            && userDto.GeographyFlags[x.GeographyID][FlagEnum.HasManagerDashboard.ToString()])
                .Select(x => x.GeographyID).ToList();
        }

        return userGeographies;
    }

    public static SupportTicketGridDto GetSupportTicketByID(QanatDbContext dbContext, int supportTicketID, int userID)
    {
        var userGeographies = ListManagedGeographyIDsByUser(dbContext, userID);
        var supportTicket = dbContext.SupportTickets.AsNoTracking()
            .Include(x => x.CreateUser)
            .Include(x => x.AssignedUser)
            .Include(x => x.Geography)
            .Include(x => x.WaterAccount)
            .Include(x => x.SupportTicketNotes)
            .SingleOrDefault(x => x.SupportTicketID == supportTicketID);

        if (supportTicket == null || !userGeographies.Contains(supportTicket.GeographyID))
        {
            return null;
        }

        return supportTicket.AsGridDto();
    }

    public static async Task<SupportTicket> CreateSupportTicket(QanatDbContext dbContext, SupportTicketUpsertDto supportTicketUpsertDto, int? userID)
    {
        var supportTicket = new SupportTicket()
        {
            GeographyID = supportTicketUpsertDto.GeographyID,
            SupportTicketStatusID = supportTicketUpsertDto.AssignedUserID != null ? (int)SupportTicketStatusEnum.Active : (int)SupportTicketStatusEnum.Unassigned,
            SupportTicketPriorityID = supportTicketUpsertDto.SupportTicketPriorityID ?? (int)SupportTicketPriorityEnum.NotPrioritized,
            SupportTicketQuestionTypeID = supportTicketUpsertDto.SupportTicketQuestionTypeID,
            Description = supportTicketUpsertDto.Description,
            CreateUserID = userID,
            DateCreated = DateTime.UtcNow,
            AssignedUserID = supportTicketUpsertDto.AssignedUserID,
            WaterAccountID = supportTicketUpsertDto.WaterAccountID,
            ContactFirstName = supportTicketUpsertDto.ContactFirstName,
            ContactLastName = supportTicketUpsertDto.ContactLastName,
            ContactEmail = supportTicketUpsertDto.ContactEmail,
            ContactPhoneNumber = supportTicketUpsertDto.ContactPhoneNumber == "null" ? null : supportTicketUpsertDto.ContactPhoneNumber,
        };

        dbContext.AddRange(supportTicket);
        await dbContext.SaveChangesAsync();

        await dbContext.Entry(supportTicket).ReloadAsync();

        return supportTicket;
    }

    public static async Task UpdateSupportTicket(QanatDbContext dbContext, SupportTicket supportTicket,
        SupportTicketUpsertDto supportTicketUpsertDto)
    {
        supportTicket.Description = supportTicketUpsertDto.Description;
        supportTicket.ContactFirstName = supportTicketUpsertDto.ContactFirstName;
        supportTicket.ContactLastName = supportTicketUpsertDto.ContactLastName;
        supportTicket.ContactEmail = supportTicketUpsertDto.ContactEmail;
        supportTicket.ContactPhoneNumber = supportTicketUpsertDto.ContactPhoneNumber;
        supportTicket.WaterAccountID = supportTicketUpsertDto.WaterAccountID != 0 ? supportTicketUpsertDto.WaterAccountID : null;
        supportTicket.DateUpdated = DateTime.UtcNow;

        await dbContext.SaveChangesAsync();
        await dbContext.Entry(supportTicket).ReloadAsync();
    }

    public static List<ErrorMessage> ValidateStatusUpdate(SupportTicketStatusUpdateDto supportTicketStatusUpdateDto)
    {
        var errors = new List<ErrorMessage>();

        if (supportTicketStatusUpdateDto.AssignedUserID.HasValue &&
            supportTicketStatusUpdateDto.SupportTicketStatusID == SupportTicketStatus.Unassigned.SupportTicketStatusID)
        {
            errors.Add(new ErrorMessage() { Type = "Status", Message = "Ticket status cannot be set to unassigned while ticket has an assigned user."});
        }
        else if (!supportTicketStatusUpdateDto.AssignedUserID.HasValue &&
            supportTicketStatusUpdateDto.SupportTicketStatusID == SupportTicketStatus.Active.SupportTicketStatusID)
        {
            errors.Add(new ErrorMessage() { Type = "Status", Message = "Ticket status cannot be set to active without an assigned user."});
        }

        return errors;
    }

    public static async Task UpdateStatus(QanatDbContext dbContext, SupportTicket supportTicket, SupportTicketStatusUpdateDto supportTicketStatusUpdateDto)
    {
        supportTicket.SupportTicketStatusID = supportTicketStatusUpdateDto.SupportTicketStatusID.Value;
        supportTicket.AssignedUserID = supportTicketStatusUpdateDto.AssignedUserID;
        supportTicket.SupportTicketPriorityID = supportTicketStatusUpdateDto.SupportTicketPriorityID.Value;
        supportTicket.DateUpdated = DateTime.UtcNow;
        supportTicket.DateClosed = supportTicketStatusUpdateDto.SupportTicketStatusID == SupportTicketStatus.Closed.SupportTicketStatusID
                ? DateTime.UtcNow
                : null;

        await dbContext.SaveChangesAsync();
    }

    public static async Task CloseSupportTicket(QanatDbContext dbContext, int supportTicketID)
    {
        var supportTicket = dbContext.SupportTickets.Single(x => x.SupportTicketID == supportTicketID);
        var currentTime = DateTime.UtcNow;
        supportTicket.DateUpdated = currentTime;
        supportTicket.DateClosed = currentTime;
        supportTicket.SupportTicketStatusID = (int)SupportTicketStatusEnum.Closed;
        await dbContext.SaveChangesAsync();
        await dbContext.Entry(supportTicket).ReloadAsync();
    }

    public static async Task ReopenSupportTicket(QanatDbContext dbContext, int supportTicketID)
    {
        var supportTicket = dbContext.SupportTickets.Single(x => x.SupportTicketID == supportTicketID);
        supportTicket.DateUpdated = DateTime.UtcNow;
        supportTicket.DateClosed = null;
        supportTicket.SupportTicketStatusID = supportTicket.AssignedUserID != null ? (int)SupportTicketStatusEnum.Active : (int)SupportTicketStatusEnum.Unassigned;
        await dbContext.SaveChangesAsync();
        await dbContext.Entry(supportTicket).ReloadAsync();
    }
}