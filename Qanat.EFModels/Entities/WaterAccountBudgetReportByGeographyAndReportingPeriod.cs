using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities;

[Keyless]
public class WaterAccountBudgetReportByGeographyAndReportingPeriod
{
    public int WaterAccountID { get; set; }
    public int WaterAccountNumber { get; set; }
    public string WaterAccountName { get; set; }
    public int ParcelCount { get; set; }
    public double ParcelArea { get; set; }
    public double UsageLocationArea { get; set; }
    public double TotalSupply { get; set; }
    public double UsageToDate { get; set; }
    public double CurrentAvailable { get; set; }
    public string ZoneIDs { get; set; }
    public Dictionary<int, decimal> WaterTypeSupplyBreakdown { get; set; }

    public static IEnumerable<WaterAccountBudgetReportDto> ListByGeographyAndReportingPeriod(QanatDbContext dbContext, int geographyID, ReportingPeriodDto reportingPeriodDto)
    {
        var geography = Geographies.GetByID(dbContext, geographyID);

        var geographyIDParam = new SqlParameter("geographyID", geographyID);
        var reportingPeriodIDParam = new SqlParameter("reportingPeriodID", reportingPeriodDto.ReportingPeriodID);

        var waterAccountBudgetReports = dbContext.WaterAccountBudgetReportByGeographyAndReportingPeriod
            .FromSqlRaw($"EXECUTE dbo.pWaterAccountBudgetReportByGeographyAndReportingPeriod @geographyID, @reportingPeriodID", geographyIDParam, reportingPeriodIDParam).ToList();

        var waterAccountBudgetReportDtos = waterAccountBudgetReports
            .OrderBy(x => x.WaterAccountName)
            .Select(x => new WaterAccountBudgetReportDto()
            {
                GeographyID = geography.GeographyID,
                GeographyName = geography.GeographyName,
                WaterAccountID = x.WaterAccountID,
                WaterAccountName = x.WaterAccountName,
                WaterAccountUrl = $"/manage/{geography.GeographyName.ToLower()}/water-accounts/{x.WaterAccountID}",
                WaterAccountNumber = x.WaterAccountNumber,
                ParcelCount = x.ParcelCount,
                ParcelArea = x.ParcelArea,
                UsageLocationArea = x.UsageLocationArea,
                TotalSupply = x.TotalSupply,
                UsageToDate = x.UsageToDate,
                CurrentAvailable = x.CurrentAvailable,
                WaterSupplyByWaterType = x.WaterTypeSupplyBreakdown ?? new Dictionary<int, decimal>(),
                ZoneIDs = x.ZoneIDs
            });

        return waterAccountBudgetReportDtos;
    }
}
