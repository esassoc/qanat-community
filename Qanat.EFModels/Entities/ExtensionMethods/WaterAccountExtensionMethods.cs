using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities;

public static class WaterAccountExtensionMethods
{
    public static WaterAccountSimpleDto AsSimpleDto(this WaterAccount waterAccount)
    {
        var dto = new WaterAccountSimpleDto()
        {
            WaterAccountID = waterAccount.WaterAccountID,
            GeographyID = waterAccount.GeographyID,
            WaterAccountNumber = waterAccount.WaterAccountNumber,
            WaterAccountName = waterAccount.WaterAccountName,
            Notes = waterAccount.Notes,
            UpdateDate = waterAccount.UpdateDate,
            WaterAccountPIN = waterAccount.WaterAccountPIN,
            CreateDate = waterAccount.CreateDate
        };
        return dto;
    }

    public static WaterAccountDto AsWaterAccountDto(this WaterAccount waterAccount, int reportingPeriodID, List<Parcel> parcels)
    {
        var totalParcelAcreage = parcels.Sum(x => x.ParcelArea);
        var totalIrrigatedAcreage = parcels.SelectMany(x => x.UsageLocations).Where(x => x.ReportingPeriodID == reportingPeriodID).Sum(x => x.Area);

        var waterAccountDto = new WaterAccountDto()
        {
            WaterAccountID = waterAccount.WaterAccountID,
            Geography = waterAccount.Geography.AsSimpleDto(),
            WaterAccountNumber = waterAccount.WaterAccountNumber,
            WaterAccountName = waterAccount.WaterAccountName,
            Notes = waterAccount.Notes,
            UpdateDate = waterAccount.UpdateDate,
            WaterAccountPIN = waterAccount.WaterAccountPIN,
            WaterAccountPINLastUsed = waterAccount.WaterAccountUsers.Any() ? waterAccount.WaterAccountUsers.Max(x => x.ClaimDate) : null,
            CreateDate = waterAccount.CreateDate,
            WaterAccountContact = waterAccount.WaterAccountContact?.AsDto(),
            Users = waterAccount.WaterAccountUsers.Select(x => x.AsWaterAccountUserMinimalDto()).ToList(),
            WaterAccountNameAndNumber = waterAccount.WaterAccountNumberAndName(),
            Parcels = parcels.Select(x => x.AsDisplayDto()).ToList(),
            Acres = totalParcelAcreage,
            IrrigatedAcres = totalIrrigatedAcreage
        };

        return waterAccountDto;
    }

    public static WaterAccountMinimalDto AsWaterAccountMinimalDto(this WaterAccount waterAccount)
    {
        var dto = new WaterAccountMinimalDto()
        {
            WaterAccountID = waterAccount.WaterAccountID,
            GeographyID = waterAccount.GeographyID,
            WaterAccountNumber = waterAccount.WaterAccountNumber,
            WaterAccountName = waterAccount.WaterAccountName,
            Notes = waterAccount.Notes,
            UpdateDate = waterAccount.UpdateDate,
            WaterAccountPIN = waterAccount.WaterAccountPIN,
            CreateDate = waterAccount.CreateDate,
            WaterAccountNameAndNumber = waterAccount.WaterAccountNumberAndName(),
            Geography = waterAccount.Geography?.AsSimpleDto()
        };
        return dto;
    }

    public static WaterAccountDisplayDto AsDisplayDto(this WaterAccount waterAccount)
    {
        return new WaterAccountDisplayDto(waterAccount.WaterAccountID, waterAccount.WaterAccountName, waterAccount.WaterAccountNumber, waterAccount.WaterAccountPIN, waterAccount.GeographyID);
    }

    public static WaterAccountLinkDisplayDto AsLinkDisplayDto(this WaterAccount waterAccount)
    {
        return new WaterAccountLinkDisplayDto()
        {
            WaterAccountID = waterAccount.WaterAccountID,
            WaterAccountName = waterAccount.WaterAccountName,
            WaterAccountNumber = waterAccount.WaterAccountNumber
        };
    }

    public static WaterAccountIndexGridDto AsIndexGridDto(this WaterAccount waterAccount, int reportingPeriodID)
    {
        return new WaterAccountIndexGridDto()
        {
            WaterAccountID = waterAccount.WaterAccountID,
            WaterAccountNumber = waterAccount.WaterAccountNumber,
            WaterAccountName = waterAccount.WaterAccountName,
            CreateDate = waterAccount.CreateDate,
            UpdateDate = waterAccount.UpdateDate,
            WaterAccountPIN = waterAccount.WaterAccountPIN,
            WaterAccountPINLastUsed = waterAccount.WaterAccountUsers.Any() ? waterAccount.WaterAccountUsers.Max(x => x.ClaimDate) : null,
            Parcels = waterAccount.WaterAccountParcels?.Where(x => x.ReportingPeriodID == reportingPeriodID).Select(x => x.Parcel.AsDisplayDto()).ToList(),
            WaterAccountContact = waterAccount.WaterAccountContact?.AsDto(),
            Users = waterAccount.WaterAccountUsers.Select(x => x.AsWaterAccountUserMinimalDto()).ToList()
        };
    }

    public static WaterAccountSearchResultDto AsSearchResultDto(this WaterAccount waterAccount)
    {
        return new WaterAccountSearchResultDto()
        {
            WaterAccountID = waterAccount.WaterAccountID,
            WaterAccountNumber = waterAccount.WaterAccountNumber,
            WaterAccountName = waterAccount.WaterAccountName,
            Notes = waterAccount.Notes,
            UpdateDate = waterAccount.UpdateDate,
            WaterAccountPIN = waterAccount.WaterAccountPIN,
            CreateDate = waterAccount.CreateDate,
            ContactName = waterAccount.WaterAccountContact?.ContactName,
            FullAddress = waterAccount.WaterAccountContact?.FullAddress,
            WaterAccountNameAndNumber = waterAccount.WaterAccountNumberAndName(),
            Parcels = waterAccount.Parcels?.Select(x => x.AsDisplayDto()).ToList()
        };
    }

    public static string WaterAccountNumberAndName(this WaterAccount waterAccount)
    {
        return waterAccount.WaterAccountName == null ? $"#{waterAccount.WaterAccountNumber}" : $"#{waterAccount.WaterAccountNumber} ({waterAccount.WaterAccountName})";
    }
}