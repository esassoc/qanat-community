//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[GeographyRole]
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Qanat.Models.DataTransferObjects;


namespace Qanat.EFModels.Entities
{
    public abstract partial class GeographyRole : IHavePrimaryKey
    {
        public static readonly GeographyRoleWaterManager WaterManager = GeographyRoleWaterManager.Instance;
        public static readonly GeographyRoleNormal Normal = GeographyRoleNormal.Instance;

        public static readonly List<GeographyRole> All;
        public static readonly ReadOnlyDictionary<int, GeographyRole> AllLookupDictionary;

        /// <summary>
        /// Static type constructor to coordinate static initialization order
        /// </summary>
        static GeographyRole()
        {
            All = new List<GeographyRole> { WaterManager, Normal };
            AllLookupDictionary = new ReadOnlyDictionary<int, GeographyRole>(All.ToDictionary(x => x.GeographyRoleID));
        }

        /// <summary>
        /// Protected constructor only for use in instantiating the set of static lookup values that match database
        /// </summary>
        protected GeographyRole(int geographyRoleID, string geographyRoleName, string geographyRoleDisplayName, string geographyRoleDescription, int sortOrder, string rights, string flags)
        {
            GeographyRoleID = geographyRoleID;
            GeographyRoleName = geographyRoleName;
            GeographyRoleDisplayName = geographyRoleDisplayName;
            GeographyRoleDescription = geographyRoleDescription;
            SortOrder = sortOrder;
            Rights = rights;
            Flags = flags;
        }

        [Key]
        public int GeographyRoleID { get; private set; }
        public string GeographyRoleName { get; private set; }
        public string GeographyRoleDisplayName { get; private set; }
        public string GeographyRoleDescription { get; private set; }
        public int SortOrder { get; private set; }
        public string Rights { get; private set; }
        public string Flags { get; private set; }
        [NotMapped]
        public int PrimaryKey { get { return GeographyRoleID; } }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public bool Equals(GeographyRole other)
        {
            if (other == null)
            {
                return false;
            }
            return other.GeographyRoleID == GeographyRoleID;
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override bool Equals(object obj)
        {
            return Equals(obj as GeographyRole);
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override int GetHashCode()
        {
            return GeographyRoleID;
        }

        public static bool operator ==(GeographyRole left, GeographyRole right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(GeographyRole left, GeographyRole right)
        {
            return !Equals(left, right);
        }

        public GeographyRoleEnum ToEnum => (GeographyRoleEnum)GetHashCode();

        public static GeographyRole ToType(int enumValue)
        {
            return ToType((GeographyRoleEnum)enumValue);
        }

        public static GeographyRole ToType(GeographyRoleEnum enumValue)
        {
            switch (enumValue)
            {
                case GeographyRoleEnum.Normal:
                    return Normal;
                case GeographyRoleEnum.WaterManager:
                    return WaterManager;
                default:
                    throw new ArgumentException("Unable to map Enum: {enumValue}");
            }
        }
    }

    public enum GeographyRoleEnum
    {
        WaterManager = 1,
        Normal = 2
    }

    public partial class GeographyRoleWaterManager : GeographyRole
    {
        private GeographyRoleWaterManager(int geographyRoleID, string geographyRoleName, string geographyRoleDisplayName, string geographyRoleDescription, int sortOrder, string rights, string flags) : base(geographyRoleID, geographyRoleName, geographyRoleDisplayName, geographyRoleDescription, sortOrder, rights, flags) {}
        public static readonly GeographyRoleWaterManager Instance = new GeographyRoleWaterManager(1, @"WaterManager", @"Water Manager", @"", 10, @"{""UserRights"":15,""WaterAccountRights"":15,""ParcelRights"":15,""WellRights"":15,""WaterTransactionRights"":15,""ReportingPeriodRights"":15,""WaterTypeRights"":15, ""ExternalMapLayerRights"": 15, ""WaterAccountUserRights"": 15, ""ZoneGroupRights"": 15, ""MonitoringWellRights"":15, ""AllocationPlanRights"": 15, ""GeographyRights"": 15, ""CustomAttributeRights"": 15, ""UsageLocationRights"": 15, ""MeterRights"": 15, ""WellMeterReadingRights"": 15, ""StatementTemplateRights"": 15}", @"{""HasManagerDashboard"":true,""CanReviewWells"":true, ""CanUseScenarioPlanner"":true}");
    }

    public partial class GeographyRoleNormal : GeographyRole
    {
        private GeographyRoleNormal(int geographyRoleID, string geographyRoleName, string geographyRoleDisplayName, string geographyRoleDescription, int sortOrder, string rights, string flags) : base(geographyRoleID, geographyRoleName, geographyRoleDisplayName, geographyRoleDescription, sortOrder, rights, flags) {}
        public static readonly GeographyRoleNormal Instance = new GeographyRoleNormal(2, @"Normal", @"Normal", @"", 20, @"{""UserRights"":1,""WaterAccountRights"":0,""ParcelRights"":0,""WellRights"":1,""WaterTransactionRights"":0,""ReportingPeriodRights"":1,""WaterTypeRights"":1, ""ExternalMapLayerRights"": 1, ""WaterAccountUserRights"": 0, ""ZoneGroupRights"": 1, ""MonitoringWellRights"":1, ""AllocationPlanRights"": 1, ""CustomAttributeRights"": 0, ""UsageLocationRights"": 0, ""MeterRights"": 0, ""WellMeterReadingRights"": 0, ""StatementTemplateRights"": 0}", @"{""HasManagerDashboard"":false,""CanReviewWells"":false, ""CanUseScenarioPlanner"":false}");
    }
}