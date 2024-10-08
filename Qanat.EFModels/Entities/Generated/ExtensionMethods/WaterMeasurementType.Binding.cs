//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[WaterMeasurementType]
namespace Qanat.EFModels.Entities
{
    public partial class WaterMeasurementType
    {
        public int PrimaryKey => WaterMeasurementTypeID;
        public WaterMeasurementCategoryType WaterMeasurementCategoryType => WaterMeasurementCategoryType.AllLookupDictionary[WaterMeasurementCategoryTypeID];
        public WaterMeasurementCalculationType? WaterMeasurementCalculationType => WaterMeasurementCalculationTypeID.HasValue ? WaterMeasurementCalculationType.AllLookupDictionary[WaterMeasurementCalculationTypeID.Value] : null;

        public static class FieldLengths
        {
            public const int WaterMeasurementTypeName = 50;
        }
    }
}