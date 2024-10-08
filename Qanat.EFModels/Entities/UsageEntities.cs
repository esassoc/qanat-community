using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using Qanat.Common.GeoSpatial;
using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities;

public class UsageEntities
{
    
    public static async Task<string> ProcessETSGSAUsageEntityGDBUpload(QanatDbContext dbContext,
        List<ETSGSAUsageEntityGdbFeature> etsgsaUsageEntityGdbFeatures, string sourceWkt,
        string geographyCoordinateSystemWkt)
    {
        var geography = dbContext.Geographies.Single(x => x.GeographyID == 7);
        var parcels = dbContext.Parcels.Where(x => x.GeographyID == 7).ToList();
        var currentUsageEntities = dbContext.UsageEntities.Where(x => x.GeographyID == 7)
            .ToList();
        dbContext.UsageEntities.RemoveRange(currentUsageEntities);

        var failedApns = new List<string>();

        var usageEntities = new List<UsageEntity>();
        foreach (var etsgsaUsageEntityGdbFeature in etsgsaUsageEntityGdbFeatures)
        {

            var apn = Regex.Replace(etsgsaUsageEntityGdbFeature.apn, @"^(...)(...)(...)$", "$1-$2-$3");
            var parcelID = parcels.SingleOrDefault(x => x.ParcelNumber == apn)?.ParcelID;
            if (parcelID == null)
            {
                failedApns.Add($"Could not find the following APN provided in the File GDB Upload: {apn}");
                continue;
            }

            var geometry4326 = etsgsaUsageEntityGdbFeature.Geometry.ProjectTo4326(sourceWkt);
            var geometryNative = geometry4326.ProjectToSrid(geography.CoordinateSystem, geographyCoordinateSystemWkt);
            var usageEntity = new UsageEntity
            {
                UsageEntityGeometry = new UsageEntityGeometry()
                {
                    Geometry4326 = geometry4326,
                    GeometryNative = geometryNative
                },
                UsageEntityCrops = new List<UsageEntityCrop>(){},
                UsageEntityName = $"{geography.GeographyName}-{etsgsaUsageEntityGdbFeature.fieldid}-{etsgsaUsageEntityGdbFeature.uniqueid}-{DateTime.UtcNow.Year}",
                GeographyID = 7,
                ParcelID = (int)parcelID,
                UsageEntityArea = etsgsaUsageEntityGdbFeature.acres
            };


            if (etsgsaUsageEntityGdbFeature.crop1_landiq != null && etsgsaUsageEntityGdbFeature.crop1_landiq != "****")
            {
                usageEntity.UsageEntityCrops.Add(new UsageEntityCrop
                {
                    UsageEntityCropName = etsgsaUsageEntityGdbFeature.crop1_landiq
                });
            }
            
            if (etsgsaUsageEntityGdbFeature.crop2_landiq != null && etsgsaUsageEntityGdbFeature.crop2_landiq != "****")
            {
                usageEntity.UsageEntityCrops.Add(new UsageEntityCrop
                {
                    UsageEntityCropName = etsgsaUsageEntityGdbFeature.crop2_landiq
                });
            }
            
            if (etsgsaUsageEntityGdbFeature.crop3_landiq != null && etsgsaUsageEntityGdbFeature.crop3_landiq != "****")
            {
                usageEntity.UsageEntityCrops.Add(new UsageEntityCrop
                {
                    UsageEntityCropName = etsgsaUsageEntityGdbFeature.crop3_landiq
                });
            }
            
            if (etsgsaUsageEntityGdbFeature.crop4_landiq != null && etsgsaUsageEntityGdbFeature.crop4_landiq != "****")
            {
                usageEntity.UsageEntityCrops.Add(new UsageEntityCrop
                {
                    UsageEntityCropName = etsgsaUsageEntityGdbFeature.crop4_landiq
                });
            }



            usageEntities.Add(usageEntity);
        }

        dbContext.UsageEntities.AddRange(usageEntities);

        await dbContext.SaveChangesAsync();
        await dbContext.Database.ExecuteSqlRawAsync("EXECUTE dbo.pMakeValidUsageEntityGeometries");
        return string.Join(", ", failedApns);

    }

    public static UsageEntityHierarchyDto GetHierarchyDtoByID(QanatDbContext dbContext, int usageEntityID)
    {
        UsageEntity hierarchyDtoByID = dbContext.UsageEntities.Include(x => x.Parcel).ThenInclude(x => x.WaterAccount)
            .SingleOrDefault(x => x.UsageEntityID == usageEntityID);
        return new UsageEntityHierarchyDto
        {
            UsageEntityID = hierarchyDtoByID.UsageEntityID,
            GeographyID = hierarchyDtoByID.GeographyID,
            WaterAccountID = hierarchyDtoByID.Parcel.WaterAccount.WaterAccountID,
            ParcelID = hierarchyDtoByID.ParcelID
        };
    }

    public static List<UsageEntityListItemDto> GetListByParcelID(QanatDbContext dbContext, int parcelID)
    {
        var parcel = dbContext.Parcels.AsNoTracking()
            .Include(x => x.WaterAccount)
            .Include(x =>x.UsageEntities).ThenInclude(x => x.UsageEntityCrops)
            .Single(x => x.ParcelID == parcelID);

        var usageEntities = parcel.UsageEntities.Select(usageEntity => new UsageEntityListItemDto
        {
            UsageEntityID = usageEntity.UsageEntityID,
            GeographyID = usageEntity.GeographyID,
            WaterAccountID = usageEntity.Parcel.WaterAccount.WaterAccountID,
            ParcelID = usageEntity.ParcelID,
            UsageEntityName = usageEntity.UsageEntityName,
            CropNames = usageEntity.UsageEntityCrops.Select(x => x.UsageEntityCropName).ToList(),
            WaterAccountName = usageEntity.Parcel.WaterAccount.WaterAccountNameAndNumber(),
            ParcelNumber = usageEntity.Parcel.ParcelNumber,
            Area = usageEntity.UsageEntityArea
        });
        
        return usageEntities.ToList();
    }

    public static List<UsageEntitySimpleDto> ListByWaterAccountID(QanatDbContext dbContext, int waterAccountID)
    {
        return  dbContext.UsageEntities.AsNoTracking()
            .Include(x => x.Parcel)
            .Where(x => x.Parcel.WaterAccountID == waterAccountID).Select(x => x.AsSimpleDto()).ToList();
    }

    public static UsageEntityPopupDto GetPopupDtoByID(QanatDbContext dbContext, int usageEntityID)
    {
        var usageEntity = dbContext.UsageEntities
            .Include(x => x.Parcel).ThenInclude(x => x.WaterAccount)
            .Include(x => x.UsageEntityCrops)
            .Single(x => x.UsageEntityID == usageEntityID);
        return new UsageEntityPopupDto
        {
            UsageEntityID = usageEntity.UsageEntityID,
            GeographyID = usageEntity.GeographyID,
            WaterAccountID = usageEntity.Parcel.WaterAccount.WaterAccountID,
            ParcelID = usageEntity.ParcelID,
            UsageEntityName = usageEntity.UsageEntityName,
            CropNames = usageEntity.UsageEntityCrops.Select(x => x.UsageEntityCropName).ToList(),
            WaterAccountName = usageEntity.Parcel.WaterAccount.WaterAccountNameAndNumber(),
            ParcelNumber = usageEntity.Parcel.ParcelNumber,
            Area = usageEntity.UsageEntityArea
        };
    }
}