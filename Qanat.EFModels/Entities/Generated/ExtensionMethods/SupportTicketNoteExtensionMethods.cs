//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[SupportTicketNote]

using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities
{
    public static partial class SupportTicketNoteExtensionMethods
    {
        public static SupportTicketNoteSimpleDto AsSimpleDto(this SupportTicketNote supportTicketNote)
        {
            var dto = new SupportTicketNoteSimpleDto()
            {
                SupportTicketNoteID = supportTicketNote.SupportTicketNoteID,
                SupportTicketID = supportTicketNote.SupportTicketID,
                Message = supportTicketNote.Message,
                InternalNote = supportTicketNote.InternalNote,
                CreateUserID = supportTicketNote.CreateUserID,
                CreateDate = supportTicketNote.CreateDate
            };
            return dto;
        }
    }
}