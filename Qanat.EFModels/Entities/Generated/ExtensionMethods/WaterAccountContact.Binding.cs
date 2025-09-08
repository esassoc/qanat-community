//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[WaterAccountContact]
namespace Qanat.EFModels.Entities
{
    public partial class WaterAccountContact
    {
        public int PrimaryKey => WaterAccountContactID;


        public static class FieldLengths
        {
            public const int ContactName = 255;
            public const int ContactEmail = 100;
            public const int ContactPhoneNumber = 30;
            public const int Address = 500;
            public const int SecondaryAddress = 100;
            public const int City = 100;
            public const int State = 20;
            public const int ZipCode = 20;
            public const int FullAddress = 747;
        }
    }
}