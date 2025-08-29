//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[UsageLocationParcelHistory]
namespace Qanat.EFModels.Entities
{
    public partial class UsageLocationParcelHistory
    {
        public int PrimaryKey => UsageLocationParcelHistoryID;


        public static class FieldLengths
        {
            public const int FromParcelNumber = 64;
            public const int ToParcelNumber = 64;
            public const int Reason = 1000;
        }
    }
}