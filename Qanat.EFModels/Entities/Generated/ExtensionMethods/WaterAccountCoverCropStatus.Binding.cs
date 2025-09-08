//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[WaterAccountCoverCropStatus]
namespace Qanat.EFModels.Entities
{
    public partial class WaterAccountCoverCropStatus
    {
        public int PrimaryKey => WaterAccountCoverCropStatusID;
        public SelfReportStatus SelfReportStatus => SelfReportStatus.AllLookupDictionary[SelfReportStatusID];

        public static class FieldLengths
        {

        }
    }
}