
using Microsoft.EntityFrameworkCore;
using Qanat.Common.GeoSpatial;
using Qanat.Common.Util;
using Qanat.EFModels.Entities;
using Qanat.Models.DataTransferObjects;
namespace Qanat.EFModels.Workflows;

public static class WellRegistryWorkflowActions
{
    public static void CreateWellRegistration(QanatDbContext dbContext, WellRegistration wellRegistration)
    {
        dbContext.WellRegistrations.Add(wellRegistration);
        dbContext.SaveChanges();
        dbContext.Entry(wellRegistration).Reload();
    }

    public static void UpdateSelectedParcel(QanatDbContext dbContext, WellRegistration wellRegistration, int? parcelID)
    {
        var irrigatedParcelToRemove = wellRegistration.WellRegistrationIrrigatedParcels
            .SingleOrDefault(x => x.ParcelID == wellRegistration.ParcelID);
        if (irrigatedParcelToRemove != null)
        {
            dbContext.WellRegistrationIrrigatedParcels.Remove(irrigatedParcelToRemove);
        }

        if (parcelID != null)
        {
            wellRegistration.WellRegistrationIrrigatedParcels.Add(new WellRegistrationIrrigatedParcel
            {
                ParcelID = (int)parcelID, 
                WellRegistrationID = wellRegistration.WellRegistrationID
            });
        }

        wellRegistration.ParcelID = parcelID;

        dbContext.SaveChanges();
        dbContext.Entry(wellRegistration).Reload();
    }

    public static void UpdateWellRegistrationLocation(QanatDbContext dbContext, WellRegistration wellRegistration, WellRegistrationLocationDto dto)
    {
        wellRegistration.LocationPoint4326 = GeometryHelper.CreateLocationPoint4326FromLatLong(dto.Latitude.Value, dto.Longitude.Value);
        wellRegistration.LocationPoint = wellRegistration.LocationPoint4326.ProjectTo2227();
        wellRegistration.ReferenceWellID = dto.ReferenceWellID;
        wellRegistration.ConfirmedWellLocation = false;

        dbContext.SaveChanges();
        dbContext.Entry(wellRegistration).Reload();
    }

    public static void ConfirmWellRegistrationLocation(QanatDbContext dbContext, WellRegistration wellRegistration, ConfirmWellRegistrationLocationDto dto)
    {
        wellRegistration.LocationPoint4326 = GeometryHelper.CreateLocationPoint4326FromLatLong(dto.Latitude.Value, dto.Longitude.Value);
        wellRegistration.LocationPoint = wellRegistration.LocationPoint4326.ProjectTo2227();
        wellRegistration.ConfirmedWellLocation = true;

        dbContext.SaveChanges();
        dbContext.Entry(wellRegistration).Reload();
    }

    public static void UpdateWellRegistrationIrrigatedParcels(QanatDbContext dbContext, WellRegistration wellRegistration, 
        WellRegistrationIrrigatedParcelsRequestDto dto)
    {
        var newIrrigatedParcels = dto.IrrigatedParcelIDs.Select(x => new WellRegistrationIrrigatedParcel()
        {
            ParcelID = x,
            WellRegistrationID = wellRegistration.WellRegistrationID
        }).ToList();

        var existingIrrigatedParcels = wellRegistration.WellRegistrationIrrigatedParcels;

        existingIrrigatedParcels.Merge(newIrrigatedParcels, dbContext.WellRegistrationIrrigatedParcels, 
            (x, y) => x.ParcelID == y.ParcelID && x.WellRegistrationID == y.WellRegistrationID);
        
        wellRegistration.SelectedIrrigatedParcels = true;
        dbContext.SaveChanges();
        dbContext.Entry(wellRegistration).Reload();
    }

    public static void UpdateWellBasicInfo(QanatDbContext dbContext, WellRegistration wellRegistration, WellRegistrationBasicInfoFormDto formDto)
    {
        wellRegistration.WellName = formDto.WellName;
        wellRegistration.StateWCRNumber = formDto.StateWellCompletionNumber;
        wellRegistration.DateDrilled = formDto.DateDrilled;
        wellRegistration.CountyWellPermitNumber = formDto.CountyWellPermit;


        // metadatum
        var wellRegistrationMetadatum = dbContext.WellRegistrationMetadata
            .SingleOrDefault(x => x.WellRegistrationID == wellRegistration.WellRegistrationID);
        if (wellRegistrationMetadatum == null)
        {
            wellRegistrationMetadatum = new WellRegistrationMetadatum()
            {
                WellRegistrationID = wellRegistration.WellRegistrationID
            };
            dbContext.WellRegistrationMetadata.Add(wellRegistrationMetadatum);
        }
        wellRegistrationMetadatum.StateWellNumber = formDto.StateWellNumber;
        wellRegistrationMetadatum.StateWellCompletionNumber = formDto.StateWellCompletionNumber;
        wellRegistrationMetadatum.CountyWellPermit = formDto.CountyWellPermit;

        // merge the water use types
        var waterUseTypes = dbContext.WellRegistrations
            .Include(x => x.WellRegistrationWaterUses)
            .Single(x => x.WellRegistrationID == wellRegistration.WellRegistrationID).WellRegistrationWaterUses
            .ToList();

        var updatedWaterUseTypes = formDto.WaterUseTypes.Where(x => x.Checked)
            .Select(x=> new WellRegistrationWaterUse()
            {
                WellRegistrationID = wellRegistration.WellRegistrationID,
                WellRegistrationWaterUseDescription = x.Description,
                WellRegistrationWaterUseTypeID = x.WaterUseTypeID
            }).ToList();

        waterUseTypes.Merge(updatedWaterUseTypes,dbContext.WellRegistrationWaterUses,
            (x, y) => dbContext.Entry(x).Property(e => e.WellRegistrationWaterUseTypeID).CurrentValue == dbContext.Entry(y).Property(e => e.WellRegistrationWaterUseTypeID).CurrentValue,
            (x,y) => x.WellRegistrationWaterUseDescription = y.WellRegistrationWaterUseDescription);

        dbContext.SaveChanges();
        dbContext.Entry(wellRegistration).Reload();
    }
    public static void UpdateWellRegistrationSupportingInfo(QanatDbContext dbContext, WellRegistration wellRegistration, WellRegistrySupportingInfoDto dto)
    {
        var wellRegistrationMetadatum = dbContext.WellRegistrationMetadata
            .SingleOrDefault(x => x.WellRegistrationID == wellRegistration.WellRegistrationID);
        if (wellRegistrationMetadatum == null)
        {
            wellRegistrationMetadatum = new WellRegistrationMetadatum()
            {
                WellRegistrationID = wellRegistration.WellRegistrationID
            };
            dbContext.WellRegistrationMetadata.Add(wellRegistrationMetadatum);
        }

        wellRegistration.WellDepth = dto.WellDepth;
        wellRegistrationMetadatum.WellDepth = dto.WellDepth;
        wellRegistrationMetadatum.CasingDiameter = dto.CasingDiameter;
        wellRegistrationMetadatum.TopOfPerforations = dto.TopOfPerforations;
        wellRegistrationMetadatum.BottomOfPerforations = dto.BottomOfPerforations;
        wellRegistrationMetadatum.ManufacturerOfWaterMeter = dto.ManufacturerOfWaterMeter;
        wellRegistrationMetadatum.SerialNumberOfWaterMeter = dto.SerialNumberOfWaterMeter;
        wellRegistrationMetadatum.ElectricMeterNumber = dto.ElectricMeterNumber;
        wellRegistrationMetadatum.PumpDischargeDiameter = dto.PumpDischargeDiameter;
        wellRegistrationMetadatum.MotorHorsePower = dto.MotorHorsePower;
        wellRegistrationMetadatum.FuelTypeID = dto.FuelTypeID;
        wellRegistrationMetadatum.FuelOther = dto.FuelOther;
        wellRegistrationMetadatum.MaximumFlow = dto.MaximumFlow;
        wellRegistrationMetadatum.IsEstimatedMax = dto.IsEstimatedMax;
        wellRegistrationMetadatum.TypicalPumpFlow = dto.TypicalPumpFlow;
        wellRegistrationMetadatum.IsEstimatedTypical = dto.IsEstimatedTypical;
        wellRegistrationMetadatum.PumpTestBy = dto.PumpTestBy;
        wellRegistrationMetadatum.PumpTestDatePerformed = dto.PumpTestDatePerformed;
        wellRegistrationMetadatum.PumpManufacturer = dto.PumpManufacturer;
        wellRegistrationMetadatum.PumpYield = dto.PumpYield;
        wellRegistrationMetadatum.PumpStaticLevel = dto.PumpStaticLevel;
        wellRegistrationMetadatum.PumpingLevel = dto.PumpingLevel;

        dbContext.SaveChanges();
        dbContext.Entry(wellRegistration).Reload();
    }

    public static void UpsertWellContacts(QanatDbContext dbContext, WellRegistration wellRegistration, WellRegistrationContactsUpsertDto dto)
    {
        var currentContacts = dbContext.WellRegistrations
            .Include(x => x.WellRegistrationContacts)
            .Single(x => x.WellRegistrationID == wellRegistration.WellRegistrationID).WellRegistrationContacts;

        var landownerWellContact = currentContacts.SingleOrDefault(x => x.WellRegistrationContactType == WellRegistrationContactType.Landowner);

        if (landownerWellContact == null)
        {
            landownerWellContact = new WellRegistrationContact();
            landownerWellContact.WellRegistrationID = wellRegistration.WellRegistrationID;
            landownerWellContact.WellRegistrationContactTypeID = (int)WellRegistrationContactTypeEnum.Landowner;
            dbContext.WellRegistrationContacts.Add(landownerWellContact);
        }
        landownerWellContact.ContactName = dto.LandownerContactName;
        landownerWellContact.BusinessName = dto.LandownerBusinessName;
        landownerWellContact.StreetAddress = dto.LandownerStreetAddress;
        landownerWellContact.City = dto.LandownerCity;
        landownerWellContact.StateID = (int)dto.LandownerStateID;
        landownerWellContact.ZipCode = dto.LandownerZipCode;
        landownerWellContact.Phone = dto.LandownerPhone;
        landownerWellContact.Email = dto.LandownerEmail;

        var ownerOperatorWellContact = currentContacts
            .SingleOrDefault(x => x.WellRegistrationContactType == WellRegistrationContactType.OwnerOperator);
        if (ownerOperatorWellContact == null)
        {
            ownerOperatorWellContact = new WellRegistrationContact();
            ownerOperatorWellContact.WellRegistrationID = wellRegistration.WellRegistrationID;
            ownerOperatorWellContact.WellRegistrationContactTypeID = (int)WellRegistrationContactTypeEnum.OwnerOperator;
            dbContext.WellRegistrationContacts.Add(ownerOperatorWellContact);
        }
        ownerOperatorWellContact.ContactName = dto.OwnerOperatorContactName;
        ownerOperatorWellContact.BusinessName = dto.OwnerOperatorBusinessName;
        ownerOperatorWellContact.StreetAddress = dto.OwnerOperatorStreetAddress;
        ownerOperatorWellContact.City = dto.OwnerOperatorCity;
        ownerOperatorWellContact.StateID = (int)dto.OwnerOperatorStateID;
        ownerOperatorWellContact.ZipCode = dto.OwnerOperatorZipCode;
        ownerOperatorWellContact.Phone = dto.OwnerOperatorPhone;
        ownerOperatorWellContact.Email = dto.OwnerOperatorEmail;

        dbContext.SaveChanges();
        dbContext.Entry(wellRegistration).Reload();
    }

    public static void AddAttachment(QanatDbContext dbContext, WellRegistration wellRegistration, WellRegistrationFileResourceUpsertDto dto, FileResource fileResource)
    {
        WellRegistrationFileResources.Create(dbContext, fileResource.FileResourceID, dto);

        dbContext.SaveChanges();
        dbContext.Entry(wellRegistration).Reload();
    }

    public static void UpdateAttachment(QanatDbContext dbContext, WellRegistration wellRegistration, WellRegistrationFileResourceUpdateDto dto)
    {
        var wellFileResource = dbContext.WellRegistrationFileResources
            .Single(x => x.WellRegistrationFileResourceID == dto.WellRegistrationFileResourceID);
        wellFileResource.FileDescription = dto.FileDescription;

        dbContext.SaveChanges();
        dbContext.Entry(wellRegistration).Reload();
    }

    public static void DeleteWellRegistry(QanatDbContext dbContext, WellRegistration wellRegistration)
    {
        dbContext.WellRegistrationContacts.RemoveRange(wellRegistration.WellRegistrationContacts);
        dbContext.WellRegistrationFileResources.RemoveRange(wellRegistration.WellRegistrationFileResources);
        dbContext.WellRegistrationWaterUses.RemoveRange(wellRegistration.WellRegistrationWaterUses);
        dbContext.WellRegistrationIrrigatedParcels.RemoveRange(wellRegistration.WellRegistrationIrrigatedParcels);
        if (wellRegistration.WellRegistrationMetadatum != null)
        {
            dbContext.WellRegistrationMetadata.Remove(wellRegistration.WellRegistrationMetadatum);
        }
        dbContext.WellRegistrations.Remove(wellRegistration);
        dbContext.SaveChanges();
    }

    public static void SubmitWell(QanatDbContext dbContext, WellRegistration wellRegistration)
    {
        wellRegistration.SubmitDate = DateTime.UtcNow;
        wellRegistration.WellRegistrationStatusID = WellRegistrationStatus.Submitted.WellRegistrationStatusID;

        dbContext.SaveChanges();
        dbContext.Entry(wellRegistration).Reload();
    }

    public static void ApproveWellRegistration(QanatDbContext dbContext, WellRegistration wellRegistration)
    {
        var well = Wells.CreateFromWellRegistration(dbContext, wellRegistration);

        wellRegistration.ApprovalDate = DateTime.UtcNow;
        wellRegistration.WellRegistrationStatusID = WellRegistrationStatus.Approved.WellRegistrationStatusID;
        wellRegistration.WellID = well.WellID;

        dbContext.SaveChanges();
        dbContext.Entry(wellRegistration).Reload();

    }

    public static void ReturnWell(QanatDbContext dbContext, WellRegistration wellRegistration)
    {
        wellRegistration.WellRegistrationStatusID = WellRegistrationStatus.Returned.WellRegistrationStatusID;

        dbContext.SaveChanges();
        dbContext.Entry(wellRegistration).Reload();
    }
}