using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities;

public static class WaterAccountContactExtensionMethods
{
    public static WaterAccountContactDto AsDto(this WaterAccountContact waterAccountContact)
    {
        var dto = new WaterAccountContactDto()
        {
            WaterAccountContactID = waterAccountContact.WaterAccountContactID,
            GeographyID = waterAccountContact.GeographyID,
            ContactName = waterAccountContact.ContactName,
            ContactEmail = waterAccountContact.ContactEmail,
            ContactPhoneNumber = waterAccountContact.ContactPhoneNumber,
            Address = waterAccountContact.Address,
            SecondaryAddress = waterAccountContact.SecondaryAddress,
            City = waterAccountContact.City,
            State = waterAccountContact.State,
            ZipCode = waterAccountContact.ZipCode,
            FullAddress = waterAccountContact.FullAddress,
            PrefersPhysicalCommunication = waterAccountContact.PrefersPhysicalCommunication,
            AddressValidated = waterAccountContact.AddressValidated,
            AddressValidationJson = waterAccountContact.AddressValidationJson,
            WaterAccounts = waterAccountContact.WaterAccounts?.Select(x => x.AsWaterAccountMinimalDto()).ToList(),
        };
        return dto;
    }

    public static WaterAccountContactSearchResultWithMatchedFieldsDto AsSearchResultWithMatchedFieldsDto(this WaterAccountContact waterAccountContact, string searchString)
    {
        return new WaterAccountContactSearchResultWithMatchedFieldsDto()
        {
            WaterAccountContact = waterAccountContact.AsSearchResultDto(),
            MatchedFields = new Dictionary<WaterAccountContactSearchMatchEnum, bool>()
            {
                {
                    WaterAccountContactSearchMatchEnum.ContactName,
                    waterAccountContact.ContactName.Contains(searchString, StringComparison.OrdinalIgnoreCase)
                },
                {
                    WaterAccountContactSearchMatchEnum.Email,
                    waterAccountContact.ContactEmail != null &&
                    waterAccountContact.ContactEmail.Contains(searchString, StringComparison.OrdinalIgnoreCase)
                },
                {
                    WaterAccountContactSearchMatchEnum.PhoneNumber,
                    waterAccountContact.ContactPhoneNumber != null &&
                    waterAccountContact.ContactPhoneNumber.Contains(searchString, StringComparison.OrdinalIgnoreCase)
                },
                {
                    WaterAccountContactSearchMatchEnum.FullAddress,
                    waterAccountContact.FullAddress != null &&
                    waterAccountContact.FullAddress.Contains(searchString, StringComparison.OrdinalIgnoreCase)
                },
            }
        };
    }

    public static WaterAccountContactSearchResultDto AsSearchResultDto(this WaterAccountContact waterAccountContact)
    {
        return new WaterAccountContactSearchResultDto()
        {
            WaterAccountContactID = waterAccountContact.WaterAccountContactID,
            ContactName = waterAccountContact.ContactName,
            ContactEmail = waterAccountContact.ContactEmail,
            ContactPhoneNumber = waterAccountContact.ContactPhoneNumber,
            FullAddress = waterAccountContact.FullAddress
        };
    }
}