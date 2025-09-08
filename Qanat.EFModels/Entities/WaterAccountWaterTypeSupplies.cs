using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities;

[Keyless]
public static class WaterAccountWaterTypeSupplies
{
    public static IEnumerable<WaterAccountWaterTypeSupplyDto> ListByYearAndGeography(QanatDbContext dbContext, int year,
        int geographyID)
    {
        var yearParam = new SqlParameter("year", year);
        var geographyIDParam = new SqlParameter("geographyID", geographyID);
        var waterAccountWaterTypeSupplies = dbContext.WaterAccountWaterTypeSupplies
            .FromSqlRaw($"EXECUTE dbo.pWaterTypeWaterAccountSupplyByYearAndGeography @year, @geographyID", yearParam,
                geographyIDParam).ToList();

        var parcelWaterSupplyAndUsageDtos = waterAccountWaterTypeSupplies.Select(x =>
            new WaterAccountWaterTypeSupplyDto()
            {
                WaterAccountID = x.WaterAccountID,
                WaterTypeID = x.WaterTypeID,
                WaterTypeName = x.WaterTypeName,
                TotalSupply = x.TotalSupply,
                SortOrder = x.SortOrder
            });

        return parcelWaterSupplyAndUsageDtos;
    }
}