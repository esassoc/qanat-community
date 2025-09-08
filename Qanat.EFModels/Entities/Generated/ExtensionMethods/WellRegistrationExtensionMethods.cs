//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[WellRegistration]

using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities
{
    public static partial class WellRegistrationExtensionMethods
    {
        public static WellRegistrationSimpleDto AsSimpleDto(this WellRegistration wellRegistration)
        {
            var dto = new WellRegistrationSimpleDto()
            {
                WellRegistrationID = wellRegistration.WellRegistrationID,
                GeographyID = wellRegistration.GeographyID,
                WellID = wellRegistration.WellID,
                WellName = wellRegistration.WellName,
                WellRegistrationStatusID = wellRegistration.WellRegistrationStatusID,
                ParcelID = wellRegistration.ParcelID,
                StateWCRNumber = wellRegistration.StateWCRNumber,
                CountyWellPermitNumber = wellRegistration.CountyWellPermitNumber,
                DateDrilled = wellRegistration.DateDrilled,
                WellDepth = wellRegistration.WellDepth,
                SubmitDate = wellRegistration.SubmitDate,
                ApprovalDate = wellRegistration.ApprovalDate,
                CreateUserID = wellRegistration.CreateUserID,
                CreateUserGuid = wellRegistration.CreateUserGuid,
                CreateUserEmail = wellRegistration.CreateUserEmail,
                ReferenceWellID = wellRegistration.ReferenceWellID,
                FairyshrimpWellID = wellRegistration.FairyshrimpWellID,
                ConfirmedWellLocation = wellRegistration.ConfirmedWellLocation,
                SelectedIrrigatedParcels = wellRegistration.SelectedIrrigatedParcels
            };
            return dto;
        }
    }
}