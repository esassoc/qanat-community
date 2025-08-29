//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[WaterMeasurement]
namespace Qanat.EFModels.Entities
{
    public partial class WaterMeasurement
    {
        public int PrimaryKey => WaterMeasurementID;
        public UnitType? UnitType => UnitTypeID.HasValue ? UnitType.AllLookupDictionary[UnitTypeID.Value] : null;

        public static class FieldLengths
        {
            public const int Comment = 500;
        }
    }
}