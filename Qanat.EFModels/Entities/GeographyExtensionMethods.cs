using System.Collections.Generic;
using System.Linq;
using NetTopologySuite.Geometries;
using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities;

public partial class GeographyExtensionMethods
{
    public static GeographyDto AsGeographyDto(this Geography geography)
    {
        var geographyDto = new GeographyDto()
        {
            GeographyID = geography.GeographyID,
            GeographyConfiguration = geography.GeographyConfiguration?.AsSimpleDto(),
            GeographyName = geography.GeographyName,
            StartYear = geography.StartYear,
            GeographyDisplayName = geography.GeographyDisplayName,
            GeographyDescription = geography.GeographyDescription,
            APNRegexPattern = geography.APNRegexPattern,
            APNRegexPatternDisplay = geography.APNRegexPatternDisplay,
            IsOpenETActive = geography.IsOpenETActive,
            OpenETShapeFilePath = geography.OpenETShapeFilePath,
            OpenETRasterTimeseriesMultipolygonColumnToUseAsIdentifier = geography.OpenETRasterTimeseriesMultipolygonColumnToUseAsIdentifier,
            CoordinateSystem = geography.CoordinateSystem,
            AreaToAcresConversionFactor = geography.AreaToAcresConversionFactor,
            IsDemoGeography = geography.IsDemoGeography,
            GSACanonicalID = geography.GSACanonicalID,
            Color = geography.Color,
            SourceOfRecordWaterMeasurementType = geography.SourceOfRecordWaterMeasurementType?.AsSimpleDto(),
            SourceOfRecordExplanation = geography.SourceOfRecordExplanation,
            WaterManagers = geography.GeographyUsers
                .Where(x => x.GeographyRole.GeographyRoleID == (int)GeographyRoleEnum.WaterManager)
                .Select(x => x.User.AsUserDto()).ToList(),
            AllocationPlansEnabled = geography.GeographyAllocationPlanConfiguration?.IsActive ?? false,
            AllocationPlansVisibleToLandowners = geography.GeographyAllocationPlanConfiguration?.IsVisibleToLandowners ?? false,
            AllocationPlansVisibleToPublic = geography.GeographyAllocationPlanConfiguration?.IsVisibleToPublic ?? false,
            BoundingBox = geography.GeographyBoundary != null ? 
                new BoundingBoxDto(new List<Geometry>() { geography.GeographyBoundary.BoundingBox }) : null,
            ContactEmail = geography.ContactEmail,
            ContactPhoneNumber = geography.ContactPhoneNumber,
            WellRegistryEnabled = geography.GeographyConfiguration?.WellRegistryEnabled ?? false,
            DefaultDisplayYear = geography.DefaultDisplayYear,
            LandownerDashboardSupplyLabel = geography.LandownerDashboardSupplyLabel,
            LandownerDashboardUsageLabel = geography.LandownerDashboardUsageLabel,
            LandingPageEnabled = geography.GeographyConfiguration?.LandingPageEnabled ?? false,
            MeterDataEnabled = geography.GeographyConfiguration?.MetersEnabled ?? false,
            DisplayUsageGeometriesAsField = geography.DisplayUsageGeometriesAsField,
            AllowLandownersToRequestAccountChanges = geography.AllowLandownersToRequestAccountChanges,
        };

        return geographyDto;
    }

    public static GeographyDisplayDto AsDisplayDto(this Geography geography)
    {
        return new GeographyDisplayDto()
        {
            GeographyID = geography.GeographyID,
            GeographyName = geography.GeographyName
        };
    }
}