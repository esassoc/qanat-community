//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[Well]
namespace Qanat.EFModels.Entities
{
    public partial class Well
    {
        public int PrimaryKey => WellID;
        public WellStatus WellStatus => WellStatus.AllLookupDictionary[WellStatusID];

        public static class FieldLengths
        {
            public const int WellName = 100;
            public const int StateWCRNumber = 100;
            public const int CountyWellPermitNumber = 100;
            public const int Notes = 500;
        }
    }
}