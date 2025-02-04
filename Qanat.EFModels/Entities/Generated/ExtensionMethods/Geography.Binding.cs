//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[Geography]
namespace Qanat.EFModels.Entities
{
    public partial class Geography
    {
        public int PrimaryKey => GeographyID;


        public static class FieldLengths
        {
            public const int GeographyName = 50;
            public const int GeographyDisplayName = 100;
            public const int GeographyDescription = 500;
            public const int APNRegexPattern = 100;
            public const int APNRegexPatternDisplay = 50;
            public const int Color = 9;
            public const int ContactEmail = 100;
            public const int ContactPhoneNumber = 30;
            public const int LandownerDashboardSupplyLabel = 200;
            public const int LandownerDashboardUsageLabel = 200;
            public const int OpenETShapeFilePath = 100;
            public const int OpenETRasterTimeseriesMultipolygonColumnToUseAsIdentifier = 50;
            public const int SourceOfRecordExplanation = 500;
            public const int WaterBudgetSlotAHeader = 255;
            public const int WaterBudgetSlotBHeader = 255;
            public const int WaterBudgetSlotCHeader = 255;
        }
    }
}