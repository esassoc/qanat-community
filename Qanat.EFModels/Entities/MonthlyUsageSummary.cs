using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities;

[Keyless]
public class MonthlyUsageSummary
{
    public int ParcelID { get; set; }
    public string ParcelNumber { get; set; }
    public int WaterMeasurementTypeID { get; set; }
    public string WaterMeasurementTypeName { get; set; }
    public string WaterMeasurementCategoryTypeName { get; set; }
    public DateTime EffectiveDate { get; set; }
    public decimal? CurrentUsageAmount { get; set; }
    public decimal? AverageUsageAmount { get; set; }
    public decimal? CurrentCumulativeUsageAmount { get; set; }
    public decimal? AverageCumulativeUsageAmount { get; set; }
    public decimal? CurrentUsageAmountDepth { get; set; }
    public decimal? AverageUsageAmountDepth { get; set; }
    public decimal? UsageEntityArea { get; set; }

    public static List<WaterAccountParcelWaterMeasurementDto> ListByWaterAccountAndYear(QanatDbContext dbContext,
        int waterAccountID, int year, int userID)
    {
        var waterAccountIDParam = new SqlParameter("waterAccountID", waterAccountID);
        var yearParam = new SqlParameter("year", year);
        var userIDParam = new SqlParameter("userID", userID);

        var waterAccountCumulativeWaterUsages = dbContext.MonthlyUsageSummary.FromSqlRaw($"EXECUTE dbo.pWaterAccountMonthlyUsageSummary @waterAccountID, @year, @userID", waterAccountIDParam, yearParam, userIDParam).ToList();
        return ListAsDto(waterAccountCumulativeWaterUsages);
    }

    public static List<WaterAccountParcelWaterMeasurementDto> ListByGeographyAndYear(QanatDbContext dbContext, int geographyID, int year)
    {
        var geographyIDParam = new SqlParameter("geographyID", geographyID);
        var yearParam = new SqlParameter("year", year);

        var geographyCumulativeWaterUsages = dbContext.MonthlyUsageSummary.FromSqlRaw($"EXECUTE dbo.pGeographyMonthlyUsageSummary @geographyID, @year", geographyIDParam, yearParam).ToList();
        return ListAsDto(geographyCumulativeWaterUsages);
    }

    private static List<WaterAccountParcelWaterMeasurementDto> ListAsDto(IEnumerable<MonthlyUsageSummary> monthlyUsageSummaries)
    {
        var waterAccountParcelWaterMeasurementDtos = new List<WaterAccountParcelWaterMeasurementDto>();
        var parcelWaterMeasurementTypeGroups = monthlyUsageSummaries.GroupBy(x => new { x.ParcelID, x.WaterMeasurementTypeID }).ToList();
        foreach (var parcelWaterMeasurementTypeGroup in parcelWaterMeasurementTypeGroups)
        {
            var parcelWaterMeasurementTypeDetails = parcelWaterMeasurementTypeGroup.First();
            var waterAccountParcelWaterMeasurementDto = new WaterAccountParcelWaterMeasurementDto
            {
                ParcelID = parcelWaterMeasurementTypeDetails.ParcelID,
                ParcelNumber = parcelWaterMeasurementTypeDetails.ParcelNumber,
                WaterMeasurementTypeID = parcelWaterMeasurementTypeDetails.WaterMeasurementTypeID,
                WaterMeasurementTypeName = parcelWaterMeasurementTypeDetails.WaterMeasurementTypeName,
                WaterMeasurementCategoryTypeName = parcelWaterMeasurementTypeDetails.WaterMeasurementCategoryTypeName,
                UsageEntityArea = parcelWaterMeasurementTypeDetails.UsageEntityArea
            };
            var waterMeasurementMonthlyValues = parcelWaterMeasurementTypeGroup.Select(x => new MonthlyUsageSummaryDto
            {
                EffectiveDate = x.EffectiveDate,
                EffectiveMonth = x.EffectiveDate.Month,
                CurrentUsageAmount = x.CurrentUsageAmount,
                AverageUsageAmount = x.AverageUsageAmount,
                CurrentCumulativeUsageAmount = x.CurrentCumulativeUsageAmount,
                AverageCumulativeUsageAmount = x.AverageCumulativeUsageAmount,
                CurrentUsageAmountDepth = x.CurrentUsageAmountDepth,
                AverageUsageAmountDepth = x.AverageUsageAmountDepth,
            }).ToList();
            waterAccountParcelWaterMeasurementDto.WaterMeasurementMonthlyValues = waterMeasurementMonthlyValues;
            waterAccountParcelWaterMeasurementDto.WaterMeasurementTotalValue = parcelWaterMeasurementTypeGroup.Sum(x => x.CurrentUsageAmount);
            waterAccountParcelWaterMeasurementDtos.Add(waterAccountParcelWaterMeasurementDto);
        }

        return waterAccountParcelWaterMeasurementDtos;
    }
}