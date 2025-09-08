using Microsoft.EntityFrameworkCore;
using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities;

public class WellFileResources
{
    public static async Task<WellFileResourceDto> CreateAsync(QanatDbContext dbContext, int wellID, int fileResourceID, WellFileResourceCreateDto wellFileCreateDto)
    {
        var wellFileResource = new WellFileResource
        {
            WellID = wellID,
            FileResourceID = fileResourceID,
            FileDescription = wellFileCreateDto.FileDescription
        };

        dbContext.Add(wellFileResource);
        await dbContext.SaveChangesAsync();

        var wellFileResourceDto = await GetByIDAsDtoAsync(dbContext, wellFileResource.WellFileResourceID);
        return wellFileResourceDto;
    }

    public static async Task<List<WellFileResourceDto>> ListByWellIDAsync(QanatDbContext dbContext, int wellID)
    {
        var wellFileResources = await dbContext.WellFileResources.AsNoTracking()
            .Include(x => x.FileResource)
            .Where(x => x.WellID == wellID)
            .OrderBy(x => x.FileResource.OriginalBaseFilename)
            .ToListAsync();

        var wellFileResourceDtos = wellFileResources
            .Select(x => x.AsWellFileResourceDto())
            .ToList();

        return wellFileResourceDtos;
    }

    public static async Task<WellFileResourceDto> GetByIDAsDtoAsync(QanatDbContext dbContext, int wellFileResourceID)
    {
        var wellFileResource = await dbContext.WellFileResources.AsNoTracking()
            .Include(x => x.FileResource)
            .SingleOrDefaultAsync(x => x.WellFileResourceID == wellFileResourceID);

        return wellFileResource?.AsWellFileResourceDto();
    }

    public static async Task<WellFileResourceDto> UpdateAsync(QanatDbContext dbContext, int wellFileResourceID, WellFileResourceUpdateDto wellFileUpdateDto)
    {
        var wellFileResource = await dbContext.WellFileResources.SingleAsync(x => x.WellFileResourceID == wellFileResourceID);

        wellFileResource.FileDescription = wellFileUpdateDto.FileDescription;

        await dbContext.SaveChangesAsync();

        var updatedWellFileResource = await GetByIDAsDtoAsync(dbContext, wellFileResource.WellFileResourceID);
        return updatedWellFileResource;
    }

    public static async Task DeleteAsync(QanatDbContext dbContext, int wellFileResourceID)
    {
        await dbContext.WellFileResources.Where(x => x.WellFileResourceID == wellFileResourceID).ExecuteDeleteAsync();
    }
}