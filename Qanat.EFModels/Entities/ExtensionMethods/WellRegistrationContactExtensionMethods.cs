using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities
{
    public static class WellRegistrationContactExtensionMethods
    {
        public static WellRegistrationContactSimpleDto AsSimpleDto(this WellRegistrationContact wellRegistrationContact)
        {
            var dto = new WellRegistrationContactSimpleDto()
            {
                WellRegistrationContactID = wellRegistrationContact.WellRegistrationContactID,
                WellRegistrationID = wellRegistrationContact.WellRegistrationID,
                WellRegistrationContactTypeID = wellRegistrationContact.WellRegistrationContactTypeID,
                ContactName = wellRegistrationContact.ContactName,
                BusinessName = wellRegistrationContact.BusinessName,
                StreetAddress = wellRegistrationContact.StreetAddress,
                City = wellRegistrationContact.City,
                StateID = wellRegistrationContact.StateID,
                ZipCode = wellRegistrationContact.ZipCode,
                Phone = wellRegistrationContact.Phone,
                Email = wellRegistrationContact.Email
            };
            return dto;
        }
    }
}