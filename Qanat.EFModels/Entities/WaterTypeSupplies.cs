using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities;

public static class WaterTypeSupplies
{
    public static IEnumerable<WaterTypeSupplyDto> ListByYearAndGeography(QanatDbContext dbContext, int year, int geographyID)
    {
        var yearParam = new SqlParameter("year", year);
        var geographyIDParam = new SqlParameter("geographyID", geographyID);
        var waterTypeSupplies = dbContext.WaterTypeSupplies.FromSqlRaw($"EXECUTE dbo.pWaterTypeSupplyByYearAndGeography @year, @geographyID", yearParam, geographyIDParam).ToList();

        var parcelWaterSupplyAndUsageDtos = waterTypeSupplies.Select(x => new WaterTypeSupplyDto()
        {
            WaterTypeID = x.WaterTypeID,
            WaterTypeName = x.WaterTypeName,
            TotalSupply = x.TotalSupply,
        });

        return parcelWaterSupplyAndUsageDtos;
    }

    
}