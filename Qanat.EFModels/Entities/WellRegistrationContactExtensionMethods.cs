using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities;

public static partial class WellRegistrationContactExtensionMethods
{
    public static WellRegistrationContactWithStateDto AsWellRegistrationContactWithStateDto(this WellRegistrationContact wellRegistrationContact)
    {
        return new WellRegistrationContactWithStateDto()
        {
            WellRegistrationContactID = wellRegistrationContact.WellRegistrationContactID,
            WellRegistrationID = wellRegistrationContact.WellRegistrationID,
            WellRegistrationContactTypeID = wellRegistrationContact.WellRegistrationContactTypeID,
            ContactName = wellRegistrationContact.ContactName,
            BusinessName = wellRegistrationContact.BusinessName,
            StreetAddress = wellRegistrationContact.StreetAddress,
            City = wellRegistrationContact.City,
            StateID = wellRegistrationContact.StateID,
            StateName = State.AllLookupDictionary[wellRegistrationContact.StateID].StateName,
            ZipCode = wellRegistrationContact.ZipCode,
            Phone = wellRegistrationContact.Phone,
            Email = wellRegistrationContact.Email
        };
    }
}