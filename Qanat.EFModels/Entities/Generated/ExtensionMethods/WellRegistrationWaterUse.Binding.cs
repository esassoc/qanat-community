//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[WellRegistrationWaterUse]
namespace Qanat.EFModels.Entities
{
    public partial class WellRegistrationWaterUse
    {
        public int PrimaryKey => WellRegistrationWaterUseID;
        public WellRegistrationWaterUseType WellRegistrationWaterUseType => WellRegistrationWaterUseType.AllLookupDictionary[WellRegistrationWaterUseTypeID];

        public static class FieldLengths
        {
            public const int WellRegistrationWaterUseDescription = 200;
        }
    }
}