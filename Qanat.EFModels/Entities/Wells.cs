using System.Text.Json;
using Qanat.Models.DataTransferObjects;
using Microsoft.EntityFrameworkCore;
using Qanat.Common.GeoSpatial;
using Qanat.Common.Util;
using Schemoto.Helpers;
using Schemoto.InstanceNamespace;

namespace Qanat.EFModels.Entities;

public class Wells
{
    public static List<WellMinimalDto> ListByGeographyIDAsMinimalDto(QanatDbContext dbContext, int geographyID) //todo: fix return type
    {
        return dbContext.Wells
            .Include(x => x.Parcel).ThenInclude(x => x.ParcelGeometry)
            .Include(x => x.WellIrrigatedParcels).ThenInclude(x => x.Parcel)
            .Include(x => x.Geography)
            .Include(x => x.WellMeters).ThenInclude(x => x.Meter)
            .AsNoTracking()
            .Where(x => x.GeographyID == geographyID)
            .Select(x => x.AsMinimalDto())
            .ToList();
    }

    public static List<WellDisplayDto> ListByGeographyIDAsDisplayDto(QanatDbContext dbContext, int geographyID)
    {
        return dbContext.Wells
            .AsNoTracking()
            .Where(x => x.GeographyID == geographyID)
            .Select(x => x.AsDisplayDto())
            .ToList();
    }

    public static List<WellMinimalDto> ListByWaterAccountIDAsMinimalDto(QanatDbContext dbContext, int waterAccountID)
    {
        return dbContext.Wells.AsNoTracking()
            .Include(x => x.Parcel)
            .Include(x => x.WellIrrigatedParcels).ThenInclude(x => x.Parcel)
            .Where(x => x.ParcelID.HasValue && x.Parcel.WaterAccountID == waterAccountID)
            .Select(x => x.AsMinimalDto()).ToList();
    }


    public static List<WellMinimalDto> ListByUserIDAndGeographyIDAsMinimalDto(QanatDbContext dbContext, int geographyID, int userID)
    {
        var waterAccountIDs = dbContext.fWaterAccountUser(userID).Where(x => x.GeographyID == geographyID)
            .Select(x => x.WaterAccountID).ToList();

        var parcelIDs = dbContext.Parcels.AsNoTracking()
            .Where(x => x.GeographyID == geographyID && x.WaterAccountID.HasValue && waterAccountIDs.Contains(x.WaterAccountID.Value))
            .Select(x => x.ParcelID).ToList();

        return dbContext.Wells.AsNoTracking()
            .Include(x => x.Parcel)
            .Include(x => x.WellIrrigatedParcels).ThenInclude(x => x.Parcel)
            .Include(x => x.WellMeters).ThenInclude(x => x.Meter)
            .Where(x => x.GeographyID == geographyID && x.ParcelID.HasValue && parcelIDs.Contains(x.ParcelID.Value))
            .Select(x => x.AsMinimalDto())
            .ToList();
    }

    public static Well GetByID(QanatDbContext dbContext, int wellID)
    {
        return dbContext.Wells.AsNoTracking()
            .Include(x => x.Parcel).ThenInclude(x => x.ParcelGeometry)
            .Include(x => x.Geography)
            .Include(x => x.WellIrrigatedParcels).ThenInclude(x => x.Parcel)
            .SingleOrDefault(x => x.WellID == wellID);
    }

    public static WellDisplayDto GetByIDAsDisplayDto(QanatDbContext dbContext, int wellID)
    {
        return dbContext.Wells.AsNoTracking()
            .SingleOrDefault(x => x.WellID == wellID).AsDisplayDto();
    }

    public static BoundingBoxDto GetBoundingBoxByWellIDs(QanatDbContext dbContext, List<int> wellIDs)
    {
        var wells = dbContext.Wells.AsNoTracking()
            .Where(x => wellIDs.Contains(x.WellID));

        var geometries = wells.Select(x => x.LocationPoint4326).ToList();
        return new BoundingBoxDto(geometries);
    }

    public static Well CreateFromWellRegistration(QanatDbContext dbContext, WellRegistration wellRegistration)
    {
        var wellRegistrationMetadatum = dbContext.WellRegistrationMetadata
            .AsNoTracking()
            .SingleOrDefault(x => x.WellRegistrationID == wellRegistration.WellRegistrationID);

        var well = new Well()
        {
            GeographyID = wellRegistration.GeographyID,
            ParcelID = wellRegistration.ParcelID,
            ParcelIsManualOverride = false,
            WellName = wellRegistration.WellName,
            LocationPoint = wellRegistration.LocationPoint,
            LocationPoint4326 = wellRegistration.LocationPoint4326,
            StateWCRNumber = wellRegistration.StateWCRNumber,
            CountyWellPermitNumber = wellRegistration.CountyWellPermitNumber,
            DateDrilled = wellRegistration.DateDrilled,
            WellDepth = wellRegistration.WellDepth,
            CasingDiameter = wellRegistrationMetadatum?.CasingDiameter,
            TopOfPerforations = wellRegistrationMetadatum?.TopOfPerforations,
            BottomOfPerforations = wellRegistrationMetadatum?.BottomOfPerforations,
            ElectricMeterNumber = wellRegistrationMetadatum?.ElectricMeterNumber,
            CreateDate = DateTime.UtcNow
        };

        dbContext.Wells.Add(well);
        dbContext.SaveChanges();
        dbContext.Entry(well).Reload();

        var wellRegistrationIrrigatedParcels = dbContext.WellRegistrationIrrigatedParcels.AsNoTracking()
            .Where(x => x.WellRegistrationID == wellRegistration.WellRegistrationID);

        var wellIrrigatedParcels = new List<WellIrrigatedParcel>();
        foreach (var wellRegistrationIrrigatedParcel in wellRegistrationIrrigatedParcels)
        {
            wellIrrigatedParcels.Add(new WellIrrigatedParcel()
            {
                WellID = well.WellID,
                ParcelID = wellRegistrationIrrigatedParcel.ParcelID,
            });
        }

        dbContext.WellIrrigatedParcels.AddRange(wellIrrigatedParcels);
        dbContext.SaveChanges();

        return well;
    }

    public static void UpdateWellDetails(QanatDbContext dbContext, ManagerWellUpdateRequestDto requestDto)
    {
        var well = dbContext.Wells.Single(x => x.WellID == requestDto.WellID);

        well.StateWCRNumber = requestDto.StateWCRNumber;
        well.CountyWellPermitNumber = requestDto.CountyWellPermitNumber;
        well.DateDrilled = requestDto.DateDrilled;
        well.WellStatusID = requestDto.WellStatusID;
        well.Notes = requestDto.Notes;
        well.WellDepth = requestDto.WellDepth;
        well.CasingDiameter = requestDto.CasingDiameter;
        well.TopOfPerforations = requestDto.TopOfPerforations;
        well.BottomOfPerforations = requestDto.BottomOfPerforations;
        well.ElectricMeterNumber = requestDto.ElectricMeterNumber;

        dbContext.SaveChanges();
    }

    public static List<ErrorMessage> ValidateWellParcelOverride(QanatDbContext dbContext, WellParcelDto wellParcelDto)
    {
        var errors = new List<ErrorMessage>();

        var well = dbContext.Wells.SingleOrDefault(x => x.WellID == wellParcelDto.WellID);
        var parcel = dbContext.Parcels.SingleOrDefault(x => x.ParcelID == wellParcelDto.ParcelID);

        if (well == null)
        {
            errors.Add(new ErrorMessage() { Type = "Well", Message = $"Well with ID {wellParcelDto.WellID} does not exist." });
        }
        if (parcel == null)
        {
            errors.Add(new ErrorMessage() { Type = "Parcel", Message = $"Parcel with ID {wellParcelDto.ParcelID} does not exist." });
        }
        if (errors.Any())
        {
            return errors;
        }

        if (well.GeographyID != parcel.GeographyID)
        {
            errors.Add(new ErrorMessage() { Type = "Geography", Message = $"Well and parcel must belong to the same geography." });
        }

        return errors;
    }

    public static void OverrideWellParcel(QanatDbContext dbContext, WellParcelDto wellParcelDto)
    {
        var well = dbContext.Wells.Single(x => x.WellID == wellParcelDto.WellID);

        well.ParcelID = wellParcelDto.ParcelID;
        well.ParcelIsManualOverride = true;

        dbContext.SaveChanges();
    }

    public static Parcel GetWellParcelFromGeometryIntersection(QanatDbContext dbContext, WellLocationDto dto)
    {
        var locationPoint4326 = GeometryHelper.CreateLocationPoint4326FromLatLong(dto.Latitude.Value, dto.Longitude.Value);
        var parcel = dbContext.Parcels.AsNoTracking()
            .FirstOrDefault(x => x.GeographyID == dto.GeographyID && x.ParcelStatusID == ParcelStatus.Assigned.ParcelStatusID && x.ParcelGeometry.Geometry4326.Intersects(locationPoint4326));

        return parcel;
    }

    public static void UpdateWellLocation(QanatDbContext dbContext, int wellID, WellLocationDto dto)
    {
        var well = dbContext.Wells
            .Include(x => x.Parcel).ThenInclude(x => x.ParcelGeometry)
            .SingleOrDefault(x => x.WellID == wellID);

        well.LocationPoint4326 = GeometryHelper.CreateLocationPoint4326FromLatLong(dto.Latitude.Value, dto.Longitude.Value);
        well.LocationPoint = well.LocationPoint4326.ProjectTo2227();

        var parcelFromIntersection = GetWellParcelFromGeometryIntersection(dbContext, dto);
        if (parcelFromIntersection == null || parcelFromIntersection.ParcelID != dto.ParcelID)
        {
            // update Parcel from geometry intersection
            well.ParcelID = parcelFromIntersection?.ParcelID;
        }

        dbContext.SaveChanges();
    }

    public static WellIrrigatedParcelsResponseDto GetWellIrrigatedParcelsDto(QanatDbContext dbContext, int wellID)
    {
        var well = dbContext.Wells
            .Include(x => x.WellIrrigatedParcels)
            .ThenInclude(x => x.Parcel)
            .ThenInclude(x => x.WaterAccount)
            .SingleOrDefault(x => x.WellID == wellID);

        return well?.AsWellIrrigatedParcelsResponseDto();
    }

    public static void UpdateWellIrrigatedParcels(QanatDbContext dbContext, int wellID, WellIrrigatedParcelsRequestDto dto)
    {
        var well = dbContext.Wells.Include(x => x.WellIrrigatedParcels)
            .SingleOrDefault(x => x.WellID == wellID);

        var newIrrigatedParcels = dto.IrrigatedParcelIDs.Select(x => new WellIrrigatedParcel()
        {
            ParcelID = x,
            WellID = well.WellID
        }).ToList();

        var existingIrrigatedParcels = well.WellIrrigatedParcels;

        existingIrrigatedParcels.Merge(newIrrigatedParcels, dbContext.WellIrrigatedParcels,
            (x, y) => x.ParcelID == y.ParcelID && x.WellID == y.WellID);
        
        dbContext.SaveChanges();
    }

    public static async Task<WellInstanceDto> GetInstanceAsync(QanatDbContext dbContext, int wellID)
    {
        var well = await dbContext.Wells.AsNoTracking()
            .SingleAsync(x => x.WellID == wellID);

        var wellInstanceDto = new WellInstanceDto()
        {
            WellID = well.WellID,
            WellName = well.WellName,
            SchemotoInstance = !string.IsNullOrEmpty(well.SchemotoInstance)
                ? JsonSerializer.Deserialize<Instance>(well.SchemotoInstance)
                : null,
        };

        return wellInstanceDto;
    }
}