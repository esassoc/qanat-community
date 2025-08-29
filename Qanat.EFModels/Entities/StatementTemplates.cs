using Microsoft.EntityFrameworkCore;
using Qanat.Models.DataTransferObjects;
using System.Text.Json;

namespace Qanat.EFModels.Entities;

public static class StatementTemplates
{
    public static List<ErrorMessage> ValidateStatementTemplate(QanatDbContext dbContext, StatementTemplateUpsertDto statementTemplateUpsertDto, int? statementTemplateID = null)
    {
        var errors = new List<ErrorMessage>();

        var templateNameConflicts = dbContext.StatementTemplates.AsNoTracking().Where(x =>
            x.TemplateTitle == statementTemplateUpsertDto.TemplateTitle &&
            x.GeographyID == statementTemplateUpsertDto.GeographyID &&
            x.StatementTemplateID != statementTemplateID).ToList();
        if (templateNameConflicts.Any())
        {
            errors.Add(new ErrorMessage() { Type = "Template Name", Message = "A template with this name already exists within this geography." });
        }

        var customFieldValidationErrors = ValidateCustomFieldsAndLabels(statementTemplateUpsertDto);
        errors.AddRange(customFieldValidationErrors);

        return errors;
    }

    private static List<ErrorMessage> ValidateCustomFieldsAndLabels(StatementTemplateUpsertDto statementTemplateUpsertDto)
    {
        var errors = new List<ErrorMessage>();

        var statementTemplateType = StatementTemplateType.All.Single(x => x.StatementTemplateTypeID == statementTemplateUpsertDto.StatementTemplateTypeID);

        // custom fields
        var statementTemplateTypeCustomFieldDefaultParagraphs = JsonSerializer.Deserialize<Dictionary<string, int>>(statementTemplateType.CustomFieldDefaultParagraphs);

        if (statementTemplateTypeCustomFieldDefaultParagraphs == null)
        {
            errors.Add(new ErrorMessage() { Type = "Custom Content Area", Message = "There was an internal error reading custom content." });
            return errors;
        }

        var customFieldNamesMatchExpected =  statementTemplateUpsertDto.CustomFieldsContent.Keys.All(key => statementTemplateTypeCustomFieldDefaultParagraphs.ContainsKey(key));
        if (!customFieldNamesMatchExpected)
        {
            errors.Add(new ErrorMessage() { Type = "Custom Content Area", Message = "The provided custom fields do not match the expected fields for this template type." });
        }

        var missingOrEmptyCustomFields = statementTemplateTypeCustomFieldDefaultParagraphs.Keys
            .Where(key => !statementTemplateUpsertDto.CustomFieldsContent.ContainsKey(key) || string.IsNullOrEmpty(statementTemplateUpsertDto.CustomFieldsContent[key])).ToList();
        if (missingOrEmptyCustomFields.Any())
        {
            var customFieldNames = string.Join(", ", missingOrEmptyCustomFields);
            errors.Add(new ErrorMessage()
            {
                Type = "Custom Content Area", 
                Message = $"Content has not been entered for the following custom content area{(missingOrEmptyCustomFields.Count > 1 ? "s" : "")}: { customFieldNames }"
            });
        }

        // custom labels
        var statementTemplateTypeCustomLabelDefaults = JsonSerializer.Deserialize<Dictionary<string, string>>(statementTemplateType.CustomLabelDefaults);

        if (statementTemplateTypeCustomLabelDefaults == null)
        {
            errors.Add(new ErrorMessage() { Type = "Custom Labels", Message = "There was an internal error reading custom labels." });
            return errors;
        }

        var customLabelNamesMatchExpected = statementTemplateUpsertDto.CustomLabels.Keys.All(key => statementTemplateTypeCustomLabelDefaults.ContainsKey(key));
        if (!customLabelNamesMatchExpected)
        {
            errors.Add(new ErrorMessage() { Type = "Custom Label", Message = "The provided custom labels are not valid for this template type." });
        }

        var missingOrEmptyCustomLabels = statementTemplateTypeCustomLabelDefaults.Keys
            .Where(key => !statementTemplateUpsertDto.CustomLabels.ContainsKey(key) || string.IsNullOrEmpty(statementTemplateUpsertDto.CustomLabels[key])).ToList();

        if (missingOrEmptyCustomLabels.Any())
        {
            var customLabelNames = string.Join(", ", missingOrEmptyCustomLabels);
            errors.Add(new ErrorMessage()
            {
                Type = "Custom Labels", 
                Message = $"A label must be provided for the following custom label slot{(missingOrEmptyCustomLabels.Count > 1 ? "s" : "")}: { customLabelNames }"
            });
        }

        return errors;
    }

    public static async Task<StatementTemplateSimpleDto> CreateStatementTemplate(QanatDbContext dbContext, int currentUserID, StatementTemplateUpsertDto statementTemplateUpsertDto)
    {
        var customFields = JsonSerializer.Serialize(statementTemplateUpsertDto.CustomFieldsContent);
        var customLabels = JsonSerializer.Serialize(statementTemplateUpsertDto.CustomLabels);

        var statementTemplate = new StatementTemplate()
        {
            GeographyID = statementTemplateUpsertDto.GeographyID,
            StatementTemplateTypeID = statementTemplateUpsertDto.StatementTemplateTypeID,
            TemplateTitle = statementTemplateUpsertDto.TemplateTitle,
            InternalDescription = statementTemplateUpsertDto.InternalDescription,
            CustomFieldsContent = customFields,
            CustomLabels = customLabels,
            UpdateUserID = currentUserID,
            LastUpdated = DateTime.UtcNow
        };

        await dbContext.StatementTemplates.AddAsync(statementTemplate);
        await dbContext.SaveChangesAsync();
        await dbContext.Entry(statementTemplate).ReloadAsync();

        var newStatementTemplate= await GetByID(dbContext, statementTemplate.StatementTemplateID);
        return newStatementTemplate.AsSimpleDto();
;    }

    public static async Task<List<StatementTemplateDto>> ListByGeographyIDAsDto(QanatDbContext dbContext, int geographyID)
    {
        var statementTemplateDtos = await dbContext.StatementTemplates.AsNoTracking()
            .Include(x => x.UpdateUser)
            .Where(x => x.GeographyID == geographyID)
            .Select(x => x.AsDto()).ToListAsync();

        return statementTemplateDtos;
    }

    public static async Task<StatementTemplate> GetByID(QanatDbContext dbContext, int statementTemplateID)
    {
        var statementTemplate = await dbContext.StatementTemplates.AsNoTracking()
            .SingleOrDefaultAsync(x => x.StatementTemplateID == statementTemplateID);

        return statementTemplate;
    }

    public static async Task<StatementTemplateSimpleDto> UpdateStatementTemplate(QanatDbContext dbContext, int statementTemplateID, int currentUserID, StatementTemplateUpsertDto statementTemplateUpsertDto)
    {
        var statementTemplate = await dbContext.StatementTemplates.SingleAsync(x => x.StatementTemplateID == statementTemplateID);

        statementTemplate.StatementTemplateTypeID = statementTemplateUpsertDto.StatementTemplateTypeID;
        statementTemplate.TemplateTitle = statementTemplateUpsertDto.TemplateTitle;
        statementTemplate.InternalDescription = statementTemplateUpsertDto.InternalDescription;
        statementTemplate.CustomFieldsContent = JsonSerializer.Serialize(statementTemplateUpsertDto.CustomFieldsContent);
        statementTemplate.CustomLabels = JsonSerializer.Serialize(statementTemplateUpsertDto.CustomLabels);
        statementTemplate.UpdateUserID = currentUserID;
        statementTemplate.LastUpdated = DateTime.UtcNow;

        await dbContext.SaveChangesAsync();

        var updatedStatementTemplate = await GetByID(dbContext, statementTemplate.StatementTemplateID);
        return updatedStatementTemplate.AsSimpleDto();
    }

    public static async Task<StatementTemplateSimpleDto> DuplicateStatementTemplateByID(QanatDbContext dbContext, int statementTemplateID, int currentUserID)
    {
        var statementTemplateUpsertDto = dbContext.StatementTemplates.AsNoTracking().Single(x => x.StatementTemplateID == statementTemplateID);

        var duplicatedStatementTemplate = new StatementTemplate()
        {
            TemplateTitle = $"Copy of {statementTemplateUpsertDto.TemplateTitle} - {DateTime.UtcNow}",
            GeographyID = statementTemplateUpsertDto.GeographyID,
            StatementTemplateTypeID = statementTemplateUpsertDto.StatementTemplateTypeID,
            InternalDescription = statementTemplateUpsertDto.InternalDescription,
            CustomFieldsContent = statementTemplateUpsertDto.CustomFieldsContent,
            CustomLabels = statementTemplateUpsertDto.CustomLabels,
            UpdateUserID = currentUserID,
            LastUpdated = DateTime.UtcNow
        };

        await dbContext.StatementTemplates.AddAsync(duplicatedStatementTemplate);
        await dbContext.SaveChangesAsync();
        await dbContext.Entry(duplicatedStatementTemplate).ReloadAsync();

        var duplicateStatementTemplate = await GetByID(dbContext, duplicatedStatementTemplate.StatementTemplateID);
        return duplicateStatementTemplate.AsSimpleDto();
    }
}