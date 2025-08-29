
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities;

public static class FileResources
{
    public static List<ErrorMessage> ValidateFileUpload(IFormFile inputFile)
    {
        var errors = new List<ErrorMessage>();
        var acceptedExtensions = new List<string> { ".pdf", ".png", ".jpg", ".docx", ".doc", ".xlsx", ".txt", ".csv" };
        var extension = Path.GetExtension(inputFile.FileName);

        if (string.IsNullOrEmpty(extension) || !acceptedExtensions.Contains(extension.ToLower()))
        {
            errors.Add(new ErrorMessage
            {
                Type = "File Resource",
                Message = $"{extension[1..].ToUpper()} is not an accepted file extension"
            });
        }

        const double maxFileSize = 200d * 1024d * 1024d;
        if (inputFile.Length > maxFileSize)
        {
            errors.Add(new ErrorMessage
            {
                Type = "File Resource",
                Message = "File size cannot exceed 200MB."
            });
        }

        return errors;
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
    public static FileResource GetByGuidString(QanatDbContext dbContext, string fileResourceGuidAsString)
    {
        var isValidGuid = Guid.TryParse(fileResourceGuidAsString, out var fileResourceGuid);
        if (isValidGuid)
        {
            return dbContext.FileResources.AsNoTracking().SingleOrDefault(x => x.FileResourceGUID == fileResourceGuid);
        }

        return null;
    }
}