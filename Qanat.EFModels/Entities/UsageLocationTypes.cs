using Microsoft.EntityFrameworkCore;
using Qanat.EFModels.Entities.ExtensionMethods;
using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities;

public static class UsageLocationTypes
{
    #region Create

    public static async Task<List<ErrorMessage>> ValidateCreateAsync(QanatDbContext dbContext, int geographyID, UsageLocationTypeUpsertDto usageLocationTypeUpsertDto)
    {
        var errors = new List<ErrorMessage>();

        var existingUsageLocationTypes = await ListAsync(dbContext, geographyID);

        // Name must be unique
        if (existingUsageLocationTypes.Any(x => x.Name == usageLocationTypeUpsertDto.Name))
        {
            errors.Add(new ErrorMessage
            {
                Type = "Usage Location Type Name",
                Message = $"The name '{usageLocationTypeUpsertDto.Name}' is already in use."
            });
        }

        // Sort order must not conflict
        if (existingUsageLocationTypes.Any(x => x.SortOrder == usageLocationTypeUpsertDto.SortOrder))
        {
            errors.Add(new ErrorMessage
            {
                Type = "Usage Location Type Sort Order",
                Message = $"The sort order '{usageLocationTypeUpsertDto.SortOrder}' is already in use."
            });
        }

        // If setting as default, ensure no other default exists
        if (usageLocationTypeUpsertDto.IsDefault && existingUsageLocationTypes.Any(x => x.IsDefault))
        {
            errors.Add(new ErrorMessage
            {
                Type = "Usage Location Type Default",
                Message = "Only one usage location type can be marked as default."
            });
        }

        return errors;
    }

    public static async Task<List<UsageLocationTypeDto>> CreateAsync(QanatDbContext dbContext, int geographyID, UsageLocationTypeUpsertDto upsertDto, UserDto callingUser)
    {
        var newUsageLocationType = new UsageLocationType()
        {
            GeographyID = geographyID,
            Name = upsertDto.Name,
            Definition = upsertDto.Definition,
            CanBeRemoteSensed = upsertDto.CanBeRemoteSensed,
            IsIncludedInUsageCalculation = upsertDto.IsIncludedInUsageCalculation,
            IsDefault = upsertDto.IsDefault,
            ColorHex = upsertDto.ColorHex,
            SortOrder = upsertDto.SortOrder,
            CreateDate = DateTime.UtcNow,
            CreateUserID = callingUser.UserID
        };

        dbContext.UsageLocationTypes.Add(newUsageLocationType);

        await dbContext.SaveChangesAsync();

        var updatedUsageLocationTypeDtos = await ListAsync(dbContext, geographyID);
        return updatedUsageLocationTypeDtos;
    }

    #endregion

    #region Read

    public static async Task<List<UsageLocationTypeDto>> ListAsync(QanatDbContext dbContext, int geographyID)
    {
        var usageLocationTypeDtos = await dbContext.UsageLocationTypes.AsNoTracking()
            .Include(x => x.Geography)
            .Include(x => x.CreateUser)
            .Include(x => x.UpdateUser)
            .Where(x => x.GeographyID == geographyID)
            .OrderBy(x => x.SortOrder)
            .Select(x => x.AsDto())
            .ToListAsync();

        return usageLocationTypeDtos;
    }

    #endregion

    #region Update

    public static async Task<List<ErrorMessage>> ValidateUpdateAsync(QanatDbContext dbContext, int geographyID, List<UsageLocationTypeUpsertDto> usageLocationTypeUpsertDtos)
    {
        var errors = new List<ErrorMessage>();

        var existingUsageLocationTypes = await ListAsync(dbContext, geographyID);

        var usageLocationTypeIDs = usageLocationTypeUpsertDtos.Where(x => x.UsageLocationTypeID.HasValue)
            .Select(x => x.UsageLocationTypeID.Value)
            .ToList();

        var missingInUpsertDtos = existingUsageLocationTypes.Where(x => !usageLocationTypeIDs.Contains(x.UsageLocationTypeID)).ToList();
        if (missingInUpsertDtos.Any())
        {
            errors.Add(new ErrorMessage
            {
                Type = "Missing Usage Location Types",
                Message = $"The following Usage Location Types are missing from the update: {string.Join(", ", missingInUpsertDtos.Select(x => x.Name))}. Please use the delete functionality to remove."
            });
        }

        var duplicateNames = usageLocationTypeUpsertDtos
            .GroupBy(x => x.Name)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

        if (duplicateNames.Any())
        {
            errors.Add(new ErrorMessage
            {
                Type = "Usage Location Type Names",
                Message = $"Provided duplicate names: {string.Join(", ", duplicateNames)}"
            });
        }

        var duplicateSortOrders = usageLocationTypeUpsertDtos
            .GroupBy(x => x.SortOrder)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

        if (duplicateSortOrders.Any())
        {
            errors.Add(new ErrorMessage
            {
                Type = "Usage Location Type Sort Orders",
                Message = $"Provided duplicate sort orders: {string.Join(", ", duplicateSortOrders)}"
            });
        }

        //Ensure sort orders are sequential starting from 1
        var expected = Enumerable.Range(1, usageLocationTypeUpsertDtos.Count);
        var actual = usageLocationTypeUpsertDtos.Select(x => x.SortOrder).OrderBy(x => x);

        if (!expected.SequenceEqual(actual))
        {
            errors.Add(new ErrorMessage
            {
                Type = "Usage Location Type Sort Orders",
                Message = "Sort orders must be sequential starting from 1. Please adjust the sort orders accordingly."
            });
        }

        var markedDefaultInUpsertDtos = usageLocationTypeUpsertDtos.Where(x => x.IsDefault).ToList();
        if (markedDefaultInUpsertDtos.Count > 1)
        {
            errors.Add(new ErrorMessage
            {
                Type = "Usage Location Type Default",
                Message = "Only one usage location type can be marked as default."
            });
        }

        var existingUsageLocationTypeIDs = existingUsageLocationTypes.Select(x => x.UsageLocationTypeID).ToList();
        var invalidUsageLocationTypes = usageLocationTypeUpsertDtos
            .Where(x => x.UsageLocationTypeID.HasValue && !existingUsageLocationTypeIDs.Contains(x.UsageLocationTypeID.Value))
            .ToList();

        if (invalidUsageLocationTypes.Any())
        {
            errors.Add(new ErrorMessage
            {
                Type = "Unknown Usage Location Types",
                Message = $"The following Usage Location Types could not be found to update: {string.Join(", ", invalidUsageLocationTypes.Select(x => x.Name))}"
            });
        }

        return errors;
    }

    public static async Task<List<UsageLocationTypeDto>> UpdateAsync(QanatDbContext dbContext, int geographyID, List<UsageLocationTypeUpsertDto> usageLocationTypeUpsertDtos, UserDto callingUser)
    {
        var existingUsageLocations = await dbContext.UsageLocationTypes
            .Where(x => x.GeographyID == geographyID)
            .ToListAsync();

        foreach (var upsertDto in usageLocationTypeUpsertDtos)
        {
            var existingUsageLocationType = existingUsageLocations
                .FirstOrDefault(x => x.UsageLocationTypeID == upsertDto.UsageLocationTypeID);

            if (existingUsageLocationType != null)
            {
                existingUsageLocationType.Name = upsertDto.Name;
                existingUsageLocationType.Definition = upsertDto.Definition;
                existingUsageLocationType.CanBeRemoteSensed = upsertDto.CanBeRemoteSensed;
                existingUsageLocationType.IsIncludedInUsageCalculation = upsertDto.IsIncludedInUsageCalculation;
                existingUsageLocationType.IsDefault = upsertDto.IsDefault;
                existingUsageLocationType.ColorHex = upsertDto.ColorHex;
                existingUsageLocationType.SortOrder = upsertDto.SortOrder;
                existingUsageLocationType.UpdateDate = DateTime.UtcNow;
                existingUsageLocationType.UpdateUserID = callingUser.UserID;

                dbContext.UsageLocationTypes.Update(existingUsageLocationType);
            }
        }

        await dbContext.SaveChangesAsync();

        var updatedUsageLocationTypeDtos = await ListAsync(dbContext, geographyID);
        return updatedUsageLocationTypeDtos;
    }

    #endregion

    #region Update Cover Crop Metadata

    public static async Task<UsageLocationTypeSimpleDto> UpdateCoverCropMetadataAsync(QanatDbContext dbContext, int geographyID, int usageLocationTypeID, UsageLocationTypeUpdateCoverCropMetadataDto updateCoverCropMetadataDto, UserDto callingUser)
    {
        var usageLocationType = await dbContext.UsageLocationTypes
            .Where(x => x.GeographyID == geographyID && x.UsageLocationTypeID == usageLocationTypeID)
            .SingleAsync();

        usageLocationType.CanBeSelectedInCoverCropForm = updateCoverCropMetadataDto.CanBeSelectedInCoverCropForm;
        usageLocationType.CountsAsCoverCropped = updateCoverCropMetadataDto.CountsAsCoverCropped;
        usageLocationType.UpdateDate = DateTime.UtcNow;
        usageLocationType.UpdateUserID = callingUser.UserID;

        dbContext.UsageLocationTypes.Update(usageLocationType);

        await dbContext.SaveChangesAsync();
        await dbContext.Entry(usageLocationType).ReloadAsync();

        return usageLocationType.AsSimpleDto();
    }

    #endregion

    #region Update Fallow Metadata

    public static async Task<UsageLocationTypeSimpleDto> UpdateFallowMetadataAsync(QanatDbContext dbContext, int geographyID, int usageLocationTypeID, UsageLocationTypeUpdateFallowMetadataDto updateFallowMetadataDto, UserDto callingUser)
    {
        var usageLocationType = await dbContext.UsageLocationTypes
            .Where(x => x.GeographyID == geographyID && x.UsageLocationTypeID == usageLocationTypeID)
            .SingleAsync();

        usageLocationType.CanBeSelectedInFallowForm = updateFallowMetadataDto.CanBeSelectedInFallowForm;
        usageLocationType.CountsAsFallowed = updateFallowMetadataDto.CountsAsFallowed;
        usageLocationType.UpdateDate = DateTime.UtcNow;
        usageLocationType.UpdateUserID = callingUser.UserID;

        dbContext.UsageLocationTypes.Update(usageLocationType);

        await dbContext.SaveChangesAsync();
        await dbContext.Entry(usageLocationType).ReloadAsync();

        return usageLocationType.AsSimpleDto();
    }

    #endregion

    #region Delete

    public static async Task<List<ErrorMessage>> ValidateDeleteAsync(QanatDbContext dbContext, int geographyID, int usageLocationTypeID)
    {
        var errors = new List<ErrorMessage>();

        var usageLocationType = await dbContext.UsageLocationTypes.AsNoTracking()
            .SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.UsageLocationTypeID == usageLocationTypeID);

        if (usageLocationType.IsDefault)
        {
            errors.Add(new ErrorMessage
            {
                Type = "Usage Location Type Default",
                Message = "Cannot delete a Usage Location Type that is marked as default. Please mark another Usage Location Type as default before deleting."
            });
        }

        var usageLocations = await dbContext.UsageLocations.AsNoTracking()
            .Where(x => x.GeographyID == geographyID && x.UsageLocationTypeID == usageLocationTypeID)
            .ToListAsync();

        if (usageLocations.Any())
        {
            errors.Add(new ErrorMessage
            {
                Type = "Usage Locations",
                Message = "Cannot delete a Usage Location Type that is associated with existing Usage Locations. Please assign a different type to the Usage Locations before deleting."
            });
        }

        return errors;
    }

    public static async Task DeleteAsync(QanatDbContext dbContext, int geographyID, int usageLocationTypeID)
    {
        var usageLocationTypeToDelete = dbContext.UsageLocationTypes.Where(x => x.GeographyID == geographyID && x.UsageLocationTypeID == usageLocationTypeID);
        await usageLocationTypeToDelete.ExecuteDeleteAsync();

        //Reindex sort orders if needed.
        var usageLocationTypes = await dbContext.UsageLocationTypes
            .Where(x => x.GeographyID == geographyID)
            .OrderBy(x => x.SortOrder)
            .ToListAsync();

        for (var i = 0; i < usageLocationTypes.Count; i++)
        {
            usageLocationTypes[i].SortOrder = i + 1;
            dbContext.UsageLocationTypes.Update(usageLocationTypes[i]);
        }

        await dbContext.SaveChangesAsync();
    }

    #endregion
}
