//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[Parcel]
namespace Qanat.EFModels.Entities
{
    public partial class Parcel
    {
        public int PrimaryKey => ParcelID;
        public ParcelStatus ParcelStatus => ParcelStatus.AllLookupDictionary[ParcelStatusID];

        public static class FieldLengths
        {
            public const int ParcelNumber = 64;
            public const int OwnerAddress = 500;
            public const int OwnerName = 500;
        }
    }
}