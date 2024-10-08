//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[GeographyUser]
namespace Qanat.EFModels.Entities
{
    public partial class GeographyUser
    {
        public int PrimaryKey => GeographyUserID;
        public GeographyRole GeographyRole => GeographyRole.AllLookupDictionary[GeographyRoleID];

        public static class FieldLengths
        {

        }
    }
}