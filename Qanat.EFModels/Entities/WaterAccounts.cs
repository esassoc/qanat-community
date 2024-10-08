using Microsoft.EntityFrameworkCore;
using Qanat.Common.GeoSpatial;
using Qanat.Common.Util;
using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities;

public class WaterAccounts
{ 
    public static List<WaterAccountMinimalDto> ListAsMinimalDtos(QanatDbContext dbContext)
    {
        return dbContext.WaterAccounts.Include(x => x.Geography)
            .AsNoTracking()
            .OrderBy(x => x.GeographyID)
            .ThenBy(x => x.WaterAccountName)
            .Select(x => x.AsWaterAccountMinimalDto()).ToList();
    }

    /// <summary>
    /// Lists all water accounts for a user.
    /// For normal users, returns all water accounts where user is either a WaterAccountUser or a water manager for the associated geography.
    /// For platform admins, returns all water accounts.
    /// </summary>
    /// <param name="dbContext"></param>
    /// <param name="user"></param>
    /// <returns></returns>
    /// 
    public static List<WaterAccountMinimalDto> ListByUserAsMinimalDtos(QanatDbContext dbContext, UserDto user)
    {
        var hasPermission = user.Role.RoleID == (int)RoleEnum.SystemAdmin;

        var waterAccountMinimalDtos = hasPermission
            ? ListAsMinimalDtos(dbContext)
            : ListByUserIDAsMinimalDtos(dbContext, user.UserID);

        return waterAccountMinimalDtos;
    }

    private static List<WaterAccountMinimalDto> ListByUserIDAsMinimalDtos(QanatDbContext dbContext, int userID)
    {
        var managedGeographyIDs = dbContext.GeographyUsers.AsNoTracking()
            .Where(x => x.UserID == userID && x.GeographyRoleID == GeographyRole.WaterManager.GeographyRoleID)
            .Select(x => x.GeographyID).ToList();

        var waterAccountMinimalDtos = dbContext.WaterAccounts.AsNoTracking()
            .Include(x => x.Geography)
            .Include(x => x.WaterAccountUsers)
            .Where(x => managedGeographyIDs.Contains(x.GeographyID) || x.WaterAccountUsers.Any(y => y.UserID == userID))
            .Select(x => x.AsWaterAccountMinimalDto()).ToList();

        return waterAccountMinimalDtos;
    }

    public static List<WaterAccountIndexGridDto> ListByGeographyIDAsIndexGridDtos(QanatDbContext dbContext, int geographyID)
    {
        return dbContext.WaterAccounts.AsNoTracking()
            .Include(x => x.Parcels)
            .Include(x => x.WaterAccountUsers).ThenInclude(x => x.User)
            .Where(x => x.GeographyID == geographyID)
            .Select(x => x.AsIndexGridDto())
            .ToList();
    }

    public static List<WaterAccountIndexGridDto> ListByGeographyIDAndUserIDAsIndexGridDtos(QanatDbContext dbContext, int geographyID, int userID)
    {
        return dbContext.WaterAccounts.AsNoTracking()
            .Include(x => x.Parcels)
            .Include(x => x.WaterAccountUsers).ThenInclude(x => x.User)
            .Where(x => x.GeographyID == geographyID && x.WaterAccountUsers.Any(y => y.UserID == userID))
            .Select(x => x.AsIndexGridDto())
            .ToList();
    }

    public static WaterAccount GetByID(QanatDbContext dbContext, int waterAccountID)
    {
        return dbContext.WaterAccounts.AsNoTracking()
            .Include(x => x.Geography)
            .Include(x => x.WaterAccountUsers)
            .ThenInclude(x => x.User)
            .Include(x => x.Parcels)
            .SingleOrDefault(x => x.WaterAccountID == waterAccountID);
    }

    public static WaterAccountDto GetByIDAsDto(QanatDbContext dbContext, int waterAccountID)
    {
       return GetByID(dbContext, waterAccountID).AsWaterAccountDto();
    }

    public static WaterAccountMinimalDto GetByIDAsMinimalDto(QanatDbContext dbContext, int waterAccountID)
    {
       return dbContext.WaterAccounts.Include(x => x.Geography).AsNoTracking()
           .Single(x => x.WaterAccountID == waterAccountID).AsWaterAccountMinimalDto();
    }

    public static WaterAccountGeoJSONDto GetByIDAsWaterAccountGeoJSONDto(QanatDbContext dbContext, int waterAccountID)
    {
        var parcels = dbContext.Parcels
            .Include(x => x.WaterAccount)
            .Include(x => x.ParcelGeometry)
            .Include(x => x.UsageEntities).ThenInclude(x => x.UsageEntityGeometry)
            .Include(x => x.UsageEntities).ThenInclude(x => x.UsageEntityCrops)
            .AsNoTracking()
            .Where(x => x.WaterAccountID == waterAccountID)
            .ToList();

        var waterAccountGeoJSONDto = new WaterAccountGeoJSONDto()
        {
            BoundingBox = new BoundingBoxDto(parcels.Select(x => x.ParcelGeometry.Geometry4326)),
            Parcels = parcels.Select(x => new ParcelWithGeoJSONDto
                {
                    ParcelID = x.ParcelID,
                    ParcelNumber = x.ParcelNumber,
                    ParcelArea = x.ParcelArea,
                    WaterAccountID = x.WaterAccountID,
                    WaterAccountNameAndNumber = x.WaterAccount.WaterAccountNameAndNumber(),
                    WaterAccountOwnerName = x.WaterAccount.ContactName,
                    GeoJSON = x.ParcelGeometry.Geometry4326.ToGeoJSON()
                }
            ).ToList(),
            UsageEntities = parcels.SelectMany(x => x.UsageEntities.Select(y => new UsageEntityWithGeoJSONDto()
            {
                Area = y.UsageEntityArea,
                CropNames = y.UsageEntityCrops.Select(z => z.UsageEntityCropName).ToList(),
                GeoJSON = y.UsageEntityGeometry?.Geometry4326.ToGeoJSON(),
                ParcelID = x.ParcelID,
                UsageEntityID = y.UsageEntityID,
                UsageEntityName = y.UsageEntityName
            })).ToList()
        };

        return waterAccountGeoJSONDto;
    }

    public static WaterAccountDto UpdateAccountEntity(QanatDbContext dbContext, int waterAccountID,
        WaterAccountUpdateDto waterAccountUpdateDto)
    {
        var account = dbContext.WaterAccounts
            .Single(x => x.WaterAccountID == waterAccountID);

        account.WaterAccountName = waterAccountUpdateDto.WaterAccountName;
        account.ContactName = waterAccountUpdateDto.ContactName;
        account.ContactAddress = waterAccountUpdateDto.ContactAddress;
        account.UpdateDate = DateTime.UtcNow;
        account.Notes = waterAccountUpdateDto.Notes ?? account.Notes;

        dbContext.SaveChanges();
        dbContext.Entry(account).Reload();
        return GetByIDAsDto(dbContext, waterAccountID);
    }

    public static WaterAccountMinimalDto SetAssociatedUsers(QanatDbContext dbContext, int waterAccountID,
        List<WaterAccountUserMinimalDto> waterAccountUserMinimalDtos, out List<int> addedUserIDs)
    {
        var newAccountUsers = waterAccountUserMinimalDtos.Select(minimalDto => new WaterAccountUser() 
        { 
            WaterAccountID = waterAccountID,
            UserID = minimalDto.User.UserID,
            ClaimDate = DateTime.UtcNow,
            WaterAccountRoleID = minimalDto.WaterAccountRoleID
        }).ToList();


        var existingWaterAccountUsers = dbContext.WaterAccountUsers
            .Where(x => x.WaterAccountID == waterAccountID).ToList();

        addedUserIDs = waterAccountUserMinimalDtos
            .Where(x => !existingWaterAccountUsers.Select(y => y.UserID).Contains(x.User.UserID))
            .Select(x => x.UserID).ToList();


        var allInDatabase = dbContext.WaterAccountUsers;
        existingWaterAccountUsers.Merge(newAccountUsers, allInDatabase, (x, y) => x.WaterAccountID == y.WaterAccountID && x.UserID == y.UserID,
            (existing, updated) =>
            {
                existing.WaterAccountRoleID = updated.WaterAccountRoleID;
            });


        dbContext.SaveChanges();

        return GetByIDAsMinimalDto(dbContext, waterAccountID);
    }

    public static void BulkCreateWithListOfNames(QanatDbContext dbContext, List<fParcelLayerUpdateDifferencesInParcelsAssociatedWithAccount> uniqueAccountNameOwnerAddress, int geographyID)
    {
        var listOfAccountsToCreate = new List<WaterAccount>();
        var currentWaterAccountPiNs = GetCurrentWaterAccountPINs(dbContext);
        var parcelStagings = dbContext.ParcelStagings;

        uniqueAccountNameOwnerAddress.ForEach(uniqueAccount =>
        {
            var parcelStaging =
                parcelStagings.First(y => y.OwnerName == uniqueAccount.AccountName && y.OwnerAddress == uniqueAccount.OwnerAddress);

            var waterAccountPIN =
                GenerateAndVerifyWaterAccountPIN("ABCDEFGHIJKLMNOPQRSTUVWXYZ,0123456789",
                    currentWaterAccountPiNs);
            currentWaterAccountPiNs.Add(waterAccountPIN);

            listOfAccountsToCreate.Add(new WaterAccount()
            {
                GeographyID = geographyID,
                WaterAccountName = parcelStaging.OwnerName,
                CreateDate = DateTime.UtcNow,
                WaterAccountPIN = waterAccountPIN
            });
        });

        dbContext.WaterAccounts.AddRange(listOfAccountsToCreate);

        dbContext.SaveChanges();
    }

    public static List<string> GetCurrentWaterAccountPINs(QanatDbContext dbContext)
    {
        return dbContext.WaterAccounts.Select(x => x.WaterAccountPIN).ToList();
    }

    private static string GenerateAndVerifyWaterAccountPIN(string qanatConfigurationWaterAccountPINChars,
        List<string> currentAccountWaterAccountPINs)
    {
        var waterAccountPIN = GenerateWaterAccountPIN(qanatConfigurationWaterAccountPINChars);
        while (currentAccountWaterAccountPINs.Contains(waterAccountPIN))
        {
            waterAccountPIN = GenerateWaterAccountPIN(qanatConfigurationWaterAccountPINChars);
        }

        return waterAccountPIN;
    }

    private static string GenerateWaterAccountPIN(string qanatConfigurationWaterAccountPINChars)
    {
        var applicableWaterAccountPINChars = qanatConfigurationWaterAccountPINChars.Split(',');
        var random = new Random();

        return
            new string(Enumerable.Repeat(applicableWaterAccountPINChars[0], 3).Select(x => x[random.Next(x.Length)]).ToArray()) + "-" +
            new string(Enumerable.Repeat(applicableWaterAccountPINChars[1], 3).Select(x => x[random.Next(x.Length)]).ToArray());
    }


    public static WaterAccountSearchSummaryDto GetBySearchString(QanatDbContext dbContext, int geographyID, string searchString)
    {
        var searchResultLimit = 10;

        // the search query to find matches
        var matchedWaterAccountsNew = dbContext.WaterAccounts.AsNoTracking()
            .Include(x => x.Parcels)
            .Where(x => x.GeographyID == geographyID)
            .ToList()
            .Select(x => new
            {
                WaterAccountID = x.WaterAccountID,
                MatchedFields = new Dictionary<WaterAccountSearchMatchEnum, bool>()
                {
                    {WaterAccountSearchMatchEnum.WaterAccountName, !string.IsNullOrEmpty(x.WaterAccountName) ? x.WaterAccountName.Contains(searchString, StringComparison.OrdinalIgnoreCase) : false},
                    {WaterAccountSearchMatchEnum.ContactAddress, x.ContactAddress?.Contains(searchString, StringComparison.OrdinalIgnoreCase) ?? false},
                    {WaterAccountSearchMatchEnum.ContactName, x.ContactName?.Contains(searchString, StringComparison.OrdinalIgnoreCase) ?? false},
                    {WaterAccountSearchMatchEnum.WaterAccountNumber, x.WaterAccountNumber.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase)},
                    {WaterAccountSearchMatchEnum.APN, string.Join("|",x.Parcels.Select(y => y.ParcelNumber)).Contains(searchString)}
                }
            })
            .ToList();

        var matches = matchedWaterAccountsNew
            .Where(x => x.MatchedFields.ContainsValue(true)).ToList();

        var filteredMatches = matches
            .Take(searchResultLimit)
            .ToDictionary(x => x.WaterAccountID, x => x.MatchedFields);

        // the query that returns what we actually need from the water account
        var waterAccountsMatched = dbContext.WaterAccounts.AsNoTracking()
            .Include(x => x.Parcels)
            .Include(x => x.Geography)
            .Include(x => x.WaterAccountUsers)
            .ThenInclude(x => x.User)
            .Where(x => x.GeographyID == geographyID && filteredMatches.Keys.Contains(x.WaterAccountID)).ToList()
            .Select(x => new WaterAccountSearchResultWithMatchedFieldsDto()
            {
                WaterAccount = x.AsSearchResultDto(),
                MatchedFields = filteredMatches[x.WaterAccountID]
            }).ToList();

        return new WaterAccountSearchSummaryDto()
        {
            TotalResults = matches.Count,
            WaterAccountSearchResults = waterAccountsMatched
        };
    }

    public static List<ErrorMessage> ValidateMergeWaterAccounts(QanatDbContext dbContext, int primaryWaterAccountID,
        int secondaryWaterAccountID, MergeWaterAccountsDto mergeDto)
    {
        var results = new List<ErrorMessage>();

        var primaryWaterAccount = dbContext.WaterAccounts.AsNoTracking()
            .SingleOrDefault(x => x.WaterAccountID == primaryWaterAccountID);

        var secondaryWaterAccount = dbContext.WaterAccounts.AsNoTracking()
            .Include(x => x.Parcels)
                .ThenInclude(x => x.WaterAccountParcelParcels)
            .SingleOrDefault(x => x.WaterAccountID == secondaryWaterAccountID);

        if (primaryWaterAccount == null)
        {
            results.Add(new ErrorMessage()
            {
                Type = "WaterAccount",
                Message = $"Could not find the primary water account with the WaterAccountID of {primaryWaterAccountID}"
            });

            return results;
        }

        if (secondaryWaterAccount == null)
        {
            results.Add(new ErrorMessage()
            {
                Type = "WaterAccount",
                Message = $"Could not find the secondary water account with the WaterAccountID of {secondaryWaterAccountID}"
            });

            return results;
        }

        // JUST IN CASE!
        if (primaryWaterAccount.GeographyID != secondaryWaterAccount.GeographyID)
        {
            results.Add(new ErrorMessage()
            {
                Type = "WaterAccountGeographies",
                Message = $"The geographies of the merged water accounts didn't match."
            });
        }

        if (mergeDto.IsDeleteMerge)
        {
            return results;
        }

        // validate the effective date 
        var minEffectiveYear = secondaryWaterAccount.Parcels.SelectMany(x => x.WaterAccountParcelParcels)
            .MaxBy(x => x.EffectiveYear)?.EffectiveYear;

        if (!mergeDto.PrimaryReportingPeriodYear.HasValue)
        {
            results.Add(new ErrorMessage()
            {
                Type = "ReportingPeriod",
                Message = "The Reporting Period field is required for a Preserve Merge."
            });
        }
        else if (minEffectiveYear.HasValue && mergeDto.PrimaryReportingPeriodYear < minEffectiveYear)
        {
            results.Add(new ErrorMessage()
            {
                Type = "Effective Year",
                Message = "The selected Effective Year must be equal to or after the the latest Effective Years of the parcels' water account relationship. " +
                          $"For the {secondaryWaterAccount.WaterAccountName} Water Account, the earliest allowable year is {minEffectiveYear}."
            });
        }

        return results;
    }

    public static async Task<WaterAccountMinimalDto> MergeWaterAccounts(QanatDbContext dbContext, int primaryWaterAccountID,
        int secondaryWaterAccountID, MergeWaterAccountsDto mergeDto)
    {
        // get both water accounts

        var primaryWaterAccount = dbContext.WaterAccounts
            .Include(x => x.Parcels)
            .Include(x => x.WaterAccountParcelWaterAccounts).Include(x => x.WaterAccountUsers).Single(x => x.WaterAccountID == primaryWaterAccountID);
        var secondaryWaterAccount = dbContext.WaterAccounts
            .Include(x => x.Parcels)
            .Include(x => x.WaterAccountParcelWaterAccounts).Include(x => x.WaterAccountUsers).Single(x =>x.WaterAccountID == secondaryWaterAccountID);

        foreach (var parcel in secondaryWaterAccount.Parcels)
        {
            parcel.WaterAccountID = primaryWaterAccountID;
        }

        if (mergeDto.IsDeleteMerge)
        {
            // in a delete merge, we want to transfer any WaterAccountParcel records over to the primary water account
            // and then delete the water account
            foreach (var waterAccountParcelToTransfer in secondaryWaterAccount.WaterAccountParcelWaterAccounts)
            {
                waterAccountParcelToTransfer.WaterAccountID = primaryWaterAccountID;
            }

            dbContext.WaterAccountUsers.RemoveRange(secondaryWaterAccount.WaterAccountUsers);
            dbContext.WaterAccounts.Remove(secondaryWaterAccount);
        }
        else
        {
            // move the parcels from the secondary account into the primary account
            // make sure to set the EndDate to the effective date - 1 on the
            // parcel water account history on the old one
            var secondaryWaterAccountParcelIDs = secondaryWaterAccount.Parcels.Select(x => x.ParcelID).ToList();

            foreach (var parcelID in secondaryWaterAccountParcelIDs)
            {
                var parcelRelationshipsForParcelID = dbContext.WaterAccountParcels
                    .Where(x => x.ParcelID == parcelID).ToList();

                parcelRelationshipsForParcelID.ForEach(parcelRelationship =>
                {
                    // dirty transfer. Notes on these types of transfers on the UpdateWaterAccountParcels method below
                    if (parcelRelationship.EffectiveYear >= mergeDto.PrimaryReportingPeriodYear)
                    {
                        dbContext.WaterAccountParcels.Remove(parcelRelationship);
                    }
                    // clean transfer
                    else if (parcelRelationship.EndYear == null)
                    {
                        parcelRelationship.EndYear = mergeDto.PrimaryReportingPeriodYear;
                    }
                });

                var newWaterAccountParcel = new WaterAccountParcel()
                {
                    EffectiveYear = mergeDto.PrimaryReportingPeriodYear.Value,
                    WaterAccountID = primaryWaterAccount.WaterAccountID,
                    ParcelID = parcelID,
                    GeographyID = primaryWaterAccount.GeographyID,
                };
                dbContext.WaterAccountParcels.Add(newWaterAccountParcel);
            }
            
            // deactivate the secondary water account?
            //TODO: do we want to delete?
            secondaryWaterAccount.UpdateDate = DateTime.UtcNow;
        }

        await dbContext.SaveChangesAsync();

        await RemoveEmptyWaterAccountsFromUsers(dbContext, [primaryWaterAccountID, secondaryWaterAccountID]);

        return GetByIDAsMinimalDto(dbContext, primaryWaterAccountID);
    }

    public static List<ErrorMessage> ValidateAddOrphanedParcelToWaterAccount(QanatDbContext dbContext, int waterAccountID, int parcelID)
    {
        var errors = new List<ErrorMessage>();

        var waterAccount = dbContext.WaterAccounts.SingleOrDefault(x => x.WaterAccountID == waterAccountID);
        if (waterAccount == null)
        {
            errors.Add(new ErrorMessage() { Type = "Parcel", Message = $"Could not find the water account with the WaterAccountID of {waterAccountID}." });
        }

        var parcel = dbContext.Parcels.SingleOrDefault(x => x.ParcelID == parcelID);
        if (parcel == null)
        {
            errors.Add(new ErrorMessage() { Type = "Parcel", Message = $"Could not find the parcel with the ParcelID of {parcelID}." });
        } 
        else if (parcel.GeographyID != waterAccount.GeographyID)
        {
            errors.Add(new ErrorMessage() { Type = "Geography", Message = "The geography of the selected water account doesn't match the selected parcel." });
        } else if (parcel.WaterAccountID.HasValue)
        {
            errors.Add(new ErrorMessage() { Type = "Parcel", Message = "The selected parcel is already associated with a water account. This operation is intended only for orphaned parcels." });
        }

        return errors;
    }

    public static async Task<WaterAccountMinimalDto> AddOrphanedParcelToWaterAccount(QanatDbContext dbContext, int waterAccountID, int parcelID)
    {
        var parcel = dbContext.Parcels.Single(x => x.ParcelID == parcelID);
        parcel.WaterAccountID = waterAccountID;

        var waterAccount = dbContext.WaterAccounts.Single(x => x.WaterAccountID == waterAccountID);

        var waterAccountParcel = new WaterAccountParcel()
        {
            WaterAccountID = waterAccountID,
            ParcelID = parcelID,
            EffectiveYear = DateTime.UtcNow.Year,
            GeographyID = waterAccount.GeographyID
        };

        await dbContext.WaterAccountParcels.AddAsync(waterAccountParcel);
        await dbContext.SaveChangesAsync();

        return GetByIDAsMinimalDto(dbContext, waterAccountID);
    }


    /**
     * SMG: I realize this method is a bit wordy and not algorithmically great, but this is the way I understood it the clearest.
     * The queries aren't big so I'm really not concerned about performance.
     *
     * There are a few things we need to consider when transferring parcels to and from water accounts.
     * 1. The effective date is CRITICAL 
     * 2. Removal of a parcel from a water account
     *  a. Clean: If the form submitted EffectiveDate is greater than the EffectiveDate on the current ownership we can simply set the EndDate
     *  b. Dirty: If the form submitted EffectiveDate is less    than the EffectiveDate on the current ownership can remove the ownership record altogether
     * 3. Transfer from another water account or adding an orphaned parcel
     *  a. Create a new record of ownership starting at the effective date on form submission
     *  b. Clean: (i.e.: the new effective date is past the current ownership on another water account) we can update the EndDate on that record
     *  c. Dirty: (i.e.: backdating ownership), we need to clean up any ownership records that are beyond that backdated EffectiveDate
     *
     * RL 11/21/23: Updated rules: Since we can no longer set an effective year in the past, not really sure how much of the dirty case we will encounter.
     * We are also adding a WaterAccountID to the Parcel table to reflect the current WaterAccount a parcel belongs to
     * We also need to set Parcel Status of Assigned or Unassigned
     * Steps should be simpler:
     * 1. Get existing parcels for this water account if any and set WaterAccountID to null for ones that are not part of the parcelIDs passed in
     * 2. Set an end year for WaterAccountParcel records for those removed parcels
     * 3. For the parcelIDs passed in, set the WaterAccountID to this WaterAccountID
     * 4. Add WaterAccountParcel records if necessary; if already exists no need for one, if not we need to check if it already belongs to another WaterAccount.  If it belongs to another WaterAccount we need to set the end year to the effective year and then add a new record to this WaterAccountParcel
     *
     * RL: 6/20/24: New rule: if a water account has zero parcels after a merge/consolidate, it will be removed from all users who can view/manage that water account
     */
    public static async Task<WaterAccountMinimalDto> UpdateWaterAccountParcels(QanatDbContext dbContext, int waterAccountID, int effectiveYear, List<int> parcelIDs, int userID)
    {
        var waterAccount = dbContext.WaterAccounts.Include(x => x.WaterAccountParcelWaterAccounts).Single(x => x.WaterAccountID == waterAccountID);
        var parcelsCurrentlyAssociatedWithWaterAccount = dbContext.Parcels.Where(x => x.WaterAccountID == waterAccountID).ToList();

        var parcelsThatShouldBeAssociatedWithWaterAccount = dbContext.Parcels.Include(x => x.WaterAccountParcelParcels).Where(x => parcelIDs.Contains(x.ParcelID)).ToList();
        // REMOVED PARCELS
        var removedParcels = parcelsCurrentlyAssociatedWithWaterAccount.Where(x => !parcelIDs.Contains(x.ParcelID)).ToList();
        foreach (var parcel in removedParcels)
        {
            var removedParcelRelationships = dbContext.WaterAccountParcels.Where(x => x.WaterAccountID == waterAccountID && x.ParcelID == parcel.ParcelID).ToList();
            // dirty transfer
            dbContext.WaterAccountParcels.RemoveRange(removedParcelRelationships.Where(x => x.EffectiveYear >= effectiveYear));
            // clean transfer
            foreach (var waterAccountParcel in removedParcelRelationships.Where(x => x.EffectiveYear <= effectiveYear && x.EndYear == null))
            {
                waterAccountParcel.EndYear = effectiveYear;
            }

            // remove them from this water account and set the parcel to Unassigned
            parcel.WaterAccountID = null;
            parcel.ParcelStatusID = (int)ParcelStatusEnum.Unassigned;

            // we also need to add a ParcelHistory record
            var newParcelHistoryRecord = ParcelHistories.CreateNew(parcel, userID, effectiveYear);
            await dbContext.ParcelHistories.AddAsync(newParcelHistoryRecord);
        }

        // ADDED OR EXISTING PARCELS
        var parcelIDsCurrentlyAssociatedWithWaterAccount = parcelsCurrentlyAssociatedWithWaterAccount.Select(x => x.ParcelID).ToList();
        foreach (var parcel in parcelsThatShouldBeAssociatedWithWaterAccount)
        {
            // set the WaterAccountID and ParcelStatus to Assigned for the parcels that should be associated to this water account
            parcel.WaterAccountID = waterAccountID;
            parcel.ParcelStatusID = (int)ParcelStatusEnum.Assigned;

            // leave the existing relationships alone
            if (parcelIDsCurrentlyAssociatedWithWaterAccount.Contains(parcel.ParcelID))
            {
                continue;
            }

            // adjust the existing relationships, (e.g. remove if past the effective date or update EndDate if accurate)
            // dirty transfer
            dbContext.WaterAccountParcels.RemoveRange(parcel.WaterAccountParcelParcels.Where(x => x.EffectiveYear >= effectiveYear));
            // clean transfer
            foreach (var waterAccountParcel in parcel.WaterAccountParcelParcels.Where(x => x.EffectiveYear <= effectiveYear && x.EndYear == null))
            {
                waterAccountParcel.EndYear = effectiveYear;
            }

            // add the new record
            var newRecord = new WaterAccountParcel()
            {
                GeographyID = waterAccount.GeographyID,
                EffectiveYear = effectiveYear,
                WaterAccountID = waterAccountID,
                ParcelID = parcel.ParcelID
            };
            await dbContext.WaterAccountParcels.AddAsync(newRecord);
            
            // we also need to add a ParcelHistory record
            var newParcelHistoryRecord = ParcelHistories.CreateNew(parcel, userID, effectiveYear);
            await dbContext.ParcelHistories.AddAsync(newParcelHistoryRecord);
        }

        await dbContext.SaveChangesAsync();

        await RemoveEmptyWaterAccountsFromUsers(dbContext, [waterAccountID]);

        return GetByIDAsMinimalDto(dbContext, waterAccountID);
    }

    public static List<ErrorMessage> ValidateUpdateWaterAccountParcels(QanatDbContext dbContext, int waterAccountID, UpdateWaterAccountParcelsDto dto)
    {
        var results = new List<ErrorMessage>();

        var waterAccount = dbContext.WaterAccounts.AsNoTracking()
            .Include(x => x.Parcels)
            .Single(x => x.WaterAccountID == waterAccountID);

        // validate the effective date
        var waterAccountParcelIDs = waterAccount.Parcels.Select(x => x.ParcelID).ToList();

        var addedParcelIDs = dto.ParcelIDs.Where(x => !waterAccountParcelIDs.Contains(x)).ToList();
        var minEffectiveYearForAddedParcels = dbContext.WaterAccountParcels
            .Where(x => addedParcelIDs.Contains(x.ParcelID)).ToList()
            .MaxBy(x => x.EffectiveYear)?.EffectiveYear;

        var minEffectiveYearForRemovedParcels = waterAccount.Parcels.Where(x => !dto.ParcelIDs.Contains(x.ParcelID))
            .SelectMany(x => x.WaterAccountParcelParcels).ToList()
            .MaxBy(x => x.EffectiveYear)?.EffectiveYear;

        var minEffectiveYear = minEffectiveYearForAddedParcels > minEffectiveYearForRemovedParcels ? minEffectiveYearForAddedParcels : minEffectiveYearForRemovedParcels;

        if (minEffectiveYear.HasValue && dto.EffectiveYear < minEffectiveYear)
        {
            results.Add(new ErrorMessage()
            {
                Type = "Effective Year",
                Message = "The selected Effective Year must be equal to or after the the latest Effective Years of the parcels' water account relationship. " +
                          $"For the selected parcels, the earliest allowable year is {minEffectiveYear}."
            });
        }

        return results;
    }

    public static async Task DeleteWaterAccount(QanatDbContext dbContext, int waterAccountID)
    {
        await dbContext.Parcels.Where(x => x.WaterAccountID == waterAccountID)
            .ExecuteUpdateAsync(x => x
                .SetProperty(y => y.WaterAccountID, y => null)
                .SetProperty(y => y.ParcelStatusID, y => (int)ParcelStatusEnum.Unassigned));

        await dbContext.ParcelHistories.Where(x => x.WaterAccountID == waterAccountID).ExecuteDeleteAsync();
        await dbContext.WaterAccountParcels.Where(x => x.WaterAccountID == waterAccountID).ExecuteDeleteAsync();
        await dbContext.WaterAccountUsers.Where(x => x.WaterAccountID == waterAccountID).ExecuteDeleteAsync();
        await dbContext.WaterAccountUserStagings.Where(x => x.WaterAccountID == waterAccountID).ExecuteDeleteAsync();
        await dbContext.WaterAccountCustomAttributes.Where(x => x.WaterAccountID == waterAccountID).ExecuteDeleteAsync();
        await dbContext.WaterAccountReconciliations.Where(x => x.WaterAccountID == waterAccountID).ExecuteDeleteAsync();
        await dbContext.WaterAccounts.Where(x => x.WaterAccountID == waterAccountID).ExecuteDeleteAsync();
    }

    public static async Task<WaterAccount> CreateWaterAccount(QanatDbContext dbContext, int geographyID, WaterAccountCreateDto waterAccountDto)
    {
        return await CreateWaterAccount(dbContext, geographyID, waterAccountDto.WaterAccountName, waterAccountDto.ContactName,
            waterAccountDto.ContactAddress);
    }

    public static async Task<WaterAccount> CreateWaterAccount(QanatDbContext dbContext, int geographyID, string waterAccountName, string contactName, string contactAddress)
    {
        var waterAccountPIN =
            GenerateAndVerifyWaterAccountPIN("ABCDEFGHIJKLMNOPQRSTUVWXYZ,0123456789",
                GetCurrentWaterAccountPINs(dbContext));
        var waterAccount = new WaterAccount
        {
            GeographyID = geographyID,
            CreateDate = DateTime.UtcNow,
            ContactName = contactName,
            ContactAddress = contactAddress,
            WaterAccountPIN = waterAccountPIN
        };
        await dbContext.WaterAccounts.AddAsync(waterAccount);
        await dbContext.SaveChangesAsync();
        await dbContext.Entry(waterAccount).ReloadAsync();
        return waterAccount;
    }

    public static List<ErrorMessage> ValidateWaterAccountName(QanatDbContext dbContext, int geographyID,
        string waterAccountName)
    {
        var results = new List<ErrorMessage>();
        var waterAccountWithSameNameAndGeography = dbContext.WaterAccounts.SingleOrDefault(x =>
            x.WaterAccountName == waterAccountName && x.GeographyID == geographyID);
        if (waterAccountWithSameNameAndGeography != null)
        {
            results.Add(new ErrorMessage()
            {
                Type = "WaterAccount",
                Message =
                    $"There is already another water account named {waterAccountName} in this geography."
            });
        }

        return results;
    }

    public static List<ZoneDisplayDto> GetZonesForParcel(WaterAccount waterAccount, Dictionary<int, ZoneDisplayDto> zonesDict, IEnumerable<int> allocationZoneIDs)
    {
        var zoneIDs = new List<int>();
        foreach (var parcel in waterAccount.Parcels)
        {
            var parcelZoneIDs = parcel.ParcelZones
                .Where(parcelZone => allocationZoneIDs.Contains(parcelZone.ZoneID))
                .Select(parcelZone => parcelZone.ZoneID);

            zoneIDs.AddRange(parcelZoneIDs);
        }

        return zoneIDs.Distinct().Select(x => zonesDict[x]).ToList();
    }

    public static List<WaterAccountRequestChangesDto> ListByGeographyIDAndUserIDAsWaterAccountRequestChangesDto(QanatDbContext dbContext, int geographyID, int userID)
    {
        var geographyAllocationPlanConfigurationZoneGroupID = GeographyAllocationPlanConfigurations
            .GetByGeographyID(dbContext, geographyID)?.ZoneGroupID;

        var ownedWaterAccountIDs = dbContext.WaterAccountUsers.AsNoTracking()
            .Where(x => x.UserID == userID && x.WaterAccountRoleID == WaterAccountRole.WaterAccountHolder.WaterAccountRoleID)
            .Select(x => x.WaterAccountID).ToList();

        return dbContext.WaterAccounts.AsNoTracking()
            .Include(x => x.WaterAccountParcelWaterAccounts)
            .ThenInclude(x => x.Parcel)
            .ThenInclude(x => x.ParcelZones)
            .ThenInclude(x => x.Zone)
            .Where(x => x.GeographyID == geographyID && ownedWaterAccountIDs.Contains(x.WaterAccountID))
            .Select(x => new WaterAccountRequestChangesDto()
            {
                WaterAccountID = x.WaterAccountID,
                WaterAccountName = x.WaterAccountName,
                WaterAccountNumber = x.WaterAccountNumber,
                ContactName = x.ContactName,
                ContactAddress = x.ContactAddress,
                Parcels = x.WaterAccountParcelWaterAccounts.Where(x => x.EndYear == null)
                    .Select(y => new WaterAccountRequestChangesParcelDto()
                    {
                        ParcelID = y.ParcelID,
                        ParcelNumber = y.Parcel.ParcelNumber,
                        WaterAccountID = x.WaterAccountID,
                        WaterAccountName = x.WaterAccountName,
                        AllocationZone = !geographyAllocationPlanConfigurationZoneGroupID.HasValue || y.Parcel.ParcelZones.All(z => z.Zone.ZoneGroupID != geographyAllocationPlanConfigurationZoneGroupID)
                            ? null
                            : y.Parcel.ParcelZones.Single(z => z.Zone.ZoneGroupID == geographyAllocationPlanConfigurationZoneGroupID).Zone.AsDisplayDto()
                    }).ToList()
            })
            .OrderBy(x => x.Parcels.Count > 0 ? 1 : 2).ThenBy(x => x.WaterAccountName)
            .ToList();
    }

    public static List<ErrorMessage> ValidateRequestedWaterAccountChanges(QanatDbContext dbContext, int geographyID, int userID, WaterAccountParcelsRequestChangesDto requestDto)
    {
        var errors = new List<ErrorMessage>();

        // check all parcels are within the same zone for each water account
        var waterAccountsWithInvalidZones = requestDto.WaterAccounts
            .Where(x => x.Parcels.DistinctBy(y => y.AllocationZone.ZoneID).Count() > 1)
            .ToList();

        if (waterAccountsWithInvalidZones.Any())
        {
            errors.Add(new ErrorMessage() { Type = "Zone", Message = "Request includes water accounts with invalid zones. Please ensure all parcels are within the same zone for each water account. " });
        }

        // check all parcels are within the expected geography (just in case)
        var parcelIDs = requestDto.WaterAccounts.SelectMany(x => x.Parcels).AsEnumerable()
            .Select(x => x.ParcelID).ToList();
        parcelIDs.AddRange(requestDto.ParcelsToRemove.Select(x => x.ParcelID).ToList());

        var parcels = dbContext.Parcels.AsNoTracking()
            .Where(x => parcelIDs.Contains(x.ParcelID)).ToList();

        if (parcels.Any(x => x.GeographyID != geographyID))
        {
            errors.Add(new ErrorMessage() { Type = "Parcel", Message = "Request includes parcels that do not match the specified geography." });
        }

        // check all water accounts are within the expected geography (just in case)
        var waterAccountIDs = requestDto.WaterAccounts.Select(x => x.WaterAccountID).ToList();
        var waterAccounts = dbContext.WaterAccounts.AsNoTracking()
            .Where(x => waterAccountIDs.Contains(x.WaterAccountID)).ToList();
        if (waterAccounts.Count != waterAccountIDs.Count)
        {
            errors.Add(new ErrorMessage() { Type = "Water Account", Message = "Request includes invalid water account IDs." });
        }
        else if (waterAccounts.Any(x => x.GeographyID != geographyID))
        {
            errors.Add(new ErrorMessage() { Type = "Water Account", Message = "Request includes water accounts that do not match the specified geography." });
        }

        // check user has permission to edit all water accounts
        var waterAccountUserIDByWaterAccountID = dbContext.WaterAccountUsers.AsNoTracking()
            .Where(x => x.UserID == userID && waterAccountIDs.Contains(x.WaterAccountID) && x.WaterAccountRoleID == WaterAccountRole.WaterAccountHolder.WaterAccountRoleID)
            .ToDictionary(x => x.WaterAccountID, y => y.WaterAccountUserID);

        foreach (var waterAccount in requestDto.WaterAccounts)
        {
            if (!waterAccountUserIDByWaterAccountID.ContainsKey(waterAccount.WaterAccountID))
            {
                errors.Add(new ErrorMessage() { Type = "Water Account User", Message = "This user does not have permission to request changes for this water account." });
            }
        }

        return errors;
    }

    public static async Task ApplyRequestedWaterAccountChanges(QanatDbContext dbContext, int geographyID, int userID, WaterAccountParcelsRequestChangesDto requestDto)
    {
        // remove current WaterAccountParcels
        var parcelIDs = requestDto.WaterAccounts.SelectMany(x => x.Parcels.Select(y => y.ParcelID)).ToList();

        var parcelIDsToInactivate = requestDto.ParcelsToRemove.Select(x => x.ParcelID).ToList();
        parcelIDs.AddRange(parcelIDsToInactivate);

        await dbContext.WaterAccountParcels
            .Where(x => x.GeographyID == geographyID && parcelIDs.Contains(x.ParcelID)).ExecuteDeleteAsync();

        // create new WaterAccountParcels and update existing Parcels' WaterAccountIDs
        var geography = Geographies.GetByID(dbContext, geographyID);

        var parcelByParcelID = dbContext.Parcels.Where(x => x.GeographyID == geographyID && parcelIDs.Contains(x.ParcelID))
            .ToDictionary(x => x.ParcelID);

        var latestParcelHistoryByParcelID = dbContext.ParcelHistories.AsNoTracking()
            .Where(x => parcelIDs.Contains(x.ParcelID))
            .GroupBy(x => x.ParcelID)
            .ToDictionary(x => x.Key, x => x.MaxBy(y => y.UpdateDate));

        var modifiedParcels = parcelIDsToInactivate.Where(x => parcelByParcelID.ContainsKey(x))
            .Select(x => parcelByParcelID[x]).ToList();

        var newWaterAccountParcels = new List<WaterAccountParcel>();
        foreach (var waterAccount in requestDto.WaterAccounts)
        {
            foreach (var parcel in waterAccount.Parcels)
            {
                if (!parcelByParcelID.ContainsKey(parcel.ParcelID)) continue;

                // add new WaterAccountParcel record
                newWaterAccountParcels.Add(new WaterAccountParcel()
                    {
                        GeographyID = geographyID,
                        WaterAccountID = waterAccount.WaterAccountID,
                        ParcelID = parcel.ParcelID,
                        EffectiveYear = geography.StartYear
                    });

                // update WaterAccount col on Parcel table
                var existingParcel = parcelByParcelID[parcel.ParcelID];
                existingParcel.WaterAccountID = waterAccount.WaterAccountID;

                // determine whether a new ParcelHistory record should be added
                var latestParcelHistory = latestParcelHistoryByParcelID.ContainsKey(parcel.ParcelID)
                    ? latestParcelHistoryByParcelID[parcel.ParcelID]
                    : null;

                if (existingParcel.WaterAccountID != latestParcelHistory?.WaterAccountID || latestParcelHistory?.EffectiveYear != geography.StartYear)
                {
                    modifiedParcels.Add(existingParcel);
                }
            }
        }
        dbContext.WaterAccountParcels.AddRange(newWaterAccountParcels);
        
        // set removed parcels to unassigned
        var parcelsToInactivate = dbContext.Parcels.Where(x => parcelIDsToInactivate.Contains(x.ParcelID)).ToList();
        foreach (var parcel in parcelsToInactivate)
        {
            parcel.WaterAccountID = null;
            parcel.ParcelStatusID = ParcelStatus.Unassigned.ParcelStatusID;
        }

        // add new ParcelHistory records
        dbContext.ParcelHistories.AddRange(modifiedParcels.Select(x => ParcelHistories.CreateNew(x, userID, geography.StartYear)).ToList());
        
        await dbContext.SaveChangesAsync();

        // finally if any of the water accounts get into a state of not having any parcels, we need to remove it from the water account users table
        var waterAccountIDs = requestDto.WaterAccounts.Select(x => x.WaterAccountID).ToList();
        await RemoveEmptyWaterAccountsFromUsers(dbContext, waterAccountIDs);
    }

    private static async Task RemoveEmptyWaterAccountsFromUsers(QanatDbContext dbContext, List<int> waterAccountIDs)
    {
        var waterAccountIDsWithNoParcels = dbContext.WaterAccounts.Include(x => x.Parcels).Where(x => waterAccountIDs.Contains(x.WaterAccountID) && !x.Parcels.Any())
            .Select(x => x.WaterAccountID).ToList();
        await dbContext.WaterAccountUsers.Where(x => waterAccountIDsWithNoParcels.Contains(x.WaterAccountID))
            .ExecuteDeleteAsync();
    }

    public static async Task<WaterAccountMinimalDto> CreateWaterAccountFromSuggestion(QanatDbContext dbContext, int geographyID, CreateWaterAccountFromSuggestionDto dto, int userID)
    {
        var waterAccount = await CreateWaterAccount(dbContext, geographyID, dto.WaterAccountName,
            dto.ContactName, dto.ContactAddress);
        var waterAccountMinimalDto = await UpdateWaterAccountParcels(dbContext, waterAccount.WaterAccountID,
            dto.EffectiveYear, dto.ParcelIDList, userID);
        return waterAccountMinimalDto;
    }
}
