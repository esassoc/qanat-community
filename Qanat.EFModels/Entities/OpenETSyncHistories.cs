using Microsoft.EntityFrameworkCore;

namespace Qanat.EFModels.Entities;

public static class OpenETSyncHistories
{
    public static OpenETSyncHistory CreateNew(QanatDbContext dbContext, int year, int month, int openETDataTypeID, int geographyID)
    {
        var openETSync = dbContext.OpenETSyncs.SingleOrDefault(x =>
            x.GeographyID == geographyID && x.Year == year && x.Month == month && x.OpenETDataTypeID == openETDataTypeID) ?? new OpenETSync()
            {
                GeographyID = geographyID,
                OpenETDataTypeID = openETDataTypeID,
                Year = year,
                Month = month
            };

        var openETSyncHistoryToAdd = new OpenETSyncHistory()
        {
            OpenETSyncResultTypeID = (int)OpenETSyncResultTypeEnum.Created,
            OpenETSync = openETSync,
            CreateDate = DateTime.UtcNow,
            UpdateDate = DateTime.UtcNow,
        };

        dbContext.OpenETSyncHistories.Add(openETSyncHistoryToAdd);
        dbContext.SaveChanges();
        dbContext.Entry(openETSyncHistoryToAdd).Reload();

        return GetByOpenETSyncHistoryID(dbContext, openETSyncHistoryToAdd.OpenETSyncHistoryID);
    }

    public static OpenETSyncHistory GetByOpenETSyncHistoryID(QanatDbContext dbContext, int openETSyncHistoryID)
    {
        return dbContext.OpenETSyncHistories
            .Include(x => x.OpenETSync)
            .SingleOrDefault(x => x.OpenETSyncHistoryID == openETSyncHistoryID);
    }

    public static async Task<OpenETSyncHistory> UpdateOpenETSyncEntityByID(QanatDbContext qanatDbContext, int openETSyncHistoryID, OpenETSyncResultTypeEnum resultType)
    {
        return await UpdateOpenETSyncEntityByID(qanatDbContext, openETSyncHistoryID, resultType, null);
    }

    public static async Task<OpenETSyncHistory> UpdateOpenETSyncEntityByID(QanatDbContext qanatDbContext, int openETSyncHistoryID, OpenETSyncResultTypeEnum resultType, string errorMessage)
    {
        return await UpdateOpenETSyncEntityByID(qanatDbContext, openETSyncHistoryID, resultType, errorMessage, null, null);
    }

    public static async Task<OpenETSyncHistory> UpdateOpenETSyncEntityByID(QanatDbContext qanatDbContext, int openETSyncHistoryID, OpenETSyncResultTypeEnum resultType, string errorMessage, string googleDriveFileID, int? fileResourceID)
    {
        var openETSyncHistory = qanatDbContext.OpenETSyncHistories
            .Single(x => x.OpenETSyncHistoryID == openETSyncHistoryID);

        openETSyncHistory.UpdateDate = DateTime.UtcNow;
        openETSyncHistory.OpenETSyncResultTypeID = (int)resultType;

        switch (resultType)
        {
            case OpenETSyncResultTypeEnum.Failed:
                openETSyncHistory.ErrorMessage = errorMessage;

                break;
            case OpenETSyncResultTypeEnum.Succeeded:

                if (string.IsNullOrEmpty(googleDriveFileID))
                {
                    openETSyncHistory.OpenETSyncResultTypeID = (int)OpenETSyncResultTypeEnum.Failed;
                    openETSyncHistory.ErrorMessage = "Did not receive a Google Drive File ID.";
                    await qanatDbContext.SaveChangesAsync();
                    throw new ArgumentException("Google Drive File ID is required for Succeeded OpenETSyncHistory.");
                }

                if (!fileResourceID.HasValue)
                {
                    openETSyncHistory.OpenETSyncResultTypeID = (int)OpenETSyncResultTypeEnum.Failed;
                    openETSyncHistory.ErrorMessage = "Did not receive a File Resource ID.";
                    await qanatDbContext.SaveChangesAsync();
                    throw new ArgumentException("File Resource ID is required for Succeeded OpenETSyncHistory.");
                }

                openETSyncHistory.GoogleDriveRasterFileID = googleDriveFileID;
                openETSyncHistory.RasterFileResourceID = fileResourceID;

                break;
        }

        await qanatDbContext.SaveChangesAsync();
        await qanatDbContext.Entry(openETSyncHistory).ReloadAsync();

        return GetByOpenETSyncHistoryID(qanatDbContext, openETSyncHistory.OpenETSyncHistoryID);
    }

    public static async Task<OpenETSyncHistory> UpdateOpenETSyncRasterCalculationMetadata(QanatDbContext qanatDbContext, int openETSyncHistoryID, OpenETRasterCalculationResultTypeEnum calculationResultType, string errorMessage)
    {
        var openETSyncHistory = qanatDbContext.OpenETSyncHistories
            .Single(x => x.OpenETSyncHistoryID == openETSyncHistoryID);

        var updateDate = DateTime.UtcNow;
        openETSyncHistory.UpdateDate = updateDate;
        openETSyncHistory.OpenETRasterCalculationResultTypeID = (int)calculationResultType;
        openETSyncHistory.LastCalculationDate = updateDate;
        openETSyncHistory.LastCalculationErrorMessage = errorMessage;

        if (calculationResultType == OpenETRasterCalculationResultTypeEnum.Succeeded)
        {
            openETSyncHistory.LastSuccessfulCalculationDate = updateDate;
        }

        await qanatDbContext.SaveChangesAsync();
        await qanatDbContext.Entry(openETSyncHistory).ReloadAsync();

        return GetByOpenETSyncHistoryID(qanatDbContext, openETSyncHistory.OpenETSyncHistoryID);
    }
}