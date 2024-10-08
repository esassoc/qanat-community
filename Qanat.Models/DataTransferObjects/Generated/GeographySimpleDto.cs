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
        public int StartYear { get; set; }
        public string GeographyDisplayName { get; set; }
        public string GeographyDescription { get; set; }
        public string APNRegexPattern { get; set; }
        public string APNRegexPatternDisplay { get; set; }
        public bool IsOpenETActive { get; set; }
        public string OpenETShapeFilePath { get; set; }
        public string OpenETRasterTimeseriesMultipolygonColumnToUseAsIdentifier { get; set; }
        public int CoordinateSystem { get; set; }
        public int AreaToAcresConversionFactor { get; set; }
        public bool IsDemoGeography { get; set; }
        public int? GSACanonicalID { get; set; }
        public string Color { get; set; }
        public int? SourceOfRecordWaterMeasurementTypeID { get; set; }
        public string SourceOfRecordExplanation { get; set; }
        public string ContactEmail { get; set; }
        public string ContactPhoneNumber { get; set; }
        public int DefaultDisplayYear { get; set; }
        public string LandownerDashboardSupplyLabel { get; set; }
        public string LandownerDashboardUsageLabel { get; set; }
        public bool DisplayUsageGeometriesAsField { get; set; }
        public bool AllowLandownersToRequestAccountChanges { get; set; }
    }
}