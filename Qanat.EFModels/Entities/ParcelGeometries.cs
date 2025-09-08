using Microsoft.EntityFrameworkCore;

namespace Qanat.EFModels.Entities;
public static class ParcelGeometries
{
    public static async Task<List<ParcelGeometry>> ListByGeographyID(QanatDbContext dbContext, int geographyID)
    {
        var parcelGeometries = await dbContext.ParcelGeometries.AsNoTracking()
            .Where(x => x.GeographyID == geographyID)
            .ToListAsync();

        return parcelGeometries;
    }
}
