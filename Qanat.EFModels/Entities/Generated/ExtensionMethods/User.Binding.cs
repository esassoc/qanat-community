//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[User]
namespace Qanat.EFModels.Entities
{
    public partial class User
    {
        public int PrimaryKey => UserID;
        public Role Role => Role.AllLookupDictionary[RoleID];
        public ScenarioPlannerRole ScenarioPlannerRole => ScenarioPlannerRole.AllLookupDictionary[ScenarioPlannerRoleID];

        public static class FieldLengths
        {
            public const int FirstName = 100;
            public const int LastName = 100;
            public const int Email = 255;
            public const int Phone = 30;
            public const int LoginName = 128;
            public const int Company = 100;
        }
    }
}