//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[MeterReading]
namespace Qanat.EFModels.Entities
{
    public partial class MeterReading
    {
        public int PrimaryKey => MeterReadingID;
        public MeterReadingUnitType MeterReadingUnitType => MeterReadingUnitType.AllLookupDictionary[MeterReadingUnitTypeID];

        public static class FieldLengths
        {
            public const int ReaderInitials = 5;
        }
    }
}