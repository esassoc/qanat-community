using Microsoft.EntityFrameworkCore;
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
        var hasPermission = user.RoleID == (int)RoleEnum.SystemAdmin;

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

    public static async Task<List<WaterAccountIndexGridDto>> ListByGeographyIDAsIndexGridDtos(QanatDbContext dbContext, int geographyID)
    {
        var currentYear = DateTime.UtcNow.Year;
        var currentReportingPeriod = await ReportingPeriods.GetByGeographyIDAndYearAsync(dbContext, geographyID, currentYear);

        var waterAccountIndexGridDtos = await dbContext.WaterAccounts.AsNoTracking()
            .Include(x => x.WaterAccountParcels).ThenInclude(x => x.Parcel)
            .Include(x => x.WaterAccountUsers).ThenInclude(x => x.User)
            .Include(x => x.WaterAccountContact)
            .Where(x => x.GeographyID == geographyID)
            .Select(x => x.AsIndexGridDto(currentReportingPeriod.ReportingPeriodID))
            .ToListAsync();

        return waterAccountIndexGridDtos;
    }

    public static async Task<List<WaterAccountIndexGridDto>> ListByGeographyIDAndUserIDAsIndexGridDtos(QanatDbContext dbContext, int geographyID, int userID)
    {
        var currentYear = DateTime.UtcNow.Year;
        var currentReportingPeriod = await ReportingPeriods.GetByGeographyIDAndYearAsync(dbContext, geographyID, currentYear);

        var waterAccountIndexGridDtos = await dbContext.WaterAccounts.AsNoTracking()
            .Include(x => x.WaterAccountParcels).ThenInclude(x => x.Parcel)
            .Include(x => x.WaterAccountUsers).ThenInclude(x => x.User)
            .Include(x => x.WaterAccountContact)
            .Where(x => x.GeographyID == geographyID && x.WaterAccountUsers.Any(y => y.UserID == userID))
            .Select(x => x.AsIndexGridDto(currentReportingPeriod.ReportingPeriodID))
            .ToListAsync();

        return waterAccountIndexGridDtos;
    }

    public static async Task<List<WaterAccountIndexGridDto>> ListByGeographyIDAndYearAsIndexGridDtos(QanatDbContext dbContext, int geographyID, int year)
    {
        var reportingPeriod = await ReportingPeriods.GetByGeographyIDAndYearAsync(dbContext, geographyID, year);
        var waterAccountIndexGridDtos = await dbContext.WaterAccounts.AsNoTracking()
            .Include(x => x.WaterAccountParcels).ThenInclude(x => x.Parcel)
            .Include(x => x.WaterAccountUsers).ThenInclude(x => x.User)
            .Include(x => x.WaterAccountContact)
            .Where(x => x.GeographyID == geographyID)
            .Select(x => x.AsIndexGridDto(reportingPeriod.ReportingPeriodID))
            .ToListAsync();

        return waterAccountIndexGridDtos;
    }

    public static WaterAccountDto GetByIDAsDto(QanatDbContext dbContext, int waterAccountID, int? reportingPeriodID = null)
    {
        var waterAccount = dbContext.WaterAccounts.AsNoTracking()
            .Include(x => x.Geography)
            .Include(x => x.WaterAccountContact)
            .Include(x => x.WaterAccountUsers).ThenInclude(x => x.User)
            .SingleOrDefault(x => x.WaterAccountID == waterAccountID);
        if (!reportingPeriodID.HasValue)
        {
            var defaultReportingPeriod = dbContext.ReportingPeriods.AsNoTracking()
                .FirstOrDefault(x => x.GeographyID == waterAccount.GeographyID && x.IsDefault);
            reportingPeriodID = defaultReportingPeriod?.ReportingPeriodID ?? dbContext.ReportingPeriods
                .First(x => x.GeographyID == waterAccount.GeographyID).ReportingPeriodID;
        }

        var parcels = dbContext.WaterAccountParcels.AsNoTracking()
            .Include(x => x.Parcel).ThenInclude(x => x.UsageLocations)
            .Where(x => x.WaterAccountID == waterAccountID && x.ReportingPeriodID == reportingPeriodID)
            .Select(x => x.Parcel).ToList();
        return waterAccount.AsWaterAccountDto(reportingPeriodID.Value, parcels);
    }

    public static WaterAccountMinimalDto GetByIDAsMinimalDto(QanatDbContext dbContext, int waterAccountID)
    {
        return dbContext.WaterAccounts.Include(x => x.Geography).AsNoTracking()
            .Single(x => x.WaterAccountID == waterAccountID).AsWaterAccountMinimalDto();
    }

    public static WaterAccountDisplayDto GetByIDAsDisplayDto(QanatDbContext dbContext, int waterAccountID)
    {
        return dbContext.WaterAccounts.AsNoTracking()
            .SingleOrDefault(x => x.WaterAccountID == waterAccountID).AsDisplayDto();
    }

    public static WaterAccountDto UpdateWaterAccount(QanatDbContext dbContext, int waterAccountID, WaterAccountUpdateDto waterAccountUpdateDto)
    {
        var waterAccount = dbContext.WaterAccounts
            .Single(x => x.WaterAccountID == waterAccountID);

        waterAccount.WaterAccountName = waterAccountUpdateDto.WaterAccountName;
        waterAccount.Notes = waterAccountUpdateDto.Notes ?? waterAccount.Notes;
        waterAccount.UpdateDate = DateTime.UtcNow;

        dbContext.SaveChanges();
        dbContext.Entry(waterAccount).Reload();
        return GetByIDAsDto(dbContext, waterAccountID);
    }

    public static List<ErrorMessage> ValidateUpdateWaterAccountContact(QanatDbContext dbContext, int waterAccountID, int waterAccountContactID)
    {
        var errors = new List<ErrorMessage>();

        var waterAccount = dbContext.WaterAccounts.AsNoTracking().SingleOrDefault(x => x.WaterAccountID == waterAccountID);
        if (waterAccount == null)
        {
            errors.Add(new ErrorMessage() { Type = "Water Account Contact", Message = "The provided water account does not exist." });
        }

        var waterAccountContact = dbContext.WaterAccountContacts.AsNoTracking()
            .SingleOrDefault(x => x.WaterAccountContactID == waterAccountContactID && x.GeographyID == waterAccount.GeographyID);

        if (waterAccountContact == null)
        {
            errors.Add(new ErrorMessage() { Type = "Water Account Contact", Message = "The provided contact does not exist within this geography."});
        }

        return errors;
    }

    public static WaterAccountDto UpdateWaterAccountContact(QanatDbContext dbContext, int waterAccountID, int? waterAccountContactID)
    {
        var waterAccount = dbContext.WaterAccounts.Single(x => x.WaterAccountID == waterAccountID);

        waterAccount.WaterAccountContactID = waterAccountContactID;
        waterAccount.UpdateDate = DateTime.UtcNow;

        dbContext.SaveChanges();
        dbContext.Entry(waterAccount).Reload();
        return GetByIDAsDto(dbContext, waterAccountID);
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

    public static WaterAccountSearchSummaryDto GetBySearchString(QanatDbContext dbContext, WaterAccountSearchDto waterAccountSearchDto, UserDto user)
    {
        const int searchResultLimit = 10;

        IQueryable<WaterAccount> waterAccounts;

        if (user.RoleID == (int)RoleEnum.SystemAdmin)
        {
            waterAccounts = dbContext.WaterAccounts;
        }
        else
        {
            var managedGeographyIDs = dbContext.GeographyUsers.AsNoTracking()
                .Where(x => x.UserID == user.UserID && x.GeographyRoleID == GeographyRole.WaterManager.GeographyRoleID)
                .Select(x => x.GeographyID).ToList();

            waterAccounts = dbContext.WaterAccounts
                .Include(x => x.WaterAccountUsers)
                .Where(x => managedGeographyIDs.Contains(x.GeographyID) || x.WaterAccountUsers.Any(y => y.UserID == user.UserID));
        }

        var waterAccountsToFilter = waterAccountSearchDto.GeographyID.HasValue ? waterAccounts.Where(x => x.GeographyID == waterAccountSearchDto.GeographyID.Value) : waterAccounts;
        var matchedWaterAccounts = waterAccountsToFilter.AsNoTracking()
                .Include(x => x.Parcels)
                .Include(x => x.WaterAccountContact)
            .Where(x =>
                x.WaterAccountName.Contains(waterAccountSearchDto.SearchString) ||
                x.WaterAccountContact != null && x.WaterAccountContact.FullAddress.Contains(waterAccountSearchDto.SearchString) ||
                x.WaterAccountContact != null && x.WaterAccountContact.ContactName.Contains(waterAccountSearchDto.SearchString) ||
                x.WaterAccountNumber.ToString().Contains(waterAccountSearchDto.SearchString) ||
                x.Parcels.Any(y => y.ParcelNumber.Contains(waterAccountSearchDto.SearchString)))
            .ToList();

            var waterAccountWithMatchedFieldsDtos = matchedWaterAccounts
                .Select(x => AsWaterAccountWithMatchedFieldsDto(waterAccountSearchDto.SearchString, x))
                .ToList();

        var waterAccountSearchResultWithMatchedFieldsDtos = waterAccountWithMatchedFieldsDtos
            .Take(searchResultLimit).ToList();

        if (waterAccountSearchDto.WaterAccountID != null && !waterAccountSearchResultWithMatchedFieldsDtos.Any(x => x.WaterAccount.WaterAccountID == waterAccountSearchDto.WaterAccountID))
        {
            var selectedWaterAccount = waterAccountsToFilter.Include(x => x.Parcels).AsNoTracking()
                .SingleOrDefault(x => x.WaterAccountID == waterAccountSearchDto.WaterAccountID);

            if (selectedWaterAccount != null)
            {
                var selectedWaterAccountDto = AsWaterAccountWithMatchedFieldsDto(waterAccountSearchDto.SearchString, selectedWaterAccount);
                waterAccountSearchResultWithMatchedFieldsDtos.Insert(0, selectedWaterAccountDto);
            }
        }

        return new WaterAccountSearchSummaryDto()
        {
            TotalResults = matchedWaterAccounts.Count(),
            WaterAccountSearchResults = waterAccountSearchResultWithMatchedFieldsDtos.Take(searchResultLimit).ToList()
        };
    }


    private static WaterAccountSearchResultWithMatchedFieldsDto AsWaterAccountWithMatchedFieldsDto(string searchString, WaterAccount waterAccount)
    {
        if (string.IsNullOrEmpty(searchString))
        {
            return new WaterAccountSearchResultWithMatchedFieldsDto()
            {
                WaterAccount = waterAccount.AsSearchResultDto(),
                MatchedFields = new Dictionary<WaterAccountSearchMatchEnum, bool>()
            };
        }

        return new WaterAccountSearchResultWithMatchedFieldsDto()
        {
            WaterAccount = waterAccount.AsSearchResultDto(),
            MatchedFields = new Dictionary<WaterAccountSearchMatchEnum, bool>()
            {
                {
                    WaterAccountSearchMatchEnum.WaterAccountName,
                    !string.IsNullOrEmpty(waterAccount.WaterAccountName) &&
                    waterAccount.WaterAccountName.Contains(searchString, StringComparison.OrdinalIgnoreCase)
                },
                {
                    WaterAccountSearchMatchEnum.ContactAddress,
                    waterAccount.WaterAccountContact?.FullAddress?.Contains(searchString, StringComparison.OrdinalIgnoreCase) ?? false
                },
                {
                    WaterAccountSearchMatchEnum.ContactName,
                    waterAccount.WaterAccountContact?.ContactName?.Contains(searchString, StringComparison.OrdinalIgnoreCase) ?? false
                },
                {
                    WaterAccountSearchMatchEnum.WaterAccountNumber,
                    waterAccount.WaterAccountNumber.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase)
                },
                {
                    WaterAccountSearchMatchEnum.APN,
                    string.Join("|", waterAccount.Parcels.Select(y => y.ParcelNumber)).Contains(searchString)
                }
            }
        };
    }

    public static async Task<List<ErrorMessage>> ValidateMergeWaterAccounts(QanatDbContext dbContext, int primaryWaterAccountID, int secondaryWaterAccountID, MergeWaterAccountsDto mergeDto, UserDto callingUser)
    {
        var results = new List<ErrorMessage>();

        if (primaryWaterAccountID == secondaryWaterAccountID)
        {
            results.Add(new ErrorMessage()
            {
                Type = "WaterAccount",
                Message = "Cannot merge the same water account"
            });

            return results;
        }

        var primaryWaterAccount = dbContext.WaterAccounts.AsNoTracking()
            .SingleOrDefault(x => x.WaterAccountID == primaryWaterAccountID);

        var secondaryWaterAccount = dbContext.WaterAccounts.AsNoTracking()
            .Include(x => x.Parcels).ThenInclude(x => x.WaterAccountParcels).ThenInclude(x => x.ReportingPeriod)
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

        // Validate reporting period.
        if (!mergeDto.ReportingPeriodID.HasValue)
        {
            results.Add(new ErrorMessage()
            {
                Type = "ReportingPeriodID",
                Message = "The Reporting Period field is required for a Preserve Merge."
            });
        }
        else
        {
            var reportingPeriod = await ReportingPeriods.GetAsync(dbContext, primaryWaterAccount.GeographyID, mergeDto.ReportingPeriodID.Value, callingUser);
            if (reportingPeriod == null)
            {
                results.Add(new ErrorMessage()
                {
                    Type = "ReportingPeriodID",
                    Message = "The Reporting Period could not be found."
                });
            }
        }

        return results;
    }

    public static async Task<WaterAccountMinimalDto> MergeWaterAccounts(QanatDbContext dbContext, int primaryWaterAccountID, int secondaryWaterAccountID, MergeWaterAccountsDto mergeDto, UserDto callingUser)
    {
        // get both water accounts
        var primaryWaterAccount = dbContext.WaterAccounts
            .Include(x => x.Parcels)
            .Include(x => x.WaterAccountParcels)
            .Include(x => x.WaterAccountUsers)
            .Single(x => x.WaterAccountID == primaryWaterAccountID);

        var secondaryWaterAccount = dbContext.WaterAccounts
            .Include(x => x.Parcels)
            .Include(x => x.WaterAccountParcels)
            .Single(x => x.WaterAccountID == secondaryWaterAccountID);

        var reportingPeriods = await dbContext.ReportingPeriods.AsNoTracking().Where(x => x.GeographyID == primaryWaterAccount.GeographyID).ToListAsync();

        if (mergeDto.IsDeleteMerge)
        {
            // if necessary, backfill WaterAccountParcel records for the parcels being merged
            // so that a record exists for each reporting period where the primary water account has parcel data
            // (this only applies to parcels that have belonged to the secondary water account since the "beginning of time")

            var parcelIDsToBeMerged = secondaryWaterAccount.WaterAccountParcels.Select(x => x.ParcelID)
                .Distinct().AsEnumerable();

            var earliestWaterAccountParcelsForParcelsToBeMerged = dbContext.Parcels.AsNoTracking()
                .Include(x => x.WaterAccountParcels).ThenInclude(x => x.ReportingPeriod)
                .Where(x => parcelIDsToBeMerged.Contains(x.ParcelID)).AsEnumerable()
                .Select(x => x.WaterAccountParcels.MinBy(y => y.ReportingPeriod.EndDate)).ToList();

            var earliestWaterAccountParcelsAssociatedWithSecondaryWaterAccount = earliestWaterAccountParcelsForParcelsToBeMerged
                .Where(x => x.WaterAccountID == secondaryWaterAccountID).ToList();

            var primaryWaterAccountReportingPeriodIDs = primaryWaterAccount.WaterAccountParcels.Select(x => x.ReportingPeriodID).ToList();
            var primaryWaterAccountReportingPeriods = reportingPeriods.Where(x => primaryWaterAccountReportingPeriodIDs.Contains(x.ReportingPeriodID)).ToList();

            var newWaterAccountParcels = new List<WaterAccountParcel>();
            foreach (var waterAccountParcel in earliestWaterAccountParcelsAssociatedWithSecondaryWaterAccount)
            {
                foreach (var reportingPeriod in primaryWaterAccountReportingPeriods.Where(x => x.EndDate < waterAccountParcel.ReportingPeriod.EndDate))
                {
                    newWaterAccountParcels.Add(new WaterAccountParcel()
                    {
                        WaterAccountID = primaryWaterAccountID,
                        ParcelID = waterAccountParcel.ParcelID,
                        GeographyID = primaryWaterAccount.GeographyID,
                        ReportingPeriodID = reportingPeriod.ReportingPeriodID
                    });
                }
            }
            await dbContext.WaterAccountParcels.AddRangeAsync(newWaterAccountParcels);

            // create history records
            var secondaryWaterAccountEarliestWaterAccountParcels = dbContext.Parcels.AsNoTracking()
                .Include(x => x.WaterAccountParcels).ThenInclude(x => x.ReportingPeriod)
                .Where(x => parcelIDsToBeMerged.Contains(x.ParcelID)).AsEnumerable()
                .Select(x => x.WaterAccountParcels.Where(y => y.WaterAccountID == secondaryWaterAccountID).MinBy(y => y.ReportingPeriod.EndDate))
                .ToList();

            var newParcelWaterAccountHistories = new List<ParcelWaterAccountHistory>();
            foreach (var waterAccountParcel in secondaryWaterAccountEarliestWaterAccountParcels)
            {
                newParcelWaterAccountHistories.Add(new ParcelWaterAccountHistory()
                {
                    GeographyID = primaryWaterAccount.GeographyID,
                    ParcelID = waterAccountParcel.ParcelID,
                    ReportingPeriodID = waterAccountParcel.ReportingPeriodID,
                    FromWaterAccountID = secondaryWaterAccountID,
                    FromWaterAccountNumber = secondaryWaterAccount.WaterAccountNumber,
                    FromWaterAccountName = secondaryWaterAccount.WaterAccountName,
                    ToWaterAccountID = primaryWaterAccountID,
                    ToWaterAccountNumber = primaryWaterAccount.WaterAccountNumber,
                    ToWaterAccountName = primaryWaterAccount.WaterAccountName,
                    Reason = $"Manually merged",
                    CreateUserID = callingUser.UserID,
                    CreateDate = DateTime.UtcNow,
                });
            }
            await dbContext.ParcelWaterAccountHistories.AddRangeAsync(newParcelWaterAccountHistories);

            // transfer secondary water account's WaterAccountParcel records to the primary water account
            // then delete the secondary water account

            foreach (var waterAccountParcelToTransfer in secondaryWaterAccount.WaterAccountParcels)
            {
                waterAccountParcelToTransfer.WaterAccountID = primaryWaterAccountID;
            }

            var parcelsToUpdate = dbContext.Parcels.Where(x => x.WaterAccountID == secondaryWaterAccountID);
            foreach (var parcel in parcelsToUpdate)
            {
                parcel.WaterAccountID = primaryWaterAccountID;
            }

            await dbContext.SaveChangesAsync();
            await DeleteWaterAccount(dbContext, secondaryWaterAccountID, callingUser);
        }
        else
        {
            var selectedReportingPeriod = reportingPeriods.Single(x => x.ReportingPeriodID == mergeDto.ReportingPeriodID!.Value);

            // Need to apply the changes for each WaterAccountParcel for the selected reportingPeriod and all those in the future.
            var reportingPeriodsToApplyMergeOn = reportingPeriods.Where(x => x.EndDate >= selectedReportingPeriod.EndDate).ToList();
            var reportingPeriodIDsToApplyMergeOn = reportingPeriodsToApplyMergeOn.Select(x => x.ReportingPeriodID).ToList();
            var waterAccountParcelsToMove = secondaryWaterAccount.WaterAccountParcels.Where(x => reportingPeriodIDsToApplyMergeOn.Contains(x.ReportingPeriodID));

            foreach (var waterAccountParcel in waterAccountParcelsToMove)
            {
                waterAccountParcel.WaterAccountID = primaryWaterAccountID;

                if (waterAccountParcel.ReportingPeriodID != selectedReportingPeriod.ReportingPeriodID) continue;

                var parcelWaterAccountHistory = new ParcelWaterAccountHistory()
                {
                    GeographyID = primaryWaterAccount.GeographyID,
                    ParcelID = waterAccountParcel.ParcelID,
                    ReportingPeriodID = waterAccountParcel.ReportingPeriodID,
                    FromWaterAccountID = secondaryWaterAccountID,
                    FromWaterAccountNumber = secondaryWaterAccount.WaterAccountNumber,
                    FromWaterAccountName = secondaryWaterAccount.WaterAccountName,
                    ToWaterAccountID = primaryWaterAccountID,
                    ToWaterAccountNumber = primaryWaterAccount.WaterAccountNumber,
                    ToWaterAccountName = primaryWaterAccount.WaterAccountName,
                    Reason = $"Manually merged.",
                    CreateUserID = callingUser.UserID,
                    CreateDate = DateTime.UtcNow,
                };

                await dbContext.ParcelWaterAccountHistories.AddAsync(parcelWaterAccountHistory);
            }

            secondaryWaterAccount.UpdateDate = DateTime.UtcNow;
            await dbContext.SaveChangesAsync();
        }

        await RemoveEmptyWaterAccountsFromUsers(dbContext, new List<int>() { primaryWaterAccountID, secondaryWaterAccountID });

        var parcelIDToReview = primaryWaterAccount.Parcels.Select(x => x.ParcelID).Union(secondaryWaterAccount.Parcels.Select(x => x.ParcelID)).Distinct().ToList();
        await ParcelHistories.MarkAsReviewedByParcelIDsAsync(dbContext, parcelIDToReview);

        return GetByIDAsMinimalDto(dbContext, primaryWaterAccountID);
    }

    public static async Task DeleteWaterAccount(QanatDbContext dbContext, int waterAccountID, UserDto callingUser)
    {
        await dbContext.Parcels.Where(x => x.WaterAccountID == waterAccountID)
            .ExecuteUpdateAsync(x => x
                .SetProperty(y => y.WaterAccountID, y => null)
                .SetProperty(y => y.ParcelStatusID, y => (int)ParcelStatusEnum.Unassigned));

        var waterAccountParcels = await dbContext.WaterAccountParcels.Include(x => x.WaterAccount).Where(x => x.WaterAccountID == waterAccountID).ToListAsync();
        foreach (var waterAccountParcel in waterAccountParcels)
        {
            var waterAccountParcelHistory = new ParcelWaterAccountHistory()
            {
                GeographyID = waterAccountParcel.GeographyID,
                ReportingPeriodID = waterAccountParcel.ReportingPeriodID,
                ParcelID = waterAccountParcel.ParcelID,
                FromWaterAccountID = waterAccountParcel.WaterAccountID,
                FromWaterAccountNumber = waterAccountParcel.WaterAccount.WaterAccountNumber,
                FromWaterAccountName = waterAccountParcel.WaterAccount.WaterAccountName,
                Reason = "Water Account Deleted",
                CreateUserID = callingUser.UserID,
                CreateDate = DateTime.UtcNow
            };

            dbContext.ParcelWaterAccountHistories.Add(waterAccountParcelHistory);
        }

        await dbContext.SaveChangesAsync();

        await dbContext.WaterAccountCoverCropStatuses.Where(x => x.WaterAccountID == waterAccountID).ExecuteDeleteAsync();
        await dbContext.StatementBatchWaterAccounts.Where(x => x.WaterAccountID == waterAccountID).ExecuteDeleteAsync();
        await dbContext.WaterAccountFallowStatuses.Where(x => x.WaterAccountID == waterAccountID).ExecuteDeleteAsync();
        await dbContext.WaterAccountParcels.Where(x => x.WaterAccountID == waterAccountID).ExecuteDeleteAsync();
        await dbContext.WaterAccountUsers.Where(x => x.WaterAccountID == waterAccountID).ExecuteDeleteAsync();
        await dbContext.WaterAccountUserStagings.Where(x => x.WaterAccountID == waterAccountID).ExecuteDeleteAsync();
        await dbContext.WaterAccountCustomAttributes.Where(x => x.WaterAccountID == waterAccountID).ExecuteDeleteAsync();
        await dbContext.WaterAccountReconciliations.Where(x => x.WaterAccountID == waterAccountID).ExecuteDeleteAsync();
        await dbContext.WaterAccounts.Where(x => x.WaterAccountID == waterAccountID).ExecuteDeleteAsync();
    }

    public static async Task<WaterAccount> CreateWaterAccount(QanatDbContext dbContext, int geographyID, WaterAccountUpsertDto waterAccountUpsertDto)
    {
        return await CreateImpl(dbContext, geographyID, waterAccountUpsertDto.WaterAccountName, waterAccountUpsertDto.ContactName, waterAccountUpsertDto.ContactEmail, waterAccountUpsertDto.ContactPhoneNumber, waterAccountUpsertDto.Address, waterAccountUpsertDto.SecondaryAddress, waterAccountUpsertDto.City, waterAccountUpsertDto.State, waterAccountUpsertDto.ZipCode, waterAccountUpsertDto.PrefersPhysicalCommunication);
    }

    public static async Task<WaterAccountMinimalDto> CreateWaterAccountFromSuggestion(QanatDbContext dbContext, int geographyID, CreateWaterAccountFromSuggestionDto dto, UserDto callingUser)
    {
        // todo: need to call geocoding api to split address from parcel to individual address fields
        var waterAccount = await CreateImpl(dbContext, geographyID, dto.WaterAccountName, dto.ContactName, null, null, dto.ContactAddress, null, null, null, null, null);
        await WaterAccountParcels.UpdateWaterAccountParcelByWaterAccountAndReportingPeriodAsync(dbContext, waterAccount.WaterAccountID, dto.ReportingPeriodID, dto.ParcelIDList, callingUser);
        return GetByIDAsMinimalDto(dbContext, waterAccount.WaterAccountID);
    }

    private static async Task<WaterAccount> CreateImpl(QanatDbContext dbContext, int geographyID, string waterAccountName, string contactName, string contactEmail, string contactPhoneNumber, string address, string secondaryAddress, string city, string state, string zipCode, bool? prefersPhysicalCommunication)
    {
        var waterAccountPIN = GenerateAndVerifyWaterAccountPIN("ABCDEFGHIJKLMNOPQRSTUVWXYZ,0123456789", GetCurrentWaterAccountPINs(dbContext));
        var waterAccount = new WaterAccount
        {
            GeographyID = geographyID,
            CreateDate = DateTime.UtcNow,
            WaterAccountPIN = waterAccountPIN,
            WaterAccountName = waterAccountName
        };

        var waterAccountContacts = await dbContext.WaterAccountContacts.AsNoTracking()
            .Where(x => x.GeographyID == geographyID && x.ContactName == contactName 
                                                     && ((x.Address == address && x.SecondaryAddress == secondaryAddress && x.City == city && x.State == state && x.ZipCode == zipCode) 
                                                         || x.FullAddress == address))
            .ToListAsync();

        if (waterAccountContacts.Any())
        {
            waterAccount.WaterAccountContactID = waterAccountContacts.First().WaterAccountContactID;
        }
        else
        {
            var waterAccountContact = new WaterAccountContact()
            {
                GeographyID = geographyID,
                ContactName = contactName,
                ContactEmail = contactEmail,
                ContactPhoneNumber = contactPhoneNumber,
                Address = address,
                SecondaryAddress = secondaryAddress,
                City = city,
                State = state,
                ZipCode = zipCode,
                PrefersPhysicalCommunication = prefersPhysicalCommunication ?? false
            };

            dbContext.WaterAccountContacts.Add(waterAccountContact);
            await dbContext.SaveChangesAsync();
            await dbContext.Entry(waterAccountContact).ReloadAsync();

            waterAccount.WaterAccountContactID = waterAccountContact.WaterAccountContactID;
        }

        await dbContext.WaterAccounts.AddAsync(waterAccount);
        await dbContext.SaveChangesAsync();
        await dbContext.Entry(waterAccount).ReloadAsync();

        return waterAccount;
    }

    public static List<ErrorMessage> ValidateWaterAccountName(QanatDbContext dbContext, int geographyID,
        string waterAccountName)
    {
        var results = new List<ErrorMessage>();
        if (waterAccountName == null)
        {
            return results;
        }

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

    public static List<ZoneDisplayDto> GetZonesForParcels(WaterAccount waterAccount, Dictionary<int, ZoneDisplayDto> zonesDict, IEnumerable<int> allocationZoneIDs, UserDto callingUser)
    {
        var zoneIDs = new List<int>();
        foreach (var parcel in waterAccount.Parcels)
        {
            var parcelZoneIDs = parcel.ParcelZones
                .Where(parcelZone => allocationZoneIDs.Contains(parcelZone.ZoneID))
                .Select(parcelZone => parcelZone.ZoneID);

            zoneIDs.AddRange(parcelZoneIDs);
        }

        callingUser.Flags.TryGetValue(Flag.IsSystemAdmin.FlagName, out var isSystemAdmin);

        var isWaterManager = false;
        callingUser.GeographyFlags.TryGetValue(waterAccount.GeographyID, out var geographyFlags);
        geographyFlags?.TryGetValue(Flag.HasManagerDashboard.FlagName, out isWaterManager);

        var callingUserIsAdminOrWaterManager = isSystemAdmin || isWaterManager;

        var zoneDisplayDtos = zoneIDs.Distinct().Select(x => zonesDict[x]);

        if (!callingUserIsAdminOrWaterManager)
        {
            zoneDisplayDtos = zoneDisplayDtos.Where(x => x.ZoneGroupDisplayToAccountHolders);
        }

        return zoneDisplayDtos.ToList();
    }

    public static List<WaterAccountRequestChangesDto> ListByGeographyIDAndUserIDAsWaterAccountRequestChangesDto(QanatDbContext dbContext, int geographyID, int userID)
    {
        var geographyAllocationPlanConfigurationZoneGroupID = GeographyAllocationPlanConfigurations
            .GetByGeographyID(dbContext, geographyID)?.ZoneGroupID;

        var ownedWaterAccountIDs = dbContext.WaterAccountUsers.AsNoTracking()
            .Where(x => x.UserID == userID && x.WaterAccountRoleID == WaterAccountRole.WaterAccountHolder.WaterAccountRoleID)
            .Select(x => x.WaterAccountID).ToList();

        var currentReportingPeriod = ReportingPeriods.GetByGeographyIDAndYearAsync(dbContext, geographyID, DateTime.UtcNow.Year).Result;

        return dbContext.WaterAccounts.AsNoTracking()
            .Include(x => x.WaterAccountContact)
            .Include(x => x.WaterAccountParcels)
            .ThenInclude(x => x.Parcel)
            .ThenInclude(x => x.ParcelZones)
            .ThenInclude(x => x.Zone)
            .Where(x => x.GeographyID == geographyID && ownedWaterAccountIDs.Contains(x.WaterAccountID))
            .ToList()
            .Select(x => new WaterAccountRequestChangesDto()
            {
                WaterAccountID = x.WaterAccountID,
                WaterAccountName = x.WaterAccountName,
                WaterAccountNumber = x.WaterAccountNumber,
                WaterAccountContact = x.WaterAccountContact?.AsDto(),
                Parcels = x.WaterAccountParcels.Where(x => x.ReportingPeriodID == currentReportingPeriod.ReportingPeriodID).ToList()
                    .Select(y => new WaterAccountRequestChangesParcelDto()
                    {
                        ParcelID = y.ParcelID,
                        ParcelNumber = y.Parcel.ParcelNumber,
                        WaterAccountID = x.WaterAccountID,
                        WaterAccountName = x.WaterAccountName,
                        AllocationZone = !geographyAllocationPlanConfigurationZoneGroupID.HasValue || y.Parcel.ParcelZones.All(z => z.Zone.ZoneGroupID != geographyAllocationPlanConfigurationZoneGroupID)
                            ? null
                            : y.Parcel.ParcelZones.Single(z => z.Zone.ZoneGroupID == geographyAllocationPlanConfigurationZoneGroupID).Zone.AsDisplayDto()
                    }).Distinct().ToList()
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

    public static async Task ApplyRequestedWaterAccountChanges(QanatDbContext dbContext, int geographyID, int userID, WaterAccountParcelsRequestChangesDto requestDto, UserDto callingUser)
    {
        var reportingPeriods = await ReportingPeriods.ListByGeographyIDAsync(dbContext, geographyID, callingUser);
        var defaultReportingPeriod = reportingPeriods.FirstOrDefault(x => x.IsDefault) ?? reportingPeriods.First();
        var reportingPeriodIDsDefaultToCurrent = reportingPeriods.Where(x => x.EndDate >= defaultReportingPeriod.EndDate).Select(x => x.ReportingPeriodID).ToList();

        // remove current WaterAccountParcels
        var parcelIDs = requestDto.WaterAccounts.SelectMany(x => x.Parcels.Select(y => y.ParcelID)).ToList();

        var parcelIDsToInactivate = requestDto.ParcelsToRemove.Select(x => x.ParcelID).ToList();
        parcelIDs.AddRange(parcelIDsToInactivate);

        await dbContext.WaterAccountParcels
            .Where(x => x.GeographyID == geographyID && parcelIDs.Contains(x.ParcelID) && reportingPeriodIDsDefaultToCurrent.Contains(x.ReportingPeriodID))
            .ExecuteDeleteAsync();

        // create new WaterAccountParcels and update existing Parcels' WaterAccountIDs
        var parcelByParcelID = await dbContext.Parcels
            .Include(x => x.WaterAccount)
            .Where(x => x.GeographyID == geographyID && parcelIDs.Contains(x.ParcelID))
            .ToDictionaryAsync(x => x.ParcelID);

        var modifiedParcels = parcelIDsToInactivate.Where(x => parcelByParcelID.ContainsKey(x))
            .Select(x => parcelByParcelID[x]).ToList();

        var waterAccountIDs = requestDto.WaterAccounts.Select(x => x.WaterAccountID).ToList();
        var parcelIDsByWaterAccountID = dbContext.WaterAccountParcels.AsNoTracking()
            .Where(x => x.GeographyID == geographyID && waterAccountIDs.Contains(x.WaterAccountID) && x.ReportingPeriodID == defaultReportingPeriod.ReportingPeriodID)
            .ToLookup(x => x.WaterAccountID, x => x.ParcelID);

        var newWaterAccountParcels = new List<WaterAccountParcel>();
        foreach (var waterAccount in requestDto.WaterAccounts)
        {
            foreach (var parcel in waterAccount.Parcels)
            {
                if (parcelIDsByWaterAccountID[waterAccount.WaterAccountID].Contains(parcel.ParcelID))
                {
                    continue; // WaterAccountParcel already exists.
                }

                // add new WaterAccountParcel records
                newWaterAccountParcels.AddRange(reportingPeriodIDsDefaultToCurrent.Select(x => new WaterAccountParcel()
                {
                    GeographyID = geographyID,
                    WaterAccountID = waterAccount.WaterAccountID,
                    ParcelID = parcel.ParcelID,
                    ReportingPeriodID = x
                }));

                // update WaterAccount col on Parcel table
                var existingParcel = parcelByParcelID[parcel.ParcelID];
                var fromWaterAccountID = existingParcel.WaterAccountID;
                var fromWaterAccountNumber = existingParcel.WaterAccount?.WaterAccountNumber;
                var fromWaterAccountName = existingParcel.WaterAccount?.WaterAccountName;

                existingParcel.WaterAccountID = waterAccount.WaterAccountID;
                existingParcel.ParcelStatusID = ParcelStatus.Assigned.ParcelStatusID;

                // Add ParcelWaterAccountHistories for new associations.
                var newParcelWaterAccountHistory = new ParcelWaterAccountHistory()
                {
                    GeographyID = existingParcel.GeographyID,
                    ParcelID = parcel.ParcelID,
                    ReportingPeriodID = defaultReportingPeriod.ReportingPeriodID,
                    ToWaterAccountID = waterAccount.WaterAccountID,
                    ToWaterAccountNumber = waterAccount.WaterAccountNumber,
                    ToWaterAccountName = waterAccount.WaterAccountName,
                    FromWaterAccountID = fromWaterAccountID,
                    FromWaterAccountNumber = fromWaterAccountNumber,
                    FromWaterAccountName = fromWaterAccountName,
                    Reason = "Added while applying requested Water Account changes.",
                    CreateUserID = callingUser.UserID,
                    CreateDate = DateTime.UtcNow
                };

                dbContext.ParcelWaterAccountHistories.Add(newParcelWaterAccountHistory);
            }
        }

        dbContext.WaterAccountParcels.AddRange(newWaterAccountParcels);

        // set removed parcels to unassigned
        var parcelsToInactivate = dbContext.Parcels.Where(x => parcelIDsToInactivate.Contains(x.ParcelID)).ToList();
        foreach (var parcel in parcelsToInactivate)
        {
            parcel.WaterAccountID = null;
            parcel.ParcelStatusID = ParcelStatus.Unassigned.ParcelStatusID;

            var waterAccountParcelToRemove = dbContext.WaterAccountParcels
                .Include(x => x.WaterAccount)
                .Include(x => x.ReportingPeriod)
                .SingleOrDefault(x => x.GeographyID == geographyID && x.ParcelID == parcel.ParcelID && x.ReportingPeriod.EndDate >= defaultReportingPeriod.EndDate);

            if (waterAccountParcelToRemove != null)
            {
                var fromWaterAccountID = waterAccountParcelToRemove.WaterAccountID;
                var fromWaterAccountNumber = waterAccountParcelToRemove.WaterAccount.WaterAccountNumber;
                var fromWaterAccountName = waterAccountParcelToRemove.WaterAccount.WaterAccountName;

                dbContext.WaterAccountParcels.Remove(waterAccountParcelToRemove);

                var parcelWaterAccountHistory = new ParcelWaterAccountHistory()
                {
                    GeographyID = parcel.GeographyID,
                    ParcelID = parcel.ParcelID,
                    ReportingPeriodID = defaultReportingPeriod.ReportingPeriodID,
                    FromWaterAccountID = fromWaterAccountID,
                    FromWaterAccountNumber = fromWaterAccountNumber,
                    FromWaterAccountName = fromWaterAccountName,
                    Reason = "Removed while applying requested Water Account changes.",
                    CreateUserID = callingUser.UserID,
                    CreateDate = DateTime.UtcNow
                };

                dbContext.ParcelWaterAccountHistories.Add(parcelWaterAccountHistory);
            }

            var parcelHistory = ParcelHistories.CreateNew(parcel, userID);
            await dbContext.ParcelHistories.AddAsync(parcelHistory);
        }

        // add new ParcelHistory records
        dbContext.ParcelHistories.AddRange(modifiedParcels.Select(x => ParcelHistories.CreateNew(x, userID)).ToList());

        await dbContext.SaveChangesAsync();

        // we need to mark any existing parcel history records for the provided parcels as IsReviewed = true
        await ParcelHistories.MarkAsReviewedByParcelIDsAsync(dbContext, modifiedParcels.Select(x => x.ParcelID).ToList());

        // finally if any of the water accounts get into a state of not having any parcels, we need to remove it from the water account users table
        await RemoveEmptyWaterAccountsFromUsers(dbContext, waterAccountIDs);
    }

    public static async Task RemoveEmptyWaterAccountsFromUsers(QanatDbContext dbContext, List<int> waterAccountIDs)
    {
        var waterAccountIDsWithNoParcels = dbContext.WaterAccounts
            .Include(x => x.WaterAccountParcels)
            .Where(x => !x.WaterAccountParcels.Any())
            .Select(x => x.WaterAccountID).ToList();

        await dbContext.WaterAccountUsers.Where(x => waterAccountIDsWithNoParcels.Contains(x.WaterAccountID)).ExecuteDeleteAsync();
    }
}
