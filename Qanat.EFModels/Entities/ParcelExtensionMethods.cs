using Qanat.Common.GeoSpatial;
using Qanat.Models.DataTransferObjects;
using System.Text.Json;

namespace Qanat.EFModels.Entities
{
    public static partial class ParcelExtensionMethods
    {
        public static ParcelMinimalDto AsParcelMinimalDto(this Parcel parcel)
        {
            var parcelMinimalDto = new ParcelMinimalDto()
            {
                ParcelID = parcel.ParcelID,
                GeographyID = parcel.GeographyID,
                WaterAccountID = parcel.WaterAccountID,
                ParcelNumber = parcel.ParcelNumber,
                ParcelArea = parcel.ParcelArea,
                ParcelStatusID = parcel.ParcelStatusID,
                OwnerAddress = parcel.OwnerAddress,
                OwnerName = parcel.OwnerName,
                WaterAccount = parcel.WaterAccount?.AsDisplayDto(),
                ParcelStatus = parcel.ParcelStatus.AsSimpleDto()
            };
            return parcelMinimalDto;
        }

        public static ParcelDisplayDto AsDisplayDto(this Parcel parcel)
        {
            return new ParcelDisplayDto
            {
                ParcelID = parcel.ParcelID,
                ParcelNumber = parcel.ParcelNumber,
                ParcelArea = parcel.ParcelArea,
                WaterAccountID = parcel.WaterAccountID,
                WaterAccountNameAndNumber = parcel.WaterAccount?.WaterAccountNameAndNumber(),
                WaterAccountOwnerName = parcel.WaterAccount?.ContactName

            };
        }

        public static ParcelDetailDto AsDetailDto(this Parcel parcel)
        {
            return new ParcelDetailDto()
            {
                ParcelID = parcel.ParcelID,
                GeographyID = parcel.GeographyID,
                GeographyName = parcel.Geography.GeographyName,
                WaterAccountID = parcel.WaterAccountID,
                ParcelNumber = parcel.ParcelNumber,
                ParcelArea = parcel.ParcelArea,
                ParcelStatusID = parcel.ParcelStatusID,
                OwnerAddress = parcel.OwnerAddress,
                OwnerName = parcel.OwnerName,
                WaterAccount = parcel.WaterAccount?.AsDisplayDto(),
                ParcelStatus = parcel.ParcelStatus.AsSimpleDto(),
                Zones = parcel.ParcelZones.Select(x => x.Zone.AsZoneMinimalDto()).ToList(),
                WellsOnParcel = parcel.Wells.OrderBy(x => x.WellID).Select(x => x.AsSimpleDto()).ToList(),
                IrrigatedByWells = parcel.WellIrrigatedParcels.OrderBy(x => x.WellID).Select(x => x.Well.AsSimpleDto()).ToList()
            };
        }
        
        public static ParcelIndexGridDto AsIndexGridDto(this vParcelDetailed parcel)
        {
            return new ParcelIndexGridDto()
            {
                ParcelID = parcel.ParcelID,
                GeographyID = parcel.GeographyID,
                ParcelNumber = parcel.ParcelNumber,
                ParcelArea = parcel.ParcelArea,
                ParcelStatusID = parcel.ParcelStatusID,
                ParcelStatusDisplayName = parcel.ParcelStatusDisplayName,
                OwnerAddress = parcel.OwnerAddress,
                OwnerName = parcel.OwnerName,
                WaterAccountName =  parcel.WaterAccountName,
                WaterAccountNumber =  parcel.WaterAccountNumber,
                Zones = parcel.Zones,
                CustomAttributes = string.IsNullOrWhiteSpace(parcel.CustomAttributes) ? null : JsonSerializer.Deserialize<Dictionary<string, string>>(parcel.CustomAttributes, GeoJsonSerializer.DefaultSerializerOptions),
                WellsOnParcel = parcel.WellsOnParcel,
                IrrigatedByWells = parcel.IrrigatedByWells
            };
        }

        public static ParcelPopupDto AsPopupDto(this Parcel parcel)
        {
            var allocationPlanZoneGroup = parcel.Geography.GeographyAllocationPlanConfiguration.ZoneGroup;
            var zone = parcel.ParcelZones.Single(x => allocationPlanZoneGroup.Zones.Select(y => y.ZoneID).Contains(x.ZoneID));

            return new ParcelPopupDto
            {
                ParcelID = parcel.ParcelID,
                ParcelNumber = parcel.ParcelNumber,
                ParcelArea = parcel.ParcelArea,
                WaterAccountID = parcel.WaterAccountID,
                WaterAccountName = parcel.WaterAccount?.WaterAccountName,
                WaterAccountNumber = parcel.WaterAccount?.WaterAccountNumber,
                GeographyName = parcel.Geography.GeographyName,
                GeographyDisplayName = parcel.Geography.GeographyDisplayName,
                AllocationZoneColor = zone.Zone.ZoneColor,
                AllocationZoneGroupName = allocationPlanZoneGroup.ZoneGroupName,
                AllocationZoneName = zone.Zone.ZoneName,
            };
        }

        public static ParcelWithGeoJSONDto AsParcelWithGeoJSONDto(this Parcel parcel)
        {
            return new ParcelWithGeoJSONDto
            {
                ParcelID = parcel.ParcelID,
                ParcelNumber = parcel.ParcelNumber,
                ParcelArea = parcel.ParcelArea,
                WaterAccountID = parcel.WaterAccountID,
                WaterAccountNameAndNumber = parcel.WaterAccount?.WaterAccountNameAndNumber(),
                WaterAccountOwnerName = parcel.WaterAccount?.ContactName,
                GeoJSON = parcel.ParcelGeometry.Geometry4326.ToGeoJSON(),
            };
        }

        public static ParcelWithGeometryDto AsParcelWithGeometryDto(this Parcel parcel)
        {
            return new ParcelWithGeometryDto
            {
                ParcelID = parcel.ParcelID,
                ParcelNumber = parcel.ParcelNumber,
                GeographyID = parcel.GeographyID,
                Geometry = parcel.ParcelGeometry.GeometryNative,
                ParcelArea = parcel.ParcelArea,
                ParcelStatus = parcel.ParcelStatus.AsSimpleDto(),
                WaterAccount = parcel.WaterAccount?.AsDisplayDto()
            };
        }
    }
}