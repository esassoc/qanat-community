//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[Role]
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Qanat.Models.DataTransferObjects;


namespace Qanat.EFModels.Entities
{
    public abstract partial class Role : IHavePrimaryKey
    {
        public static readonly RoleSystemAdmin SystemAdmin = RoleSystemAdmin.Instance;
        public static readonly RoleNoAccess NoAccess = RoleNoAccess.Instance;
        public static readonly RoleNormal Normal = RoleNormal.Instance;
        public static readonly RolePendingLogin PendingLogin = RolePendingLogin.Instance;

        public static readonly List<Role> All;
        public static readonly List<RoleSimpleDto> AllAsSimpleDto;
        public static readonly ReadOnlyDictionary<int, Role> AllLookupDictionary;
        public static readonly ReadOnlyDictionary<int, RoleSimpleDto> AllAsSimpleDtoLookupDictionary;

        /// <summary>
        /// Static type constructor to coordinate static initialization order
        /// </summary>
        static Role()
        {
            All = new List<Role> { SystemAdmin, NoAccess, Normal, PendingLogin };
            AllAsSimpleDto = new List<RoleSimpleDto> { SystemAdmin.AsSimpleDto(), NoAccess.AsSimpleDto(), Normal.AsSimpleDto(), PendingLogin.AsSimpleDto() };
            AllLookupDictionary = new ReadOnlyDictionary<int, Role>(All.ToDictionary(x => x.RoleID));
            AllAsSimpleDtoLookupDictionary = new ReadOnlyDictionary<int, RoleSimpleDto>(AllAsSimpleDto.ToDictionary(x => x.RoleID));
        }

        /// <summary>
        /// Protected constructor only for use in instantiating the set of static lookup values that match database
        /// </summary>
        protected Role(int roleID, string roleName, string roleDisplayName, string roleDescription, int sortOrder, string rights, string flags)
        {
            RoleID = roleID;
            RoleName = roleName;
            RoleDisplayName = roleDisplayName;
            RoleDescription = roleDescription;
            SortOrder = sortOrder;
            Rights = rights;
            Flags = flags;
        }

        [Key]
        public int RoleID { get; private set; }
        public string RoleName { get; private set; }
        public string RoleDisplayName { get; private set; }
        public string RoleDescription { get; private set; }
        public int SortOrder { get; private set; }
        public string Rights { get; private set; }
        public string Flags { get; private set; }
        [NotMapped]
        public int PrimaryKey { get { return RoleID; } }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public bool Equals(Role other)
        {
            if (other == null)
            {
                return false;
            }
            return other.RoleID == RoleID;
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override bool Equals(object obj)
        {
            return Equals(obj as Role);
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override int GetHashCode()
        {
            return RoleID;
        }

        public static bool operator ==(Role left, Role right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Role left, Role right)
        {
            return !Equals(left, right);
        }

        public RoleEnum ToEnum => (RoleEnum)GetHashCode();

        public static Role ToType(int enumValue)
        {
            return ToType((RoleEnum)enumValue);
        }

        public static Role ToType(RoleEnum enumValue)
        {
            switch (enumValue)
            {
                case RoleEnum.NoAccess:
                    return NoAccess;
                case RoleEnum.Normal:
                    return Normal;
                case RoleEnum.PendingLogin:
                    return PendingLogin;
                case RoleEnum.SystemAdmin:
                    return SystemAdmin;
                default:
                    throw new ArgumentException("Unable to map Enum: {enumValue}");
            }
        }
    }

    public enum RoleEnum
    {
        SystemAdmin = 1,
        NoAccess = 2,
        Normal = 3,
        PendingLogin = 4
    }

    public partial class RoleSystemAdmin : Role
    {
        private RoleSystemAdmin(int roleID, string roleName, string roleDisplayName, string roleDescription, int sortOrder, string rights, string flags) : base(roleID, roleName, roleDisplayName, roleDescription, sortOrder, rights, flags) {}
        public static readonly RoleSystemAdmin Instance = new RoleSystemAdmin(1, @"SystemAdmin", @"System Administrator", @"", 30, @"{""CustomRichTextRights"":15,""FieldDefinitionRights"":15,""FileResourceRights"":15,""UserRights"":15,""WaterAccountRights"":15,""ParcelRights"":15,""TagRights"":15,""WellRights"":15,""WaterTransactionRights"":15,""GETActionRights"":15,""ReportingPeriodRights"":15,""WaterTypeRights"":15,""GeographyRights"":15, ""ExternalMapLayerRights"":15, ""WaterAccountUserRights"": 15, ""ZoneGroupRights"": 15, ""MonitoringWellRights"":15, ""AllocationPlanRights"": 15, ""FrequentlyAskedQuestionRights"": 15, ""CustomAttributeRights"": 15, ""UsageEntityRights"": 15}", @"{""CanImpersonateUsers"":true,""HasManagerDashboard"":true,""HasAdminDashboard"":true,""CanClaimWaterAccounts"":true,""CanRegisterWells"":true,""CanReviewWells"":true, ""CanUseScenarioPlanner"":true}");
    }

    public partial class RoleNoAccess : Role
    {
        private RoleNoAccess(int roleID, string roleName, string roleDisplayName, string roleDescription, int sortOrder, string rights, string flags) : base(roleID, roleName, roleDisplayName, roleDescription, sortOrder, rights, flags) {}
        public static readonly RoleNoAccess Instance = new RoleNoAccess(2, @"NoAccess", @"No Access", @"", 10, @"{""CustomRichTextRights"":0,""FieldDefinitionRights"":0,""FileResourceRights"":0,""UserRights"":0,""WaterAccountRights"":0,""ParcelRights"":0,""TagRights"":0,""WellRights"":0,""WaterTransactionRights"":0,""GETActionRights"":0,""ReportingPeriodRights"":0,""WaterTypeRights"":0,""GeographyRights"":0, ""ExternalMapLayerRights"":0, ""WaterAccountUserRights"": 0, ""ZoneGroupRights"": 0, ""MonitoringWellRights"": 0, ""AllocationPlanRights"": 0, ""FrequentlyAskedQuestionRights"": 1, ""CustomAttributeRights"": 0, ""UsageEntityRights"": 0}", @"{""CanImpersonateUsers"":false,""HasManagerDashboard"":false,""HasAdminDashboard"":false,""CanClaimWaterAccounts"":false,""CanRegisterWells"":false,""CanReviewWells"":false, ""CanUseScenarioPlanner"":false}");
    }

    public partial class RoleNormal : Role
    {
        private RoleNormal(int roleID, string roleName, string roleDisplayName, string roleDescription, int sortOrder, string rights, string flags) : base(roleID, roleName, roleDisplayName, roleDescription, sortOrder, rights, flags) {}
        public static readonly RoleNormal Instance = new RoleNormal(3, @"Normal", @"Normal", @"", 20, @"{""CustomRichTextRights"":1,""FieldDefinitionRights"":1,""FileResourceRights"":1,""UserRights"":1,""WaterAccountRights"":0,""ParcelRights"":0,""TagRights"":0,""WellRights"":0,""WaterTransactionRights"":0,""GETActionRights"":0,""ReportingPeriodRights"":0,""WaterTypeRights"":0,""GeographyRights"":1, ""ExternalMapLayerRights"":0, ""WaterAccountUserRights"": 0, ""ZoneGroupRights"": 1, ""MonitoringWellRights"": 0, ""AllocationPlanRights"": 0, ""FrequentlyAskedQuestionRights"": 1, ""CustomAttributeRights"": 0, ""UsageEntityRights"": 0}", @"{""CanImpersonateUsers"":false,""HasManagerDashboard"":false,""HasAdminDashboard"":false,""CanClaimWaterAccounts"":true,""CanRegisterWells"":true,""CanReviewWells"":false, ""CanUseScenarioPlanner"":false}");
    }

    public partial class RolePendingLogin : Role
    {
        private RolePendingLogin(int roleID, string roleName, string roleDisplayName, string roleDescription, int sortOrder, string rights, string flags) : base(roleID, roleName, roleDisplayName, roleDescription, sortOrder, rights, flags) {}
        public static readonly RolePendingLogin Instance = new RolePendingLogin(4, @"PendingLogin", @"Pending Login", @"", 10, @"{""CustomRichTextRights"":0,""FieldDefinitionRights"":0,""FileResourceRights"":0,""UserRights"":0,""WaterAccountRights"":0,""ParcelRights"":0,""TagRights"":0,""WellRights"":0,""WaterTransactionRights"":0,""GETActionRights"":0,""ReportingPeriodRights"":0,""WaterTypeRights"":0,""GeographyRights"":0, ""ExternalMapLayerRights"":0, ""WaterAccountUserRights"": 0, ""ZoneGroupRights"": 0, ""MonitoringWellRights"": 0, ""AllocationPlanRights"": 0, ""FrequentlyAskedQuestionRights"": 1, ""CustomAttributeRights"": 0, ""UsageEntityRights"": 0}", @"{""CanImpersonateUsers"":false,""HasManagerDashboard"":false,""HasAdminDashboard"":false,""CanClaimWaterAccounts"":false,""CanRegisterWells"":false,""CanReviewWells"":false, ""CanUseScenarioPlanner"":false}");
    }
}