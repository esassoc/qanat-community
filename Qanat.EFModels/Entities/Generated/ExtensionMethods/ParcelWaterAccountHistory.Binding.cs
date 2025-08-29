//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ParcelWaterAccountHistory]
namespace Qanat.EFModels.Entities
{
    public partial class ParcelWaterAccountHistory
    {
        public int PrimaryKey => ParcelWaterAccountHistoryID;


        public static class FieldLengths
        {
            public const int FromWaterAccountName = 255;
            public const int ToWaterAccountName = 255;
            public const int Reason = 1000;
        }
    }
}