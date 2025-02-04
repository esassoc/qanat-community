//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[Permission]
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Qanat.Models.DataTransferObjects;


namespace Qanat.EFModels.Entities
{
    public abstract partial class Permission : IHavePrimaryKey
    {
        public static readonly PermissionCustomRichTextRights CustomRichTextRights = PermissionCustomRichTextRights.Instance;
        public static readonly PermissionFieldDefinitionRights FieldDefinitionRights = PermissionFieldDefinitionRights.Instance;
        public static readonly PermissionFileResourceRights FileResourceRights = PermissionFileResourceRights.Instance;
        public static readonly PermissionUserRights UserRights = PermissionUserRights.Instance;
        public static readonly PermissionWaterAccountRights WaterAccountRights = PermissionWaterAccountRights.Instance;
        public static readonly PermissionParcelRights ParcelRights = PermissionParcelRights.Instance;
        public static readonly PermissionTagRights TagRights = PermissionTagRights.Instance;
        public static readonly PermissionWellRights WellRights = PermissionWellRights.Instance;
        public static readonly PermissionWaterTransactionRights WaterTransactionRights = PermissionWaterTransactionRights.Instance;
        public static readonly PermissionReportingPeriodRights ReportingPeriodRights = PermissionReportingPeriodRights.Instance;
        public static readonly PermissionWaterTypeRights WaterTypeRights = PermissionWaterTypeRights.Instance;
        public static readonly PermissionGeographyRights GeographyRights = PermissionGeographyRights.Instance;
        public static readonly PermissionExternalMapLayerRights ExternalMapLayerRights = PermissionExternalMapLayerRights.Instance;
        public static readonly PermissionWaterAccountUserRights WaterAccountUserRights = PermissionWaterAccountUserRights.Instance;
        public static readonly PermissionZoneGroupRights ZoneGroupRights = PermissionZoneGroupRights.Instance;
        public static readonly PermissionMonitoringWellRights MonitoringWellRights = PermissionMonitoringWellRights.Instance;
        public static readonly PermissionAllocationPlanRights AllocationPlanRights = PermissionAllocationPlanRights.Instance;
        public static readonly PermissionFrequentlyAskedQuestionRights FrequentlyAskedQuestionRights = PermissionFrequentlyAskedQuestionRights.Instance;
        public static readonly PermissionCustomAttributeRights CustomAttributeRights = PermissionCustomAttributeRights.Instance;
        public static readonly PermissionUsageEntityRights UsageEntityRights = PermissionUsageEntityRights.Instance;
        public static readonly PermissionModelRights ModelRights = PermissionModelRights.Instance;
        public static readonly PermissionScenarioRights ScenarioRights = PermissionScenarioRights.Instance;
        public static readonly PermissionScenarioRunRights ScenarioRunRights = PermissionScenarioRunRights.Instance;

        public static readonly List<Permission> All;
        public static readonly List<PermissionSimpleDto> AllAsSimpleDto;
        public static readonly ReadOnlyDictionary<int, Permission> AllLookupDictionary;
        public static readonly ReadOnlyDictionary<int, PermissionSimpleDto> AllAsSimpleDtoLookupDictionary;

        /// <summary>
        /// Static type constructor to coordinate static initialization order
        /// </summary>
        static Permission()
        {
            All = new List<Permission> { CustomRichTextRights, FieldDefinitionRights, FileResourceRights, UserRights, WaterAccountRights, ParcelRights, TagRights, WellRights, WaterTransactionRights, ReportingPeriodRights, WaterTypeRights, GeographyRights, ExternalMapLayerRights, WaterAccountUserRights, ZoneGroupRights, MonitoringWellRights, AllocationPlanRights, FrequentlyAskedQuestionRights, CustomAttributeRights, UsageEntityRights, ModelRights, ScenarioRights, ScenarioRunRights };
            AllAsSimpleDto = new List<PermissionSimpleDto> { CustomRichTextRights.AsSimpleDto(), FieldDefinitionRights.AsSimpleDto(), FileResourceRights.AsSimpleDto(), UserRights.AsSimpleDto(), WaterAccountRights.AsSimpleDto(), ParcelRights.AsSimpleDto(), TagRights.AsSimpleDto(), WellRights.AsSimpleDto(), WaterTransactionRights.AsSimpleDto(), ReportingPeriodRights.AsSimpleDto(), WaterTypeRights.AsSimpleDto(), GeographyRights.AsSimpleDto(), ExternalMapLayerRights.AsSimpleDto(), WaterAccountUserRights.AsSimpleDto(), ZoneGroupRights.AsSimpleDto(), MonitoringWellRights.AsSimpleDto(), AllocationPlanRights.AsSimpleDto(), FrequentlyAskedQuestionRights.AsSimpleDto(), CustomAttributeRights.AsSimpleDto(), UsageEntityRights.AsSimpleDto(), ModelRights.AsSimpleDto(), ScenarioRights.AsSimpleDto(), ScenarioRunRights.AsSimpleDto() };
            AllLookupDictionary = new ReadOnlyDictionary<int, Permission>(All.ToDictionary(x => x.PermissionID));
            AllAsSimpleDtoLookupDictionary = new ReadOnlyDictionary<int, PermissionSimpleDto>(AllAsSimpleDto.ToDictionary(x => x.PermissionID));
        }

        /// <summary>
        /// Protected constructor only for use in instantiating the set of static lookup values that match database
        /// </summary>
        protected Permission(int permissionID, string permissionName, string permissionDisplayName)
        {
            PermissionID = permissionID;
            PermissionName = permissionName;
            PermissionDisplayName = permissionDisplayName;
        }

        [Key]
        public int PermissionID { get; private set; }
        public string PermissionName { get; private set; }
        public string PermissionDisplayName { get; private set; }
        [NotMapped]
        public int PrimaryKey { get { return PermissionID; } }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public bool Equals(Permission other)
        {
            if (other == null)
            {
                return false;
            }
            return other.PermissionID == PermissionID;
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override bool Equals(object obj)
        {
            return Equals(obj as Permission);
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override int GetHashCode()
        {
            return PermissionID;
        }

        public static bool operator ==(Permission left, Permission right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Permission left, Permission right)
        {
            return !Equals(left, right);
        }

        public PermissionEnum ToEnum => (PermissionEnum)GetHashCode();

        public static Permission ToType(int enumValue)
        {
            return ToType((PermissionEnum)enumValue);
        }

        public static Permission ToType(PermissionEnum enumValue)
        {
            switch (enumValue)
            {
                case PermissionEnum.AllocationPlanRights:
                    return AllocationPlanRights;
                case PermissionEnum.CustomAttributeRights:
                    return CustomAttributeRights;
                case PermissionEnum.CustomRichTextRights:
                    return CustomRichTextRights;
                case PermissionEnum.ExternalMapLayerRights:
                    return ExternalMapLayerRights;
                case PermissionEnum.FieldDefinitionRights:
                    return FieldDefinitionRights;
                case PermissionEnum.FileResourceRights:
                    return FileResourceRights;
                case PermissionEnum.FrequentlyAskedQuestionRights:
                    return FrequentlyAskedQuestionRights;
                case PermissionEnum.GeographyRights:
                    return GeographyRights;
                case PermissionEnum.ModelRights:
                    return ModelRights;
                case PermissionEnum.MonitoringWellRights:
                    return MonitoringWellRights;
                case PermissionEnum.ParcelRights:
                    return ParcelRights;
                case PermissionEnum.ReportingPeriodRights:
                    return ReportingPeriodRights;
                case PermissionEnum.ScenarioRights:
                    return ScenarioRights;
                case PermissionEnum.ScenarioRunRights:
                    return ScenarioRunRights;
                case PermissionEnum.TagRights:
                    return TagRights;
                case PermissionEnum.UsageEntityRights:
                    return UsageEntityRights;
                case PermissionEnum.UserRights:
                    return UserRights;
                case PermissionEnum.WaterAccountRights:
                    return WaterAccountRights;
                case PermissionEnum.WaterAccountUserRights:
                    return WaterAccountUserRights;
                case PermissionEnum.WaterTransactionRights:
                    return WaterTransactionRights;
                case PermissionEnum.WaterTypeRights:
                    return WaterTypeRights;
                case PermissionEnum.WellRights:
                    return WellRights;
                case PermissionEnum.ZoneGroupRights:
                    return ZoneGroupRights;
                default:
                    throw new ArgumentException("Unable to map Enum: {enumValue}");
            }
        }
    }

    public enum PermissionEnum
    {
        CustomRichTextRights = 2,
        FieldDefinitionRights = 3,
        FileResourceRights = 4,
        UserRights = 5,
        WaterAccountRights = 6,
        ParcelRights = 7,
        TagRights = 8,
        WellRights = 9,
        WaterTransactionRights = 10,
        ReportingPeriodRights = 12,
        WaterTypeRights = 13,
        GeographyRights = 14,
        ExternalMapLayerRights = 15,
        WaterAccountUserRights = 16,
        ZoneGroupRights = 17,
        MonitoringWellRights = 18,
        AllocationPlanRights = 19,
        FrequentlyAskedQuestionRights = 20,
        CustomAttributeRights = 21,
        UsageEntityRights = 22,
        ModelRights = 23,
        ScenarioRights = 24,
        ScenarioRunRights = 25
    }

    public partial class PermissionCustomRichTextRights : Permission
    {
        private PermissionCustomRichTextRights(int permissionID, string permissionName, string permissionDisplayName) : base(permissionID, permissionName, permissionDisplayName) {}
        public static readonly PermissionCustomRichTextRights Instance = new PermissionCustomRichTextRights(2, @"CustomRichTextRights", @"CustomRichTextRights");
    }

    public partial class PermissionFieldDefinitionRights : Permission
    {
        private PermissionFieldDefinitionRights(int permissionID, string permissionName, string permissionDisplayName) : base(permissionID, permissionName, permissionDisplayName) {}
        public static readonly PermissionFieldDefinitionRights Instance = new PermissionFieldDefinitionRights(3, @"FieldDefinitionRights", @"FieldDefinitionRights");
    }

    public partial class PermissionFileResourceRights : Permission
    {
        private PermissionFileResourceRights(int permissionID, string permissionName, string permissionDisplayName) : base(permissionID, permissionName, permissionDisplayName) {}
        public static readonly PermissionFileResourceRights Instance = new PermissionFileResourceRights(4, @"FileResourceRights", @"FileResourceRights");
    }

    public partial class PermissionUserRights : Permission
    {
        private PermissionUserRights(int permissionID, string permissionName, string permissionDisplayName) : base(permissionID, permissionName, permissionDisplayName) {}
        public static readonly PermissionUserRights Instance = new PermissionUserRights(5, @"UserRights", @"UserRights");
    }

    public partial class PermissionWaterAccountRights : Permission
    {
        private PermissionWaterAccountRights(int permissionID, string permissionName, string permissionDisplayName) : base(permissionID, permissionName, permissionDisplayName) {}
        public static readonly PermissionWaterAccountRights Instance = new PermissionWaterAccountRights(6, @"WaterAccountRights", @"WaterAccountRights");
    }

    public partial class PermissionParcelRights : Permission
    {
        private PermissionParcelRights(int permissionID, string permissionName, string permissionDisplayName) : base(permissionID, permissionName, permissionDisplayName) {}
        public static readonly PermissionParcelRights Instance = new PermissionParcelRights(7, @"ParcelRights", @"ParcelRights");
    }

    public partial class PermissionTagRights : Permission
    {
        private PermissionTagRights(int permissionID, string permissionName, string permissionDisplayName) : base(permissionID, permissionName, permissionDisplayName) {}
        public static readonly PermissionTagRights Instance = new PermissionTagRights(8, @"TagRights", @"TagRights");
    }

    public partial class PermissionWellRights : Permission
    {
        private PermissionWellRights(int permissionID, string permissionName, string permissionDisplayName) : base(permissionID, permissionName, permissionDisplayName) {}
        public static readonly PermissionWellRights Instance = new PermissionWellRights(9, @"WellRights", @"WellRights");
    }

    public partial class PermissionWaterTransactionRights : Permission
    {
        private PermissionWaterTransactionRights(int permissionID, string permissionName, string permissionDisplayName) : base(permissionID, permissionName, permissionDisplayName) {}
        public static readonly PermissionWaterTransactionRights Instance = new PermissionWaterTransactionRights(10, @"WaterTransactionRights", @"WaterTransactionRights");
    }

    public partial class PermissionReportingPeriodRights : Permission
    {
        private PermissionReportingPeriodRights(int permissionID, string permissionName, string permissionDisplayName) : base(permissionID, permissionName, permissionDisplayName) {}
        public static readonly PermissionReportingPeriodRights Instance = new PermissionReportingPeriodRights(12, @"ReportingPeriodRights", @"ReportingPeriodRights");
    }

    public partial class PermissionWaterTypeRights : Permission
    {
        private PermissionWaterTypeRights(int permissionID, string permissionName, string permissionDisplayName) : base(permissionID, permissionName, permissionDisplayName) {}
        public static readonly PermissionWaterTypeRights Instance = new PermissionWaterTypeRights(13, @"WaterTypeRights", @"WaterTypeRights");
    }

    public partial class PermissionGeographyRights : Permission
    {
        private PermissionGeographyRights(int permissionID, string permissionName, string permissionDisplayName) : base(permissionID, permissionName, permissionDisplayName) {}
        public static readonly PermissionGeographyRights Instance = new PermissionGeographyRights(14, @"GeographyRights", @"GeographyRights");
    }

    public partial class PermissionExternalMapLayerRights : Permission
    {
        private PermissionExternalMapLayerRights(int permissionID, string permissionName, string permissionDisplayName) : base(permissionID, permissionName, permissionDisplayName) {}
        public static readonly PermissionExternalMapLayerRights Instance = new PermissionExternalMapLayerRights(15, @"ExternalMapLayerRights", @"ExternalMapLayerRights");
    }

    public partial class PermissionWaterAccountUserRights : Permission
    {
        private PermissionWaterAccountUserRights(int permissionID, string permissionName, string permissionDisplayName) : base(permissionID, permissionName, permissionDisplayName) {}
        public static readonly PermissionWaterAccountUserRights Instance = new PermissionWaterAccountUserRights(16, @"WaterAccountUserRights", @"WaterAccountUserRights");
    }

    public partial class PermissionZoneGroupRights : Permission
    {
        private PermissionZoneGroupRights(int permissionID, string permissionName, string permissionDisplayName) : base(permissionID, permissionName, permissionDisplayName) {}
        public static readonly PermissionZoneGroupRights Instance = new PermissionZoneGroupRights(17, @"ZoneGroupRights", @"ZoneGroupRights");
    }

    public partial class PermissionMonitoringWellRights : Permission
    {
        private PermissionMonitoringWellRights(int permissionID, string permissionName, string permissionDisplayName) : base(permissionID, permissionName, permissionDisplayName) {}
        public static readonly PermissionMonitoringWellRights Instance = new PermissionMonitoringWellRights(18, @"MonitoringWellRights", @"MonitoringWellRights");
    }

    public partial class PermissionAllocationPlanRights : Permission
    {
        private PermissionAllocationPlanRights(int permissionID, string permissionName, string permissionDisplayName) : base(permissionID, permissionName, permissionDisplayName) {}
        public static readonly PermissionAllocationPlanRights Instance = new PermissionAllocationPlanRights(19, @"AllocationPlanRights", @"AllocationPlanRights");
    }

    public partial class PermissionFrequentlyAskedQuestionRights : Permission
    {
        private PermissionFrequentlyAskedQuestionRights(int permissionID, string permissionName, string permissionDisplayName) : base(permissionID, permissionName, permissionDisplayName) {}
        public static readonly PermissionFrequentlyAskedQuestionRights Instance = new PermissionFrequentlyAskedQuestionRights(20, @"FrequentlyAskedQuestionRights", @"FrequentlyAskedQuestionRights");
    }

    public partial class PermissionCustomAttributeRights : Permission
    {
        private PermissionCustomAttributeRights(int permissionID, string permissionName, string permissionDisplayName) : base(permissionID, permissionName, permissionDisplayName) {}
        public static readonly PermissionCustomAttributeRights Instance = new PermissionCustomAttributeRights(21, @"CustomAttributeRights", @"CustomAttributeRights");
    }

    public partial class PermissionUsageEntityRights : Permission
    {
        private PermissionUsageEntityRights(int permissionID, string permissionName, string permissionDisplayName) : base(permissionID, permissionName, permissionDisplayName) {}
        public static readonly PermissionUsageEntityRights Instance = new PermissionUsageEntityRights(22, @"UsageEntityRights", @"UsageEntityRights");
    }

    public partial class PermissionModelRights : Permission
    {
        private PermissionModelRights(int permissionID, string permissionName, string permissionDisplayName) : base(permissionID, permissionName, permissionDisplayName) {}
        public static readonly PermissionModelRights Instance = new PermissionModelRights(23, @"ModelRights", @"ModelRights");
    }

    public partial class PermissionScenarioRights : Permission
    {
        private PermissionScenarioRights(int permissionID, string permissionName, string permissionDisplayName) : base(permissionID, permissionName, permissionDisplayName) {}
        public static readonly PermissionScenarioRights Instance = new PermissionScenarioRights(24, @"ScenarioRights", @"ScenarioRights");
    }

    public partial class PermissionScenarioRunRights : Permission
    {
        private PermissionScenarioRunRights(int permissionID, string permissionName, string permissionDisplayName) : base(permissionID, permissionName, permissionDisplayName) {}
        public static readonly PermissionScenarioRunRights Instance = new PermissionScenarioRunRights(25, @"ScenarioRunRights", @"ScenarioRunRights");
    }
}