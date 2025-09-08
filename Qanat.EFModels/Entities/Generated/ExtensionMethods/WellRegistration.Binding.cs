//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[WellRegistration]
namespace Qanat.EFModels.Entities
{
    public partial class WellRegistration
    {
        public int PrimaryKey => WellRegistrationID;
        public WellRegistrationStatus WellRegistrationStatus => WellRegistrationStatus.AllLookupDictionary[WellRegistrationStatusID];

        public static class FieldLengths
        {
            public const int WellName = 100;
            public const int StateWCRNumber = 100;
            public const int CountyWellPermitNumber = 100;
            public const int CreateUserEmail = 255;
        }
    }
}