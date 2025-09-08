using Microsoft.EntityFrameworkCore;
using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities;

public static class WaterMeasurementSelfReportFileResources
{
    public static async Task<WaterMeasurementSelfReportFileResourceDto> CreateAsync(QanatDbContext dbContext, int selfReportID, int fileResourceFileResourceID, WaterMeasurementSelfReportFileResourceCreateDto selfReportFileResourceUpsertDto)
    {
        var selfReportFileResource = new WaterMeasurementSelfReportFileResource()
        {
            WaterMeasurementSelfReportID = selfReportID,
            FileResourceID = fileResourceFileResourceID,
            FileDescription = selfReportFileResourceUpsertDto.FileDescription
        };

        dbContext.WaterMeasurementSelfReportFileResources.Add(selfReportFileResource);
        await dbContext.SaveChangesAsync();

        var newSelfReportFileResourceDto = await GetAsync(dbContext, selfReportFileResource.WaterMeasurementSelfReportFileResourceID);
        return newSelfReportFileResourceDto;
    }

    public static async Task<List<WaterMeasurementSelfReportFileResourceDto>> ListAsync(QanatDbContext dbContext, int selfReportID)
    {
        var selfReportFileResources = await dbContext.WaterMeasurementSelfReportFileResources.AsNoTracking()
            .Include(x => x.FileResource)
            .Where(x => x.WaterMeasurementSelfReportID == selfReportID)
            .OrderBy(x => x.FileResource.OriginalBaseFilename)
            .ToListAsync();

        var selfReportFileResourceDtos = selfReportFileResources.Select(x => x.AsDto()).ToList();
        return selfReportFileResourceDtos;
    }

    public static async Task<WaterMeasurementSelfReportFileResourceDto> GetAsync(QanatDbContext dbContext, int selfReportFileResourceID)
    {
        var selfReportFileResource = await dbContext.WaterMeasurementSelfReportFileResources.AsNoTracking()
            .Include(x => x.FileResource)
            .Where(x => x.WaterMeasurementSelfReportFileResourceID == selfReportFileResourceID)
            .FirstOrDefaultAsync();

        var selfReportFileResourceDto = selfReportFileResource.AsDto();
        return selfReportFileResourceDto;
    }

    public static async Task<WaterMeasurementSelfReportFileResourceDto> UpdateAsync(QanatDbContext dbContext, int selfReportFileResourceID, WaterMeasurementSelfReportFileResourceUpdateDto selfReportFileResourceUpsertDto)
    {
        var selfReportFileResource = await dbContext.WaterMeasurementSelfReportFileResources
            .Where(x => x.WaterMeasurementSelfReportFileResourceID == selfReportFileResourceID)
            .FirstOrDefaultAsync();

        selfReportFileResource.FileDescription = selfReportFileResourceUpsertDto.FileDescription;
        await dbContext.SaveChangesAsync();

        var updatedSelfReportFileResourceDto = await GetAsync(dbContext, selfReportFileResource.WaterMeasurementSelfReportFileResourceID);
        return updatedSelfReportFileResourceDto;
    }

    public static async Task DeleteAsync(QanatDbContext dbContext, int selfReportFileResourceID)
    {
        var selfReportFileResource = await dbContext.WaterMeasurementSelfReportFileResources
            .Where(x => x.WaterMeasurementSelfReportFileResourceID == selfReportFileResourceID)
            .FirstOrDefaultAsync();

        dbContext.WaterMeasurementSelfReportFileResources.Remove(selfReportFileResource);
        await dbContext.SaveChangesAsync();
    }
}
