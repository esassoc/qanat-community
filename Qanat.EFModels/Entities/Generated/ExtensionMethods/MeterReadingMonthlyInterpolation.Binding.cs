//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[MeterReadingMonthlyInterpolation]
namespace Qanat.EFModels.Entities
{
    public partial class MeterReadingMonthlyInterpolation
    {
        public int PrimaryKey => MeterReadingMonthlyInterpolationID;
        public MeterReadingUnitType MeterReadingUnitType => MeterReadingUnitType.AllLookupDictionary[MeterReadingUnitTypeID];

        public static class FieldLengths
        {

        }
    }
}