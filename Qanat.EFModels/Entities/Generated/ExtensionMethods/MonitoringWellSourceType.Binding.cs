//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[MonitoringWellSourceType]
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Qanat.Models.DataTransferObjects;


namespace Qanat.EFModels.Entities
{
    public abstract partial class MonitoringWellSourceType : IHavePrimaryKey
    {
        public static readonly MonitoringWellSourceTypeCNRA CNRA = MonitoringWellSourceTypeCNRA.Instance;
        public static readonly MonitoringWellSourceTypeYoloWRID YoloWRID = MonitoringWellSourceTypeYoloWRID.Instance;

        public static readonly List<MonitoringWellSourceType> All;
        public static readonly ReadOnlyDictionary<int, MonitoringWellSourceType> AllLookupDictionary;

        /// <summary>
        /// Static type constructor to coordinate static initialization order
        /// </summary>
        static MonitoringWellSourceType()
        {
            All = new List<MonitoringWellSourceType> { CNRA, YoloWRID };
            AllLookupDictionary = new ReadOnlyDictionary<int, MonitoringWellSourceType>(All.ToDictionary(x => x.MonitoringWellSourceTypeID));
        }

        /// <summary>
        /// Protected constructor only for use in instantiating the set of static lookup values that match database
        /// </summary>
        protected MonitoringWellSourceType(int monitoringWellSourceTypeID, string monitoringWellSourceTypeName, string monitoringWellSourceTypeDisplayName)
        {
            MonitoringWellSourceTypeID = monitoringWellSourceTypeID;
            MonitoringWellSourceTypeName = monitoringWellSourceTypeName;
            MonitoringWellSourceTypeDisplayName = monitoringWellSourceTypeDisplayName;
        }

        [Key]
        public int MonitoringWellSourceTypeID { get; private set; }
        public string MonitoringWellSourceTypeName { get; private set; }
        public string MonitoringWellSourceTypeDisplayName { get; private set; }
        [NotMapped]
        public int PrimaryKey { get { return MonitoringWellSourceTypeID; } }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public bool Equals(MonitoringWellSourceType other)
        {
            if (other == null)
            {
                return false;
            }
            return other.MonitoringWellSourceTypeID == MonitoringWellSourceTypeID;
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override bool Equals(object obj)
        {
            return Equals(obj as MonitoringWellSourceType);
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override int GetHashCode()
        {
            return MonitoringWellSourceTypeID;
        }

        public static bool operator ==(MonitoringWellSourceType left, MonitoringWellSourceType right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(MonitoringWellSourceType left, MonitoringWellSourceType right)
        {
            return !Equals(left, right);
        }

        public MonitoringWellSourceTypeEnum ToEnum => (MonitoringWellSourceTypeEnum)GetHashCode();

        public static MonitoringWellSourceType ToType(int enumValue)
        {
            return ToType((MonitoringWellSourceTypeEnum)enumValue);
        }

        public static MonitoringWellSourceType ToType(MonitoringWellSourceTypeEnum enumValue)
        {
            switch (enumValue)
            {
                case MonitoringWellSourceTypeEnum.CNRA:
                    return CNRA;
                case MonitoringWellSourceTypeEnum.YoloWRID:
                    return YoloWRID;
                default:
                    throw new ArgumentException("Unable to map Enum: {enumValue}");
            }
        }
    }

    public enum MonitoringWellSourceTypeEnum
    {
        CNRA = 1,
        YoloWRID = 2
    }

    public partial class MonitoringWellSourceTypeCNRA : MonitoringWellSourceType
    {
        private MonitoringWellSourceTypeCNRA(int monitoringWellSourceTypeID, string monitoringWellSourceTypeName, string monitoringWellSourceTypeDisplayName) : base(monitoringWellSourceTypeID, monitoringWellSourceTypeName, monitoringWellSourceTypeDisplayName) {}
        public static readonly MonitoringWellSourceTypeCNRA Instance = new MonitoringWellSourceTypeCNRA(1, @"CNRA", @"California Natural Resources Agency");
    }

    public partial class MonitoringWellSourceTypeYoloWRID : MonitoringWellSourceType
    {
        private MonitoringWellSourceTypeYoloWRID(int monitoringWellSourceTypeID, string monitoringWellSourceTypeName, string monitoringWellSourceTypeDisplayName) : base(monitoringWellSourceTypeID, monitoringWellSourceTypeName, monitoringWellSourceTypeDisplayName) {}
        public static readonly MonitoringWellSourceTypeYoloWRID Instance = new MonitoringWellSourceTypeYoloWRID(2, @"YoloWRID", @"Yolo WRID");
    }
}