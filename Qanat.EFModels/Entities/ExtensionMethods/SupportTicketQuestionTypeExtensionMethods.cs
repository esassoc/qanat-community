using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities
{
    public static class SupportTicketQuestionTypeExtensionMethods
    {
        public static SupportTicketQuestionTypeSimpleDto AsSimpleDto(this SupportTicketQuestionType supportTicketQuestionType)
        {
            var dto = new SupportTicketQuestionTypeSimpleDto()
            {
                SupportTicketQuestionTypeID = supportTicketQuestionType.SupportTicketQuestionTypeID,
                SupportTicketQuestionTypeName = supportTicketQuestionType.SupportTicketQuestionTypeName,
                SupportTicketQuestionTypeDisplayName = supportTicketQuestionType.SupportTicketQuestionTypeDisplayName
            };
            return dto;
        }
    }
}