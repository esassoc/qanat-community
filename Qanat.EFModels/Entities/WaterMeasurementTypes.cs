using Qanat.Models.DataTransferObjects;
using Microsoft.EntityFrameworkCore;

namespace Qanat.EFModels.Entities;

public class WaterMeasurementTypes
{
    public static readonly List<string> OpenETWaterMeasurementTypeNames =
    [
        "OpenET Evapotranspiration",
        "OpenET Precipitation",
        "OpenET Consumptive Use"
    ];

    public static List<WaterMeasurementTypeSimpleDto> ListAsSimpleDto(QanatDbContext dbContext, int geographyID)
    {
        return dbContext.WaterMeasurementTypes.AsNoTracking()
            .Include(x => x.Geography)
            .Where(x => x.GeographyID == geographyID)
            .OrderBy(x => x.SortOrder)
            .Select(x => x.AsSimpleDtoWithExtras()).ToList();
    }

    public static List<WaterMeasurementTypeSimpleDto> ListActiveAndEditableAsSimpleDto(QanatDbContext dbContext, int geographyID)
    {
        return dbContext.WaterMeasurementTypes.AsNoTracking()
            .Include(x => x.Geography)
            .Where(x => x.GeographyID == geographyID && x.IsActive && x.IsUserEditable)
            .OrderBy(x => x.SortOrder)
            .Select(x => x.AsSimpleDtoWithExtras()).ToList();
    }

    public static List<WaterMeasurementTypeSimpleDto> ListActiveAndSelfReportableAsSimpleDto(QanatDbContext dbContext, int geographyID)
    {
        return dbContext.WaterMeasurementTypes.AsNoTracking()
            .Include(x => x.Geography)
            .Where(x => x.GeographyID == geographyID && x.IsActive && x.IsSelfReportable)
            .OrderBy(x => x.SortOrder)
            .Select(x => x.AsSimpleDtoWithExtras()).ToList();
    }

    public static void UpdateIsActiveByGeographyIDAndMeasurementTypeNames(QanatDbContext dbContext, int geographyID, List<string> measurementNames, bool isActiveValue)
    {
        dbContext.WaterMeasurementTypes
            .Where(x => x.GeographyID == geographyID && measurementNames.Contains(x.WaterMeasurementTypeName)).ToList()
            .ForEach(x => x.IsActive = isActiveValue);

        dbContext.SaveChanges();
    }

    public static async Task<List<WaterMeasurementType>> GetDependencyChainForWaterMeasurementType(QanatDbContext dbContext, int geographyID, int waterMeasurementTypeID, List<WaterMeasurementType> dependencyChain = null)
    {
        dependencyChain ??= new List<WaterMeasurementType>();

        var waterMeasurementType = await dbContext.WaterMeasurementTypes
            .Include(x => x.WaterMeasurementTypeDependencyWaterMeasurementTypes).ThenInclude(x => x.DependsOnWaterMeasurementType) //Need dependencies to run calculations.
            .Include(x => x.WaterMeasurementTypeDependencyDependsOnWaterMeasurementTypes).ThenInclude(x => x.WaterMeasurementType) //Need the inverse relationship to crawl the dependency tree, wish the generated names were a bit more friendly.
            .AsNoTracking()
            .SingleAsync(x => x.GeographyID == geographyID && x.WaterMeasurementTypeID == waterMeasurementTypeID && x.IsActive);

        var dependantWaterMeasurementTypeIDs = waterMeasurementType.WaterMeasurementTypeDependencyDependsOnWaterMeasurementTypes
            .Where(x => x.WaterMeasurementType.IsActive && x.WaterMeasurementType.WaterMeasurementCalculationTypeID.HasValue)
            .Select(x => x.WaterMeasurementTypeID).ToList();

        foreach (var dependantWaterMeasurementTypeID in dependantWaterMeasurementTypeIDs)
        {
            var dependantType = await dbContext.WaterMeasurementTypes.AsNoTracking()
                .Include(x => x.WaterMeasurementTypeDependencyWaterMeasurementTypes).ThenInclude(x => x.DependsOnWaterMeasurementType)
                .SingleAsync(x => x.GeographyID == geographyID && x.WaterMeasurementTypeID == dependantWaterMeasurementTypeID && x.IsActive);

            if (dependencyChain.Select(x => x.WaterMeasurementTypeID).Contains(dependantType.WaterMeasurementTypeID))
            {
                continue; //Already added the type.
            }

            dependencyChain.Add(dependantType);

            var additionalDependants = await GetDependencyChainForWaterMeasurementType(dbContext, geographyID, dependantType.WaterMeasurementTypeID, dependencyChain);
            foreach (var additionalDependant in additionalDependants)
            {
                if (!dependencyChain.Select(x => x.WaterMeasurementTypeID).Contains(additionalDependant.WaterMeasurementTypeID))
                {
                    dependencyChain.Add(additionalDependant);
                }
            }
        }

        return dependencyChain;
    }

    public static async Task<WaterMeasurementTypeSimpleDto> GetAsync(QanatDbContext dbContext, int geographyID, int waterMeasurementTypeID)
    {
        var waterMeasurementType = await dbContext.WaterMeasurementTypes.AsNoTracking()
            .Include(x => x.Geography)
            .SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.WaterMeasurementTypeID == waterMeasurementTypeID);

        var waterMeasurementTypeAsSimpleDto = waterMeasurementType?.AsSimpleDtoWithExtras();
        return waterMeasurementTypeAsSimpleDto;
    }
}