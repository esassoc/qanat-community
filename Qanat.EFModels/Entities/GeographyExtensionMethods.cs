using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using NetTopologySuite.Geometries;
using Qanat.Models.DataTransferObjects;
using Qanat.Models.DataTransferObjects.Geography;

namespace Qanat.EFModels.Entities;

public partial class GeographyExtensionMethods
{
    public static GeographyDto AsDto(this Geography geography)
    {
        var geographyDto = new GeographyDto()
        {
            GeographyID = geography.GeographyID,
            GeographyConfiguration = geography.GeographyConfiguration?.AsSimpleDto(),
            GeographyName = geography.GeographyName,
            DefaultReportingPeriod = geography.DefaultReportingPeriod?.AsSimpleDto(),
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
            WellRegistryEnabled = geography.GeographyConfiguration?.WellRegistryEnabled ?? false,
            LandownerDashboardSupplyLabel = geography.LandownerDashboardSupplyLabel,
            LandownerDashboardUsageLabel = geography.LandownerDashboardUsageLabel,
            LandingPageEnabled = geography.GeographyConfiguration?.LandingPageEnabled ?? false,
            MeterDataEnabled = geography.GeographyConfiguration?.MetersEnabled ?? false,
            DisplayUsageGeometriesAsField = geography.DisplayUsageGeometriesAsField,
            AllowLandownersToRequestAccountChanges = geography.AllowLandownersToRequestAccountChanges,
            AllowWaterMeasurementSelfReporting = geography.AllowWaterMeasurementSelfReporting,
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
            DisplayUsageGeometriesAsField = geography.DisplayUsageGeometriesAsField,
            AllowLandownersToRequestAccountChanges = geography.AllowLandownersToRequestAccountChanges,
            AllowWaterMeasurementSelfReporting = geography.AllowWaterMeasurementSelfReporting,
            AllocationPlansEnabled = geography.GeographyAllocationPlanConfiguration?.IsActive ?? false,
            AllocationPlansVisibleToLandowners = geography.GeographyAllocationPlanConfiguration?.IsVisibleToLandowners ?? false,
            AllocationPlansVisibleToPublic = geography.GeographyAllocationPlanConfiguration?.IsVisibleToPublic ?? false,
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
            AllocationPlansVisibleToLandowners =
                geography.GeographyAllocationPlanConfiguration?.IsVisibleToLandowners ?? false,
            AllocationPlansVisibleToPublic = geography.GeographyAllocationPlanConfiguration?.IsVisibleToPublic ?? false,
            ContactEmail = geography.ContactEmail,
            ContactPhoneNumber = geography.ContactPhoneNumber,
            LandingPageEnabled = geography.GeographyConfiguration?.LandingPageEnabled ?? false,
            MeterDataEnabled = geography.GeographyConfiguration?.MetersEnabled ?? false,
            FeeCalculatorEnabled = geography.FeeCalculatorEnabled,
            AllowWaterMeasurementSelfReporting = geography.AllowWaterMeasurementSelfReporting,
            DisplayUsageGeometriesAsField = geography.DisplayUsageGeometriesAsField
        };
    }

    public static GeographySimpleDto AsSimpleDtoWithDefaultYear(this Geography geography)
    {
        var simpleDto = geography.AsSimpleDto();
        var defaultYear = geography.DefaultReportingPeriod?.EndDate.Year;
        simpleDto.DefaultDisplayYear = defaultYear ?? DateTime.UtcNow.Year;
        return simpleDto;
    }
}