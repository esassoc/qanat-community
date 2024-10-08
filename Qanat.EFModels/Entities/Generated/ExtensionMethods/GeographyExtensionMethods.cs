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
                SourceOfRecordWaterMeasurementTypeID = geography.SourceOfRecordWaterMeasurementTypeID,
                SourceOfRecordExplanation = geography.SourceOfRecordExplanation,
                ContactEmail = geography.ContactEmail,
                ContactPhoneNumber = geography.ContactPhoneNumber,
                DefaultDisplayYear = geography.DefaultDisplayYear,
                LandownerDashboardSupplyLabel = geography.LandownerDashboardSupplyLabel,
                LandownerDashboardUsageLabel = geography.LandownerDashboardUsageLabel,
                DisplayUsageGeometriesAsField = geography.DisplayUsageGeometriesAsField,
                AllowLandownersToRequestAccountChanges = geography.AllowLandownersToRequestAccountChanges
            };
            return dto;
        }
    }
}