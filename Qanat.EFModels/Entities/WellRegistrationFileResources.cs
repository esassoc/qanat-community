using Microsoft.EntityFrameworkCore;
using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities;

public class WellRegistrationFileResources
{
    private static IQueryable<WellRegistrationFileResource> GetImpl(QanatDbContext dbContext)
    {
        return dbContext.WellRegistrationFileResources.AsNoTracking()
            .Include(x => x.FileResource);
    }

    public static WellRegistrationFileResourceDto GetByIDAsDto(QanatDbContext dbContext, int wellRegistrationFileResourceID)
    {
        var wellRegistrationFileResource = GetImpl(dbContext)
            .SingleOrDefault(x => x.WellRegistrationFileResourceID == wellRegistrationFileResourceID);
        return wellRegistrationFileResource?.AsWellRegistrationFileResourceDto();
    }

    public static List<WellRegistrationFileResourceDto> ListByWellRegistrationID(QanatDbContext dbContext, int wellRegistrationID)
    {
        return GetImpl(dbContext).Where(x => x.WellRegistrationID == wellRegistrationID)
            .OrderBy(x => x.FileResource.OriginalBaseFilename)
            .Select(x => x.AsWellRegistrationFileResourceDto()).ToList();
    }

    public static string CreateFileResourceCanonicalName(int wellRegistrationID, string prettyFileName, DateTime createDate)
    {
        return $"wells/{wellRegistrationID}/{prettyFileName}_{createDate:yyyyMMddHHmmss}";
    }

    public static WellRegistrationFileResourceDto Create(QanatDbContext dbContext, int fileResourceID, WellRegistrationFileResourceUpsertDto wellRegistrationFileResourceUpsertDto)
    {
        var wellRegistrationFileResource = new WellRegistrationFileResource
        {
            WellRegistrationID = wellRegistrationFileResourceUpsertDto.WellRegistrationID,
            FileResourceID = fileResourceID,
            FileDescription = wellRegistrationFileResourceUpsertDto.FileDescription
        };

        dbContext.Add(wellRegistrationFileResource);
        dbContext.SaveChanges();
        
        return GetByIDAsDto(dbContext, wellRegistrationFileResource.WellRegistrationFileResourceID);
    }

    public static void Delete(QanatDbContext dbContext, WellRegistrationFileResource wellRegistrationFileResource)
    {
        dbContext.WellRegistrationFileResources.Remove(wellRegistrationFileResource);
        dbContext.SaveChanges();
    }
}