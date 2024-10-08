using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Qanat.Models.DataTransferObjects;
using System.Threading.Tasks;
using NetTopologySuite.Features;

namespace Qanat.EFModels.Entities;

public static class UploadedGdbs
{
    private const string CanonicalNamePrefix = "UploadedParcelGdb_";

    public static async Task<UploadedGdb> CreateNew(QanatDbContext dbContext, int userID, int geographyID)
    {
        var uploadedGdb = new UploadedGdb();
        var uploadDate = DateTime.UtcNow;
        uploadedGdb.UserID = userID;
        uploadedGdb.GeographyID = geographyID;
        uploadedGdb.UploadDate = uploadDate;
        uploadedGdb.Finalized = false;
        var canonicalName = $"{CanonicalNamePrefix}{uploadDate:yyyyMMddhhmmss}.gdb";
        uploadedGdb.CanonicalName = canonicalName;
        dbContext.UploadedGdbs.Add(uploadedGdb);
        await dbContext.SaveChangesAsync();
        await dbContext.Entry(uploadedGdb).ReloadAsync();

        return uploadedGdb;
    }


    public static List<ErrorMessage> ValidateUploadedGDB(QanatDbContext dbContext, int userID)
    {
        var results = new List<ErrorMessage>();
        var uploadedGdbDto = dbContext.UploadedGdbs.AsNoTracking().OrderByDescending(x => x.UploadDate)
            .FirstOrDefault(x => x.UserID == userID);
        if (uploadedGdbDto == null)
        {
            results.Add(new ErrorMessage()  { Type = "Missing file", Message = "You have not uploaded a file for review. Please upload one."});
        }

        return results;
    }

    public static List<ErrorMessage> ValidateEffectiveYear(QanatDbContext dbContext, int effectiveYear, int geographyID)
    {
        var results = new List<ErrorMessage>();
        var lastEffectiveYear = WaterAccountParcels.GetNextAvailableEffectiveYearForGeography(dbContext, geographyID);
        if (lastEffectiveYear > effectiveYear)
        {
            results.Add(new ErrorMessage() { Type = "Effective Year", Message = $"Your Effective Year must be greater than equal to {lastEffectiveYear}" });
        }

        return results;
    }

    public static UploadedGdbDto GetByID(QanatDbContext dbContext, int uploadedGdbID)
    {
        var dto = dbContext.UploadedGdbs.Include(x => x.User).Include(x => x.Geography).Single(x => x.UploadedGdbID == uploadedGdbID).AsDto();
        return dto;
    }
}