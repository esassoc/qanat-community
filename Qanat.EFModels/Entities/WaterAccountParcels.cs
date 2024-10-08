using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Qanat.EFModels.Entities;

public static class WaterAccountParcels
{
    public static int GetNextAvailableEffectiveYearForGeography(QanatDbContext dbContext, int geographyID)
    {
        var waterAccountParcels = dbContext.ParcelHistories.AsNoTracking()
            .Where(x => x.GeographyID == geographyID);
        if (waterAccountParcels.Any())
        {
            return waterAccountParcels.Max(x => x.EffectiveYear);
        }

        var geographyStartYear = Geographies.GetByID(dbContext, geographyID).StartYear;
        return geographyStartYear;
    }
}