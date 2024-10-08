//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[WaterAccountUser]
namespace Qanat.EFModels.Entities
{
    public partial class WaterAccountUser
    {
        public int PrimaryKey => WaterAccountUserID;
        public WaterAccountRole WaterAccountRole => WaterAccountRole.AllLookupDictionary[WaterAccountRoleID];

        public static class FieldLengths
        {

        }
    }
}