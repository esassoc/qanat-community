using Microsoft.EntityFrameworkCore;
using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities;

public class OpenETSyncs
{
    public static List<OpenETSyncDto> ListByGeographyID(QanatDbContext dbContext, int geographyID)
    {
        return GetImpl(dbContext).AsNoTracking()
            .Where(x => x.GeographyID == geographyID)
            .OrderByDescending(x => x.Year).ThenByDescending(x => x.Month)
            .Select(x => x.AsOpenETSyncDto()).ToList();
    }

    public static OpenETSync GetByID(QanatDbContext dbContext, int openETSyncID)
    {
        return GetImpl(dbContext).SingleOrDefault(x => x.OpenETSyncID == openETSyncID);
    }

    public static OpenETSyncDto FinalizeSyncByID(QanatDbContext dbContext, int openETSyncID)
    {
        var openETSync = GetByID(dbContext, openETSyncID);

        openETSync.FinalizeDate = DateTime.UtcNow;
        dbContext.SaveChanges();

        var updatedSync = GetByID(dbContext, openETSyncID);
        var openETSyncDto = updatedSync.AsOpenETSyncDto();
        return openETSyncDto;
    }

    private static IQueryable<OpenETSync> GetImpl(QanatDbContext dbContext)
    {
        return dbContext.OpenETSyncs
            .Include(x => x.Geography)
            .Include(x => x.OpenETSyncHistories).ThenInclude(x => x.RasterFileResource);
    }
}