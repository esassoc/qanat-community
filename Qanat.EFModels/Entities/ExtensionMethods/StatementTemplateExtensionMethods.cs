using System.Text.Json;
using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities;

public static class StatementTemplateExtensionMethods
{
    public static StatementTemplateSimpleDto AsSimpleDto(this StatementTemplate statementTemplate)
    {
        var dto = new StatementTemplateSimpleDto()
        {
            StatementTemplateID = statementTemplate.StatementTemplateID,
            GeographyID = statementTemplate.GeographyID,
            StatementTemplateTypeID = statementTemplate.StatementTemplateTypeID,
            TemplateName = statementTemplate.TemplateTitle,
            LastUpdated = statementTemplate.LastUpdated,
            UpdateUserID = statementTemplate.UpdateUserID,
            Description = statementTemplate.InternalDescription,
            CustomFieldsContent = statementTemplate.CustomFieldsContent,
            CustomLabels = statementTemplate.CustomLabels
        };
        return dto;
    }

    public static StatementTemplateDto AsDto(this StatementTemplate statementTemplate)
    {
        return new StatementTemplateDto()
        {
            StatementTemplateID = statementTemplate.StatementTemplateID,
            GeographyID = statementTemplate.GeographyID,
            StatementTemplateType = statementTemplate.StatementTemplateType.AsSimpleDto(),
            TemplateTitle = statementTemplate.TemplateTitle,
            LastUpdated = statementTemplate.LastUpdated,
            UpdateUserID = statementTemplate.UpdateUserID,
            UpdateUserFullName = statementTemplate.UpdateUser?.FullName,
            InternalDescription = statementTemplate.InternalDescription,
            CustomFieldsContent = JsonSerializer.Deserialize<Dictionary<string, string>>(statementTemplate.CustomFieldsContent),
            CustomLabels = JsonSerializer.Deserialize<Dictionary<string, string>>(statementTemplate.CustomLabels)
        };
    }
}