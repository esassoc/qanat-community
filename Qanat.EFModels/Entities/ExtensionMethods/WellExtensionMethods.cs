using NetTopologySuite.Geometries;
using Qanat.Common.GeoSpatial;
using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities;

public static class WellExtensionMethods
{
    public static WellSimpleDto AsSimpleDto(this Well well)
    {
        var dto = new WellSimpleDto()
        {
            WellID = well.WellID,
            GeographyID = well.GeographyID,
            ParcelID = well.ParcelID,
            ParcelIsManualOverride = well.ParcelIsManualOverride,
            WellName = well.WellName,
            StateWCRNumber = well.StateWCRNumber,
            CountyWellPermitNumber = well.CountyWellPermitNumber,
            DateDrilled = well.DateDrilled,
            CreateDate = well.CreateDate,
            WellStatusID = well.WellStatusID,
            Notes = well.Notes,
            WellDepth = well.WellDepth,
            CasingDiameter = well.CasingDiameter,
            TopOfPerforations = well.TopOfPerforations,
            BottomOfPerforations = well.BottomOfPerforations,
            ElectricMeterNumber = well.ElectricMeterNumber
        };
        return dto;
    }

    public static WellMinimalDto AsMinimalDto(this Well well)
    {
        return new WellMinimalDto()
        {
            WellID = well.WellID,
            GeographyID = well.GeographyID,
            WellName = well.WellName,
            ParcelID = well.ParcelID,
            StateWCRNumber = well.StateWCRNumber,
            CountyWellPermitNumber = well.CountyWellPermitNumber,
            DateDrilled = well.DateDrilled,
            Latitude = well.Latitude,
            Longitude = well.Longitude,
            WellStatusID = well.WellStatusID,
            WellStatusDisplayName = well.WellStatus.WellStatusDisplayName,
            ParcelNumber = well.Parcel?.ParcelNumber,
            MeterSerialNumber = well.WellMeters?.SingleOrDefault(x => !x.EndDate.HasValue)?.Meter.SerialNumber,
            IrrigatesParcels = well.WellIrrigatedParcels?.Select(x => x.Parcel.AsParcelMinimalDto()).ToList(),
            WaterAccountID = well.Parcel?.WaterAccountID,
            Notes = well.Notes,
            WellDepth = well.WellDepth,
            CasingDiameter = well.CasingDiameter,
            TopOfPerforations = well.TopOfPerforations,
            BottomOfPerforations = well.BottomOfPerforations,
            ElectricMeterNumber = well.ElectricMeterNumber
        };
    }

    public static WellLocationDto AsLocationDto(this Well well, bool includeParcelGeoJson = false)
    {
        var wellLocationDto = new WellLocationDto
        {
            WellID = well.WellID,
            Latitude = well.Latitude,
            Longitude = well.Longitude,
            GeographyID = well.GeographyID
        };

        if (well.Parcel == null)
        {
            return wellLocationDto;
        }

        wellLocationDto.ParcelID = well.ParcelID;
        wellLocationDto.ParcelNumber = well.Parcel.ParcelNumber;

        var geometries = new List<Geometry> { well.Parcel.ParcelGeometry.Geometry4326 };
        if (well.LocationPoint4326 != null)
        {
            geometries.Add(well.LocationPoint4326);
        }
        wellLocationDto.BoundingBox = new BoundingBoxDto(geometries);

        if (includeParcelGeoJson && well.Parcel.ParcelGeometry.Geometry4326 != null)
        {
            wellLocationDto.ParcelGeoJson = well.Parcel.ParcelGeometry.Geometry4326.ToGeoJSON();
        }

        return wellLocationDto;
    }

    public static WellIrrigatedParcelsResponseDto AsWellIrrigatedParcelsResponseDto(this Well well)
    {
        return new WellIrrigatedParcelsResponseDto()
        {
            WellID = well.WellID,
            GeographyID = well.GeographyID,
            Latitude = well.Latitude,
            Longitude = well.Longitude,
            IrrigatedParcels = well.WellIrrigatedParcels.Select(x => x.Parcel.AsDisplayDto()).ToList()
        };
    }

    public static WellDisplayDto AsDisplayDto(this Well well)
    {
        return new WellDisplayDto()
        {
            WellID = well.WellID,
            WellName = well.WellName,
            StateWCRNumber = well.StateWCRNumber,
            ParcelID = well.ParcelID,
            GeographyID = well.GeographyID
        };
    }

    public static WellLinkDisplayDto AsLinkDisplayDto(this Well well)
    {
        return new WellLinkDisplayDto()
        {
            WellID = well.WellID,
            WellName = well.WellName
        };
    }
}