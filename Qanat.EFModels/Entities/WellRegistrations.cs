using Microsoft.EntityFrameworkCore;
using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities;

public static class WellRegistrations
{
    public static WellRegistration GetByID(QanatDbContext dbContext, int wellRegistrationID)
    {
        return dbContext.WellRegistrations
            .Include(x => x.Parcel)
            .ThenInclude(x => x.ParcelGeometry)
            .Include(x => x.Geography).AsNoTracking()
            .SingleOrDefault(x => x.WellRegistrationID == wellRegistrationID);
    }

    public static List<WellRegistrationGridRowDto> ListByGeographyID(QanatDbContext dbContext, int geographyID)
    {
        var wells = dbContext.WellRegistrations
            .Include(x => x.Parcel)
            .Include(x => x.CreateUser)
            .Include(x => x.WellRegistrationMetadatum)
            .Include(x => x.WellRegistrationContacts)
            .Include(x => x.WellRegistrationIrrigatedParcels).ThenInclude(x => x.Parcel)
            .Include(x => x.WellRegistrationWaterUses)
            .Where(x => x.GeographyID == geographyID).ToList();

        var wellRegistrationDtos = wells
            .Select(x => new WellRegistrationGridRowDto()
            {
                WellRegistrationID = x.WellRegistrationID,
                GeographyID = x.GeographyID,
                WellID = x.WellID,
                WellName = x.WellName,
                WellRegistrationStatusID = x.WellRegistrationStatusID,
                ParcelID = x.ParcelID,
                ParcelNumber = x.Parcel?.ParcelNumber,
                StateWCRNumber = x.StateWCRNumber,
                CountyWellPermitNumber = x.CountyWellPermitNumber,
                DateDrilled = x.DateDrilled,
                SubmitDate = x.SubmitDate,
                ApprovalDate = x.ApprovalDate,
                CreateUserEmail = x.CreateUserEmail,
                CreateUserName = x.CreateUser?.FullName,
                Latitude = x.Latitude,
                Longitude = x.Longitude,
                IrrigatedParcels = x.WellRegistrationIrrigatedParcels.Select(y => new WellRegistrationGridRowDto.WellRegistrationGridRowIrrigatedParcelDto()
                {
                    ParcelID = y.ParcelID,
                    ParcelNumber = y.Parcel.ParcelNumber
                }).ToList(),
                LandownerName = x.WellRegistrationContacts.SingleOrDefault(y => y.WellRegistrationContactTypeID == WellRegistrationContactType.Landowner.WellRegistrationContactTypeID)?.ContactName,
                LandownerBusinessName = x.WellRegistrationContacts.SingleOrDefault(y => y.WellRegistrationContactTypeID == WellRegistrationContactType.Landowner.WellRegistrationContactTypeID)?.BusinessName,
                LandownerStreetAddress = x.WellRegistrationContacts.SingleOrDefault(y => y.WellRegistrationContactTypeID == WellRegistrationContactType.Landowner.WellRegistrationContactTypeID)?.StreetAddress,
                LandownerCity = x.WellRegistrationContacts.SingleOrDefault(y => y.WellRegistrationContactTypeID == WellRegistrationContactType.Landowner.WellRegistrationContactTypeID)?.City,
                LandownerState = x.WellRegistrationContacts.SingleOrDefault(y => y.WellRegistrationContactTypeID == WellRegistrationContactType.Landowner.WellRegistrationContactTypeID)?.State.StateName,
                LandownerZipCode = x.WellRegistrationContacts.SingleOrDefault(y => y.WellRegistrationContactTypeID == WellRegistrationContactType.Landowner.WellRegistrationContactTypeID)?.ZipCode,
                LandownerPhone = x.WellRegistrationContacts.SingleOrDefault(y => y.WellRegistrationContactTypeID == WellRegistrationContactType.Landowner.WellRegistrationContactTypeID)?.Phone,
                LandownerEmail = x.WellRegistrationContacts.SingleOrDefault(y => y.WellRegistrationContactTypeID == WellRegistrationContactType.Landowner.WellRegistrationContactTypeID)?.Email,
                OwnerOperatorName = x.WellRegistrationContacts.SingleOrDefault(y => y.WellRegistrationContactTypeID == WellRegistrationContactType.OwnerOperator.WellRegistrationContactTypeID)?.ContactName,
                OwnerOperatorBusinessName = x.WellRegistrationContacts.SingleOrDefault(y => y.WellRegistrationContactTypeID == WellRegistrationContactType.OwnerOperator.WellRegistrationContactTypeID)?.BusinessName,
                OwnerOperatorStreetAddress = x.WellRegistrationContacts.SingleOrDefault(y => y.WellRegistrationContactTypeID == WellRegistrationContactType.OwnerOperator.WellRegistrationContactTypeID)?.StreetAddress,
                OwnerOperatorCity = x.WellRegistrationContacts.SingleOrDefault(y => y.WellRegistrationContactTypeID == WellRegistrationContactType.OwnerOperator.WellRegistrationContactTypeID)?.City,
                OwnerOperatorState = x.WellRegistrationContacts.SingleOrDefault(y => y.WellRegistrationContactTypeID == WellRegistrationContactType.OwnerOperator.WellRegistrationContactTypeID)?.State.StateName,
                OwnerOperatorZipCode = x.WellRegistrationContacts.SingleOrDefault(y => y.WellRegistrationContactTypeID == WellRegistrationContactType.OwnerOperator.WellRegistrationContactTypeID)?.ZipCode,
                OwnerOperatorPhone = x.WellRegistrationContacts.SingleOrDefault(y => y.WellRegistrationContactTypeID == WellRegistrationContactType.OwnerOperator.WellRegistrationContactTypeID)?.Phone,
                OwnerOperatorEmail = x.WellRegistrationContacts.SingleOrDefault(y => y.WellRegistrationContactTypeID == WellRegistrationContactType.OwnerOperator.WellRegistrationContactTypeID)?.Email,
                WellRegistrationMetadatum = x.WellRegistrationMetadatum?.AsSimpleDto(),
                AgriculturalWaterUse = x.WellRegistrationWaterUses.SingleOrDefault(y => y.WellRegistrationWaterUseTypeID == WellRegistrationWaterUseType.Agricultural.WellRegistrationWaterUseTypeID) != null,
                AgriculturalWaterUseDescription = x.WellRegistrationWaterUses.SingleOrDefault(y => y.WellRegistrationWaterUseTypeID == WellRegistrationWaterUseType.Agricultural.WellRegistrationWaterUseTypeID)?.WellRegistrationWaterUseDescription,
                StockWateringWaterUse = x.WellRegistrationWaterUses.SingleOrDefault(y => y.WellRegistrationWaterUseTypeID == WellRegistrationWaterUseType.StockWatering.WellRegistrationWaterUseTypeID) != null,
                StockWateringWaterUseDescription = x.WellRegistrationWaterUses.SingleOrDefault(y => y.WellRegistrationWaterUseTypeID == WellRegistrationWaterUseType.StockWatering.WellRegistrationWaterUseTypeID)?.WellRegistrationWaterUseDescription,
                DomesticWaterUse = x.WellRegistrationWaterUses.SingleOrDefault(y => y.WellRegistrationWaterUseTypeID == WellRegistrationWaterUseType.Domestic.WellRegistrationWaterUseTypeID) != null,
                DomesticWaterUseDescription = x.WellRegistrationWaterUses.SingleOrDefault(y => y.WellRegistrationWaterUseTypeID == WellRegistrationWaterUseType.Domestic.WellRegistrationWaterUseTypeID)?.WellRegistrationWaterUseDescription,
                PublicMunicipalWaterUse = x.WellRegistrationWaterUses.SingleOrDefault(y => y.WellRegistrationWaterUseTypeID == WellRegistrationWaterUseType.PublicMunicipal.WellRegistrationWaterUseTypeID) != null,
                PublicMunicipalWaterUseDescription = x.WellRegistrationWaterUses.SingleOrDefault(y => y.WellRegistrationWaterUseTypeID == WellRegistrationWaterUseType.PublicMunicipal.WellRegistrationWaterUseTypeID)?.WellRegistrationWaterUseDescription,
                PrivateMunicipalWaterUse = x.WellRegistrationWaterUses.SingleOrDefault(y => y.WellRegistrationWaterUseTypeID == WellRegistrationWaterUseType.PrivateMunicipal.WellRegistrationWaterUseTypeID) != null,
                PrivateMunicipalWaterUseDescription = x.WellRegistrationWaterUses.SingleOrDefault(y => y.WellRegistrationWaterUseTypeID == WellRegistrationWaterUseType.PrivateMunicipal.WellRegistrationWaterUseTypeID)?.WellRegistrationWaterUseDescription,
                OtherWaterUse = x.WellRegistrationWaterUses.SingleOrDefault(y => y.WellRegistrationWaterUseTypeID == WellRegistrationWaterUseType.Other.WellRegistrationWaterUseTypeID) != null,
                OtherWaterUseDescription = x.WellRegistrationWaterUses.SingleOrDefault(y => y.WellRegistrationWaterUseTypeID == WellRegistrationWaterUseType.Other.WellRegistrationWaterUseTypeID)?.WellRegistrationWaterUseDescription,
            }).ToList();

        return wellRegistrationDtos;
    }

    public static List<WellRegistrationUserDetailDto> ListByUserAsWellRegistrationUserDetailDto(QanatDbContext dbContext, int userID)
    {
        return dbContext.WellRegistrations.AsNoTracking()
            .Include(x => x.Geography)
            .Include(x => x.Parcel)
            .Where(x => x.CreateUserID == userID)
            .ToList()
            .Select(x => new WellRegistrationUserDetailDto()
            {
                WellRegistrationID = x.WellRegistrationID,
                Geography = x.Geography.AsDisplayDto(),
                Parcel = x.Parcel?.AsDisplayDto(),
                WellRegistrationStatus = x.WellRegistrationStatus.AsSimpleDto(),
                WellName = x.WellName,
                StateWCRNumber = x.StateWCRNumber,
                CountyWellPermitNumber = x.CountyWellPermitNumber,
                DateDrilled = x.DateDrilled,
                WellDepth = x.WellDepth,
                SubmitDate = x.SubmitDate,
                ApprovalDate = x.ApprovalDate,
                CreateUserID = x.CreateUserID,
                Latitude = x.Latitude,
                Longitude = x.Longitude
            }).ToList();
    }

    public static List<SubmittedWellRegistrationListItemDto> ListSubmittedWellsByGeographyID(QanatDbContext dbContext, int geographyID)
    {
        var wellRegistrations = dbContext.WellRegistrations.AsNoTracking()
            .Include(x => x.Parcel)
            .Include(x => x.CreateUser)
            .Where(x => x.WellRegistrationStatusID == WellRegistrationStatus.Submitted.WellRegistrationStatusID && x.GeographyID == geographyID)
            .Select(x => new SubmittedWellRegistrationListItemDto()
            {
                WellRegistrationID = x.WellRegistrationID,
                APN = x.Parcel != null ? x.Parcel.ParcelNumber : null,
                ParcelID = x.ParcelID,
                CreatedBy = x.CreateUser.FullName,
                CreatedByUserID = x.CreateUserID,
                DateSubmitted = x.SubmitDate,
                WellName = x.WellName,
            }).ToList();
        return wellRegistrations;
    }

    public static WellRegistrationBasicInfoFormDto GetWellRegistrationBasicInfoDto(QanatDbContext dbContext, int wellRegistrationID)
    {
        var wellRegistration = dbContext.WellRegistrations
            .Include(x => x.ReferenceWell)
            .Include(x => x.WellRegistrationMetadatum)
            .Include(x => x.WellRegistrationWaterUses)
            .SingleOrDefault(x => x.WellRegistrationID == wellRegistrationID);

        if (wellRegistration == null)
        {
            return new WellRegistrationBasicInfoFormDto();
        }

        return new WellRegistrationBasicInfoFormDto()
        {
            WellName = wellRegistration.WellName,
            StateWellNumber = wellRegistration.WellRegistrationMetadatum?.StateWellNumber,
            StateWellCompletionNumber = wellRegistration.StateWCRNumber,
            CountyWellPermit = wellRegistration.CountyWellPermitNumber,
            DateDrilled = wellRegistration.DateDrilled,
            ReferenceWell = wellRegistration.ReferenceWell?.AsSimpleDto(),
            WaterUseTypes = WellRegistrationWaterUseType.AllAsSimpleDto.Select(x =>
            {
                var wellWaterUseType = wellRegistration.WellRegistrationWaterUses
                        .SingleOrDefault(y => y.WellRegistrationWaterUseTypeID == x.WellRegistrationWaterUseTypeID);
                return new WellRegistrationBasicInfoFormDto.WaterUseTypesUsed
                {
                    WaterUseTypeID = x.WellRegistrationWaterUseTypeID, 
                    Description = wellWaterUseType?.WellRegistrationWaterUseDescription,
                    Checked = wellWaterUseType != null
                };
            }).ToList()
        };
    }

    public static WellRegistrationIrrigatedParcelsResponseDto GetWellRegistrationIrrigatedParcelsDto(QanatDbContext dbContext, int wellRegistrationID)
    {
        var wellRegistration = dbContext.WellRegistrations
            .Include(x => x.WellRegistrationIrrigatedParcels)
                .ThenInclude(x => x.Parcel)
                    .ThenInclude(x => x.WaterAccount)
            .SingleOrDefault(x => x.WellRegistrationID == wellRegistrationID);

        return wellRegistration.AsWellRegistrationIrrigatedParcelsDto();
    }

    public static ConfirmWellRegistrationLocationDto GetConfirmWellRegistrationLocationDto(QanatDbContext dbContext, int wellRegistrationID)
    {
        var wellRegistration = dbContext.WellRegistrations
            .Include(x =>x.Parcel).ThenInclude(x => x.ParcelGeometry)
            .Include(x => x.WellRegistrationIrrigatedParcels).ThenInclude(x => x.Parcel)
            .SingleOrDefault(x => x.WellRegistrationID == wellRegistrationID);

        return wellRegistration.AsConfirmWellRegistrationLocationDto();
    }

    public static WellRegistrySupportingInfoDto GetWellRegistrySupportingInfoDto(QanatDbContext dbContext, int wellRegistrationID)
    {
        var wellRegistration = dbContext.WellRegistrations
            .Include(x => x.WellRegistrationMetadatum)
            .Include(x => x.WellRegistrationWaterUses)
            .SingleOrDefault(x => x.WellRegistrationID == wellRegistrationID);

        if (wellRegistration == null)
        {
            return new WellRegistrySupportingInfoDto();
        }

        return new WellRegistrySupportingInfoDto()
        {
            WellDepth = wellRegistration.WellDepth,
            CasingDiameter = wellRegistration.WellRegistrationMetadatum?.CasingDiameter,
            TopOfPerforations = wellRegistration.WellRegistrationMetadatum?.TopOfPerforations,
            BottomOfPerforations = wellRegistration.WellRegistrationMetadatum?.BottomOfPerforations,
            ManufacturerOfWaterMeter = wellRegistration.WellRegistrationMetadatum?.ManufacturerOfWaterMeter,
            SerialNumberOfWaterMeter = wellRegistration.WellRegistrationMetadatum?.SerialNumberOfWaterMeter,
            ElectricMeterNumber = wellRegistration.WellRegistrationMetadatum?.ElectricMeterNumber,
            PumpDischargeDiameter = wellRegistration.WellRegistrationMetadatum?.PumpDischargeDiameter,
            MotorHorsePower = wellRegistration.WellRegistrationMetadatum?.MotorHorsePower,
            FuelTypeID = wellRegistration.WellRegistrationMetadatum?.FuelTypeID,
            FuelOther = wellRegistration.WellRegistrationMetadatum?.FuelOther,
            MaximumFlow = wellRegistration.WellRegistrationMetadatum?.MaximumFlow,
            IsEstimatedMax = wellRegistration.WellRegistrationMetadatum?.IsEstimatedMax,
            TypicalPumpFlow = wellRegistration.WellRegistrationMetadatum?.TypicalPumpFlow,
            IsEstimatedTypical = wellRegistration.WellRegistrationMetadatum?.IsEstimatedTypical,
            PumpTestBy = wellRegistration.WellRegistrationMetadatum?.PumpTestBy,
            PumpTestDatePerformed = wellRegistration.WellRegistrationMetadatum?.PumpTestDatePerformed,
            PumpManufacturer = wellRegistration.WellRegistrationMetadatum?.PumpManufacturer,
            PumpYield = wellRegistration.WellRegistrationMetadatum?.PumpYield,
            PumpStaticLevel = wellRegistration.WellRegistrationMetadatum?.PumpStaticLevel,
            PumpingLevel = wellRegistration.WellRegistrationMetadatum?.PumpingLevel
        };
    }

    public static WellRegistration GetByIDWithTrackingForWorkflow(QanatDbContext dbContext, int wellRegistrationID)
    {
        return dbContext.WellRegistrations
            .Include(x => x.Parcel)
            .ThenInclude(x => x.ParcelGeometry)
            .Include(x => x.WellRegistrationIrrigatedParcels)
            .Include(x => x.WellRegistrationFileResources).ThenInclude(x => x.FileResource)
            .Include(x => x.WellRegistrationWaterUses)
            .Include(x => x.WellRegistrationContacts)
            .Include(x => x.WellRegistrationMetadatum)
            .Include(x => x.Geography)
            .SingleOrDefault(x => x.WellRegistrationID == wellRegistrationID);
    }

    public static bool IsLandownerSameAsOwnerOperator(WellRegistrationContactSimpleDto landownerDto,
        WellRegistrationContactSimpleDto ownerOperatorDto)
    {
        if (landownerDto.ContactName == ownerOperatorDto.ContactName &&
            landownerDto.BusinessName == ownerOperatorDto.BusinessName &&
            landownerDto.StreetAddress == ownerOperatorDto.StreetAddress &&
            landownerDto.City == ownerOperatorDto.City &&
            landownerDto.StateID == ownerOperatorDto.StateID &&
            landownerDto.ZipCode == ownerOperatorDto.ZipCode &&
            landownerDto.Phone == ownerOperatorDto.Phone &&
            landownerDto.Email == ownerOperatorDto.Email)
        {
            return false;
        }

        return true;
    }

    public static WellRegistrationDetailedDto GetByIDAsDetailedDto(QanatDbContext dbContext, int wellRegistrationID)
    {
        var wellRegistration = dbContext.WellRegistrations.AsNoTracking()
            .Include(x => x.Parcel).ThenInclude(x => x.ParcelGeometry)
            .Include(x => x.Geography)
            .SingleOrDefault(x => x.WellRegistrationID == wellRegistrationID);

        var irrigatedParcelIDs = dbContext.WellRegistrationIrrigatedParcels.Where(x => x.WellRegistrationID == wellRegistrationID)
            .Select(x => x.ParcelID).ToList();
        var irrigatedParcels = dbContext.Parcels.Where(x => irrigatedParcelIDs.Contains(x.ParcelID))
            .Select(x => x.AsDisplayDto()).ToList();
        var landownerContact = dbContext.WellRegistrationContacts.SingleOrDefault(x =>
            x.WellRegistrationID == wellRegistrationID && x.WellRegistrationContactTypeID == (int)WellRegistrationContactTypeEnum.Landowner);
        var operatorContact = dbContext.WellRegistrationContacts.SingleOrDefault(x =>
            x.WellRegistrationID == wellRegistrationID && x.WellRegistrationContactTypeID == (int)WellRegistrationContactTypeEnum.OwnerOperator);
        var wellRegistrationMetadatum = dbContext.WellRegistrationMetadata.SingleOrDefault(x => x.WellRegistrationID == wellRegistrationID);
        var wellRegistrationWaterUses = dbContext.WellRegistrationWaterUses.Where(x => x.WellRegistrationID == wellRegistrationID).ToList();
        var wellFileResources = dbContext.WellRegistrationFileResources.Include(x => x.FileResource)
            .Where(x => x.WellRegistrationID == wellRegistrationID).ToList();

        var wellRegistrationDetailedDto = new WellRegistrationDetailedDto()
        {
            WellRegistrationID = wellRegistration.WellRegistrationID,
            GeographyID = wellRegistration.GeographyID,
            Geography = wellRegistration.Geography.AsDisplayDto(),
            WellID = wellRegistration.WellID,
            WellName = wellRegistration.WellName,
            WellStatusRegistrationID = wellRegistration.WellRegistrationStatusID,
            WellRegistrationStatus = wellRegistration.WellRegistrationStatus.AsSimpleDto(),
            ParcelID = wellRegistration.ParcelID,
            Parcel = wellRegistration.Parcel.AsDisplayDto(),
            StateWCRNumber = wellRegistration.StateWCRNumber,
            CountyWellPermitNumber = wellRegistration.CountyWellPermitNumber,
            DateDrilled = wellRegistration.DateDrilled,
            WellDepth = wellRegistration.WellDepth,
            SubmitDate = wellRegistration.SubmitDate,
            ApprovalDate = wellRegistration.ApprovalDate,
            CreateUserGuid = wellRegistration.CreateUserGuid,
            CreateUserEmail = wellRegistration.CreateUserEmail,
            FairyshrimpWellID = wellRegistration.FairyshrimpWellID,
            Latitude = wellRegistration.Latitude,
            Longitude = wellRegistration.Longitude,
            CreateUserID = wellRegistration.CreateUserID,
            IrrigatedParcels = irrigatedParcels,
        };

        if (landownerContact != null)
        {
            wellRegistrationDetailedDto.LandownerContact = new WellRegistrationContactWithStateDto()
            {
                WellRegistrationContactID = landownerContact.WellRegistrationContactID,
                WellRegistrationID = landownerContact.WellRegistrationID,
                WellRegistrationContactTypeID = landownerContact.WellRegistrationContactTypeID,
                ContactName = landownerContact.ContactName,
                BusinessName = landownerContact.BusinessName,
                StreetAddress = landownerContact.StreetAddress,
                City = landownerContact.City,
                StateID = landownerContact.StateID,
                StateName = State.AllLookupDictionary[landownerContact.StateID].StateName,
                ZipCode = landownerContact.ZipCode,
                Phone = landownerContact.Phone,
                Email = landownerContact.Email
            };
        }

        if (operatorContact != null)
        {
            wellRegistrationDetailedDto.OwnerOperatorContact = new WellRegistrationContactWithStateDto()
            {
                WellRegistrationContactID = operatorContact.WellRegistrationContactID,
                WellRegistrationID = operatorContact.WellRegistrationID,
                WellRegistrationContactTypeID = operatorContact.WellRegistrationContactTypeID,
                ContactName = operatorContact.ContactName,
                BusinessName = operatorContact.BusinessName,
                StreetAddress = operatorContact.StreetAddress,
                City = operatorContact.City,
                StateID = operatorContact.StateID,
                StateName = State.AllLookupDictionary[landownerContact.StateID].StateName,
                ZipCode = operatorContact.ZipCode,
                Phone = operatorContact.Phone,
                Email = operatorContact.Email
            };
        }

        if (wellRegistrationMetadatum != null)
        {
            wellRegistrationDetailedDto.WellRegistrationMetadatum = wellRegistrationMetadatum.AsSimpleDto();
        }

        if (wellRegistrationWaterUses != null)
        {
            wellRegistrationDetailedDto.WellRegistrationWaterUses = wellRegistrationWaterUses.Select(x => new WellRegistrationWaterUseDisplayDto()
            {
                WellRegistrationWaterUseID = x.WellRegistrationWaterUseID,
                WellRegistrationID = x.WellRegistrationID,
                WellRegistrationWaterUseTypeID = x.WellRegistrationWaterUseTypeID,
                WellRegistrationWaterUseTypeDisplayName = WellRegistrationWaterUseType.AllLookupDictionary[x.WellRegistrationWaterUseTypeID].WellRegistrationWaterUseTypeDisplayName,
                WellRegistrationWaterUseDescription = x.WellRegistrationWaterUseDescription,
            }).ToList();
        }

        if (wellFileResources != null)
        {
            wellRegistrationDetailedDto.WellRegistrationFileResources = wellFileResources.Select(x => new WellRegistrationFileResourceDto()
            {
                FileDescription = x.FileDescription,
                FileResource = x.FileResource.AsSimpleDto(),
                FileResourceID = x.FileResourceID,
                WellRegistrationFileResourceID = x.WellRegistrationFileResourceID,
            }).ToList();
        }

        return wellRegistrationDetailedDto;
    }
}