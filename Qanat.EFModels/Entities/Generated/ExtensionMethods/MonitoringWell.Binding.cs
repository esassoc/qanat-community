//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[MonitoringWell]
namespace Qanat.EFModels.Entities
{
    public partial class MonitoringWell
    {
        public int PrimaryKey => MonitoringWellID;
        public MonitoringWellSourceType MonitoringWellSourceType => MonitoringWellSourceType.AllLookupDictionary[MonitoringWellSourceTypeID];

        public static class FieldLengths
        {
            public const int SiteCode = 255;
            public const int MonitoringWellName = 100;
        }
    }
}