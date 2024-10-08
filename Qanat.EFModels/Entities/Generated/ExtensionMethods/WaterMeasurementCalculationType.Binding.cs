//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[WaterMeasurementCalculationType]
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Qanat.Models.DataTransferObjects;


namespace Qanat.EFModels.Entities
{
    public abstract partial class WaterMeasurementCalculationType : IHavePrimaryKey
    {
        public static readonly WaterMeasurementCalculationTypeCalculateEffectivePrecip CalculateEffectivePrecip = WaterMeasurementCalculationTypeCalculateEffectivePrecip.Instance;
        public static readonly WaterMeasurementCalculationTypeCalculateSurfaceWaterConsumption CalculateSurfaceWaterConsumption = WaterMeasurementCalculationTypeCalculateSurfaceWaterConsumption.Instance;
        public static readonly WaterMeasurementCalculationTypeETMinusPrecipMinusTotalSurfaceWater ETMinusPrecipMinusTotalSurfaceWater = WaterMeasurementCalculationTypeETMinusPrecipMinusTotalSurfaceWater.Instance;
        public static readonly WaterMeasurementCalculationTypeCalculatePrecipitationCreditOffset CalculatePrecipitationCreditOffset = WaterMeasurementCalculationTypeCalculatePrecipitationCreditOffset.Instance;
        public static readonly WaterMeasurementCalculationTypeCalculatePositiveConsumedGroundwater CalculatePositiveConsumedGroundwater = WaterMeasurementCalculationTypeCalculatePositiveConsumedGroundwater.Instance;
        public static readonly WaterMeasurementCalculationTypeCalculateUnadjustedExtractedGroundwater CalculateUnadjustedExtractedGroundwater = WaterMeasurementCalculationTypeCalculateUnadjustedExtractedGroundwater.Instance;
        public static readonly WaterMeasurementCalculationTypeCalculateExtractedGroundwater CalculateExtractedGroundwater = WaterMeasurementCalculationTypeCalculateExtractedGroundwater.Instance;
        public static readonly WaterMeasurementCalculationTypeCalculateExtractedAgainstSupply CalculateExtractedAgainstSupply = WaterMeasurementCalculationTypeCalculateExtractedAgainstSupply.Instance;
        public static readonly WaterMeasurementCalculationTypeCalculateOpenETConsumptiveUse CalculateOpenETConsumptiveUse = WaterMeasurementCalculationTypeCalculateOpenETConsumptiveUse.Instance;

        public static readonly List<WaterMeasurementCalculationType> All;
        public static readonly List<WaterMeasurementCalculationTypeSimpleDto> AllAsSimpleDto;
        public static readonly ReadOnlyDictionary<int, WaterMeasurementCalculationType> AllLookupDictionary;
        public static readonly ReadOnlyDictionary<int, WaterMeasurementCalculationTypeSimpleDto> AllAsSimpleDtoLookupDictionary;

        /// <summary>
        /// Static type constructor to coordinate static initialization order
        /// </summary>
        static WaterMeasurementCalculationType()
        {
            All = new List<WaterMeasurementCalculationType> { CalculateEffectivePrecip, CalculateSurfaceWaterConsumption, ETMinusPrecipMinusTotalSurfaceWater, CalculatePrecipitationCreditOffset, CalculatePositiveConsumedGroundwater, CalculateUnadjustedExtractedGroundwater, CalculateExtractedGroundwater, CalculateExtractedAgainstSupply, CalculateOpenETConsumptiveUse };
            AllAsSimpleDto = new List<WaterMeasurementCalculationTypeSimpleDto> { CalculateEffectivePrecip.AsSimpleDto(), CalculateSurfaceWaterConsumption.AsSimpleDto(), ETMinusPrecipMinusTotalSurfaceWater.AsSimpleDto(), CalculatePrecipitationCreditOffset.AsSimpleDto(), CalculatePositiveConsumedGroundwater.AsSimpleDto(), CalculateUnadjustedExtractedGroundwater.AsSimpleDto(), CalculateExtractedGroundwater.AsSimpleDto(), CalculateExtractedAgainstSupply.AsSimpleDto(), CalculateOpenETConsumptiveUse.AsSimpleDto() };
            AllLookupDictionary = new ReadOnlyDictionary<int, WaterMeasurementCalculationType>(All.ToDictionary(x => x.WaterMeasurementCalculationTypeID));
            AllAsSimpleDtoLookupDictionary = new ReadOnlyDictionary<int, WaterMeasurementCalculationTypeSimpleDto>(AllAsSimpleDto.ToDictionary(x => x.WaterMeasurementCalculationTypeID));
        }

        /// <summary>
        /// Protected constructor only for use in instantiating the set of static lookup values that match database
        /// </summary>
        protected WaterMeasurementCalculationType(int waterMeasurementCalculationTypeID, string waterMeasurementCalculationTypeName, string waterMeasurementCalculationTypeDisplayName)
        {
            WaterMeasurementCalculationTypeID = waterMeasurementCalculationTypeID;
            WaterMeasurementCalculationTypeName = waterMeasurementCalculationTypeName;
            WaterMeasurementCalculationTypeDisplayName = waterMeasurementCalculationTypeDisplayName;
        }

        [Key]
        public int WaterMeasurementCalculationTypeID { get; private set; }
        public string WaterMeasurementCalculationTypeName { get; private set; }
        public string WaterMeasurementCalculationTypeDisplayName { get; private set; }
        [NotMapped]
        public int PrimaryKey { get { return WaterMeasurementCalculationTypeID; } }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public bool Equals(WaterMeasurementCalculationType other)
        {
            if (other == null)
            {
                return false;
            }
            return other.WaterMeasurementCalculationTypeID == WaterMeasurementCalculationTypeID;
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override bool Equals(object obj)
        {
            return Equals(obj as WaterMeasurementCalculationType);
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override int GetHashCode()
        {
            return WaterMeasurementCalculationTypeID;
        }

        public static bool operator ==(WaterMeasurementCalculationType left, WaterMeasurementCalculationType right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(WaterMeasurementCalculationType left, WaterMeasurementCalculationType right)
        {
            return !Equals(left, right);
        }

        public WaterMeasurementCalculationTypeEnum ToEnum => (WaterMeasurementCalculationTypeEnum)GetHashCode();

        public static WaterMeasurementCalculationType ToType(int enumValue)
        {
            return ToType((WaterMeasurementCalculationTypeEnum)enumValue);
        }

        public static WaterMeasurementCalculationType ToType(WaterMeasurementCalculationTypeEnum enumValue)
        {
            switch (enumValue)
            {
                case WaterMeasurementCalculationTypeEnum.CalculateEffectivePrecip:
                    return CalculateEffectivePrecip;
                case WaterMeasurementCalculationTypeEnum.CalculateExtractedAgainstSupply:
                    return CalculateExtractedAgainstSupply;
                case WaterMeasurementCalculationTypeEnum.CalculateExtractedGroundwater:
                    return CalculateExtractedGroundwater;
                case WaterMeasurementCalculationTypeEnum.CalculateOpenETConsumptiveUse:
                    return CalculateOpenETConsumptiveUse;
                case WaterMeasurementCalculationTypeEnum.CalculatePositiveConsumedGroundwater:
                    return CalculatePositiveConsumedGroundwater;
                case WaterMeasurementCalculationTypeEnum.CalculatePrecipitationCreditOffset:
                    return CalculatePrecipitationCreditOffset;
                case WaterMeasurementCalculationTypeEnum.CalculateSurfaceWaterConsumption:
                    return CalculateSurfaceWaterConsumption;
                case WaterMeasurementCalculationTypeEnum.CalculateUnadjustedExtractedGroundwater:
                    return CalculateUnadjustedExtractedGroundwater;
                case WaterMeasurementCalculationTypeEnum.ETMinusPrecipMinusTotalSurfaceWater:
                    return ETMinusPrecipMinusTotalSurfaceWater;
                default:
                    throw new ArgumentException("Unable to map Enum: {enumValue}");
            }
        }
    }

    public enum WaterMeasurementCalculationTypeEnum
    {
        CalculateEffectivePrecip = 1,
        CalculateSurfaceWaterConsumption = 2,
        ETMinusPrecipMinusTotalSurfaceWater = 3,
        CalculatePrecipitationCreditOffset = 4,
        CalculatePositiveConsumedGroundwater = 5,
        CalculateUnadjustedExtractedGroundwater = 6,
        CalculateExtractedGroundwater = 7,
        CalculateExtractedAgainstSupply = 8,
        CalculateOpenETConsumptiveUse = 9
    }

    public partial class WaterMeasurementCalculationTypeCalculateEffectivePrecip : WaterMeasurementCalculationType
    {
        private WaterMeasurementCalculationTypeCalculateEffectivePrecip(int waterMeasurementCalculationTypeID, string waterMeasurementCalculationTypeName, string waterMeasurementCalculationTypeDisplayName) : base(waterMeasurementCalculationTypeID, waterMeasurementCalculationTypeName, waterMeasurementCalculationTypeDisplayName) {}
        public static readonly WaterMeasurementCalculationTypeCalculateEffectivePrecip Instance = new WaterMeasurementCalculationTypeCalculateEffectivePrecip(1, @"CalculateEffectivePrecip", @"Calculate Effective Precip");
    }

    public partial class WaterMeasurementCalculationTypeCalculateSurfaceWaterConsumption : WaterMeasurementCalculationType
    {
        private WaterMeasurementCalculationTypeCalculateSurfaceWaterConsumption(int waterMeasurementCalculationTypeID, string waterMeasurementCalculationTypeName, string waterMeasurementCalculationTypeDisplayName) : base(waterMeasurementCalculationTypeID, waterMeasurementCalculationTypeName, waterMeasurementCalculationTypeDisplayName) {}
        public static readonly WaterMeasurementCalculationTypeCalculateSurfaceWaterConsumption Instance = new WaterMeasurementCalculationTypeCalculateSurfaceWaterConsumption(2, @"CalculateSurfaceWaterConsumption", @"Calculate SurfaceWater Consumption");
    }

    public partial class WaterMeasurementCalculationTypeETMinusPrecipMinusTotalSurfaceWater : WaterMeasurementCalculationType
    {
        private WaterMeasurementCalculationTypeETMinusPrecipMinusTotalSurfaceWater(int waterMeasurementCalculationTypeID, string waterMeasurementCalculationTypeName, string waterMeasurementCalculationTypeDisplayName) : base(waterMeasurementCalculationTypeID, waterMeasurementCalculationTypeName, waterMeasurementCalculationTypeDisplayName) {}
        public static readonly WaterMeasurementCalculationTypeETMinusPrecipMinusTotalSurfaceWater Instance = new WaterMeasurementCalculationTypeETMinusPrecipMinusTotalSurfaceWater(3, @"ETMinusPrecipMinusTotalSurfaceWater", @"ET - Precip - TotalSurfaceWater");
    }

    public partial class WaterMeasurementCalculationTypeCalculatePrecipitationCreditOffset : WaterMeasurementCalculationType
    {
        private WaterMeasurementCalculationTypeCalculatePrecipitationCreditOffset(int waterMeasurementCalculationTypeID, string waterMeasurementCalculationTypeName, string waterMeasurementCalculationTypeDisplayName) : base(waterMeasurementCalculationTypeID, waterMeasurementCalculationTypeName, waterMeasurementCalculationTypeDisplayName) {}
        public static readonly WaterMeasurementCalculationTypeCalculatePrecipitationCreditOffset Instance = new WaterMeasurementCalculationTypeCalculatePrecipitationCreditOffset(4, @"CalculatePrecipitationCreditOffset", @"Calculate Precipitation Credit Offset");
    }

    public partial class WaterMeasurementCalculationTypeCalculatePositiveConsumedGroundwater : WaterMeasurementCalculationType
    {
        private WaterMeasurementCalculationTypeCalculatePositiveConsumedGroundwater(int waterMeasurementCalculationTypeID, string waterMeasurementCalculationTypeName, string waterMeasurementCalculationTypeDisplayName) : base(waterMeasurementCalculationTypeID, waterMeasurementCalculationTypeName, waterMeasurementCalculationTypeDisplayName) {}
        public static readonly WaterMeasurementCalculationTypeCalculatePositiveConsumedGroundwater Instance = new WaterMeasurementCalculationTypeCalculatePositiveConsumedGroundwater(5, @"CalculatePositiveConsumedGroundwater", @"Calculate Positive Consumed Groundwater");
    }

    public partial class WaterMeasurementCalculationTypeCalculateUnadjustedExtractedGroundwater : WaterMeasurementCalculationType
    {
        private WaterMeasurementCalculationTypeCalculateUnadjustedExtractedGroundwater(int waterMeasurementCalculationTypeID, string waterMeasurementCalculationTypeName, string waterMeasurementCalculationTypeDisplayName) : base(waterMeasurementCalculationTypeID, waterMeasurementCalculationTypeName, waterMeasurementCalculationTypeDisplayName) {}
        public static readonly WaterMeasurementCalculationTypeCalculateUnadjustedExtractedGroundwater Instance = new WaterMeasurementCalculationTypeCalculateUnadjustedExtractedGroundwater(6, @"CalculateUnadjustedExtractedGroundwater", @"Calculate Unadjusted Extracted Groundwater");
    }

    public partial class WaterMeasurementCalculationTypeCalculateExtractedGroundwater : WaterMeasurementCalculationType
    {
        private WaterMeasurementCalculationTypeCalculateExtractedGroundwater(int waterMeasurementCalculationTypeID, string waterMeasurementCalculationTypeName, string waterMeasurementCalculationTypeDisplayName) : base(waterMeasurementCalculationTypeID, waterMeasurementCalculationTypeName, waterMeasurementCalculationTypeDisplayName) {}
        public static readonly WaterMeasurementCalculationTypeCalculateExtractedGroundwater Instance = new WaterMeasurementCalculationTypeCalculateExtractedGroundwater(7, @"CalculateExtractedGroundwater", @"Calculate Extracted Groundwater");
    }

    public partial class WaterMeasurementCalculationTypeCalculateExtractedAgainstSupply : WaterMeasurementCalculationType
    {
        private WaterMeasurementCalculationTypeCalculateExtractedAgainstSupply(int waterMeasurementCalculationTypeID, string waterMeasurementCalculationTypeName, string waterMeasurementCalculationTypeDisplayName) : base(waterMeasurementCalculationTypeID, waterMeasurementCalculationTypeName, waterMeasurementCalculationTypeDisplayName) {}
        public static readonly WaterMeasurementCalculationTypeCalculateExtractedAgainstSupply Instance = new WaterMeasurementCalculationTypeCalculateExtractedAgainstSupply(8, @"CalculateExtractedAgainstSupply", @"Calculate Extracted Against Supply");
    }

    public partial class WaterMeasurementCalculationTypeCalculateOpenETConsumptiveUse : WaterMeasurementCalculationType
    {
        private WaterMeasurementCalculationTypeCalculateOpenETConsumptiveUse(int waterMeasurementCalculationTypeID, string waterMeasurementCalculationTypeName, string waterMeasurementCalculationTypeDisplayName) : base(waterMeasurementCalculationTypeID, waterMeasurementCalculationTypeName, waterMeasurementCalculationTypeDisplayName) {}
        public static readonly WaterMeasurementCalculationTypeCalculateOpenETConsumptiveUse Instance = new WaterMeasurementCalculationTypeCalculateOpenETConsumptiveUse(9, @"CalculateOpenETConsumptiveUse", @"Calculate Open ET Consumptive Use");
    }
}