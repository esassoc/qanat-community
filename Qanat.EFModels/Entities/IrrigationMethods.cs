using Microsoft.EntityFrameworkCore;
using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities;

public static class IrrigationMethods
{
    public static async Task<List<IrrigationMethodSimpleDto>> ListAsSimpleDtoAsync(QanatDbContext dbContext, int geographyID)
    {
        var irrigationMethods = await dbContext.IrrigationMethods.AsNoTracking()
            .Where(im => im.GeographyID == geographyID)
            .OrderBy(im => im.DisplayOrder)
            .ToListAsync();

        return irrigationMethods.Select(im => im.AsSimpleDto()).ToList();
    }
}
