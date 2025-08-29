using NetTopologySuite.Geometries;
using Qanat.Models.DataTransferObjects;
using Qanat.Models.DataTransferObjects.Geography;

namespace Qanat.EFModels.Entities;

public static class GeographyExtensionMethods
{
    public static GeographySimpleDto AsSimpleDto(this Geography geography)
    {
        var dto = new GeographySimpleDto()
        {
            GeographyID = geography.GeographyID,
            GeographyConfigurationID = geography.GeographyConfigurationID,
            GeographyName = geography.GeographyName,
            GeographyDisplayName = geography.GeographyDisplayName,
            GeographyDescription = geography.GeographyDescription,
            APNRegexPattern = geography.APNRegexPattern,
            APNRegexPatternDisplay = geography.APNRegexPatternDisplay,
            GSACanonicalID = geography.GSACanonicalID,
            Color = geography.Color,
            ContactEmail = geography.ContactEmail,
            ContactPhoneNumber = geography.ContactPhoneNumber,
            ContactAddressLine1 = geography.ContactAddressLine1,
            ContactAddressLine2 = geography.ContactAddressLine2,
            LandownerDashboardSupplyLabel = geography.LandownerDashboardSupplyLabel,
            LandownerDashboardUsageLabel = geography.LandownerDashboardUsageLabel,
            CoordinateSystem = geography.CoordinateSystem,
            AreaToAcresConversionFactor = geography.AreaToAcresConversionFactor,
            IsOpenETActive = geography.IsOpenETActive,
            OpenETShapeFilePath = geography.OpenETShapeFilePath,
            OpenETRasterTimeseriesMultipolygonColumnToUseAsIdentifier = geography.OpenETRasterTimeseriesMultipolygonColumnToUseAsIdentifier,
            SourceOfRecordWaterMeasurementTypeID = geography.SourceOfRecordWaterMeasurementTypeID,
            SourceOfRecordExplanation = geography.SourceOfRecordExplanation,
            ShowSupplyOnWaterBudgetComponent = geography.ShowSupplyOnWaterBudgetComponent,
            WaterBudgetSlotAHeader = geography.WaterBudgetSlotAHeader,
            WaterBudgetSlotAWaterMeasurementTypeID = geography.WaterBudgetSlotAWaterMeasurementTypeID,
            WaterBudgetSlotBHeader = geography.WaterBudgetSlotBHeader,
            WaterBudgetSlotBWaterMeasurementTypeID = geography.WaterBudgetSlotBWaterMeasurementTypeID,
            WaterBudgetSlotCHeader = geography.WaterBudgetSlotCHeader,
            WaterBudgetSlotCWaterMeasurementTypeID = geography.WaterBudgetSlotCWaterMeasurementTypeID,
            FeeCalculatorEnabled = geography.FeeCalculatorEnabled,
            AllowWaterMeasurementSelfReporting = geography.AllowWaterMeasurementSelfReporting,
            AllowFallowSelfReporting = geography.AllowFallowSelfReporting,
            AllowCoverCropSelfReporting = geography.AllowCoverCropSelfReporting,
            AllowLandownersToRequestAccountChanges = geography.AllowLandownersToRequestAccountChanges,
            IsDemoGeography = geography.IsDemoGeography
        };
        return dto;
    }

    public static GeographyDto AsDto(this Geography geography)
    {
        var geographyDto = new GeographyDto()
        {
            GeographyID = geography.GeographyID,
            GeographyConfiguration = geography.GeographyConfiguration?.AsSimpleDto(),
            GeographyName = geography.GeographyName,
            GeographyDisplayName = geography.GeographyDisplayName,
            GeographyDescription = geography.GeographyDescription,
            APNRegexPattern = geography.APNRegexPattern,
            APNRegexPatternDisplay = geography.APNRegexPatternDisplay,
            IsOpenETActive = geography.IsOpenETActive,
            CoordinateSystem = geography.CoordinateSystem,
            AreaToAcresConversionFactor = geography.AreaToAcresConversionFactor,
            IsDemoGeography = geography.IsDemoGeography,
            GSACanonicalID = geography.GSACanonicalID,
            Color = geography.Color,
            SourceOfRecordWaterMeasurementType = geography.SourceOfRecordWaterMeasurementType?.AsSimpleDto(),
            SourceOfRecordExplanation = geography.SourceOfRecordExplanation,
            AllocationPlansEnabled = geography.GeographyAllocationPlanConfiguration?.IsActive ?? false,
            AllocationPlansVisibleToLandowners = geography.GeographyAllocationPlanConfiguration?.IsVisibleToLandowners ?? false,
            AllocationPlansVisibleToPublic = geography.GeographyAllocationPlanConfiguration?.IsVisibleToPublic ?? false,
            BoundingBox = geography.GeographyBoundary != null ? new BoundingBoxDto(new List<Geometry>() { geography.GeographyBoundary.BoundingBox }) : null,
            ContactEmail = geography.ContactEmail,
            ContactPhoneNumber = geography.ContactPhoneNumber,
            ContactAddressLine1 = geography.ContactAddressLine1,
            ContactAddressLine2 = geography.ContactAddressLine2,
            WellRegistryEnabled = geography.GeographyConfiguration?.WellRegistryEnabled ?? false,
            LandownerDashboardSupplyLabel = geography.LandownerDashboardSupplyLabel,
            LandownerDashboardUsageLabel = geography.LandownerDashboardUsageLabel,
            LandingPageEnabled = geography.GeographyConfiguration?.LandingPageEnabled ?? false,
            MeterDataEnabled = geography.GeographyConfiguration?.MetersEnabled ?? false,
            AllowLandownersToRequestAccountChanges = geography.AllowLandownersToRequestAccountChanges,
            AllowWaterMeasurementSelfReporting = geography.AllowWaterMeasurementSelfReporting,
            AllowFallowSelfReporting = geography.AllowFallowSelfReporting,
            ShowSupplyOnWaterBudgetComponent = geography.ShowSupplyOnWaterBudgetComponent,
            WaterBudgetSlotAHeader = geography.WaterBudgetSlotAHeader,
            WaterBudgetSlotBHeader = geography.WaterBudgetSlotBHeader,
            WaterBudgetSlotCHeader = geography.WaterBudgetSlotCHeader,
            FeeCalculatorEnabled = geography.FeeCalculatorEnabled,
        };

        return geographyDto;
    }

    public static GeographyMinimalDto AsMinimalDto(this Geography geography)
    {
        return new GeographyMinimalDto()
        {
            GeographyID = geography.GeographyID,
            GeographyName = geography.GeographyName,
            GeographyDisplayName = geography.GeographyDisplayName,
            IsOpenETActive = geography.IsOpenETActive,
            AllowLandownersToRequestAccountChanges = geography.AllowLandownersToRequestAccountChanges,
            AllowWaterMeasurementSelfReporting = geography.AllowWaterMeasurementSelfReporting,
            AllowFallowSelfReporting = geography.AllowFallowSelfReporting,
            AllowCoverCropSelfReporting = geography.AllowCoverCropSelfReporting,
            AllocationPlansEnabled = geography.GeographyAllocationPlanConfiguration?.IsActive ?? false,
            AllocationPlansVisibleToLandowners = geography.GeographyAllocationPlanConfiguration?.IsVisibleToLandowners ?? false,
            AllocationPlansVisibleToPublic = geography.GeographyAllocationPlanConfiguration?.IsVisibleToPublic ?? false,
            SourceOfRecordWaterMeasurementTypeID = geography.SourceOfRecordWaterMeasurementTypeID,
            GeographyConfiguration = geography.GeographyConfiguration.AsSimpleDto(),
        };
    }

    public static GeographyDisplayDto AsDisplayDto(this Geography geography)
    {
        return new GeographyDisplayDto()
        {
            GeographyID = geography.GeographyID,
            GeographyName = geography.GeographyName
        };
    }

    public static GeographyPublicDto AsPublicDto(this Geography geography)
    {
        return new GeographyPublicDto()
        {
            GeographyID = geography.GeographyID,
            GeographyName = geography.GeographyName,
            GeographyDisplayName = geography.GeographyDisplayName,
            GeographyDescription = geography.GeographyDescription,
            IsDemoGeography = geography.IsDemoGeography,
            Color = geography.Color,
            AllocationPlansVisibleToLandowners = geography.GeographyAllocationPlanConfiguration?.IsVisibleToLandowners ?? false,
            AllocationPlansVisibleToPublic = geography.GeographyAllocationPlanConfiguration?.IsVisibleToPublic ?? false,
            ContactEmail = geography.ContactEmail,
            ContactPhoneNumber = geography.ContactPhoneNumber,
            LandingPageEnabled = geography.GeographyConfiguration?.LandingPageEnabled ?? false,
            MeterDataEnabled = geography.GeographyConfiguration?.MetersEnabled ?? false,
            WellRegistryEnabled = geography.GeographyConfiguration?.WellRegistryEnabled ?? false,
            FeeCalculatorEnabled = geography.FeeCalculatorEnabled,
            AllowWaterMeasurementSelfReporting = geography.AllowWaterMeasurementSelfReporting
        };
    }
}