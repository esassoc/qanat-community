//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[WaterAccountRole]
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Qanat.Models.DataTransferObjects;


namespace Qanat.EFModels.Entities
{
    public abstract partial class WaterAccountRole : IHavePrimaryKey
    {
        public static readonly WaterAccountRoleWaterAccountHolder WaterAccountHolder = WaterAccountRoleWaterAccountHolder.Instance;
        public static readonly WaterAccountRoleWaterAccountViewer WaterAccountViewer = WaterAccountRoleWaterAccountViewer.Instance;

        public static readonly List<WaterAccountRole> All;
        public static readonly List<WaterAccountRoleSimpleDto> AllAsSimpleDto;
        public static readonly ReadOnlyDictionary<int, WaterAccountRole> AllLookupDictionary;
        public static readonly ReadOnlyDictionary<int, WaterAccountRoleSimpleDto> AllAsSimpleDtoLookupDictionary;

        /// <summary>
        /// Static type constructor to coordinate static initialization order
        /// </summary>
        static WaterAccountRole()
        {
            All = new List<WaterAccountRole> { WaterAccountHolder, WaterAccountViewer };
            AllAsSimpleDto = new List<WaterAccountRoleSimpleDto> { WaterAccountHolder.AsSimpleDto(), WaterAccountViewer.AsSimpleDto() };
            AllLookupDictionary = new ReadOnlyDictionary<int, WaterAccountRole>(All.ToDictionary(x => x.WaterAccountRoleID));
            AllAsSimpleDtoLookupDictionary = new ReadOnlyDictionary<int, WaterAccountRoleSimpleDto>(AllAsSimpleDto.ToDictionary(x => x.WaterAccountRoleID));
        }

        /// <summary>
        /// Protected constructor only for use in instantiating the set of static lookup values that match database
        /// </summary>
        protected WaterAccountRole(int waterAccountRoleID, string waterAccountRoleName, string waterAccountRoleDisplayName, string waterAccountRoleDescription, int sortOrder, string rights, string flags)
        {
            WaterAccountRoleID = waterAccountRoleID;
            WaterAccountRoleName = waterAccountRoleName;
            WaterAccountRoleDisplayName = waterAccountRoleDisplayName;
            WaterAccountRoleDescription = waterAccountRoleDescription;
            SortOrder = sortOrder;
            Rights = rights;
            Flags = flags;
        }

        [Key]
        public int WaterAccountRoleID { get; private set; }
        public string WaterAccountRoleName { get; private set; }
        public string WaterAccountRoleDisplayName { get; private set; }
        public string WaterAccountRoleDescription { get; private set; }
        public int SortOrder { get; private set; }
        public string Rights { get; private set; }
        public string Flags { get; private set; }
        [NotMapped]
        public int PrimaryKey { get { return WaterAccountRoleID; } }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public bool Equals(WaterAccountRole other)
        {
            if (other == null)
            {
                return false;
            }
            return other.WaterAccountRoleID == WaterAccountRoleID;
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override bool Equals(object obj)
        {
            return Equals(obj as WaterAccountRole);
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override int GetHashCode()
        {
            return WaterAccountRoleID;
        }

        public static bool operator ==(WaterAccountRole left, WaterAccountRole right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(WaterAccountRole left, WaterAccountRole right)
        {
            return !Equals(left, right);
        }

        public WaterAccountRoleEnum ToEnum => (WaterAccountRoleEnum)GetHashCode();

        public static WaterAccountRole ToType(int enumValue)
        {
            return ToType((WaterAccountRoleEnum)enumValue);
        }

        public static WaterAccountRole ToType(WaterAccountRoleEnum enumValue)
        {
            switch (enumValue)
            {
                case WaterAccountRoleEnum.WaterAccountHolder:
                    return WaterAccountHolder;
                case WaterAccountRoleEnum.WaterAccountViewer:
                    return WaterAccountViewer;
                default:
                    throw new ArgumentException("Unable to map Enum: {enumValue}");
            }
        }
    }

    public enum WaterAccountRoleEnum
    {
        WaterAccountHolder = 1,
        WaterAccountViewer = 2
    }

    public partial class WaterAccountRoleWaterAccountHolder : WaterAccountRole
    {
        private WaterAccountRoleWaterAccountHolder(int waterAccountRoleID, string waterAccountRoleName, string waterAccountRoleDisplayName, string waterAccountRoleDescription, int sortOrder, string rights, string flags) : base(waterAccountRoleID, waterAccountRoleName, waterAccountRoleDisplayName, waterAccountRoleDescription, sortOrder, rights, flags) {}
        public static readonly WaterAccountRoleWaterAccountHolder Instance = new WaterAccountRoleWaterAccountHolder(1, @"WaterAccountHolder", @"Account Holder", @"", 10, @"{""WaterAccountRights"":15,""ParcelRights"":15,""WaterTransactionRights"":15,""WaterAccountUserRights"":15,""AllocationPlanRights"":1, ""UsageEntityRights"": 1}", @"{}");
    }

    public partial class WaterAccountRoleWaterAccountViewer : WaterAccountRole
    {
        private WaterAccountRoleWaterAccountViewer(int waterAccountRoleID, string waterAccountRoleName, string waterAccountRoleDisplayName, string waterAccountRoleDescription, int sortOrder, string rights, string flags) : base(waterAccountRoleID, waterAccountRoleName, waterAccountRoleDisplayName, waterAccountRoleDescription, sortOrder, rights, flags) {}
        public static readonly WaterAccountRoleWaterAccountViewer Instance = new WaterAccountRoleWaterAccountViewer(2, @"WaterAccountViewer", @"Viewer", @"", 20, @"{""WaterAccountRights"":1,""ParcelRights"":1,""WaterTransactionRights"":1,""WaterAccountUserRights"":1,""AllocationPlanRights"":1, ""UsageEntityRights"": 1}", @"{}");
    }
}