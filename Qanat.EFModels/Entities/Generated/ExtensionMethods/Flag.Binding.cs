//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[Flag]
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Qanat.Models.DataTransferObjects;


namespace Qanat.EFModels.Entities
{
    public abstract partial class Flag : IHavePrimaryKey
    {
        public static readonly FlagCanImpersonateUsers CanImpersonateUsers = FlagCanImpersonateUsers.Instance;
        public static readonly FlagHasManagerDashboard HasManagerDashboard = FlagHasManagerDashboard.Instance;
        public static readonly FlagHasAdminDashboard HasAdminDashboard = FlagHasAdminDashboard.Instance;
        public static readonly FlagCanClaimWaterAccounts CanClaimWaterAccounts = FlagCanClaimWaterAccounts.Instance;
        public static readonly FlagCanRegisterWells CanRegisterWells = FlagCanRegisterWells.Instance;
        public static readonly FlagCanReviewWells CanReviewWells = FlagCanReviewWells.Instance;
        public static readonly FlagCanUseScenarioPlanner CanUseScenarioPlanner = FlagCanUseScenarioPlanner.Instance;

        public static readonly List<Flag> All;
        public static readonly List<FlagSimpleDto> AllAsSimpleDto;
        public static readonly ReadOnlyDictionary<int, Flag> AllLookupDictionary;
        public static readonly ReadOnlyDictionary<int, FlagSimpleDto> AllAsSimpleDtoLookupDictionary;

        /// <summary>
        /// Static type constructor to coordinate static initialization order
        /// </summary>
        static Flag()
        {
            All = new List<Flag> { CanImpersonateUsers, HasManagerDashboard, HasAdminDashboard, CanClaimWaterAccounts, CanRegisterWells, CanReviewWells, CanUseScenarioPlanner };
            AllAsSimpleDto = new List<FlagSimpleDto> { CanImpersonateUsers.AsSimpleDto(), HasManagerDashboard.AsSimpleDto(), HasAdminDashboard.AsSimpleDto(), CanClaimWaterAccounts.AsSimpleDto(), CanRegisterWells.AsSimpleDto(), CanReviewWells.AsSimpleDto(), CanUseScenarioPlanner.AsSimpleDto() };
            AllLookupDictionary = new ReadOnlyDictionary<int, Flag>(All.ToDictionary(x => x.FlagID));
            AllAsSimpleDtoLookupDictionary = new ReadOnlyDictionary<int, FlagSimpleDto>(AllAsSimpleDto.ToDictionary(x => x.FlagID));
        }

        /// <summary>
        /// Protected constructor only for use in instantiating the set of static lookup values that match database
        /// </summary>
        protected Flag(int flagID, string flagName, string flagDisplayName)
        {
            FlagID = flagID;
            FlagName = flagName;
            FlagDisplayName = flagDisplayName;
        }

        [Key]
        public int FlagID { get; private set; }
        public string FlagName { get; private set; }
        public string FlagDisplayName { get; private set; }
        [NotMapped]
        public int PrimaryKey { get { return FlagID; } }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public bool Equals(Flag other)
        {
            if (other == null)
            {
                return false;
            }
            return other.FlagID == FlagID;
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override bool Equals(object obj)
        {
            return Equals(obj as Flag);
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override int GetHashCode()
        {
            return FlagID;
        }

        public static bool operator ==(Flag left, Flag right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Flag left, Flag right)
        {
            return !Equals(left, right);
        }

        public FlagEnum ToEnum => (FlagEnum)GetHashCode();

        public static Flag ToType(int enumValue)
        {
            return ToType((FlagEnum)enumValue);
        }

        public static Flag ToType(FlagEnum enumValue)
        {
            switch (enumValue)
            {
                case FlagEnum.CanClaimWaterAccounts:
                    return CanClaimWaterAccounts;
                case FlagEnum.CanImpersonateUsers:
                    return CanImpersonateUsers;
                case FlagEnum.CanRegisterWells:
                    return CanRegisterWells;
                case FlagEnum.CanReviewWells:
                    return CanReviewWells;
                case FlagEnum.CanUseScenarioPlanner:
                    return CanUseScenarioPlanner;
                case FlagEnum.HasAdminDashboard:
                    return HasAdminDashboard;
                case FlagEnum.HasManagerDashboard:
                    return HasManagerDashboard;
                default:
                    throw new ArgumentException("Unable to map Enum: {enumValue}");
            }
        }
    }

    public enum FlagEnum
    {
        CanImpersonateUsers = 1,
        HasManagerDashboard = 2,
        HasAdminDashboard = 3,
        CanClaimWaterAccounts = 4,
        CanRegisterWells = 5,
        CanReviewWells = 6,
        CanUseScenarioPlanner = 7
    }

    public partial class FlagCanImpersonateUsers : Flag
    {
        private FlagCanImpersonateUsers(int flagID, string flagName, string flagDisplayName) : base(flagID, flagName, flagDisplayName) {}
        public static readonly FlagCanImpersonateUsers Instance = new FlagCanImpersonateUsers(1, @"CanImpersonateUsers", @"CanImpersonateUsers");
    }

    public partial class FlagHasManagerDashboard : Flag
    {
        private FlagHasManagerDashboard(int flagID, string flagName, string flagDisplayName) : base(flagID, flagName, flagDisplayName) {}
        public static readonly FlagHasManagerDashboard Instance = new FlagHasManagerDashboard(2, @"HasManagerDashboard", @"HasManagerDashboard");
    }

    public partial class FlagHasAdminDashboard : Flag
    {
        private FlagHasAdminDashboard(int flagID, string flagName, string flagDisplayName) : base(flagID, flagName, flagDisplayName) {}
        public static readonly FlagHasAdminDashboard Instance = new FlagHasAdminDashboard(3, @"HasAdminDashboard", @"HasAdminDashboard");
    }

    public partial class FlagCanClaimWaterAccounts : Flag
    {
        private FlagCanClaimWaterAccounts(int flagID, string flagName, string flagDisplayName) : base(flagID, flagName, flagDisplayName) {}
        public static readonly FlagCanClaimWaterAccounts Instance = new FlagCanClaimWaterAccounts(4, @"CanClaimWaterAccounts", @"CanClaimWaterAccounts");
    }

    public partial class FlagCanRegisterWells : Flag
    {
        private FlagCanRegisterWells(int flagID, string flagName, string flagDisplayName) : base(flagID, flagName, flagDisplayName) {}
        public static readonly FlagCanRegisterWells Instance = new FlagCanRegisterWells(5, @"CanRegisterWells", @"CanRegisterWells");
    }

    public partial class FlagCanReviewWells : Flag
    {
        private FlagCanReviewWells(int flagID, string flagName, string flagDisplayName) : base(flagID, flagName, flagDisplayName) {}
        public static readonly FlagCanReviewWells Instance = new FlagCanReviewWells(6, @"CanReviewWells", @"CanReviewWells");
    }

    public partial class FlagCanUseScenarioPlanner : Flag
    {
        private FlagCanUseScenarioPlanner(int flagID, string flagName, string flagDisplayName) : base(flagID, flagName, flagDisplayName) {}
        public static readonly FlagCanUseScenarioPlanner Instance = new FlagCanUseScenarioPlanner(7, @"CanUseScenarioPlanner", @"CanUseScenarioPlanner");
    }
}