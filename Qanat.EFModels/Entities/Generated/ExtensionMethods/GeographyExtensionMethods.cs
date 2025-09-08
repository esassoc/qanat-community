//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[Geography]

using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities
{
    public static partial class GeographyExtensionMethods
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
                LandownerDashboardSupplyLabel = geography.LandownerDashboardSupplyLabel,
                LandownerDashboardUsageLabel = geography.LandownerDashboardUsageLabel,
                CoordinateSystem = geography.CoordinateSystem,
                AreaToAcresConversionFactor = geography.AreaToAcresConversionFactor,
                DefaultReportingPeriodID = geography.DefaultReportingPeriodID,
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
                DisplayUsageGeometriesAsField = geography.DisplayUsageGeometriesAsField,
                AllowLandownersToRequestAccountChanges = geography.AllowLandownersToRequestAccountChanges,
                IsDemoGeography = geography.IsDemoGeography
            };
            return dto;
        }
    }
}