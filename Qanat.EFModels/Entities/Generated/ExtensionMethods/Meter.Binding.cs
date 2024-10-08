//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[Meter]
namespace Qanat.EFModels.Entities
{
    public partial class Meter
    {
        public int PrimaryKey => MeterID;
        public MeterStatus MeterStatus => MeterStatus.AllLookupDictionary[MeterStatusID];

        public static class FieldLengths
        {
            public const int SerialNumber = 25;
            public const int DeviceName = 255;
            public const int Make = 100;
            public const int ModelNumber = 25;
        }
    }
}