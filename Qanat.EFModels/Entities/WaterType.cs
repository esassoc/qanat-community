using Microsoft.EntityFrameworkCore;
using Qanat.Common.Util;
using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities;

public static class WaterTypes
{
    public static List<WaterTypeSimpleDto> ListAsSimpleDto(QanatDbContext dbContext, int geographyID)
    {
        return ListByGeographyID(dbContext, geographyID).Select(x => x.AsSimpleDto()).ToList();
    }

    public static List<WaterType> ListByGeographyID(QanatDbContext dbContext, int geographyID)
    {
        return dbContext.WaterTypes.AsNoTracking()
            .Where(x => x.GeographyID == geographyID)
            .OrderBy(x => x.SortOrder).ToList();
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

    public static void UpdateForGeography(QanatDbContext dbContext, int geographyID, List<WaterTypeSimpleDto> waterTypeDtos)
    {
        var updatedWaterTypes = waterTypeDtos.Select(x => new WaterType()
        {
            WaterTypeID = x.WaterTypeID,
            WaterTypeName = x.WaterTypeName,
            WaterTypeSlug = x.WaterTypeName.Replace(" ", "-")?.ToLower(),
            GeographyID = geographyID,
            IsAppliedProportionally = x.IsAppliedProportionally,
            IsSourcedFromApi = x.IsSourcedFromApi,
            WaterTypeDefinition = x.WaterTypeDefinition,
            SortOrder = x.SortOrder,
            IsActive = x.IsActive,
            WaterTypeColor = x.WaterTypeColor
        }).ToList();

        var allInDatabase = dbContext.WaterTypes;
        var existingWaterTypes = allInDatabase.Where(x => x.GeographyID == geographyID).ToList();

        existingWaterTypes.Merge(updatedWaterTypes, allInDatabase,
            (x, y) => dbContext.Entry(x).Property(e => e.WaterTypeID).CurrentValue
                      == dbContext.Entry(y).Property(e => e.WaterTypeID).CurrentValue,
            (x, y) =>
            {
                x.WaterTypeName = y.WaterTypeName;
                x.GeographyID = y.GeographyID;
                x.IsAppliedProportionally = y.IsAppliedProportionally;
                x.WaterTypeDefinition = y.WaterTypeDefinition;
                x.WaterTypeSlug = y.WaterTypeSlug;
                x.IsSourcedFromApi = y.IsSourcedFromApi;
                x.SortOrder = y.SortOrder;
                x.IsActive = y.IsActive;
                x.WaterTypeColor = y.WaterTypeColor;
            });

        dbContext.SaveChanges();
    }
}