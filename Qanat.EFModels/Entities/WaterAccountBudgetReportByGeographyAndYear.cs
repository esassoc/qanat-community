using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities;

[Keyless]
public class WaterAccountBudgetReportByGeographyAndYear
{
    public int WaterAccountID { get; set; }
    public int WaterAccountNumber { get; set; }
    public string WaterAccountName { get; set; }
    public double AcresManaged { get; set; }
    public decimal? TotalSupply { get; set; }
    public decimal? UsageToDate { get; set; }
    public decimal CurrentAvailable { get; set; }

    public static IEnumerable<WaterAccountBudgetReportDto> ListByGeographyAndEffectiveDate(QanatDbContext dbContext,
        int geographyID, DateTime startDate, DateTime endDate,
        bool includeWaterSupplyBreakdown = true)
    {
        var waterSupplyBreakdownByAccountID = new Dictionary<int, Dictionary<int, decimal>>();
        if (includeWaterSupplyBreakdown)
        {
            waterSupplyBreakdownByAccountID =
                ParcelSupplies.GetSupplyByWaterAccountID(dbContext, geographyID, startDate, endDate);
        }

        var geography = Geographies.GetByID(dbContext, geographyID);

        var geographyIDParam = new SqlParameter("geographyID", geographyID);
        var yearParam = new SqlParameter("year", endDate.Year);

        var waterAccountBudgetReports = dbContext.WaterAccountBudgetReportByGeographyAndDateRanges
            .FromSqlRaw(
                $"EXECUTE dbo.pWaterAccountBudgetReportByGeographyAndYear @geographyID, @year",
                geographyIDParam, yearParam).ToList();

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
                AcresManaged = x.AcresManaged,
                TotalSupply = x.TotalSupply ?? 0,
                UsageToDate = x.UsageToDate ?? 0,
                CurrentAvailable = x.CurrentAvailable,
                WaterSupplyByWaterType =
                    includeWaterSupplyBreakdown && waterSupplyBreakdownByAccountID.ContainsKey(x.WaterAccountID)
                        ? waterSupplyBreakdownByAccountID[x.WaterAccountID]
                        : new Dictionary<int, decimal>()
            });

        return waterAccountBudgetReportDtos;
    }
}
