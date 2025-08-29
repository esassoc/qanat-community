using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities
{
    public static class StatementTemplateTypeExtensionMethods
    {
        public static StatementTemplateTypeSimpleDto AsSimpleDto(this StatementTemplateType statementTemplateType)
        {
            var dto = new StatementTemplateTypeSimpleDto()
            {
                StatementTemplateTypeID = statementTemplateType.StatementTemplateTypeID,
                StatementTemplateTypeName = statementTemplateType.StatementTemplateTypeName,
                StatementTemplateTypeDisplayName = statementTemplateType.StatementTemplateTypeDisplayName,
                CustomFieldDefaultParagraphs = statementTemplateType.CustomFieldDefaultParagraphs,
                CustomLabelDefaults = statementTemplateType.CustomLabelDefaults
            };
            return dto;
        }
    }
}