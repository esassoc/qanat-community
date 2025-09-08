using Qanat.Models.DataTransferObjects;
using NetTopologySuite.Geometries;
using Qanat.Common.GeoSpatial;

namespace Qanat.EFModels.Entities
{
    public static partial class WellRegistrationExtensionMethods
    {
        public static WellRegistrationMinimalDto AsMinimalDto(this WellRegistration wellRegistration)
        {
            return new WellRegistrationMinimalDto()
            {
                WellRegistrationID = wellRegistration.WellRegistrationID,
                GeographyID = wellRegistration.GeographyID,
                WellName = wellRegistration.WellName,
                WellRegistrationStatusID = wellRegistration.WellRegistrationStatusID,
                ParcelID = wellRegistration.ParcelID,
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
            };
        }

        public static WellRegistrationLocationDto AsLocationDto(this WellRegistration wellRegistration, bool includeParcelGeoJson = false)
        {
            var wellLocationDto = new WellRegistrationLocationDto
            {
                WellRegistrationID = wellRegistration.WellRegistrationID,
                Latitude = wellRegistration.Latitude,
                Longitude = wellRegistration.Longitude,
                ReferenceWellID = wellRegistration.ReferenceWellID,
            };

            if (wellRegistration.Parcel == null)
            {
                return wellLocationDto;
            }

            wellLocationDto.ParcelID = wellRegistration.ParcelID;
            wellLocationDto.ParcelNumber = wellRegistration.Parcel.ParcelNumber;

            var geometries = new List<Geometry> { wellRegistration.Parcel.ParcelGeometry.Geometry4326 };
            if (wellRegistration.LocationPoint4326 != null)
            {
                geometries.Add(wellRegistration.LocationPoint4326);
            }
            wellLocationDto.BoundingBox = new BoundingBoxDto(geometries);

            if (includeParcelGeoJson && wellRegistration.Parcel.ParcelGeometry.Geometry4326 != null)
            {
                wellLocationDto.ParcelGeoJson = wellRegistration.Parcel.ParcelGeometry.Geometry4326.ToGeoJSON();
            }

            return wellLocationDto;
        }

        public static ConfirmWellRegistrationLocationDto AsConfirmWellRegistrationLocationDto(this WellRegistration wellRegistration)
        {
            var wellRegistrationLocationDto = new ConfirmWellRegistrationLocationDto
            {
                WellRegistrationID = wellRegistration.WellRegistrationID,
                Latitude = wellRegistration.Latitude,
                Longitude = wellRegistration.Longitude
            };

            if (wellRegistration.Parcel == null)
            {
                return wellRegistrationLocationDto;
            }

            wellRegistrationLocationDto.ParcelID = wellRegistration.ParcelID;
            wellRegistrationLocationDto.ParcelNumber = wellRegistration.Parcel.ParcelNumber;

            wellRegistrationLocationDto.ReferenceWellID = wellRegistration.ReferenceWellID;

            var geometries = new List<Geometry> { wellRegistration.Parcel.ParcelGeometry.Geometry4326 };
            if (wellRegistration.LocationPoint4326 != null)
            {
                geometries.Add(wellRegistration.LocationPoint4326);
            }
            wellRegistrationLocationDto.BoundingBox = new BoundingBoxDto(geometries);

            if (wellRegistration.Parcel.ParcelGeometry.Geometry4326 != null)
            {
                wellRegistrationLocationDto.ParcelGeoJson = wellRegistration.Parcel.ParcelGeometry.Geometry4326.ToGeoJSON();
            }

            return wellRegistrationLocationDto;
        }

        public static WellRegistrationIrrigatedParcelsResponseDto AsWellRegistrationIrrigatedParcelsDto(this WellRegistration wellRegistration)
        {
            return new WellRegistrationIrrigatedParcelsResponseDto
            {
                WellRegistrationID = wellRegistration.WellRegistrationID,
                Latitude = wellRegistration.Latitude,
                Longitude = wellRegistration.Longitude,
                IrrigatedParcels = wellRegistration.WellRegistrationIrrigatedParcels.Select(x => x.Parcel.AsDisplayDto()).ToList(),
                GeographyID = wellRegistration.GeographyID
            };
        }
    }
}