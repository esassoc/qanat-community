//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ParcelHistory]
namespace Qanat.EFModels.Entities
{
    public partial class ParcelHistory
    {
        public int PrimaryKey => ParcelHistoryID;
        public ParcelStatus ParcelStatus => ParcelStatus.AllLookupDictionary[ParcelStatusID];

        public static class FieldLengths
        {
            public const int OwnerName = 500;
            public const int OwnerAddress = 500;
        }
    }
}