//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[WellRegistrationMetadatum]
namespace Qanat.EFModels.Entities
{
    public partial class WellRegistrationMetadatum
    {
        public int PrimaryKey => WellRegistrationMetadatumID;
        public FuelType? FuelType => FuelTypeID.HasValue ? FuelType.AllLookupDictionary[FuelTypeID.Value] : null;

        public static class FieldLengths
        {
            public const int WellName = 100;
            public const int StateWellNumber = 100;
            public const int StateWellCompletionNumber = 100;
            public const int CountyWellPermit = 100;
            public const int ManufacturerOfWaterMeter = 100;
            public const int SerialNumberOfWaterMeter = 100;
            public const int ElectricMeterNumber = 100;
            public const int FuelOther = 100;
            public const int PumpTestBy = 100;
            public const int PumpManufacturer = 100;
        }
    }
}