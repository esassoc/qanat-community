//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[WellRegistrationContact]
namespace Qanat.EFModels.Entities
{
    public partial class WellRegistrationContact
    {
        public int PrimaryKey => WellRegistrationContactID;
        public WellRegistrationContactType WellRegistrationContactType => WellRegistrationContactType.AllLookupDictionary[WellRegistrationContactTypeID];
        public State State => State.AllLookupDictionary[StateID];

        public static class FieldLengths
        {
            public const int ContactName = 100;
            public const int BusinessName = 100;
            public const int StreetAddress = 100;
            public const int City = 100;
            public const int ZipCode = 10;
            public const int Phone = 20;
            public const int Email = 100;
        }
    }
}