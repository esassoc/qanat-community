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
        public static readonly WaterMeasurementCalculationTypeCalculateEffectivePrecipByZone CalculateEffectivePrecipByZone = WaterMeasurementCalculationTypeCalculateEffectivePrecipByZone.Instance;
        public static readonly WaterMeasurementCalculationTypeCalculateSurfaceWaterConsumption CalculateSurfaceWaterConsumption = WaterMeasurementCalculationTypeCalculateSurfaceWaterConsumption.Instance;
        public static readonly WaterMeasurementCalculationTypeETMinusPrecipMinusTotalSurfaceWater ETMinusPrecipMinusTotalSurfaceWater = WaterMeasurementCalculationTypeETMinusPrecipMinusTotalSurfaceWater.Instance;
        public static readonly WaterMeasurementCalculationTypeCalculatePrecipitationCreditOffset CalculatePrecipitationCreditOffset = WaterMeasurementCalculationTypeCalculatePrecipitationCreditOffset.Instance;
        public static readonly WaterMeasurementCalculationTypeCalculatePositiveConsumedGroundwater CalculatePositiveConsumedGroundwater = WaterMeasurementCalculationTypeCalculatePositiveConsumedGroundwater.Instance;
        public static readonly WaterMeasurementCalculationTypeCalculateUnadjustedExtractedGroundwater CalculateUnadjustedExtractedGroundwater = WaterMeasurementCalculationTypeCalculateUnadjustedExtractedGroundwater.Instance;
        public static readonly WaterMeasurementCalculationTypeCalculateExtractedGroundwater CalculateExtractedGroundwater = WaterMeasurementCalculationTypeCalculateExtractedGroundwater.Instance;
        public static readonly WaterMeasurementCalculationTypeCalculateExtractedAgainstSupply CalculateExtractedAgainstSupply = WaterMeasurementCalculationTypeCalculateExtractedAgainstSupply.Instance;
        public static readonly WaterMeasurementCalculationTypeCalculateOpenETConsumptiveUse CalculateOpenETConsumptiveUse = WaterMeasurementCalculationTypeCalculateOpenETConsumptiveUse.Instance;
        public static readonly WaterMeasurementCalculationTypeCalculateConsumedGroundwater CalculateConsumedGroundwater = WaterMeasurementCalculationTypeCalculateConsumedGroundwater.Instance;
        public static readonly WaterMeasurementCalculationTypeCalculateEffectivePrecipByScalarValue CalculateEffectivePrecipByScalarValue = WaterMeasurementCalculationTypeCalculateEffectivePrecipByScalarValue.Instance;
        public static readonly WaterMeasurementCalculationTypeCoverCropAdjustment CoverCropAdjustment = WaterMeasurementCalculationTypeCoverCropAdjustment.Instance;

        public static readonly List<WaterMeasurementCalculationType> All;
        public static readonly ReadOnlyDictionary<int, WaterMeasurementCalculationType> AllLookupDictionary;

        /// <summary>
        /// Static type constructor to coordinate static initialization order
        /// </summary>
        static WaterMeasurementCalculationType()
        {
            All = new List<WaterMeasurementCalculationType> { CalculateEffectivePrecipByZone, CalculateSurfaceWaterConsumption, ETMinusPrecipMinusTotalSurfaceWater, CalculatePrecipitationCreditOffset, CalculatePositiveConsumedGroundwater, CalculateUnadjustedExtractedGroundwater, CalculateExtractedGroundwater, CalculateExtractedAgainstSupply, CalculateOpenETConsumptiveUse, CalculateConsumedGroundwater, CalculateEffectivePrecipByScalarValue, CoverCropAdjustment };
            AllLookupDictionary = new ReadOnlyDictionary<int, WaterMeasurementCalculationType>(All.ToDictionary(x => x.WaterMeasurementCalculationTypeID));
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
                case WaterMeasurementCalculationTypeEnum.CalculateConsumedGroundwater:
                    return CalculateConsumedGroundwater;
                case WaterMeasurementCalculationTypeEnum.CalculateEffectivePrecipByScalarValue:
                    return CalculateEffectivePrecipByScalarValue;
                case WaterMeasurementCalculationTypeEnum.CalculateEffectivePrecipByZone:
                    return CalculateEffectivePrecipByZone;
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
                case WaterMeasurementCalculationTypeEnum.CoverCropAdjustment:
                    return CoverCropAdjustment;
                case WaterMeasurementCalculationTypeEnum.ETMinusPrecipMinusTotalSurfaceWater:
                    return ETMinusPrecipMinusTotalSurfaceWater;
                default:
                    throw new ArgumentException("Unable to map Enum: {enumValue}");
            }
        }
    }

    public enum WaterMeasurementCalculationTypeEnum
    {
        CalculateEffectivePrecipByZone = 1,
        CalculateSurfaceWaterConsumption = 2,
        ETMinusPrecipMinusTotalSurfaceWater = 3,
        CalculatePrecipitationCreditOffset = 4,
        CalculatePositiveConsumedGroundwater = 5,
        CalculateUnadjustedExtractedGroundwater = 6,
        CalculateExtractedGroundwater = 7,
        CalculateExtractedAgainstSupply = 8,
        CalculateOpenETConsumptiveUse = 9,
        CalculateConsumedGroundwater = 10,
        CalculateEffectivePrecipByScalarValue = 11,
        CoverCropAdjustment = 12
    }

    public partial class WaterMeasurementCalculationTypeCalculateEffectivePrecipByZone : WaterMeasurementCalculationType
    {
        private WaterMeasurementCalculationTypeCalculateEffectivePrecipByZone(int waterMeasurementCalculationTypeID, string waterMeasurementCalculationTypeName, string waterMeasurementCalculationTypeDisplayName) : base(waterMeasurementCalculationTypeID, waterMeasurementCalculationTypeName, waterMeasurementCalculationTypeDisplayName) {}
        public static readonly WaterMeasurementCalculationTypeCalculateEffectivePrecipByZone Instance = new WaterMeasurementCalculationTypeCalculateEffectivePrecipByZone(1, @"CalculateEffectivePrecipByZone", @"Calculate Effective Precip By Zone");
    }

    public partial class WaterMeasurementCalculationTypeCalculateSurfaceWaterConsumption : WaterMeasurementCalculationType
    {
        private WaterMeasurementCalculationTypeCalculateSurfaceWaterConsumption(int waterMeasurementCalculationTypeID, string waterMeasurementCalculationTypeName, string waterMeasurementCalculationTypeDisplayName) : base(waterMeasurementCalculationTypeID, waterMeasurementCalculationTypeName, waterMeasurementCalculationTypeDisplayName) {}
        public static readonly WaterMeasurementCalculationTypeCalculateSurfaceWaterConsumption Instance = new WaterMeasurementCalculationTypeCalculateSurfaceWaterConsumption(2, @"CalculateSurfaceWaterConsumption", @"Calculate Surface Water Consumption");
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

    public partial class WaterMeasurementCalculationTypeCalculateConsumedGroundwater : WaterMeasurementCalculationType
    {
        private WaterMeasurementCalculationTypeCalculateConsumedGroundwater(int waterMeasurementCalculationTypeID, string waterMeasurementCalculationTypeName, string waterMeasurementCalculationTypeDisplayName) : base(waterMeasurementCalculationTypeID, waterMeasurementCalculationTypeName, waterMeasurementCalculationTypeDisplayName) {}
        public static readonly WaterMeasurementCalculationTypeCalculateConsumedGroundwater Instance = new WaterMeasurementCalculationTypeCalculateConsumedGroundwater(10, @"CalculateConsumedGroundwater", @"Calculate Consumed Groundwater");
    }

    public partial class WaterMeasurementCalculationTypeCalculateEffectivePrecipByScalarValue : WaterMeasurementCalculationType
    {
        private WaterMeasurementCalculationTypeCalculateEffectivePrecipByScalarValue(int waterMeasurementCalculationTypeID, string waterMeasurementCalculationTypeName, string waterMeasurementCalculationTypeDisplayName) : base(waterMeasurementCalculationTypeID, waterMeasurementCalculationTypeName, waterMeasurementCalculationTypeDisplayName) {}
        public static readonly WaterMeasurementCalculationTypeCalculateEffectivePrecipByScalarValue Instance = new WaterMeasurementCalculationTypeCalculateEffectivePrecipByScalarValue(11, @"CalculateEffectivePrecipByScalarValue", @"Calculate Effective Precip By Scalar Value");
    }

    public partial class WaterMeasurementCalculationTypeCoverCropAdjustment : WaterMeasurementCalculationType
    {
        private WaterMeasurementCalculationTypeCoverCropAdjustment(int waterMeasurementCalculationTypeID, string waterMeasurementCalculationTypeName, string waterMeasurementCalculationTypeDisplayName) : base(waterMeasurementCalculationTypeID, waterMeasurementCalculationTypeName, waterMeasurementCalculationTypeDisplayName) {}
        public static readonly WaterMeasurementCalculationTypeCoverCropAdjustment Instance = new WaterMeasurementCalculationTypeCoverCropAdjustment(12, @"CoverCropAdjustment", @"Cover Crop Adjustment");
    }
}