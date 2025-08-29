using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities;

public static class WaterTypeMonthlySupplies
{
    public static IEnumerable<WaterAccountWaterTypeMonthlySupplyDto> ListByYearAndWaterAccount(QanatDbContext dbContext, int year, int waterAccountID)
    {
        var yearParam = new SqlParameter("year", year);
        var waterAccountIDParam = new SqlParameter("waterAccountID", waterAccountID);
        var waterTypeSupplies = dbContext.WaterTypeMonthlySupplies
            .FromSqlRaw($"EXECUTE dbo.pWaterTypeMonthlySupplyByYearAndWaterAccount @year, @waterAccountID", yearParam, waterAccountIDParam).ToList();

        var waterAccountTotalWaterTypeMonthlySupplyDtos = waterTypeSupplies
            .GroupBy(x => x.WaterTypeID)
            .Select(x =>
            {
                var dto = new WaterAccountWaterTypeMonthlySupplyDto()
                {
                    WaterAccountID = x.First().WaterAccountID,
                    WaterTypeID = x.Key,
                    WaterTypeName = x.First().WaterTypeName,
                    WaterTypeColor = x.First().WaterTypeColor,
                    WaterTypeSortOrder = x.First().WaterTypeSortOrder,
                    WaterTypeDefinition = x.First().WaterTypeDefinition,
                    TotalSupply = x.Sum(y => y.CurrentSupplyAmount)
                };

                var waterTypeMonthlySuppliesWithValues = x.Where(y => y.CurrentCumulativeSupplyAmount.HasValue).ToList();
                if (waterTypeMonthlySuppliesWithValues.Any())
                {
                    dto.CumulativeSupplyDepthByMonth = waterTypeMonthlySuppliesWithValues.ToDictionary(
                        y => y.EffectiveDate.Month,
                        y => y.CurrentCumulativeSupplyAmountDepth);
                    dto.CumulativeSupplyByMonth = waterTypeMonthlySuppliesWithValues.ToDictionary(
                        y => y.EffectiveDate.Month,
                        y => y.CurrentCumulativeSupplyAmount);
                }

                return dto;
            });

        return waterAccountTotalWaterTypeMonthlySupplyDtos;
    }
}