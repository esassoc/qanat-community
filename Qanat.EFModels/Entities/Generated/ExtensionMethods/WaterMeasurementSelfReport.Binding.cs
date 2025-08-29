//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[WaterMeasurementSelfReport]
namespace Qanat.EFModels.Entities
{
    public partial class WaterMeasurementSelfReport
    {
        public int PrimaryKey => WaterMeasurementSelfReportID;
        public SelfReportStatus WaterMeasurementSelfReportStatus => SelfReportStatus.AllLookupDictionary[WaterMeasurementSelfReportStatusID];

        public static class FieldLengths
        {

        }
    }
}