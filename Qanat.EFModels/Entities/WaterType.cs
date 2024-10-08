using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities;

public static class WaterTypes
{
    public static List<WaterTypeSimpleDto> ListWaterTypesAsSimpleDto(QanatDbContext dbContext, int geographyID)
    {
        return ListByGeographyID(dbContext, geographyID).Select(x => x.AsSimpleDto()).ToList();
    }

    public static List<WaterType> ListByGeographyID(QanatDbContext dbContext, int geographyID)
    {
        return dbContext.WaterTypes.AsNoTracking()
            .Where(x => x.GeographyID == geographyID)
            .OrderBy(x => x.SortOrder).ToList();
    }

    public static List<WaterType> ListByGeographyIDList(QanatDbContext dbContext, IEnumerable<int> geographyIDList)
    {
        return dbContext.WaterTypes.AsNoTracking()
            .Where(x => geographyIDList.Contains(x.GeographyID)).OrderBy(x => x.GeographyID).ThenBy(x => x.SortOrder).ToList();
    }

    public static List<WaterType> List(QanatDbContext dbContext)
    {
        return dbContext.WaterTypes.AsNoTracking()
            .OrderBy(x => x.GeographyID).ThenBy(x => x.SortOrder).ToList();
    }

    public static string GetNameByID(QanatDbContext dbContext, int waterTypeID)
    {
        var waterType = dbContext.WaterTypes.SingleOrDefault(x => x.WaterTypeID == waterTypeID);
        return waterType?.WaterTypeName;
    }
}