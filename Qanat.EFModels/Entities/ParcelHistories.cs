using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities;

public class ParcelHistories
{
    public static bool GeographyHasUnreviewedParcels(QanatDbContext dbContext, int geographyID)
    {
        var unreviewedParcelOwnershipHistories = dbContext.ParcelHistories.AsNoTracking()
            .Where(x => x.GeographyID == geographyID && !x.IsReviewed).ToList();
        return unreviewedParcelOwnershipHistories.Any();
    }

    public static List<ParcelChangesGridItemDto> ListParcelChangesGridItemDtosByGeographyID(QanatDbContext dbContext, int geographyID)
    {
        var parcelOwnershipHistoriesDict = dbContext.ParcelHistories.AsNoTracking()
            .Where(x => x.GeographyID == geographyID)
            .GroupBy(x => x.ParcelID)
            .ToDictionary(x => x.Key, x => x.OrderByDescending(y => y.UpdateDate).Take(2).ToList());

        var parcels = dbContext.Parcels.AsNoTracking()
            .Include(x => x.WaterAccount)
            .Where(x => x.GeographyID == geographyID).ToList();

        var parcelChangesGridItemDtos = new List<ParcelChangesGridItemDto>();
        foreach (var parcel in parcels)
        {
            // each parcel should have at least one ownership history record
            var currentParcelHistory = parcelOwnershipHistoriesDict[parcel.ParcelID].First();
            var previousParcelHistory = parcelOwnershipHistoriesDict[parcel.ParcelID].Skip(1).FirstOrDefault();

            parcelChangesGridItemDtos.Add(new ParcelChangesGridItemDto()
            {
                ParcelID = parcel.ParcelID,
                ParcelNumber = parcel.ParcelNumber,
                GeographyID = parcel.GeographyID,
                WaterAccount = parcel.WaterAccount?.AsLinkDisplayDto(),
                ParcelStatus = parcel.ParcelStatus.AsSimpleDto(),
                IsReviewed = currentParcelHistory.IsReviewed,
                ReviewDate = currentParcelHistory.ReviewDate,
                CurrentParcelHistoryUploadDate = currentParcelHistory.UpdateDate,
                PreviousParcelHistoryUploadDate = previousParcelHistory?.UpdateDate,
                ParcelFieldDiffs = new List<ParcelFieldDiffDto>()
                {
                    new()
                    {
                        FieldName = "Owner Name",
                        FieldShortName = "Owner",
                        CurrentFieldValue = currentParcelHistory.OwnerName,
                        PreviousFieldValue = previousParcelHistory?.OwnerName
                    },
                    new()
                    {
                        FieldName = "Owner Address",
                        FieldShortName = "Address",
                        CurrentFieldValue = currentParcelHistory.OwnerAddress,
                        PreviousFieldValue = previousParcelHistory?.OwnerAddress
                    },
                    new()
                    {
                        FieldName = "Status",
                        FieldShortName = "Status",
                        CurrentFieldValue = currentParcelHistory.ParcelStatus?.ParcelStatusDisplayName,
                        PreviousFieldValue = previousParcelHistory?.ParcelStatus?.ParcelStatusDisplayName
                    },
                    new()
                    {
                        FieldName = "Parcel Area (acres)",
                        FieldShortName = "Parcel Area",
                        CurrentFieldValue = Math.Round(currentParcelHistory.ParcelArea, 4, MidpointRounding.ToEven).ToString(CultureInfo.CurrentCulture),
                        PreviousFieldValue = previousParcelHistory != null
                            ? Math.Round(previousParcelHistory.ParcelArea, 4, MidpointRounding.ToEven).ToString(CultureInfo.CurrentCulture)
                            : null
                    },
                    //MK 2/21/2025 TODO: Think about how to use the new WaterAccountParcelHistory table to show the water account changes. For now just comment it out.
                    //new() 
                    //{
                    //    FieldName = "Water Account",
                    //    FieldShortName = "Water Account",
                    //    CurrentFieldValue = currentParcelHistory.WaterAccount?.WaterAccountNumberAndName(),
                    //    PreviousFieldValue = previousParcelHistory?.WaterAccount?.WaterAccountNumberAndName()
                    //}
                }
            });
        }

        return parcelChangesGridItemDtos;
    }

    public static async Task MarkAsReviewedByParcelIDsAsync(QanatDbContext dbContext, List<int> parcelIDs)
    {
        await dbContext.ParcelHistories
            .Where(x => parcelIDs.Contains(x.ParcelID)).ExecuteUpdateAsync(x => x
                .SetProperty(y => y.IsReviewed, y => true)
                .SetProperty(y => y.ReviewDate, y => DateTime.UtcNow));
    }

    public static ParcelHistory CreateNew(Parcel parcel, int userID)
    {
        var parcelHistory = new ParcelHistory()
        {
            GeographyID = parcel.GeographyID,
            ParcelID = parcel.ParcelID,
            UpdateDate = DateTime.UtcNow,
            UpdateUserID = userID,
            ParcelArea = (decimal)parcel.ParcelArea,
            OwnerName = parcel.OwnerName,
            OwnerAddress = parcel.OwnerAddress,
            ParcelStatusID = parcel.ParcelStatusID,
            IsManualOverride = true,
            IsReviewed = true,
            ReviewDate = DateTime.UtcNow
        };
        return parcelHistory;
    }
}