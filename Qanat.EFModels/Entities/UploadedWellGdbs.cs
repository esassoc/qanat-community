using Microsoft.EntityFrameworkCore;
using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities;

public static class UploadedWellGdbs
{
    private const string CanonicalNamePrefix = "UploadedWellGdb_";

    public static UploadedWellGdbDto GetByID(QanatDbContext dbContext, int uploadedGdbID)
    {
        var dto = dbContext.UploadedWellGdbs.Include(x => x.User)
            .Include(x => x.Geography)
            .Single(x => x.UploadedWellGdbID == uploadedGdbID).AsUploadedWellGdbDto();
        return dto;
    }

    public static async Task<UploadedWellGdb> CreateNew(QanatDbContext dbContext, int userID, int geographyID)
    {
        var uploadedWellGdb = new UploadedWellGdb();
        var uploadDate = DateTime.UtcNow;
        uploadedWellGdb.UserID = userID;
        uploadedWellGdb.GeographyID = geographyID;
        uploadedWellGdb.UploadDate = uploadDate;
        uploadedWellGdb.Finalized = false;
        var canonicalName = $"{CanonicalNamePrefix}{uploadDate:yyyyMMddhhmmss}.gdb";
        uploadedWellGdb.CanonicalName = canonicalName;
        dbContext.UploadedWellGdbs.Add(uploadedWellGdb);
        await dbContext.SaveChangesAsync();
        await dbContext.Entry(uploadedWellGdb).ReloadAsync();

        return uploadedWellGdb;
    }
}