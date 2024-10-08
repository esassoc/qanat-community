using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities;

public static partial class WaterAccountExtensionMethods
{
    public static WaterAccountDto AsWaterAccountDto(this WaterAccount waterAccount)
    {
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
            ContactName = waterAccount.ContactName,
            ContactAddress = waterAccount.ContactAddress,
            Users = waterAccount.WaterAccountUsers.Select(x => x.AsWaterAccountUserMinimalDto()).ToList(),
            WaterAccountNameAndNumber = waterAccount.WaterAccountNameAndNumber(),
            Parcels = waterAccount.Parcels?.Select(x => x.AsDisplayDto()).ToList()
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
            ContactName = waterAccount.ContactName,
            ContactAddress = waterAccount.ContactAddress,
            WaterAccountNameAndNumber = waterAccount.WaterAccountNameAndNumber(),
            Geography = waterAccount.Geography?.AsSimpleDto()
        };
        return dto;
    }

    public static WaterAccountDisplayDto AsDisplayDto(this WaterAccount waterAccount)
    {
        return new WaterAccountDisplayDto(waterAccount.WaterAccountID, waterAccount.WaterAccountName, waterAccount.WaterAccountNumber, waterAccount.WaterAccountPIN);
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

    public static WaterAccountIndexGridDto AsIndexGridDto(this WaterAccount waterAccount)
    {
        return new WaterAccountIndexGridDto()
        {
            WaterAccountID = waterAccount.WaterAccountID,
            WaterAccountNumber = waterAccount.WaterAccountNumber,
            WaterAccountName = waterAccount.WaterAccountName,
            ContactName = waterAccount.ContactName,
            ContactAddress = waterAccount.ContactAddress,
            Notes = waterAccount.Notes,
            CreateDate = waterAccount.CreateDate,
            UpdateDate = waterAccount.UpdateDate,
            WaterAccountPIN = waterAccount.WaterAccountPIN,
            WaterAccountPINLastUsed = waterAccount.WaterAccountUsers.Any() ? waterAccount.WaterAccountUsers.Max(x => x.ClaimDate) : null,
            Parcels = waterAccount.Parcels?.Select(x => x.AsDisplayDto()).ToList(),
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
            ContactName = waterAccount.ContactName,
            ContactAddress = waterAccount.ContactAddress,
            WaterAccountNameAndNumber = waterAccount.WaterAccountNameAndNumber(),
            Parcels = waterAccount.Parcels?.Select(x => x.AsDisplayDto()).ToList()
        };
    }


    public static string WaterAccountNameAndNumber(this WaterAccount waterAccount)
    {
        return waterAccount.WaterAccountName == null ? $"#{waterAccount.WaterAccountNumber}" : $"#{waterAccount.WaterAccountNumber} ({waterAccount.WaterAccountName})";
    }
}