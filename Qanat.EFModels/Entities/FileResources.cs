using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Qanat.EFModels.Entities;

public class FileResources
{
    public static FileResource GetByGuidString(QanatDbContext dbContext, string fileResourceGuidAsString)
    {
        var isValidGuid = Guid.TryParse(fileResourceGuidAsString, out var fileResourceGuid);
        if (isValidGuid)
        {
            return dbContext.FileResources.AsNoTracking().SingleOrDefault(x => x.FileResourceGUID == fileResourceGuid);
        }

        return null;
    }

    public static FileResource Create(QanatDbContext dbContext, IFormFile file, string canonicalName, int createUserID, DateTime createDate)
    {
        var clientFilename = file.FileName;
        var extension = clientFilename.Split('.').Last();
        var fileResourceGuid = Guid.NewGuid();

        var fileResource = new FileResource
        {
            CreateDate = createDate,
            CreateUserID = createUserID,
            FileResourceGUID = fileResourceGuid,
            OriginalBaseFilename = clientFilename,
            OriginalFileExtension = extension,
            FileResourceCanonicalName = canonicalName
        };

        dbContext.FileResources.Add(fileResource);
        dbContext.SaveChanges();

        return fileResource;
    }
}