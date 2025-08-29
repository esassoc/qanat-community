using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using Qanat.Common.GeoSpatial;
using Qanat.Common.Util;
using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities;

public static class UsageStatementWaterAccounts
{
    public static readonly string UsageStatementDefaultTitle = "Water Account Groundwater Usage Summary";

    public static List<UsageStatementWaterAccountDto> ListByStatementBatchID(QanatDbContext dbContext, int statementBatchID)
    {
        var usageStatementWaterAccountDtos = WaterAccountForUsageStatement.ListByStatementBatchID(dbContext, statementBatchID);
        usageStatementWaterAccountDtos = PopulateUsageStatementWaterAccountDtos(dbContext, statementBatchID, usageStatementWaterAccountDtos);

        return usageStatementWaterAccountDtos;
    }

    public static List<UsageStatementWaterAccountDto> GetByWaterAccountIDAndStatementBatchID(QanatDbContext dbContext, int waterAccountID, int statementBatchID)
    {
        var statementBatch = dbContext.StatementBatches.AsNoTracking().Single(x => x.StatementBatchID == statementBatchID);

        var usageStatementWaterAccountDto = WaterAccountForUsageStatement.GetByWaterAccountIDandReportingPeriodID(dbContext, waterAccountID, statementBatch.ReportingPeriodID);
        var usageStatementWaterAccountDtos = PopulateUsageStatementWaterAccountDtos(dbContext, statementBatchID, new() { usageStatementWaterAccountDto });

        return usageStatementWaterAccountDtos;
    }

    private static List<UsageStatementWaterAccountDto> PopulateUsageStatementWaterAccountDtos(QanatDbContext dbContext, int statementBatchID, List<UsageStatementWaterAccountDto> usageStatementWaterAccountDtos)
    {
        var statementBatch = dbContext.StatementBatches.AsNoTracking()
            .Include(x => x.StatementTemplate)
            .Single(x => x.StatementBatchID == statementBatchID);

        usageStatementWaterAccountDtos = PopulateUsageStatementWaterAccountDtos(dbContext, statementBatch.GeographyID, statementBatch.ReportingPeriodID, statementBatch.StatementTemplate.TemplateTitle, usageStatementWaterAccountDtos);

        var customFields = JsonSerializer.Deserialize<Dictionary<string, string>>(statementBatch.StatementTemplate.CustomFieldsContent);
        var customLabels = JsonSerializer.Deserialize<Dictionary<string, string>>(statementBatch.StatementTemplate.CustomLabels);

        usageStatementWaterAccountDtos.ForEach(x =>
        {
            x.CustomFields = customFields;
            x.CustomLabels = customLabels;
        });

        return usageStatementWaterAccountDtos;
    }

    public static List<UsageStatementWaterAccountDto> GetForPreviewByWaterAccountID(QanatDbContext dbContext, int geographyID, int reportingPeriodID, StatementTemplatePdfPreviewRequestDto requestDto)
    {
        var statementTitle = string.IsNullOrEmpty(requestDto.StatementTemplateTitle) ? UsageStatementDefaultTitle : requestDto.StatementTemplateTitle;

        var usageStatementWaterAccountDto = WaterAccountForUsageStatement.GetByWaterAccountIDandReportingPeriodID(dbContext, requestDto.WaterAccountID, reportingPeriodID);
        var usageStatementWaterAccountDtos = PopulateUsageStatementWaterAccountDtos(dbContext, geographyID, reportingPeriodID, statementTitle, new() { usageStatementWaterAccountDto });

        var statementTemplateType = StatementTemplateType.All.Single(x => x.StatementTemplateTypeID == requestDto.StatementTemplateTypeID);
        var customFieldDefaultParagraphs = JsonSerializer.Deserialize<Dictionary<string, int>>(statementTemplateType.CustomFieldDefaultParagraphs);

        var customFieldsContentWithDefaultText = new Dictionary<string, string>();
        foreach (var key in customFieldDefaultParagraphs.Keys)
        {
            var value = requestDto.CustomFields != null && requestDto.CustomFields.ContainsKey(key) && !string.IsNullOrEmpty(requestDto.CustomFields[key])
                ? requestDto.CustomFields[key]
                : DefaultTextHelper.GetDefaultTextByNumberOfParagraphsForHtml(customFieldDefaultParagraphs[key]);

            customFieldsContentWithDefaultText.Add(key, value);
        }

        usageStatementWaterAccountDtos.ForEach(x =>
        {
            x.CustomFields = customFieldsContentWithDefaultText;
            x.CustomLabels = requestDto.CustomLabels;
        });

        return usageStatementWaterAccountDtos;
    }

    private static List<UsageStatementWaterAccountDto> PopulateUsageStatementWaterAccountDtos(QanatDbContext dbContext, int geographyID, int reportingPeriodID, string statementTemplateTitle, List<UsageStatementWaterAccountDto> usageStatementWaterAccountDtos)
    {
        var geography = dbContext.Geographies.AsNoTracking()
            .Single(x => x.GeographyID == geographyID);

        var reportingPeriod = dbContext.ReportingPeriods.AsNoTracking()
            .Single(x => x.ReportingPeriodID == reportingPeriodID);
        
        var totalSupplyForYearByWaterAccountID = WaterAccountWaterTypeSupplies.ListByYearAndGeography(dbContext, reportingPeriod.EndDate.Year, geographyID)
            .GroupBy(x => x.WaterAccountID)
            .ToLookup(x => x.Key, x => x.ToList().OrderBy(y => y.SortOrder));

        var defaultSupplyTableRows = dbContext.WaterTypes.AsNoTracking().Where(x => x.GeographyID == geographyID)
            .Select(x => new UsageStatementSupplyTableRowDto()
            {
                SupplyType = x.WaterTypeName,
                StartingAllocation = null,
                StartingAllocationFormatted = "-",
                UsageValueFormatted = "-",
                RemainingBalanceFormatted = "-"
            }).ToList();

        var sourceOfRecordWaterMeasurementTypeID = geography.SourceOfRecordWaterMeasurementTypeID;
        var sourceOfRecordUsageByParcelID = MonthlyUsageSummary.ListByGeographyAndYearAsWaterMeasurementDtos(dbContext, geographyID, reportingPeriod.EndDate.Year)
            .Where(x => x.WaterMeasurementTypeID == sourceOfRecordWaterMeasurementTypeID)
            .ToDictionary(x => x.ParcelID);

        var parcelIDs = usageStatementWaterAccountDtos.SelectMany(x => x.ParcelIDs).ToList();
        var parcelGeometryByParcelID = dbContext.ParcelGeometries.Where(x => parcelIDs.Contains(x.ParcelID))
            .ToDictionary(x => x.ParcelID, x => x.Geometry4326);

        var statementDate = DateTime.UtcNow;

        foreach (var usageStatementWaterAccountDto in usageStatementWaterAccountDtos)
        {
            usageStatementWaterAccountDto.StatementTitle = statementTemplateTitle;
            usageStatementWaterAccountDto.StatementDate = statementDate;

            usageStatementWaterAccountDto.ParcelAreaFormatted = usageStatementWaterAccountDto.ParcelArea.ToString("N2");
            usageStatementWaterAccountDto.UsageAreaFormatted = usageStatementWaterAccountDto.UsageArea.ToString("N2");

            // parcel geometries
            var parcelGeometryKeys = parcelGeometryByParcelID.Keys
                .Where(key => usageStatementWaterAccountDto.ParcelIDs.Contains(key)).ToList();
            var parcelGeometries = parcelGeometryKeys.Select(key => parcelGeometryByParcelID[key]).ToArray();

            usageStatementWaterAccountDto.ParcelGeoJSON = new GeometryCollection(parcelGeometries).ToGeoJSON();

            // supply & usage values

            var totalSupply = totalSupplyForYearByWaterAccountID[usageStatementWaterAccountDto.WaterAccountID].Any()
                ? totalSupplyForYearByWaterAccountID[usageStatementWaterAccountDto.WaterAccountID]
                    .Sum(x => x.Sum(y => y.TotalSupply))
                : null;

            var usageEntryKeys = sourceOfRecordUsageByParcelID.Keys.Where(key => usageStatementWaterAccountDto.ParcelIDs.Contains(key)).ToList();
            var totalUsage = usageEntryKeys.Any() ? usageEntryKeys.Sum(key => sourceOfRecordUsageByParcelID[key].WaterMeasurementTotalValue) : null;

            var parcelAreaAsDecimal = (decimal)usageStatementWaterAccountDto.ParcelArea;

            var supplyDepth = totalSupply / parcelAreaAsDecimal;
            var usageDepth = totalUsage / parcelAreaAsDecimal;
            var balanceVolume = totalSupply - totalUsage;
            var balanceDepth = balanceVolume / parcelAreaAsDecimal;

            usageStatementWaterAccountDto.SupplyDepthFormatted = supplyDepth.HasValue ? supplyDepth.Value.ToString("N2") : "-";
            usageStatementWaterAccountDto.UsageDepthFormatted = usageDepth.HasValue ? usageDepth.Value.ToString("N2") : "-";
            usageStatementWaterAccountDto.UsageVolumeFormatted = totalUsage.HasValue ? totalUsage.Value.ToString("N2") : "-";
            usageStatementWaterAccountDto.BalanceVolumeFormatted = balanceVolume.HasValue ? balanceVolume.Value.ToString("N2") : "-";
            usageStatementWaterAccountDto.BalanceDepthFormatted = balanceDepth.HasValue ? balanceDepth.Value.ToString("N2") : "-";

            var supplyTableRowDtos = totalSupplyForYearByWaterAccountID[usageStatementWaterAccountDto.WaterAccountID].Any()
                ? totalSupplyForYearByWaterAccountID[usageStatementWaterAccountDto.WaterAccountID].SelectMany(x =>
                    x.Select(y => new UsageStatementSupplyTableRowDto()
                    {
                        SupplyType = y.WaterTypeName,
                        StartingAllocation = y.TotalSupply / parcelAreaAsDecimal
                    })
                ).ToList()
                : null;
            usageStatementWaterAccountDto.SupplyTableRowDtos = PopulateSupplyTableRows(supplyTableRowDtos, totalUsage / parcelAreaAsDecimal) ?? defaultSupplyTableRows;


            // vega spec
            var usageChartDtos = usageEntryKeys.SelectMany(key => 
                sourceOfRecordUsageByParcelID[key].WaterMeasurementMonthlyValues.Select(x => new MonthlyUsageChartDataDto()
                {
                    EffectiveDate = x.EffectiveDate,
                    Value = x.CurrentUsageAmount ?? 0
                })).ToList();
            usageStatementWaterAccountDto.UsageChartDataDtos = usageChartDtos;
        }

        return usageStatementWaterAccountDtos;
    }

    private static List<UsageStatementSupplyTableRowDto> PopulateSupplyTableRows(List<UsageStatementSupplyTableRowDto> supplyTableRowDtos, decimal? usageValue)
    {
        if (supplyTableRowDtos == null) return null;

        foreach (var dto in supplyTableRowDtos)
        {
            dto.StartingAllocationFormatted = dto.StartingAllocation.HasValue ? dto.StartingAllocation.Value.ToString("N2") : "-";

            if (dto.StartingAllocation.HasValue && usageValue.HasValue)
            {
                dto.UsageValueFormatted = usageValue.Value > dto.StartingAllocation
                    ? dto.StartingAllocationFormatted
                    : usageValue.Value.ToString("N2");

                var remainingBalance = usageValue.Value > dto.StartingAllocation
                    ? 0
                    : dto.StartingAllocation.Value - usageValue.Value;

                dto.RemainingBalanceFormatted = remainingBalance.ToString("N2");

                usageValue = remainingBalance == 0 ? usageValue - dto.StartingAllocation : 0;
            }
            else
            {
                dto.UsageValueFormatted = "-";
                dto.RemainingBalanceFormatted = "-";
            }
        }

        return supplyTableRowDtos;
    }
}
