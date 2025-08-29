using Microsoft.EntityFrameworkCore;
using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities;

public static class WaterAccountParcels
{
    public static async Task<List<ParcelMinimalDto>> ListByWaterAccountIDAndReportingPeriodIDAsync(QanatDbContext dbContext, int waterAccountID, int reportingPeriodID)
    {
        var parcels = await dbContext.WaterAccountParcels.AsNoTracking()
            .Include(x => x.Parcel)
            .Include(x => x.ReportingPeriod)
            .Where(x => x.WaterAccountID == waterAccountID && x.ReportingPeriod.ReportingPeriodID == reportingPeriodID)
            .Select(x => x.Parcel)
            .ToListAsync();

        var parcelMinimalDtos = parcels.Select(x => x.AsParcelMinimalDto()).ToList();
        return parcelMinimalDtos;
    }

    public static async Task<List<ParcelMinimalDto>> ListByWaterAccountIDAndYearAsync(QanatDbContext dbContext, int waterAccountID, int year)
    {
        var parcels = await dbContext.WaterAccountParcels.AsNoTracking()
            .Include(x => x.Parcel)
            .Include(x => x.ReportingPeriod)
            .Where(x => x.WaterAccountID == waterAccountID && x.ReportingPeriod.EndDate.Year == year)
            .Select(x => x.Parcel)
            .ToListAsync();

        var parcelMinimalDtos = parcels.Select(x => x.AsParcelMinimalDto()).ToList();
        return parcelMinimalDtos;
    }

    public static async Task<List<WaterAccountMinimalAndReportingPeriodSimpleDto>> ListByParcelIDAsync(QanatDbContext dbContext, int parcelID)
    {
        var waterAccountParcels = await dbContext.WaterAccountParcels.AsNoTracking()
            .Include(x => x.WaterAccount)
            .Include(x => x.ReportingPeriod)
            .Where(x => x.ParcelID == parcelID)
            .ToListAsync();

        var waterAccountMinimalAndReportingPeriodSimpleDtos = waterAccountParcels.Select(x => new WaterAccountMinimalAndReportingPeriodSimpleDto()
        {
            WaterAccount = x.WaterAccount.AsWaterAccountMinimalDto(),
            ReportingPeriod = x.ReportingPeriod.AsSimpleDto()
        })
        .OrderByDescending(x => x.ReportingPeriod.EndDate).ToList();

        return waterAccountMinimalAndReportingPeriodSimpleDtos;
    }

    public static async Task<List<ErrorMessage>> ValidateWaterAccountParcelUpdateByParcelAsync(QanatDbContext dbContext, int parcelID, UpdateWaterAccountParcelsByParcelDto updateParcelWaterAccountsDto)
    {
        var errorMessages = new List<ErrorMessage>();

        // Validate that each ReportingPeriodID exists.
        var reportingPeriodIDs = updateParcelWaterAccountsDto.ReportingPeriodWaterAccounts.Select(x => x.ReportingPeriodID).Distinct().ToList();
        var reportingPeriods = await dbContext.ReportingPeriods.AsNoTracking().Where(x => reportingPeriodIDs.Contains(x.ReportingPeriodID)).ToListAsync();
        var missingReportingPeriodIDs = reportingPeriodIDs.Except(reportingPeriods.Select(x => x.ReportingPeriodID)).ToList();
        foreach (var missingReportingPeriodID in missingReportingPeriodIDs)
        {
            errorMessages.Add(new ErrorMessage() { Type = "Reporting Period", Message = $"Reporting Period {missingReportingPeriodID} does not exist." });
        }

        // Validate that each WaterAccount exists.
        var waterAccountIDs = updateParcelWaterAccountsDto.ReportingPeriodWaterAccounts.Where(x => x.WaterAccountID.HasValue).Select(x => x.WaterAccountID.Value).Distinct().ToList();
        var waterAccounts = await dbContext.WaterAccounts.AsNoTracking().Where(x => waterAccountIDs.Contains(x.WaterAccountID)).ToListAsync();
        var missingWaterAccountIDs = waterAccountIDs.Except(waterAccounts.Select(x => x.WaterAccountID)).ToList();
        foreach (var missingWaterAccountID in missingWaterAccountIDs)
        {
            errorMessages.Add(new ErrorMessage() { Type = "Water Account", Message = $"Water Account {missingWaterAccountID} does not exist." });
        }

        // Validate that the Parcel is only assigned to one WaterAccount per ReportingPeriodID.
        var groupedByReportingPeriodID = updateParcelWaterAccountsDto.ReportingPeriodWaterAccounts.GroupBy(x => x.ReportingPeriodID);
        foreach (var group in groupedByReportingPeriodID)
        {
            var groupedWaterAccountIDs = group.Select(x => x.WaterAccountID).ToList();
            if (groupedWaterAccountIDs.Count > 1)
            {
                errorMessages.Add(new ErrorMessage() { Type = "Reporting Period", Message = $"Parcel {parcelID} cannot be assigned to more than one WaterAccount per Reporting Period." });
            }
        }

        return errorMessages;
    }

    public static async Task<List<WaterAccountMinimalAndReportingPeriodSimpleDto>> UpdateWaterAccountParcelsByParcelAsync(QanatDbContext dbContext, int parcelID, UpdateWaterAccountParcelsByParcelDto updateParcelWaterAccountsDto, UserDto callingUser)
    {
        //MK 2/21/2025: This is doing a lot of work, so wrapping it in a transaction that we can rollback if anything goes wrong.
        await using var transaction = await dbContext.Database.BeginTransactionAsync();
        try
        {
            var parcel = await dbContext.Parcels
                .SingleAsync(x => x.ParcelID == parcelID);

            var existingWaterAccountParcels = await dbContext.WaterAccountParcels
                .Include(x => x.WaterAccount)
                .Include(x => x.ReportingPeriod)
                .Where(x => x.ParcelID == parcelID)
                .ToListAsync();

            // Find current Reporting Period for DateTime.UtcNow. Need to do this first because of the CHECK constraint on Parcel (CK_Parcel_WaterAccountID_ParcelStatusID), was hoping that wasn't checked until the end of the transaction.
            var currentReportingPeriod = await ReportingPeriods.GetByGeographyIDAndYearAsync(dbContext, parcel.GeographyID, DateTime.UtcNow.Year);
            var entryForCurrentReportingPeriod = updateParcelWaterAccountsDto.ReportingPeriodWaterAccounts.SingleOrDefault(x => x.ReportingPeriodID == currentReportingPeriod.ReportingPeriodID);
            if (entryForCurrentReportingPeriod?.WaterAccountID == null) // If there is no entry for that Reporting Period mark it as unassigned.
            {
                parcel.WaterAccountID = null;
                parcel.ParcelStatusID = ParcelStatus.Unassigned.ParcelStatusID;
            }
            else if (parcel.WaterAccountID != entryForCurrentReportingPeriod.WaterAccountID) // If its different then the Parcel.WaterAccountID update that.
            {
                parcel.WaterAccountID = entryForCurrentReportingPeriod.WaterAccountID;
                if (parcel.ParcelStatusID == ParcelStatus.Unassigned.ParcelStatusID || parcel.ParcelStatusID == ParcelStatus.Excluded.ParcelStatusID || parcel.ParcelStatusID == ParcelStatus.Inactive.ParcelStatusID)
                {
                    parcel.ParcelStatusID = ParcelStatus.Assigned.ParcelStatusID;
                }
            }

            // Remove the association for the parcel if it is not in the list.
            var removedAssociations = existingWaterAccountParcels
                .Where(x => updateParcelWaterAccountsDto.ReportingPeriodWaterAccounts.Any(y => y.ReportingPeriodID == x.ReportingPeriod.ReportingPeriodID && y.WaterAccountID == null))
                .ToList();

            foreach (var removedAssociation in removedAssociations)
            {
                var waterAccountParcel = existingWaterAccountParcels.First(x => x.WaterAccountID == removedAssociation.WaterAccount.WaterAccountID && x.ReportingPeriodID == removedAssociation.ReportingPeriod.ReportingPeriodID);
                dbContext.WaterAccountParcels.Remove(waterAccountParcel);

                // Add ParcelWaterAccountHistory for removed associations.
                var newParcelWaterAccountHistory = new ParcelWaterAccountHistory()
                {
                    GeographyID = parcel.GeographyID,
                    ParcelID = parcel.ParcelID,
                    ReportingPeriodID = removedAssociation.ReportingPeriod.ReportingPeriodID,
                    FromWaterAccountID = removedAssociation.WaterAccount.WaterAccountID,
                    FromWaterAccountNumber = removedAssociation.WaterAccount.WaterAccountNumber,
                    FromWaterAccountName = removedAssociation.WaterAccount.WaterAccountName,
                    Reason = "Manually removed.",
                    CreateUserID = callingUser.UserID,
                    CreateDate = DateTime.UtcNow
                };

                dbContext.ParcelWaterAccountHistories.Add(newParcelWaterAccountHistory);
            }

            var waterAccounts = await dbContext.WaterAccounts.AsNoTracking()
                .Where(x => updateParcelWaterAccountsDto.ReportingPeriodWaterAccounts.Select(y => y.WaterAccountID).Contains(x.WaterAccountID))
                .ToListAsync();

            // UpdateAsync the association for the parcel if it is in the list.
            var updatedAssociations = updateParcelWaterAccountsDto.ReportingPeriodWaterAccounts
                .Where(x => existingWaterAccountParcels.Any(y => y.ReportingPeriod.ReportingPeriodID == x.ReportingPeriodID && y.WaterAccountID != x.WaterAccountID && x.WaterAccountID.HasValue)) // Ensure not null
                .ToList();

            foreach (var updatedAssociation in updatedAssociations)
            {
                var existingAssociation = existingWaterAccountParcels.First(x => x.ReportingPeriod.ReportingPeriodID == updatedAssociation.ReportingPeriodID);

                // Add ParcelWaterAccountHistories for updated associations.
                var toWaterAccount = waterAccounts.First(x => x.WaterAccountID == updatedAssociation.WaterAccountID);
                var newParcelWaterAccountHistory = new ParcelWaterAccountHistory()
                {
                    GeographyID = parcel.GeographyID,
                    ParcelID = parcel.ParcelID,
                    ReportingPeriodID = updatedAssociation.ReportingPeriodID,
                    FromWaterAccountID = existingAssociation.WaterAccountID,
                    FromWaterAccountNumber = existingAssociation.WaterAccount.WaterAccountNumber,
                    FromWaterAccountName = existingAssociation.WaterAccount.WaterAccountName,
                    ToWaterAccountID = updatedAssociation.WaterAccountID,
                    ToWaterAccountNumber = toWaterAccount.WaterAccountNumber,
                    ToWaterAccountName = toWaterAccount.WaterAccountName,
                    Reason = "Manually updated.",
                    CreateUserID = callingUser.UserID,
                    CreateDate = DateTime.UtcNow
                };

                dbContext.ParcelWaterAccountHistories.Add(newParcelWaterAccountHistory);

                existingAssociation.WaterAccountID = updatedAssociation.WaterAccountID!.Value;
                dbContext.WaterAccountParcels.Update(existingAssociation);
            }

            // Add new associations.
            var addedAssociations = updateParcelWaterAccountsDto.ReportingPeriodWaterAccounts
                .Where(x => existingWaterAccountParcels.All(y => y.ReportingPeriod.ReportingPeriodID != x.ReportingPeriodID) && x.WaterAccountID.HasValue) // Ensure not null
                .ToList();

            foreach (var addedAssociation in addedAssociations)
            {
                var waterAccount = waterAccounts.First(x => x.WaterAccountID == addedAssociation.WaterAccountID);
                var newWaterAccountParcel = new WaterAccountParcel()
                {
                    GeographyID = parcel.GeographyID,
                    WaterAccountID = addedAssociation.WaterAccountID!.Value,
                    ParcelID = parcelID,
                    ReportingPeriodID = addedAssociation.ReportingPeriodID
                };

                dbContext.WaterAccountParcels.Add(newWaterAccountParcel);

                // Add ParcelWaterAccountHistories for added associations
                var newParcelWaterAccountHistory = new ParcelWaterAccountHistory()
                {
                    GeographyID = parcel.GeographyID,
                    ParcelID = parcel.ParcelID,
                    ReportingPeriodID = addedAssociation.ReportingPeriodID,
                    ToWaterAccountID = addedAssociation.WaterAccountID,
                    ToWaterAccountNumber = waterAccount.WaterAccountNumber,
                    ToWaterAccountName = waterAccount.WaterAccountName,
                    Reason = "Manually added.",
                    CreateUserID = callingUser.UserID,
                    CreateDate = DateTime.UtcNow
                };

                dbContext.ParcelWaterAccountHistories.Add(newParcelWaterAccountHistory);
            }

            await dbContext.SaveChangesAsync();
            await transaction.CommitAsync();

            var updatedWaterAccountParcels = await ListByParcelIDAsync(dbContext, parcelID);
            return updatedWaterAccountParcels;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public static async Task<List<ErrorMessage>> ValidateUpdateWaterAccountParcelByWaterAccountAndReportingPeriodAsync(QanatDbContext dbContext, int waterAccountID, WaterAccountParcelsUpdateDto waterAccountParcelsUpdateDto, UserDto callingUser)
    {
        var results = new List<ErrorMessage>();

        var waterAccount = await dbContext.WaterAccounts.AsNoTracking()
            .Include(x => x.Parcels)
            .SingleAsync(x => x.WaterAccountID == waterAccountID);

        var reportingPeriod = await ReportingPeriods.GetAsync(dbContext, waterAccount.GeographyID, waterAccountParcelsUpdateDto.ReportingPeriodID, callingUser);
        if (reportingPeriod == null)
        {
            results.Add(new ErrorMessage()
            {
                Type = "Reporting Period",
                Message = "The Reporting Period could not be found."
            });
        }

        foreach (var parcelID in waterAccountParcelsUpdateDto.ParcelIDs)
        {
            var parcel = await dbContext.Parcels.SingleOrDefaultAsync(x => x.ParcelID == parcelID);
            if (parcel == null)
            {
                results.Add(new ErrorMessage()
                {
                    Type = "Parcel",
                    Message = $"Could not find the parcel with the ParcelID of {parcelID}."
                });
            }
            else if (parcel.GeographyID != waterAccount.GeographyID)
            {
                results.Add(new ErrorMessage()
                {
                    Type = "Geography",
                    Message = "The geography of the selected water account doesn't match the selected parcel."
                });
            }
        }

        return results;
    }

    public static async Task<List<ParcelMinimalDto>> UpdateWaterAccountParcelByWaterAccountAndReportingPeriodAsync(QanatDbContext dbContext, int waterAccountID, int reportingPeriodID, List<int> parcelIDs, UserDto callingUser)
    {
        var waterAccount = dbContext.WaterAccounts.Include(x => x.WaterAccountParcels).Single(x => x.WaterAccountID == waterAccountID);
        var reportingPeriodToStartAt = await ReportingPeriods.GetAsync(dbContext, waterAccount.GeographyID, reportingPeriodID);

        var reportingPeriodsToUpdateParcels = await dbContext.ReportingPeriods.AsNoTracking()
            .Where(x => x.GeographyID == waterAccount.GeographyID && x.EndDate >= reportingPeriodToStartAt.EndDate)
            .ToListAsync();

        var parcelIDsToMarkAsReviewed = new List<int>();

        //MK 2/25/2025: This is doing a lot of work, so wrapping it in a transaction that we can rollback if anything goes wrong.
        await using var transaction = await dbContext.Database.BeginTransactionAsync();
        try
        {
            foreach (var reportingPeriod in reportingPeriodsToUpdateParcels)
            {
                var waterAccountParcelsForReportingPeriod = await dbContext.WaterAccountParcels
                    .Include(x => x.ReportingPeriod)
                    .Include(x => x.WaterAccount)
                    .Where(x => x.GeographyID == waterAccount.GeographyID && x.WaterAccountID == waterAccountID && x.ReportingPeriodID == reportingPeriod.ReportingPeriodID)
                    .ToListAsync();

                var waterAccountParcelsToRemove = waterAccountParcelsForReportingPeriod.Where(x => !parcelIDs.Contains(x.ParcelID)).ToList();

                foreach (var removedAssociation in waterAccountParcelsToRemove)
                {
                    var parcel = dbContext.Parcels
                        .Include(x => x.WaterAccountParcels)
                        .Single(x => x.ParcelID == removedAssociation.ParcelID);

                    // If we are operating on the current year's reporting period, and the parcel status is assigned, we need to set it to unassigned and add a ParcelHistory.
                    if (reportingPeriod.EndDate.Year == DateTime.UtcNow.Year && parcel.ParcelStatusID == ParcelStatus.Assigned.ParcelStatusID)
                    {
                        parcel.WaterAccountID = null;
                        parcel.ParcelStatusID = ParcelStatus.Unassigned.ParcelStatusID;
                        var newParcelHistoryRecord = ParcelHistories.CreateNew(parcel, callingUser.UserID);
                        await dbContext.ParcelHistories.AddAsync(newParcelHistoryRecord);

                        if (!parcelIDsToMarkAsReviewed.Contains(parcel.ParcelID))
                        {
                            parcelIDsToMarkAsReviewed.Add(parcel.ParcelID);
                        }
                    }

                    dbContext.WaterAccountParcels.Remove(removedAssociation);

                    // Add ParcelWaterAccountHistory for removed associations.
                    var newParcelWaterAccountHistory = new ParcelWaterAccountHistory()
                    {
                        GeographyID = parcel.GeographyID,
                        ParcelID = parcel.ParcelID,
                        ReportingPeriodID = removedAssociation.ReportingPeriod.ReportingPeriodID,
                        FromWaterAccountID = removedAssociation.WaterAccount.WaterAccountID,
                        FromWaterAccountNumber = removedAssociation.WaterAccount.WaterAccountNumber,
                        FromWaterAccountName = removedAssociation.WaterAccount.WaterAccountName,
                        Reason = "Manually removed.",
                        CreateUserID = callingUser.UserID,
                        CreateDate = DateTime.UtcNow
                    };

                    dbContext.ParcelWaterAccountHistories.Add(newParcelWaterAccountHistory);
                }

                var waterAccountParcelIDsToAdd = parcelIDs.Except(waterAccountParcelsForReportingPeriod.Select(x => x.ParcelID)).ToList();
                foreach (var parcelID in waterAccountParcelIDsToAdd)
                {
                    // Check if there is an existing relationship for this parcel and reporting period.
                    var existingWaterAccountParcel = await dbContext.WaterAccountParcels
                        .Include(x => x.WaterAccount)
                        .SingleOrDefaultAsync(x => x.WaterAccountID != waterAccount.WaterAccountID && x.ParcelID == parcelID && x.ReportingPeriodID == reportingPeriod.ReportingPeriodID);

                    var fromWaterAccountID = existingWaterAccountParcel?.WaterAccountID;
                    var fromWaterAccountNumber = existingWaterAccountParcel?.WaterAccount?.WaterAccountNumber;
                    var fromWaterAccountName = existingWaterAccountParcel?.WaterAccount?.WaterAccountName;

                    if (existingWaterAccountParcel != null)
                    {
                        dbContext.WaterAccountParcels.Remove(existingWaterAccountParcel);
                    }

                    if (reportingPeriod.EndDate.Year == DateTime.UtcNow.Year)
                    {
                        //Need to set the Parcel's WaterAccountID and status to assigned if it was unassigned.
                        var parcel = dbContext.Parcels.Single(x => x.ParcelID == parcelID);
                        parcel.WaterAccountID = waterAccountID;

                        if (parcel.ParcelStatusID == ParcelStatus.Unassigned.ParcelStatusID)
                        {
                            parcel.ParcelStatusID = ParcelStatus.Assigned.ParcelStatusID;

                            var parcelHistory = ParcelHistories.CreateNew(parcel, callingUser.UserID);
                            await dbContext.ParcelHistories.AddAsync(parcelHistory);
                        }
                    }

                    var newWaterAccountParcel = new WaterAccountParcel()
                    {
                        GeographyID = waterAccount.GeographyID,
                        WaterAccountID = waterAccountID,
                        ParcelID = parcelID,
                        ReportingPeriodID = reportingPeriod.ReportingPeriodID
                    };

                    await dbContext.WaterAccountParcels.AddAsync(newWaterAccountParcel);

                    // Add a ParcelWaterAccountHistory for the updated association.
                    var existingWaterAccountParcelNewHistory = new ParcelWaterAccountHistory()
                    {
                        GeographyID = waterAccount.GeographyID,
                        ParcelID = parcelID,
                        ReportingPeriodID = reportingPeriod.ReportingPeriodID,
                        FromWaterAccountID = fromWaterAccountID,
                        FromWaterAccountNumber = fromWaterAccountNumber,
                        FromWaterAccountName = fromWaterAccountName,
                        ToWaterAccountID = waterAccountID,
                        ToWaterAccountNumber = waterAccount.WaterAccountNumber,
                        ToWaterAccountName = waterAccount.WaterAccountName,
                        Reason = fromWaterAccountID.HasValue ? "Manually updated." : "Manually added.",
                        CreateUserID = callingUser.UserID,
                        CreateDate = DateTime.UtcNow
                    };

                    dbContext.ParcelWaterAccountHistories.Add(existingWaterAccountParcelNewHistory);
                }
            }

            await dbContext.SaveChangesAsync();

            // We need to mark any existing parcel history records for the affected parcels as IsReviewed = true.
            await ParcelHistories.MarkAsReviewedByParcelIDsAsync(dbContext, parcelIDsToMarkAsReviewed);

            // We need to remove any empty water accounts from users.
            await WaterAccounts.RemoveEmptyWaterAccountsFromUsers(dbContext, [waterAccountID]);

            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            throw;
        }

        var updatedParcels = await ListByWaterAccountIDAndReportingPeriodIDAsync(dbContext, waterAccountID, reportingPeriodToStartAt.ReportingPeriodID);
        return updatedParcels;
    }


    public static async Task<List<ErrorMessage>> ValidateAddOrphanedParcelToWaterAccountAsync(QanatDbContext dbContext, int waterAccountID, int parcelID)
    {
        var errors = new List<ErrorMessage>();

        //These come in on the route so they should be not founds, and this code won't get hit.
        var parcel = await dbContext.Parcels.SingleAsync(x => x.ParcelID == parcelID);
        var waterAccount = await dbContext.WaterAccounts.SingleAsync(x => x.WaterAccountID == waterAccountID);

        if (parcel.GeographyID != waterAccount.GeographyID)
        {
            errors.Add(new ErrorMessage() { Type = "Geography", Message = "The geography of the selected water account doesn't match the selected parcel." });
        }

        if (parcel.WaterAccountID.HasValue)
        {
            errors.Add(new ErrorMessage() { Type = "Parcel", Message = "The selected parcel is already associated with a water account. This operation is intended only for orphaned parcels." });
        }

        var reportingPeriod = await ReportingPeriods.GetByGeographyIDAndYearAsync(dbContext, parcel.GeographyID, DateTime.UtcNow.Year);
        if (reportingPeriod == null)
        {
            errors.Add(new ErrorMessage() { Type = "ReportingPeriodID", Message = "Could not find the reporting period for the selected parcel." });
            return errors;
        }

        var waterAccountParcel = await ListByWaterAccountIDAndReportingPeriodIDAsync(dbContext, waterAccountID, reportingPeriod.ReportingPeriodID);
        if (waterAccountParcel.Any(x => x.ParcelID == parcelID))
        {
            errors.Add(new ErrorMessage() { Type = "Water Account Parcel", Message = "The selected parcel is already associated with the selected water account." });
        }

        return errors;
    }

    public static async Task<ParcelMinimalAndReportingPeriodSimpleDto> AddOrphanedParcelToWaterAccountAsync(QanatDbContext dbContext, int waterAccountID, int parcelID, UserDto callingUser)
    {
        var parcel = dbContext.Parcels.Single(x => x.ParcelID == parcelID);
        parcel.WaterAccountID = waterAccountID;

        var markHistory = false;
        if (parcel.ParcelStatusID == ParcelStatus.Unassigned.ParcelStatusID)
        {
            parcel.ParcelStatusID = ParcelStatus.Assigned.ParcelStatusID;
            markHistory = true;
        }

        var reportingPeriod = await ReportingPeriods.GetByGeographyIDAndYearAsSimpleDtoAsync(dbContext, parcel.GeographyID, DateTime.UtcNow.Year);
        var waterAccount = await dbContext.WaterAccounts.SingleAsync(x => x.WaterAccountID == waterAccountID);

        var waterAccountParcel = new WaterAccountParcel()
        {
            GeographyID = waterAccount.GeographyID,
            WaterAccountID = waterAccount.WaterAccountID,
            ParcelID = parcel.ParcelID,
            ReportingPeriodID = reportingPeriod.ReportingPeriodID
        };

        await dbContext.WaterAccountParcels.AddAsync(waterAccountParcel);

        var newParcelWaterAccountHistory = new ParcelWaterAccountHistory()
        {
            GeographyID = parcel.GeographyID,
            ParcelID = parcel.ParcelID,
            ReportingPeriodID = reportingPeriod.ReportingPeriodID,
            ToWaterAccountID = waterAccount.WaterAccountID,
            ToWaterAccountNumber = waterAccount.WaterAccountNumber,
            ToWaterAccountName = waterAccount.WaterAccountName,
            Reason = "Manually added.",
            CreateUserID = callingUser.UserID,
            CreateDate = DateTime.UtcNow
        };

        dbContext.ParcelWaterAccountHistories.Add(newParcelWaterAccountHistory);

        await dbContext.SaveChangesAsync();

        if (markHistory)
        {
            await ParcelHistories.MarkAsReviewedByParcelIDsAsync(dbContext, [parcel.ParcelID]);
        }

        var updatedParcel = new ParcelMinimalAndReportingPeriodSimpleDto()
        {
            Parcel = parcel.AsParcelMinimalDto(),
            ReportingPeriod = reportingPeriod
        };

        return updatedParcel;
    }

    public static async Task<List<ErrorMessage>> ValidateCopyFromReportingPeriodAsync(QanatDbContext dbContext, int geographyID, int fromReportingPeriodID, int toReportingPeriodID, UserDto callingUser)
    {
        var errorList = new List<ErrorMessage>();

        var fromReportingPeriod = await ReportingPeriods.GetAsync(dbContext, geographyID, fromReportingPeriodID);
        if (fromReportingPeriod == null)
        {
            errorList.Add(new ErrorMessage() { Type = "To Reporting Period", Message = $"Could not find a valid Reporting Period with the ID {toReportingPeriodID}." });
        }

        if (fromReportingPeriodID == toReportingPeriodID)
        {
            errorList.Add(new ErrorMessage() { Type = "To Reporting Period", Message = $"Can not copy from the same Reporting Period." });
        }

        return errorList;
    }

    public static async Task<List<WaterAccountParcelSimpleDto>> CopyFromReportingPeriodAsync(QanatDbContext dbContext, int geographyID, int fromReportingPeriodID, int toReportingPeriodID, UserDto callingUser)
    {
        var fromReportingPeriod = await ReportingPeriods.GetAsync(dbContext, geographyID, fromReportingPeriodID);
        var utcNow = DateTime.UtcNow;

        var waterAccountParcelsToAdd = new List<WaterAccountParcel>();
        var parcelWaterAccountHistoriesToAdd = new List<ParcelWaterAccountHistory>();

        var previousWaterAccountParcelsForToReportingPeriod = await dbContext.WaterAccountParcels
            .Include(x => x.WaterAccount)
            .Where(x => x.GeographyID == geographyID && x.ReportingPeriodID == toReportingPeriodID)
            .ToListAsync();

        var fromWaterAccountParcels = await dbContext.WaterAccountParcels.AsNoTracking()
            .Include(x => x.WaterAccount)
            .Where(x => x.GeographyID == geographyID && x.ReportingPeriodID == fromReportingPeriodID)
            .ToListAsync();

        /*MK 5/23/2025: The variable names are a bit confusing here and don't match up to the history table. History's "TO" is the copy's "FROM" and the History's "FROM is the previous value, either null or the previous water account. 
                        Need to loop through the previous water account parcels to account for copying to null. Then loop through the "FROM" water account parcels to account for copying from null.
        */
        foreach (var previousWaterAccountParcel in previousWaterAccountParcelsForToReportingPeriod)
        {
            var fromWaterAccountParcel = fromWaterAccountParcels.FirstOrDefault(x => x.ParcelID == previousWaterAccountParcel.ParcelID);
            var parcelWaterAccountHistory = new ParcelWaterAccountHistory()
            {
                GeographyID = geographyID,
                ParcelID = previousWaterAccountParcel.ParcelID,
                ReportingPeriodID = toReportingPeriodID,
                FromWaterAccountID = previousWaterAccountParcel.WaterAccountID,
                FromWaterAccountNumber = previousWaterAccountParcel.WaterAccount?.WaterAccountNumber,
                FromWaterAccountName = previousWaterAccountParcel.WaterAccount?.WaterAccountName,
                ToWaterAccountID = fromWaterAccountParcel?.WaterAccountID,
                ToWaterAccountNumber = fromWaterAccountParcel?.WaterAccount.WaterAccountNumber,
                ToWaterAccountName = fromWaterAccountParcel?.WaterAccount.WaterAccountName,
                Reason = $"Copied from Reporting Period {fromReportingPeriod.Name}.",
                CreateUserID = callingUser.UserID,
                CreateDate = utcNow,
            };

            parcelWaterAccountHistoriesToAdd.Add(parcelWaterAccountHistory);
        }

        foreach (var fromWaterAccountParcel in fromWaterAccountParcels)
        {
            var previousWaterAccountParcel = previousWaterAccountParcelsForToReportingPeriod.FirstOrDefault(x => x.ParcelID == fromWaterAccountParcel.ParcelID);
            if (previousWaterAccountParcel == null)
            {
                var parcelWaterAccountHistory = new ParcelWaterAccountHistory()
                {
                    GeographyID = geographyID,
                    ParcelID = fromWaterAccountParcel.ParcelID,
                    ReportingPeriodID = toReportingPeriodID,
                    FromWaterAccountID = null,
                    FromWaterAccountNumber = null,
                    FromWaterAccountName = null,
                    ToWaterAccountID = fromWaterAccountParcel.WaterAccountID,
                    ToWaterAccountNumber = fromWaterAccountParcel.WaterAccount.WaterAccountNumber,
                    ToWaterAccountName = fromWaterAccountParcel.WaterAccount.WaterAccountName,
                    Reason = $"Copied from Reporting Period {fromReportingPeriod.Name}.",
                    CreateUserID = callingUser.UserID,
                    CreateDate = utcNow,
                };

                parcelWaterAccountHistoriesToAdd.Add(parcelWaterAccountHistory);
            }

            var waterAccountParcel = new WaterAccountParcel()
            {
                GeographyID = geographyID,
                WaterAccountID = fromWaterAccountParcel.WaterAccountID,
                ParcelID = fromWaterAccountParcel.ParcelID,
                ReportingPeriodID = toReportingPeriodID
            };

            waterAccountParcelsToAdd.Add(waterAccountParcel);
        }

        //Wipe existing water account parcels for the "to" reporting period and add the new WaterAccountParcels and ParcelWaterAccountHistories.
        await dbContext.WaterAccountParcels
            .Where(x => x.GeographyID == geographyID && x.ReportingPeriodID == toReportingPeriodID)
            .ExecuteDeleteAsync();

        await dbContext.WaterAccountParcels.AddRangeAsync(waterAccountParcelsToAdd);
        await dbContext.ParcelWaterAccountHistories.AddRangeAsync(parcelWaterAccountHistoriesToAdd);

        await dbContext.SaveChangesAsync();

        var copiedWaterAccountParcels = await dbContext.WaterAccountParcels.AsNoTracking()
            .Where(x => x.GeographyID == geographyID && x.ReportingPeriodID == toReportingPeriodID)
            .Select(x => x.AsSimpleDto())
            .ToListAsync();

        return copiedWaterAccountParcels;
    }
}