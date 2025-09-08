using Microsoft.EntityFrameworkCore;
using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities;

public static class WaterAccountSuggestions
{
    public static List<WaterAccountSuggestion> ListByGeography(QanatDbContext dbContext, int geographyID)
    {
        var waterAccountSuggestions = dbContext.WaterAccountSuggestions.FromSql($"EXECUTE dbo.pWaterAccountSuggestion {geographyID}").ToList();
        return waterAccountSuggestions;
    }

    public static List<WaterAccountSuggestionDto> ListByGeographyAsDto(QanatDbContext dbContext, int geographyID)
    {
        var waterAccountSuggestions = ListByGeography(dbContext, geographyID);
        return waterAccountSuggestions.Select(x =>
        {
            return new WaterAccountSuggestionDto()
            {
                WaterAccountName = x.WaterAccountName,
                ParcelNumbers = x.Parcels != null ? string.Join(", ", x.Parcels.Select(y => y.ParcelNumber)) : string.Empty,
                ParcelIDList = x.Parcels != null ? string.Join(", ", x.Parcels.Select(y => y.ParcelID)) : string.Empty,
                WellIDList = x.WellIDList,
                WellIDs = x.WellIDList != null ? x.WellIDList.Split(',').Select(wid => new WellLinkDisplayDto(){WellID = int.Parse(wid)}).ToList() : new List<WellLinkDisplayDto>(),
                ContactName = x.ContactName,
                ContactAddress = x.ContactAddress,
                ParcelArea = x.ParcelArea,
                Zones = x.Zones,
                Parcels = x.Parcels != null ? x.Parcels.Select(y => new ParcelLinkDisplayDto() { ParcelID = y.ParcelID, LinkDisplay = y.ParcelNumber }).ToList() : new List<ParcelLinkDisplayDto>()
            };
        }).ToList();
    }
}