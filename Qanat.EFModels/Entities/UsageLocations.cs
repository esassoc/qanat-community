using Microsoft.EntityFrameworkCore;
using Qanat.Common.GeoSpatial;
using Qanat.Common.Util;
using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities;

public static class UsageLocations
{
    public static async Task<List<UsageLocationDto>> ListByParcelAsync(QanatDbContext dbContext, int geographyID, int parcelID, bool currentUserIsAdminOrWaterManager)
    {
        var usageLocations = await dbContext.UsageLocations.AsNoTracking()
            .Include(x => x.Geography)
            .Include(x => x.ReportingPeriod)
            .Include(x => x.UsageLocationType)
            .Include(x => x.Parcel.WaterAccountParcels).ThenInclude(x => x.WaterAccount).ThenInclude(x => x.WaterAccountFallowStatuses)
            .Include(x => x.Parcel.WaterAccountParcels).ThenInclude(x => x.WaterAccount).ThenInclude(x => x.WaterAccountCoverCropStatuses)
            .Include(x => x.UsageLocationCrops)
            .Include(x => x.CreateUser)
            .Include(x => x.UpdateUser)
            .Where(x => x.GeographyID == geographyID && x.ParcelID == parcelID && (currentUserIsAdminOrWaterManager || x.ReportingPeriod.ReadyForAccountHolders))
            .OrderBy(x => x.Name)
            .ToListAsync();

        var usageLocationDtos = usageLocations.Select(x => x.AsDto()).ToList();
        return usageLocationDtos;
    }

    public static async Task<List<UsageLocation>> ListByGeographyAndReportedDate(QanatDbContext dbContext, int geographyID, DateTime reportedDate, bool onlyCanBeRemoteSensed = false)
    {
        var usageLocations = await dbContext.UsageLocations.AsNoTracking()
            .Include(x => x.UsageLocationType)
            .Include(x => x.ReportingPeriod)
            .Where(x => x.GeographyID == geographyID
                     && (x.ReportingPeriod.StartDate.Year < reportedDate.Year || x.ReportingPeriod.StartDate.Year == reportedDate.Year && x.ReportingPeriod.StartDate.Month <= reportedDate.Month)
                     && (x.ReportingPeriod.EndDate.Year > reportedDate.Year || x.ReportingPeriod.EndDate.Year == reportedDate.Year && x.ReportingPeriod.EndDate.Month >= reportedDate.Month
                     && (!onlyCanBeRemoteSensed || x.UsageLocationType.CanBeRemoteSensed))
                  )
            .ToListAsync();

        return usageLocations;
    }

    public static async Task<List<UsageLocationByReportingPeriodIndexGridDto>> ListByGeographyAndReportingPeriodAsync(QanatDbContext dbContext, int geographyID, int reportingPeriodID)
    {
        var usageLocations = await dbContext.UsageLocations.AsNoTracking()
            .Include(x => x.ReportingPeriod)
            .Include(x => x.UsageLocationType)
            .Include(x => x.Parcel).ThenInclude(x => x.WaterAccountParcels).ThenInclude(x => x.WaterAccount)
            .Include(x => x.UsageLocationCrops)
            .Where(x => x.GeographyID == geographyID && x.ReportingPeriodID == reportingPeriodID)
            .ToListAsync();

        var usageLocationDtos = usageLocations.Select(x => new UsageLocationByReportingPeriodIndexGridDto()
        {
            UsageLocationID = x.UsageLocationID,
            UsageLocationTypeID = x.UsageLocationTypeID,
            UsageLocationTypeName = x.UsageLocationType?.Name,
            WaterAccountID = x.Parcel.WaterAccountParcels.FirstOrDefault(wap => wap.ReportingPeriodID == reportingPeriodID)?.WaterAccountID,
            WaterAccountNumberAndName = x.Parcel.WaterAccountParcels.FirstOrDefault(wap => wap.ReportingPeriodID == reportingPeriodID)?.WaterAccount?.WaterAccountNumberAndName(),
            ParcelID = x.Parcel.ParcelID,
            ParcelNumber = x.Parcel.ParcelNumber,
            ReportingPeriodID = x.ReportingPeriod.ReportingPeriodID,
            ReportingPeriodName = x.ReportingPeriod.Name,
            Name = x.Name,
            Area = x.Area,
            Crops = x.UsageLocationCrops.Select(c => c.Name).ToList()
        }).ToList();

        return usageLocationDtos;
    }

    public static async Task<List<UsageLocationByReportingPeriodIndexGridDto>> ListByGeographyAndReportingPeriodForUserAsync(QanatDbContext dbContext, int geographyID, int reportingPeriodID, UserDto callingUser)
    {
        var hasCurrentGeographyFlags = callingUser.GeographyFlags.TryGetValue(geographyID, out var geographyFlags);
        var isWaterManager = false;
        if (hasCurrentGeographyFlags)
        {
            geographyFlags.TryGetValue(Flag.HasManagerDashboard.FlagName, out isWaterManager);
        }

        callingUser.Flags.TryGetValue(Flag.IsSystemAdmin.FlagName, out var isAdmin);
        var callingUserIsAdminOrWaterManager = isAdmin || isWaterManager;

        var usageLocations = await dbContext.UsageLocations.AsNoTracking()
            .Include(x => x.ReportingPeriod)
            .Include(x => x.UsageLocationType)
            .Include(x => x.Parcel).ThenInclude(x => x.WaterAccountParcels).ThenInclude(x => x.WaterAccount).ThenInclude(x => x.WaterAccountUsers)
            .Include(x => x.UsageLocationCrops)
            .Where(x => x.GeographyID == geographyID && x.ReportingPeriodID == reportingPeriodID && (callingUserIsAdminOrWaterManager || x.Parcel.WaterAccount.WaterAccountUsers.Select(wau => wau.UserID).Contains(callingUser.UserID)))
            .ToListAsync();

        var usageLocationDtos = usageLocations.Select(x => new UsageLocationByReportingPeriodIndexGridDto()
        {
            UsageLocationID = x.UsageLocationID,
            UsageLocationTypeID = x.UsageLocationTypeID,
            UsageLocationTypeName = x.UsageLocationType?.Name,
            WaterAccountID = x.Parcel.WaterAccountParcels.FirstOrDefault(wap => wap.ReportingPeriodID == reportingPeriodID)?.WaterAccountID,
            WaterAccountNumberAndName = x.Parcel.WaterAccountParcels.FirstOrDefault(wap => wap.ReportingPeriodID == reportingPeriodID)?.WaterAccount?.WaterAccountNumberAndName(),
            ParcelID = x.Parcel.ParcelID,
            ParcelNumber = x.Parcel.ParcelNumber,
            ReportingPeriodID = x.ReportingPeriod.ReportingPeriodID,
            ReportingPeriodName = x.ReportingPeriod.Name,
            Name = x.Name,
            Area = x.Area,
            Crops = x.UsageLocationCrops.Select(c => c.Name).ToList()
        }).ToList();

        return usageLocationDtos;
    }

    public static async Task<List<UsageLocationDto>> ListByWaterAccountAsync(QanatDbContext dbContext, int geographyID, int waterAccountID)
    {
        var usageLocations = await dbContext.UsageLocations
            .Include(x => x.Geography)
            .Include(x => x.ReportingPeriod)
            .Include(x => x.UsageLocationType)
            .Include(x => x.Parcel).ThenInclude(x => x.WaterAccountParcels).ThenInclude(x => x.WaterAccount).ThenInclude(x => x.WaterAccountFallowStatuses)
            .Include(x => x.Parcel).ThenInclude(x => x.WaterAccountParcels).ThenInclude(x => x.WaterAccount).ThenInclude(x => x.WaterAccountCoverCropStatuses)
            .Include(x => x.UsageLocationCrops)
            .Include(x => x.CreateUser)
            .Include(x => x.UpdateUser)
            .Where(x => x.GeographyID == geographyID && x.Parcel.WaterAccountParcels.Where(wap => wap.ReportingPeriodID == x.ReportingPeriodID).Select(wap => wap.WaterAccountID).Contains(waterAccountID))
            .OrderBy(x => x.Name)
            .ToListAsync();

        var usageLocationDtos = usageLocations.Select(x => x.AsDto()).ToList();
        return usageLocationDtos;
    }

    public static async Task<UsageLocationDto> GetAsync(QanatDbContext dbContext, int geographyID, int parcelID, int usageLocationID)
    {
        var usageLocation = await dbContext.UsageLocations.AsNoTracking()
            .Include(x => x.Geography)
            .Include(x => x.ReportingPeriod)
            .Include(x => x.UsageLocationType)
            .Include(x => x.Parcel).ThenInclude(x => x.WaterAccountParcels).ThenInclude(x => x.WaterAccount).ThenInclude(x => x.WaterAccountFallowStatuses)
            .Include(x => x.Parcel).ThenInclude(x => x.WaterAccountParcels).ThenInclude(x => x.WaterAccount).ThenInclude(x => x.WaterAccountCoverCropStatuses)
            .Include(x => x.UsageLocationCrops)
            .Include(x => x.CreateUser)
            .Include(x => x.UpdateUser)
            .SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ParcelID == parcelID && x.UsageLocationID == usageLocationID);

        var usageLocationSourceOfRecords = await dbContext.vWaterMeasurementSourceOfRecords.AsNoTracking()
            .Where(x => x.UsageLocationID == usageLocationID)
            .ToListAsync();

        var usageLocationDto = usageLocation?.AsDto(usageLocationSourceOfRecords);
        return usageLocationDto;
    }

    public static UsageLocationHierarchyDto GetHierarchyDtoByID(QanatDbContext dbContext, int usageLocationID)
    {
        var usageLocation = dbContext.UsageLocations
            .Include(x => x.Parcel).AsNoTracking()
            .SingleOrDefault(x => x.UsageLocationID == usageLocationID);

        var hierarchyDto = usageLocation != null
            ? new UsageLocationHierarchyDto
            {
                UsageLocationID = usageLocation.UsageLocationID,
                GeographyID = usageLocation.GeographyID,
                WaterAccountID = usageLocation.Parcel.WaterAccountID,
                ParcelID = usageLocation.ParcelID
            }
            : null;

        return hierarchyDto;
    }

    public static async Task<List<ErrorMessage>> ValidateReplaceFromParcelsAsync(QanatDbContext dbContext, int geographyID, int reportingPeriodID)
    {
        var errors = new List<ErrorMessage>();
        await Task.CompletedTask;
        return errors;
    }

    public static async Task<List<UsageLocationByReportingPeriodIndexGridDto>> ReplaceFromParcelsAsync(QanatDbContext dbContext, int geographyID, int reportingPeriodID)
    {
        await dbContext.WaterMeasurements
            .Include(x => x.UsageLocation)
            .Where(x => x.UsageLocation.GeographyID == geographyID && x.UsageLocation.ReportingPeriodID == reportingPeriodID)
            .ExecuteDeleteAsync();

        await dbContext.UsageLocationParcelHistories
            .Include(x => x.UsageLocation)
            .Where(x => x.GeographyID == geographyID && x.UsageLocation.ReportingPeriodID == reportingPeriodID)
            .ExecuteDeleteAsync();

        await dbContext.UsageLocationHistories
            .Include(x => x.UsageLocation)
            .Where(x => x.GeographyID == geographyID && x.UsageLocation.ReportingPeriodID == reportingPeriodID)
            .ExecuteDeleteAsync();

        await dbContext.UsageLocations
            .Where(x => x.GeographyID == geographyID && x.ReportingPeriodID == reportingPeriodID)
            .ExecuteDeleteAsync();

        var parcels = await dbContext.Parcels.AsNoTracking()
            .Where(x => x.GeographyID == geographyID)
            .ToListAsync();

        var parcelGeometries = await dbContext.ParcelGeometries.AsNoTracking()
            .Where(x => x.GeographyID == geographyID)
            .ToListAsync();

        var defaultUsageLocationType = await dbContext.UsageLocationTypes.AsNoTracking()
            .SingleAsync(x => x.GeographyID == geographyID && x.IsDefault);

        var usageLocationsToAdd = new List<UsageLocation>();
        foreach (var parcel in parcels)
        {
            var parcelGeometry = parcelGeometries.SingleOrDefault(x => x.ParcelID == parcel.ParcelID);
            var usageLocation = new UsageLocation()
            {
                GeographyID = geographyID,
                ParcelID = parcel.ParcelID,
                ReportingPeriodID = reportingPeriodID,
                UsageLocationTypeID = defaultUsageLocationType.UsageLocationTypeID,
                Name = parcel.ParcelNumber,
                Area = parcel.ParcelArea,
                UsageLocationGeometry = parcelGeometry != null
                    ? new UsageLocationGeometry
                    {
                        Geometry4326 = parcelGeometry.Geometry4326,
                        GeometryNative = parcelGeometry.GeometryNative
                    }
                    : null
            };

            usageLocationsToAdd.Add(usageLocation);
        }

        await dbContext.UsageLocations.AddRangeAsync(usageLocationsToAdd);
        await dbContext.SaveChangesAsync();

        var newUsageLocations = await ListByGeographyAndReportingPeriodAsync(dbContext, geographyID, reportingPeriodID);
        return newUsageLocations;
    }

    public static async Task<List<AlertMessageDto>> ProcessUsageLocationGDBUpload(QanatDbContext dbContext, int geographyID, int reportingPeriodID, List<UsageLocationGdbFeature> usageLocationGDBFeatures, string sourceWkt, string geographyCoordinateSystemWkt, bool isReplace, UserDto callingUser)
    {
        var result = new List<AlertMessageDto>();
        var geography = await dbContext.Geographies.AsNoTracking()
            .SingleAsync(x => x.GeographyID == geographyID);

        var parcels = await dbContext.Parcels.AsNoTracking()
            .Where(x => x.GeographyID == geographyID)
            .ToListAsync();

        var existingUsageLocations = dbContext.UsageLocations
            .Include(x => x.UsageLocationGeometry)
            .Where(x => x.GeographyID == geographyID && x.ReportingPeriodID == reportingPeriodID);

        var usageLocationTypes = await UsageLocationTypes.ListAsync(dbContext, geographyID);
        var defaultUsageLocationTypeID = usageLocationTypes.Single(x => x.Geography.GeographyID == geographyID && x.IsDefault).UsageLocationTypeID;

        var duplicateNames = usageLocationGDBFeatures.GroupBy(x => x.UsageLocationName)
            .Where(x => x.Count() > 1)
            .Select(x => x.Key).ToList();

        if (duplicateNames.Count > 0)
        {
            result.Add(new AlertMessageDto() { AlertMessageType = AlertMessageTypeEnum.Error, Message = $"Duplicate names found in gdb. The following names appear more than once: {string.Join(", ", duplicateNames)}. Please review and then resubmit." });
            return result;
        }

        if (isReplace)
        {
            var usageLocationIDsToRemove = existingUsageLocations.ToList().Select(x => x.UsageLocationID).ToList();

            await dbContext.WaterMeasurements
                .Where(x => usageLocationIDsToRemove.Contains(x.UsageLocationID))
                .ExecuteDeleteAsync();

            await dbContext.UsageLocationParcelHistories
                .Where(x => usageLocationIDsToRemove.Contains(x.UsageLocationID))
                .ExecuteDeleteAsync();

            await dbContext.UsageLocationHistories
                .Where(x => usageLocationIDsToRemove.Contains(x.UsageLocationID))
                .ExecuteDeleteAsync();

            await existingUsageLocations.ExecuteDeleteAsync();
        }

        var failedApns = new List<string>();

        var usageLocations = new List<UsageLocation>();
        foreach (var usageLocationFromGDB in usageLocationGDBFeatures)
        {
            if (!Parcels.IsValidParcelNumber(geography.APNRegexPattern, usageLocationFromGDB.APN))
            {
                failedApns.Add($"The following APN is not in a valid format for this geography: {usageLocationFromGDB.APN} for ID {usageLocationFromGDB.UsageLocationName}.");
                continue;
            }
            var parcelID = parcels.SingleOrDefault(x => x.ParcelNumber == usageLocationFromGDB.APN)?.ParcelID;
            if (parcelID == null)
            {
                failedApns.Add($"Could not find the following APN provided in the File GDB Upload: {usageLocationFromGDB.APN} for ID {usageLocationFromGDB.UsageLocationName}.");
                continue;
            }

            var existingUsageLocation = await existingUsageLocations.FirstOrDefaultAsync(x => x.Name != usageLocationFromGDB.UsageLocationName);
            if (existingUsageLocation is { UsageLocationGeometry: not null })
            {
                dbContext.UsageLocationGeometries.Remove(existingUsageLocation.UsageLocationGeometry);
            }

            var geometry4326 = usageLocationFromGDB.Geometry.ProjectTo4326(sourceWkt);
            var geometryNative = geometry4326.ProjectToSrid(geography.CoordinateSystem, geographyCoordinateSystemWkt);
            var area = geometryNative.Area / geography.AreaToAcresConversionFactor;
            var usageLocation = new UsageLocation
            {
                GeographyID = geography.GeographyID,
                ParcelID = (int)parcelID,
                ReportingPeriodID = reportingPeriodID,
                Name = $"{usageLocationFromGDB.UsageLocationName}",
                Area = area,
                UsageLocationGeometry = new UsageLocationGeometry()
                {
                    Geometry4326 = geometry4326,
                    GeometryNative = geometryNative,
                },
                UsageLocationCrops = new List<UsageLocationCrop>() { },
                UsageLocationTypeID = usageLocationFromGDB.UsageLocationType != null 
                    ? usageLocationTypes.SingleOrDefault(x => x.Name == usageLocationFromGDB.UsageLocationType)?.UsageLocationTypeID 
                    : defaultUsageLocationTypeID
            };

            if (usageLocationFromGDB.Crop1 != null && usageLocationFromGDB.Crop1 != "****")
            {
                usageLocation.UsageLocationCrops.Add(new UsageLocationCrop
                {
                    Name = usageLocationFromGDB.Crop1
                });
            }

            if (usageLocationFromGDB.Crop2 != null && usageLocationFromGDB.Crop2 != "****")
            {
                usageLocation.UsageLocationCrops.Add(new UsageLocationCrop
                {
                    Name = usageLocationFromGDB.Crop2
                });
            }

            if (usageLocationFromGDB.Crop3 != null && usageLocationFromGDB.Crop3 != "****")
            {
                usageLocation.UsageLocationCrops.Add(new UsageLocationCrop
                {
                    Name = usageLocationFromGDB.Crop3
                });
            }

            if (usageLocationFromGDB.Crop4 != null && usageLocationFromGDB.Crop4 != "****")
            {
                usageLocation.UsageLocationCrops.Add(new UsageLocationCrop
                {
                    Name = usageLocationFromGDB.Crop4
                });
            }

            usageLocations.Add(usageLocation);
        }

        var newUsageLocations = new List<UsageLocation>();
        var updatedUsageLocations = new List<UsageLocation>();
        var previousUsageLocationTypes = existingUsageLocations.ToDictionary(x => x.UsageLocationID, location => location.UsageLocationTypeID);

        if (isReplace)
        {
            dbContext.UsageLocations.AddRange(usageLocations);
            newUsageLocations.AddRange(usageLocations);
        }
        else
        {
            var usageLocationsList = existingUsageLocations.ToList();
            newUsageLocations = usageLocationsList.MergeNew(usageLocations, dbContext.UsageLocations, (x, y) => x.Name == y.Name);
            updatedUsageLocations = usageLocationsList.MergeUpdate(usageLocations, (x, y) => x.Name == y.Name, (x, y) =>
            {
                x.ParcelID = y.ParcelID;
                x.Area = y.Area;
                x.UsageLocationGeometry = y.UsageLocationGeometry;
                x.ReportingPeriodID = y.ReportingPeriodID;
                x.UsageLocationCrops = y.UsageLocationCrops;
                x.UsageLocationTypeID = y.UsageLocationTypeID;
                x.UpdateDate = DateTime.Now;
            });
        }

        var usageLocationHistories = new List<UsageLocationHistory>();
        foreach (var usageLocation in newUsageLocations)
        {
            usageLocationHistories.Add(new UsageLocationHistory()
            {
                GeographyID = geography.GeographyID,
                UsageLocation = usageLocation,
                UsageLocationTypeID = usageLocation.UsageLocationTypeID,
                Note = $"Created from Usage Location GDB Upload.",
                CreateUserID = callingUser.UserID,
                CreateDate = DateTime.UtcNow
            });
        }

        foreach (var updatedUsageLocation in updatedUsageLocations)
        {
            var previousUsageLocationType = previousUsageLocationTypes.Single(x => x.Key == updatedUsageLocation.UsageLocationID);
            if (previousUsageLocationType.Value != updatedUsageLocation.UsageLocationTypeID)
            {
                usageLocationHistories.Add(new UsageLocationHistory()
                {
                    GeographyID = geography.GeographyID,
                    UsageLocationID = updatedUsageLocation.UsageLocationID,
                    UsageLocationTypeID = updatedUsageLocation.UsageLocationTypeID,
                    Note = $"Updated from Usage Location GDB Upload.",
                    CreateUserID = callingUser.UserID,
                    CreateDate = DateTime.UtcNow
                });
            }
        }

        await dbContext.UsageLocationHistories.AddRangeAsync(usageLocationHistories);

        await dbContext.SaveChangesAsync();

        await dbContext.Database.ExecuteSqlRawAsync("EXECUTE dbo.pMakeValidUsageLocationGeometries");

        failedApns.ForEach(x => result.Add(new AlertMessageDto(){ AlertMessageType = AlertMessageTypeEnum.Warn, Message = x}));
        return result;
    }

    #region Bulk Update Usage Location Type

    public static async Task<List<ErrorMessage>> ValidateBulkUpdateUsageLocationTypeAsync(QanatDbContext dbContext, int geographyID, UsageLocationBulkUpdateUsageLocationTypeDto updateDto)
    {
        var errors = new List<ErrorMessage>();
        var usageLocationType = await dbContext.UsageLocationTypes.FirstOrDefaultAsync(x => x.GeographyID == geographyID && x.UsageLocationTypeID == updateDto.UsageLocationTypeID);
        if (usageLocationType == null)
        {
            errors.Add(new ErrorMessage("Usage Location Type", $"No valid Usage Location Type found with the ID: {updateDto.UsageLocationTypeID}."));
        }

        return errors;
    }

    public static async Task<List<UsageLocationDto>> BulkUpdateUsageLocationTypeAsync(QanatDbContext dbContext, int geographyID, int reportingPeriodID, UsageLocationBulkUpdateUsageLocationTypeDto updateDto, UserDto callingUser)
    {
        var usageLocations = await dbContext.UsageLocations
            .Include(x => x.UsageLocationType)
            .Where(x => x.GeographyID == geographyID && x.ReportingPeriodID == reportingPeriodID && updateDto.UsageLocationIDs.Contains(x.UsageLocationID))
            .ToListAsync();

        var usageLocationsToRecalculate = usageLocations
            .Where(x => x.UsageLocationType.WaterMeasurementTypeID.HasValue)
            .ToList();

        var usageLocationsByWaterMeasurementID = usageLocationsToRecalculate
            .GroupBy(x => x.UsageLocationType.WaterMeasurementTypeID!.Value)
            .ToDictionary(g => g.Key, g => g.Select(x => x.UsageLocationID).ToList());

        var histories = new List<UsageLocationHistory>();
        foreach (var usageLocation in usageLocations)
        {
            if (usageLocation.UsageLocationTypeID != updateDto.UsageLocationTypeID)
            {
                var usageLocationHistory = new UsageLocationHistory()
                {
                    GeographyID = geographyID,
                    UsageLocationID = usageLocation.UsageLocationID,
                    UsageLocationTypeID = updateDto.UsageLocationTypeID,
                    Note = updateDto.Note,
                    CreateUserID = callingUser.UserID,
                    CreateDate = DateTime.UtcNow
                };

                histories.Add(usageLocationHistory);

                usageLocation.UsageLocationTypeID = updateDto.UsageLocationTypeID;
                usageLocation.UpdateUserID = callingUser.UserID;
                usageLocation.UpdateDate = DateTime.UtcNow;
            }
        }

        await dbContext.UsageLocationHistories.AddRangeAsync(histories);
        await dbContext.SaveChangesAsync();

        var usageLocationType = await dbContext.UsageLocationTypes.AsNoTracking()
            .SingleAsync(x => x.GeographyID == geographyID && x.UsageLocationTypeID == updateDto.UsageLocationTypeID);

        if (usageLocationType.WaterMeasurementTypeID.HasValue)
        {
            var reportingPeriod = await dbContext.ReportingPeriods.AsNoTracking()
                .SingleAsync(x => x.ReportingPeriodID == reportingPeriodID && x.GeographyID == geographyID);

            var effectiveDates = reportingPeriod.GetLastDayOfEachMonth();
            var usageLocationIDs = usageLocations.Select(x => x.UsageLocationID).ToList();
            foreach (var effectiveDate in effectiveDates)
            {
                await WaterMeasurementCalculations.RunMeasurementTypeForGeographyAsync(dbContext, geographyID, usageLocationType.WaterMeasurementTypeID.Value, effectiveDate, usageLocationIDs);
            }
        }
        else if(usageLocationsToRecalculate.Any())
        {
            var reportingPeriod = await dbContext.ReportingPeriods.AsNoTracking()
                .SingleAsync(x => x.ReportingPeriodID == reportingPeriodID && x.GeographyID == geographyID);

            var effectiveDates = reportingPeriod.GetLastDayOfEachMonth();
            foreach (var grouping in usageLocationsByWaterMeasurementID)
            {
                foreach (var effectiveDate in effectiveDates)
                {
                    await WaterMeasurementCalculations.RunMeasurementTypeForGeographyAsync(dbContext, geographyID, grouping.Key, effectiveDate, grouping.Value);
                }
            }
        }

        var updatedUsageLocations = await dbContext.UsageLocations.AsNoTracking()
            .Include(x => x.Geography)
            .Include(x => x.ReportingPeriod)
            .Include(x => x.UsageLocationType)
            .Include(x => x.Parcel).ThenInclude(x => x.WaterAccountParcels).ThenInclude(x => x.WaterAccount).ThenInclude(x => x.WaterAccountFallowStatuses)
            .Include(x => x.Parcel).ThenInclude(x => x.WaterAccountParcels).ThenInclude(x => x.WaterAccount).ThenInclude(x => x.WaterAccountCoverCropStatuses)
            .Include(x => x.UsageLocationCrops)
            .Include(x => x.CreateUser)
            .Include(x => x.UpdateUser)
            .Where(x => x.GeographyID == geographyID && updateDto.UsageLocationIDs.Contains(x.UsageLocationID))
            .ToListAsync();

        var usageLocationDtos = updatedUsageLocations.Select(x => x.AsDto()).ToList();
        return usageLocationDtos;
    }

    #endregion

    #region Update Cover Crop 

    public static async Task<List<ErrorMessage>> ValidateUpdateCoverCropAsync(QanatDbContext dbContext, int geographyID, int parcelID, int usageLocationID, UsageLocationUpdateCoverCropDto updateCoverCropDto)
    {
        var errors = new List<ErrorMessage>();

        var usageLocationType = await dbContext.UsageLocationTypes.AsNoTracking()
            .FirstOrDefaultAsync(x => x.GeographyID == geographyID && x.UsageLocationTypeID == updateCoverCropDto.UsageLocationTypeID);

        if (usageLocationType == null)
        {
            errors.Add(new ErrorMessage($"Usage Location Type ID", $"Could not find a Usage Location Type with the ID {updateCoverCropDto.UsageLocationTypeID}."));
        }
        else if (!usageLocationType.CanBeSelectedInCoverCropForm)
        {
            errors.Add(new ErrorMessage($"Usage Location Type", $"{usageLocationType.Name} does not support cover crops."));
        }

        return errors;
    }

    public static async Task<UsageLocationDto> UpdateCoverCropAsync(QanatDbContext dbContext, int geographyID, int parcelID, int usageLocationID, UsageLocationUpdateCoverCropDto updateCoverCropDto, UserDto callingUser)
    {
        var usageLocation = await dbContext.UsageLocations
            .Include(x => x.Geography)
            .Include(x => x.UsageLocationType)
            .Include(x => x.Parcel).ThenInclude(x => x.WaterAccountParcels).ThenInclude(x => x.WaterAccount).ThenInclude(x => x.WaterAccountCoverCropStatuses)
            .SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ParcelID == parcelID && x.UsageLocationID == usageLocationID);

        if (usageLocation.UsageLocationTypeID != updateCoverCropDto.UsageLocationTypeID)
        {
            var usageLocationHistory = new UsageLocationHistory()
            {
                GeographyID = geographyID,
                UsageLocationID = usageLocation.UsageLocationID,
                UsageLocationTypeID = updateCoverCropDto.UsageLocationTypeID,
                Note = $"Updated from self report cover crop.",
                CreateUserID = callingUser.UserID,
                CreateDate = DateTime.UtcNow
            };

            await dbContext.UsageLocationHistories.AddAsync(usageLocationHistory);

            usageLocation.UsageLocationTypeID = updateCoverCropDto.UsageLocationTypeID;
            usageLocation.CoverCropNote = updateCoverCropDto.CoverCropNote;
            usageLocation.UpdateUserID = callingUser.UserID;
            usageLocation.UpdateDate = DateTime.UtcNow;

            await dbContext.SaveChangesAsync();
        }

        var updatedUsageLocation = await GetAsync(dbContext, geographyID, parcelID, usageLocationID);
        return updatedUsageLocation;
    }

    #endregion

    #region Update Fallowing

    public static async Task<List<ErrorMessage>> ValidateUpdateFallowingAsync(QanatDbContext dbContext, int geographyID, int parcelID, int usageLocationID, UsageLocationUpdateFallowingDto updateFallowingDto)
    {
        var errors = new List<ErrorMessage>();

        var usageLocationType = await dbContext.UsageLocationTypes.AsNoTracking()
            .FirstOrDefaultAsync(x => x.GeographyID == geographyID && x.UsageLocationTypeID == updateFallowingDto.UsageLocationTypeID);

        if (usageLocationType == null)
        {
            errors.Add(new ErrorMessage($"Usage Location Type ID", $"Could not find a Usage Location Type with the ID {updateFallowingDto.UsageLocationTypeID}."));
        }
        else if (!usageLocationType.CanBeSelectedInFallowForm)
        {
            errors.Add(new ErrorMessage($"Usage Location Type", $"{usageLocationType.Name} does not support fallowing."));
        }

        return errors;
    }

    public static async Task<UsageLocationDto> UpdateFallowingAsync(QanatDbContext dbContext, int geographyID, int parcelID, int usageLocationID, UsageLocationUpdateFallowingDto updateFallowingDto, UserDto callingUser)
    {
        var usageLocation = await dbContext.UsageLocations
            .Include(x => x.Geography)
            .Include(x => x.UsageLocationType)
            .Include(x => x.Parcel).ThenInclude(x => x.WaterAccountParcels).ThenInclude(x => x.WaterAccount).ThenInclude(x => x.WaterAccountCoverCropStatuses)
            .SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ParcelID == parcelID && x.UsageLocationID == usageLocationID);

        if (usageLocation.UsageLocationID != updateFallowingDto.UsageLocationTypeID)
        {
            var usageLocationHistory = new UsageLocationHistory()
            {
                GeographyID = geographyID,
                UsageLocationID = usageLocation.UsageLocationID,
                UsageLocationTypeID = updateFallowingDto.UsageLocationTypeID,
                Note = $"Updated from self report fallowing.",
                CreateUserID = callingUser.UserID,
                CreateDate = DateTime.UtcNow
            };

            await dbContext.UsageLocationHistories.AddAsync(usageLocationHistory);

            usageLocation.UsageLocationTypeID = updateFallowingDto.UsageLocationTypeID;
            usageLocation.FallowNote = updateFallowingDto.FallowingNote;
            usageLocation.UpdateUserID = callingUser.UserID;
            usageLocation.UpdateDate = DateTime.UtcNow;

            await dbContext.SaveChangesAsync();
        }


        var updatedUsageLocation = await GetAsync(dbContext, geographyID, parcelID, usageLocationID);
        return updatedUsageLocation;
    }

    #endregion

    #region Migrate Usage Locations

    public static async Task<List<ErrorMessage>> ValidateMigrateUsageLocationsAsync(QanatDbContext dbContext, int geographyID, int parcelID, UsageLocationMigrationDto usageLocationMigrationDto)
    {
        var errors = new List<ErrorMessage>();

        var usageLocations = await dbContext.UsageLocations.AsNoTracking()
            .Where(x => x.GeographyID == geographyID)
            .ToListAsync();

        foreach (var usageLocationID in usageLocationMigrationDto.UsageLocationIDs)
        {
            var usageLocation = usageLocations.FirstOrDefault(x => x.UsageLocationID == usageLocationID);
            if (usageLocation == null)
            {
                errors.Add(new ErrorMessage("Usage Location ID", $"Usage Location with ID {usageLocationID} does not exist in the specified Geography."));
            }
        }

        return errors;
    }

    public static async Task<List<UsageLocationDto>> MigrateUsageLocationsAsync(QanatDbContext dbContext, int geographyID, int toParcelID, UsageLocationMigrationDto usageLocationMigrationDto, UserDto callingUser)
    {
        var usageLocations = await dbContext.UsageLocations
            .Include(x => x.Geography)
            .Include(x => x.UsageLocationType)
            .Include(x => x.Parcel).ThenInclude(x => x.WaterAccountParcels).ThenInclude(x => x.WaterAccount).ThenInclude(x => x.WaterAccountFallowStatuses)
            .Include(x => x.Parcel).ThenInclude(x => x.WaterAccountParcels).ThenInclude(x => x.WaterAccount).ThenInclude(x => x.WaterAccountCoverCropStatuses)
            .Include(x => x.ReportingPeriod)
            .Include(x => x.UsageLocationCrops)
            .Include(x => x.CreateUser)
            .Include(x => x.UpdateUser)
            .Where(x => x.GeographyID == geographyID && usageLocationMigrationDto.UsageLocationIDs.Contains(x.UsageLocationID))
            .ToListAsync();

        var toParcel = await dbContext.Parcels.AsNoTracking()
            .FirstOrDefaultAsync(x => x.ParcelID == toParcelID);

        foreach (var usageLocation in usageLocations)
        {
            if (usageLocation.ParcelID == toParcel.ParcelID)
            {
                continue;
            }

            var usageLocationParcelHistory = new UsageLocationParcelHistory()
            {
                GeographyID = usageLocation.GeographyID,
                UsageLocationID = usageLocation.UsageLocationID,
                ReportingPeriodID = usageLocation.ReportingPeriodID,

                FromParcelID = usageLocation.ParcelID,
                FromParcelNumber = usageLocation.Parcel.ParcelNumber,

                ToParcelID = toParcel.ParcelID,
                ToParcelNumber = toParcel.ParcelNumber,

                Reason = "Manually migrate.",

                CreateUserID = callingUser.UserID,
                CreateDate = DateTime.UtcNow
            };

            dbContext.UsageLocationParcelHistories.Add(usageLocationParcelHistory);

            usageLocation.ParcelID = toParcelID;
        }

        dbContext.UsageLocations.UpdateRange(usageLocations);
        await dbContext.SaveChangesAsync();

        var usageLocationDtos = usageLocations.Select(x => x.AsDto()).ToList();
        return usageLocationDtos;
    }

    #endregion

}