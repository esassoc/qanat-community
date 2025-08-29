using Microsoft.EntityFrameworkCore;
using Qanat.Models.DataTransferObjects;
using Qanat.Models.DataTransferObjects.SupportTicket;

namespace Qanat.EFModels.Entities;

public class SupportTicketNotes
{
    public static List<SupportTicketNoteFeedDto> GetNotesForSupportTicket(QanatDbContext dbContext, int supportTicketID)
    {
        var notes = dbContext.SupportTicketNotes.Where(x => x.SupportTicketID == supportTicketID)
            .Include(x => x.CreateUser)
            .Select(x => new SupportTicketNoteFeedDto()
            {
                SupportTicketNoteID = x.SupportTicketNoteID,
                SupportTicketID = x.SupportTicketID,
                Message = x.Message,
                InternalNote = x.InternalNote,
                CreateUserID = x.CreateUserID,
                CreateUserFullName = x.CreateUser.FullName,
                CreateDate = x.CreateDate
            }).ToList();
        return notes;
    }

    public static async Task CreateSupportTicketNote(QanatDbContext dbContext, SupportTicketNoteSimpleDto supportTicketNoteSimpleDto)
    {
        var supportTicket =
            dbContext.SupportTickets.Single(x => x.SupportTicketID == supportTicketNoteSimpleDto.SupportTicketID);
        supportTicket.DateUpdated = DateTime.UtcNow;
        ;
        var supportTicketNote = new SupportTicketNote()
        {
            SupportTicketID = supportTicketNoteSimpleDto.SupportTicketID,
            Message = supportTicketNoteSimpleDto.Message,
            CreateDate = DateTime.UtcNow,
            CreateUserID = supportTicketNoteSimpleDto.CreateUserID,
            InternalNote = supportTicketNoteSimpleDto.InternalNote
        };

        dbContext.SupportTicketNotes.Add(supportTicketNote);
        await dbContext.SaveChangesAsync();
    }
}