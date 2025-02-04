//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ScenarioPlannerRole]
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Qanat.Models.DataTransferObjects;


namespace Qanat.EFModels.Entities
{
    public abstract partial class ScenarioPlannerRole : IHavePrimaryKey
    {
        public static readonly ScenarioPlannerRoleNoAccess NoAccess = ScenarioPlannerRoleNoAccess.Instance;
        public static readonly ScenarioPlannerRoleScenarioUser ScenarioUser = ScenarioPlannerRoleScenarioUser.Instance;

        public static readonly List<ScenarioPlannerRole> All;
        public static readonly List<ScenarioPlannerRoleSimpleDto> AllAsSimpleDto;
        public static readonly ReadOnlyDictionary<int, ScenarioPlannerRole> AllLookupDictionary;
        public static readonly ReadOnlyDictionary<int, ScenarioPlannerRoleSimpleDto> AllAsSimpleDtoLookupDictionary;

        /// <summary>
        /// Static type constructor to coordinate static initialization order
        /// </summary>
        static ScenarioPlannerRole()
        {
            All = new List<ScenarioPlannerRole> { NoAccess, ScenarioUser };
            AllAsSimpleDto = new List<ScenarioPlannerRoleSimpleDto> { NoAccess.AsSimpleDto(), ScenarioUser.AsSimpleDto() };
            AllLookupDictionary = new ReadOnlyDictionary<int, ScenarioPlannerRole>(All.ToDictionary(x => x.ScenarioPlannerRoleID));
            AllAsSimpleDtoLookupDictionary = new ReadOnlyDictionary<int, ScenarioPlannerRoleSimpleDto>(AllAsSimpleDto.ToDictionary(x => x.ScenarioPlannerRoleID));
        }

        /// <summary>
        /// Protected constructor only for use in instantiating the set of static lookup values that match database
        /// </summary>
        protected ScenarioPlannerRole(int scenarioPlannerRoleID, string scenarioPlannerRoleName, string scenarioPlannerRoleDisplayName, string scenarioPlannerRoleDescription, int sortOrder, string rights, string flags)
        {
            ScenarioPlannerRoleID = scenarioPlannerRoleID;
            ScenarioPlannerRoleName = scenarioPlannerRoleName;
            ScenarioPlannerRoleDisplayName = scenarioPlannerRoleDisplayName;
            ScenarioPlannerRoleDescription = scenarioPlannerRoleDescription;
            SortOrder = sortOrder;
            Rights = rights;
            Flags = flags;
        }

        [Key]
        public int ScenarioPlannerRoleID { get; private set; }
        public string ScenarioPlannerRoleName { get; private set; }
        public string ScenarioPlannerRoleDisplayName { get; private set; }
        public string ScenarioPlannerRoleDescription { get; private set; }
        public int SortOrder { get; private set; }
        public string Rights { get; private set; }
        public string Flags { get; private set; }
        [NotMapped]
        public int PrimaryKey { get { return ScenarioPlannerRoleID; } }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public bool Equals(ScenarioPlannerRole other)
        {
            if (other == null)
            {
                return false;
            }
            return other.ScenarioPlannerRoleID == ScenarioPlannerRoleID;
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override bool Equals(object obj)
        {
            return Equals(obj as ScenarioPlannerRole);
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override int GetHashCode()
        {
            return ScenarioPlannerRoleID;
        }

        public static bool operator ==(ScenarioPlannerRole left, ScenarioPlannerRole right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ScenarioPlannerRole left, ScenarioPlannerRole right)
        {
            return !Equals(left, right);
        }

        public ScenarioPlannerRoleEnum ToEnum => (ScenarioPlannerRoleEnum)GetHashCode();

        public static ScenarioPlannerRole ToType(int enumValue)
        {
            return ToType((ScenarioPlannerRoleEnum)enumValue);
        }

        public static ScenarioPlannerRole ToType(ScenarioPlannerRoleEnum enumValue)
        {
            switch (enumValue)
            {
                case ScenarioPlannerRoleEnum.NoAccess:
                    return NoAccess;
                case ScenarioPlannerRoleEnum.ScenarioUser:
                    return ScenarioUser;
                default:
                    throw new ArgumentException("Unable to map Enum: {enumValue}");
            }
        }
    }

    public enum ScenarioPlannerRoleEnum
    {
        NoAccess = 1,
        ScenarioUser = 2
    }

    public partial class ScenarioPlannerRoleNoAccess : ScenarioPlannerRole
    {
        private ScenarioPlannerRoleNoAccess(int scenarioPlannerRoleID, string scenarioPlannerRoleName, string scenarioPlannerRoleDisplayName, string scenarioPlannerRoleDescription, int sortOrder, string rights, string flags) : base(scenarioPlannerRoleID, scenarioPlannerRoleName, scenarioPlannerRoleDisplayName, scenarioPlannerRoleDescription, sortOrder, rights, flags) {}
        public static readonly ScenarioPlannerRoleNoAccess Instance = new ScenarioPlannerRoleNoAccess(1, @"NoAccess", @"No Access", null, 10, @"{""ModelRights"": 0, ""ScenarioRights"": 0, ""ScenarioRunRights"": 0}", @"{}");
    }

    public partial class ScenarioPlannerRoleScenarioUser : ScenarioPlannerRole
    {
        private ScenarioPlannerRoleScenarioUser(int scenarioPlannerRoleID, string scenarioPlannerRoleName, string scenarioPlannerRoleDisplayName, string scenarioPlannerRoleDescription, int sortOrder, string rights, string flags) : base(scenarioPlannerRoleID, scenarioPlannerRoleName, scenarioPlannerRoleDisplayName, scenarioPlannerRoleDescription, sortOrder, rights, flags) {}
        public static readonly ScenarioPlannerRoleScenarioUser Instance = new ScenarioPlannerRoleScenarioUser(2, @"ScenarioUser", @"Scenario User", null, 20, @"{""ModelRights"": 15, ""ScenarioRights"": 15, ""ScenarioRunRights"": 15}", @"{}");
    }
}