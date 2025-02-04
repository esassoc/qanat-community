using System.Linq;
using Microsoft.EntityFrameworkCore;
using Qanat.Models.DataTransferObjects;

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

        var geographyReportingPeriods = dbContext.ReportingPeriods.AsNoTracking().Where(x => x.GeographyID == geographyID);
        var geographyStartDate = geographyReportingPeriods.Min(x => x.StartDate);
        var geographyStartYear = geographyStartDate.Year;
        return geographyStartYear;
    }

    public static List<Parcel> ListByWaterAccountIDAndYear(QanatDbContext dbContext, int waterAccountID, int year)
    {
        var parcels = dbContext.WaterAccountParcels
            .Include(x => x.Parcel)
            .AsNoTracking()
            .Where(x => x.WaterAccountID == waterAccountID
                        // EffectiveYear can equal EndYear, so that means:
                        // EffectiveYear 2024, EndYear 2024 = only showing up for 2024
                        // EffectiveYear 2024, EndYear 2025 = showing up for 2024 and 2025
                        && x.EffectiveYear <= year && (x.EndYear == null || x.EndYear >= year)
            )
            .Select(x => x.Parcel).Distinct().ToList();

        return parcels;
    }
}