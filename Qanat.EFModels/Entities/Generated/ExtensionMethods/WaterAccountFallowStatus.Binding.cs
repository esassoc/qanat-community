//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[WaterAccountFallowStatus]
namespace Qanat.EFModels.Entities
{
    public partial class WaterAccountFallowStatus
    {
        public int PrimaryKey => WaterAccountFallowStatusID;
        public SelfReportStatus SelfReportStatus => SelfReportStatus.AllLookupDictionary[SelfReportStatusID];

        public static class FieldLengths
        {

        }
    }
}