//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[Geography]

namespace Qanat.Models.DataTransferObjects
{
    public partial class GeographySimpleDto
    {
        public int GeographyID { get; set; }
        public int GeographyConfigurationID { get; set; }
        public string GeographyName { get; set; }
        public string GeographyDisplayName { get; set; }
        public string GeographyDescription { get; set; }
        public string APNRegexPattern { get; set; }
        public string APNRegexPatternDisplay { get; set; }
        public int? GSACanonicalID { get; set; }
        public string Color { get; set; }
        public string ContactEmail { get; set; }
        public string ContactPhoneNumber { get; set; }
        public string LandownerDashboardSupplyLabel { get; set; }
        public string LandownerDashboardUsageLabel { get; set; }
        public int CoordinateSystem { get; set; }
        public int AreaToAcresConversionFactor { get; set; }
        public int? DefaultReportingPeriodID { get; set; }
        public bool IsOpenETActive { get; set; }
        public string OpenETShapeFilePath { get; set; }
        public string OpenETRasterTimeseriesMultipolygonColumnToUseAsIdentifier { get; set; }
        public int? SourceOfRecordWaterMeasurementTypeID { get; set; }
        public string SourceOfRecordExplanation { get; set; }
        public bool ShowSupplyOnWaterBudgetComponent { get; set; }
        public string WaterBudgetSlotAHeader { get; set; }
        public int? WaterBudgetSlotAWaterMeasurementTypeID { get; set; }
        public string WaterBudgetSlotBHeader { get; set; }
        public int? WaterBudgetSlotBWaterMeasurementTypeID { get; set; }
        public string WaterBudgetSlotCHeader { get; set; }
        public int? WaterBudgetSlotCWaterMeasurementTypeID { get; set; }
        public bool FeeCalculatorEnabled { get; set; }
        public bool AllowWaterMeasurementSelfReporting { get; set; }
        public bool DisplayUsageGeometriesAsField { get; set; }
        public bool AllowLandownersToRequestAccountChanges { get; set; }
        public bool IsDemoGeography { get; set; }
    }
}