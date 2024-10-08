using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Qanat.Common.Util;
using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities;

public static class CustomAttributes
{
    public static List<CustomAttribute> ListByGeographyIDAndTypeID(QanatDbContext dbContext, int geographyID, int customAttributeTypeID)
    {
        return dbContext.CustomAttributes.AsNoTracking()
            .Where(x => x.GeographyID == geographyID && x.CustomAttributeTypeID == customAttributeTypeID)
            .OrderBy(x => x.CustomAttributeName).ToList();
    }

    public static List<CustomAttributeSimpleDto> ListByGeographyIDAndTypeIDAsSimpleDto(QanatDbContext dbContext, int geographyID, int customAttributeTypeID)
    {
        return ListByGeographyIDAndTypeID(dbContext, geographyID, customAttributeTypeID)
            .Select(x => x.AsSimpleDto()).ToList();
    }

    public static void MergeByGeographyIDAndTypeID(QanatDbContext dbContext, int geographyID, int customAttributeTypeID, List<CustomAttributeSimpleDto> customAttributeSimpleDtos)
    {
        var newCustomAttributes = customAttributeSimpleDtos
            .Select(x => new CustomAttribute()
            {
                GeographyID = geographyID,
                CustomAttributeTypeID = customAttributeTypeID,
                CustomAttributeName = x.CustomAttributeName
            }).ToList();

        var allInDb = dbContext.CustomAttributes;
        var existingCustomAttributes = allInDb
            .Where(x => x.GeographyID == geographyID && x.CustomAttributeTypeID == customAttributeTypeID).ToList();

        existingCustomAttributes.Merge(newCustomAttributes, allInDb,
            (x, y) => x.GeographyID == y.GeographyID && x.CustomAttributeTypeID == y.CustomAttributeTypeID &&
                      x.CustomAttributeName == y.CustomAttributeName);

        dbContext.SaveChanges();
    }

    public static EntityCustomAttributesDto ListAllByWaterAccountIDAsEntityCustomAttributeDtos(QanatDbContext dbContext, int waterAccountID)
    {
        var waterAccount = dbContext.WaterAccounts.AsNoTracking().Single(x => x.WaterAccountID == waterAccountID);
        var waterAccountCustomAttributes = dbContext.WaterAccountCustomAttributes.AsNoTracking()
            .SingleOrDefault(x => x.WaterAccountID == waterAccountID);

        // grab full, ordered attribute list from CustomAttributes
        var customAttributes = ListByGeographyIDAndTypeID(dbContext, waterAccount.GeographyID, CustomAttributeType.WaterAccount.CustomAttributeTypeID);
        if (!customAttributes.Any())
        {
            return new EntityCustomAttributesDto();
        }
        var customAttributeDictionary = customAttributes.ToDictionary(x => x.CustomAttributeName, x => string.Empty);

        // populate attribute values from WaterAccountCustomAttributes
        if (waterAccountCustomAttributes != null)
        {
            var waterAccountCustomAttributeDictionary = JsonSerializer.Deserialize<Dictionary<string, string>>(waterAccountCustomAttributes.CustomAttributes);
            foreach (var key in waterAccountCustomAttributeDictionary.Keys.Where(x => customAttributeDictionary.ContainsKey(x)))
            {
                customAttributeDictionary[key] = waterAccountCustomAttributeDictionary[key];
            }
        }

        return new EntityCustomAttributesDto() { CustomAttributes = customAttributeDictionary };
    }

    public static EntityCustomAttributesDto ListAllByParcelIDAsEntityCustomAttributeDtos(QanatDbContext dbContext, int parcelID)
    {
        var parcel = dbContext.Parcels.AsNoTracking().Single(x => x.ParcelID == parcelID);
        var parcelCustomAttributes = dbContext.ParcelCustomAttributes.AsNoTracking()
            .SingleOrDefault(x => x.ParcelID == parcelID);

        // grab full, ordered attribute list from CustomAttributes
        var customAttributes = ListByGeographyIDAndTypeID(dbContext, parcel.GeographyID, CustomAttributeType.Parcel.CustomAttributeTypeID);
        if (!customAttributes.Any())
        {
            return new EntityCustomAttributesDto();
        }
        var customAttributeDictionary = customAttributes.ToDictionary(x => x.CustomAttributeName, x => string.Empty);

        // populate attribute values from WaterAccountCustomAttributes
        if (parcelCustomAttributes != null)
        {
            var waterAccountCustomAttributeDictionary = JsonSerializer.Deserialize<Dictionary<string, string>>(parcelCustomAttributes.CustomAttributes);
            foreach (var key in waterAccountCustomAttributeDictionary.Keys.Where(x => customAttributeDictionary.ContainsKey(x)))
            {
                customAttributeDictionary[key] = waterAccountCustomAttributeDictionary[key];
            }
        }

        return new EntityCustomAttributesDto() { CustomAttributes = customAttributeDictionary };
    }
}