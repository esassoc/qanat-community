//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[WaterMeasurementCategoryType]
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Qanat.Models.DataTransferObjects;


namespace Qanat.EFModels.Entities
{
    public abstract partial class WaterMeasurementCategoryType : IHavePrimaryKey
    {
        public static readonly WaterMeasurementCategoryTypeET ET = WaterMeasurementCategoryTypeET.Instance;
        public static readonly WaterMeasurementCategoryTypePrecip Precip = WaterMeasurementCategoryTypePrecip.Instance;
        public static readonly WaterMeasurementCategoryTypeMeter Meter = WaterMeasurementCategoryTypeMeter.Instance;
        public static readonly WaterMeasurementCategoryTypeSurfaceWater SurfaceWater = WaterMeasurementCategoryTypeSurfaceWater.Instance;
        public static readonly WaterMeasurementCategoryTypeCalculated Calculated = WaterMeasurementCategoryTypeCalculated.Instance;
        public static readonly WaterMeasurementCategoryTypePrecipitationCredit PrecipitationCredit = WaterMeasurementCategoryTypePrecipitationCredit.Instance;
        public static readonly WaterMeasurementCategoryTypeManualAdjustment ManualAdjustment = WaterMeasurementCategoryTypeManualAdjustment.Instance;

        public static readonly List<WaterMeasurementCategoryType> All;
        public static readonly List<WaterMeasurementCategoryTypeSimpleDto> AllAsSimpleDto;
        public static readonly ReadOnlyDictionary<int, WaterMeasurementCategoryType> AllLookupDictionary;
        public static readonly ReadOnlyDictionary<int, WaterMeasurementCategoryTypeSimpleDto> AllAsSimpleDtoLookupDictionary;

        /// <summary>
        /// Static type constructor to coordinate static initialization order
        /// </summary>
        static WaterMeasurementCategoryType()
        {
            All = new List<WaterMeasurementCategoryType> { ET, Precip, Meter, SurfaceWater, Calculated, PrecipitationCredit, ManualAdjustment };
            AllAsSimpleDto = new List<WaterMeasurementCategoryTypeSimpleDto> { ET.AsSimpleDto(), Precip.AsSimpleDto(), Meter.AsSimpleDto(), SurfaceWater.AsSimpleDto(), Calculated.AsSimpleDto(), PrecipitationCredit.AsSimpleDto(), ManualAdjustment.AsSimpleDto() };
            AllLookupDictionary = new ReadOnlyDictionary<int, WaterMeasurementCategoryType>(All.ToDictionary(x => x.WaterMeasurementCategoryTypeID));
            AllAsSimpleDtoLookupDictionary = new ReadOnlyDictionary<int, WaterMeasurementCategoryTypeSimpleDto>(AllAsSimpleDto.ToDictionary(x => x.WaterMeasurementCategoryTypeID));
        }

        /// <summary>
        /// Protected constructor only for use in instantiating the set of static lookup values that match database
        /// </summary>
        protected WaterMeasurementCategoryType(int waterMeasurementCategoryTypeID, string waterMeasurementCategoryTypeName, string waterMeasurementCategoryTypeDisplayName)
        {
            WaterMeasurementCategoryTypeID = waterMeasurementCategoryTypeID;
            WaterMeasurementCategoryTypeName = waterMeasurementCategoryTypeName;
            WaterMeasurementCategoryTypeDisplayName = waterMeasurementCategoryTypeDisplayName;
        }

        [Key]
        public int WaterMeasurementCategoryTypeID { get; private set; }
        public string WaterMeasurementCategoryTypeName { get; private set; }
        public string WaterMeasurementCategoryTypeDisplayName { get; private set; }
        [NotMapped]
        public int PrimaryKey { get { return WaterMeasurementCategoryTypeID; } }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public bool Equals(WaterMeasurementCategoryType other)
        {
            if (other == null)
            {
                return false;
            }
            return other.WaterMeasurementCategoryTypeID == WaterMeasurementCategoryTypeID;
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override bool Equals(object obj)
        {
            return Equals(obj as WaterMeasurementCategoryType);
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override int GetHashCode()
        {
            return WaterMeasurementCategoryTypeID;
        }

        public static bool operator ==(WaterMeasurementCategoryType left, WaterMeasurementCategoryType right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(WaterMeasurementCategoryType left, WaterMeasurementCategoryType right)
        {
            return !Equals(left, right);
        }

        public WaterMeasurementCategoryTypeEnum ToEnum => (WaterMeasurementCategoryTypeEnum)GetHashCode();

        public static WaterMeasurementCategoryType ToType(int enumValue)
        {
            return ToType((WaterMeasurementCategoryTypeEnum)enumValue);
        }

        public static WaterMeasurementCategoryType ToType(WaterMeasurementCategoryTypeEnum enumValue)
        {
            switch (enumValue)
            {
                case WaterMeasurementCategoryTypeEnum.Calculated:
                    return Calculated;
                case WaterMeasurementCategoryTypeEnum.ET:
                    return ET;
                case WaterMeasurementCategoryTypeEnum.ManualAdjustment:
                    return ManualAdjustment;
                case WaterMeasurementCategoryTypeEnum.Meter:
                    return Meter;
                case WaterMeasurementCategoryTypeEnum.Precip:
                    return Precip;
                case WaterMeasurementCategoryTypeEnum.PrecipitationCredit:
                    return PrecipitationCredit;
                case WaterMeasurementCategoryTypeEnum.SurfaceWater:
                    return SurfaceWater;
                default:
                    throw new ArgumentException("Unable to map Enum: {enumValue}");
            }
        }
    }

    public enum WaterMeasurementCategoryTypeEnum
    {
        ET = 1,
        Precip = 2,
        Meter = 3,
        SurfaceWater = 4,
        Calculated = 5,
        PrecipitationCredit = 6,
        ManualAdjustment = 7
    }

    public partial class WaterMeasurementCategoryTypeET : WaterMeasurementCategoryType
    {
        private WaterMeasurementCategoryTypeET(int waterMeasurementCategoryTypeID, string waterMeasurementCategoryTypeName, string waterMeasurementCategoryTypeDisplayName) : base(waterMeasurementCategoryTypeID, waterMeasurementCategoryTypeName, waterMeasurementCategoryTypeDisplayName) {}
        public static readonly WaterMeasurementCategoryTypeET Instance = new WaterMeasurementCategoryTypeET(1, @"ET", @"ET");
    }

    public partial class WaterMeasurementCategoryTypePrecip : WaterMeasurementCategoryType
    {
        private WaterMeasurementCategoryTypePrecip(int waterMeasurementCategoryTypeID, string waterMeasurementCategoryTypeName, string waterMeasurementCategoryTypeDisplayName) : base(waterMeasurementCategoryTypeID, waterMeasurementCategoryTypeName, waterMeasurementCategoryTypeDisplayName) {}
        public static readonly WaterMeasurementCategoryTypePrecip Instance = new WaterMeasurementCategoryTypePrecip(2, @"Precip", @"Precip");
    }

    public partial class WaterMeasurementCategoryTypeMeter : WaterMeasurementCategoryType
    {
        private WaterMeasurementCategoryTypeMeter(int waterMeasurementCategoryTypeID, string waterMeasurementCategoryTypeName, string waterMeasurementCategoryTypeDisplayName) : base(waterMeasurementCategoryTypeID, waterMeasurementCategoryTypeName, waterMeasurementCategoryTypeDisplayName) {}
        public static readonly WaterMeasurementCategoryTypeMeter Instance = new WaterMeasurementCategoryTypeMeter(3, @"Meter", @"Meter");
    }

    public partial class WaterMeasurementCategoryTypeSurfaceWater : WaterMeasurementCategoryType
    {
        private WaterMeasurementCategoryTypeSurfaceWater(int waterMeasurementCategoryTypeID, string waterMeasurementCategoryTypeName, string waterMeasurementCategoryTypeDisplayName) : base(waterMeasurementCategoryTypeID, waterMeasurementCategoryTypeName, waterMeasurementCategoryTypeDisplayName) {}
        public static readonly WaterMeasurementCategoryTypeSurfaceWater Instance = new WaterMeasurementCategoryTypeSurfaceWater(4, @"SurfaceWater", @"Surface Water");
    }

    public partial class WaterMeasurementCategoryTypeCalculated : WaterMeasurementCategoryType
    {
        private WaterMeasurementCategoryTypeCalculated(int waterMeasurementCategoryTypeID, string waterMeasurementCategoryTypeName, string waterMeasurementCategoryTypeDisplayName) : base(waterMeasurementCategoryTypeID, waterMeasurementCategoryTypeName, waterMeasurementCategoryTypeDisplayName) {}
        public static readonly WaterMeasurementCategoryTypeCalculated Instance = new WaterMeasurementCategoryTypeCalculated(5, @"Calculated", @"Calculated");
    }

    public partial class WaterMeasurementCategoryTypePrecipitationCredit : WaterMeasurementCategoryType
    {
        private WaterMeasurementCategoryTypePrecipitationCredit(int waterMeasurementCategoryTypeID, string waterMeasurementCategoryTypeName, string waterMeasurementCategoryTypeDisplayName) : base(waterMeasurementCategoryTypeID, waterMeasurementCategoryTypeName, waterMeasurementCategoryTypeDisplayName) {}
        public static readonly WaterMeasurementCategoryTypePrecipitationCredit Instance = new WaterMeasurementCategoryTypePrecipitationCredit(6, @"Precipitation Credit", @"PrecipitationCredit");
    }

    public partial class WaterMeasurementCategoryTypeManualAdjustment : WaterMeasurementCategoryType
    {
        private WaterMeasurementCategoryTypeManualAdjustment(int waterMeasurementCategoryTypeID, string waterMeasurementCategoryTypeName, string waterMeasurementCategoryTypeDisplayName) : base(waterMeasurementCategoryTypeID, waterMeasurementCategoryTypeName, waterMeasurementCategoryTypeDisplayName) {}
        public static readonly WaterMeasurementCategoryTypeManualAdjustment Instance = new WaterMeasurementCategoryTypeManualAdjustment(7, @"Manual Adjustment", @"ManualAdjustment");
    }
}