//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[SupportTicketQuestionType]

using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities
{
    public static partial class SupportTicketQuestionTypeExtensionMethods
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