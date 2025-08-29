//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[WellRegistrationWaterUseType]
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Qanat.Models.DataTransferObjects;


namespace Qanat.EFModels.Entities
{
    public abstract partial class WellRegistrationWaterUseType : IHavePrimaryKey
    {
        public static readonly WellRegistrationWaterUseTypeAgricultural Agricultural = WellRegistrationWaterUseTypeAgricultural.Instance;
        public static readonly WellRegistrationWaterUseTypeStockWatering StockWatering = WellRegistrationWaterUseTypeStockWatering.Instance;
        public static readonly WellRegistrationWaterUseTypeDomestic Domestic = WellRegistrationWaterUseTypeDomestic.Instance;
        public static readonly WellRegistrationWaterUseTypePublicMunicipal PublicMunicipal = WellRegistrationWaterUseTypePublicMunicipal.Instance;
        public static readonly WellRegistrationWaterUseTypePrivateMunicipal PrivateMunicipal = WellRegistrationWaterUseTypePrivateMunicipal.Instance;
        public static readonly WellRegistrationWaterUseTypeOther Other = WellRegistrationWaterUseTypeOther.Instance;

        public static readonly List<WellRegistrationWaterUseType> All;
        public static readonly ReadOnlyDictionary<int, WellRegistrationWaterUseType> AllLookupDictionary;

        /// <summary>
        /// Static type constructor to coordinate static initialization order
        /// </summary>
        static WellRegistrationWaterUseType()
        {
            All = new List<WellRegistrationWaterUseType> { Agricultural, StockWatering, Domestic, PublicMunicipal, PrivateMunicipal, Other };
            AllLookupDictionary = new ReadOnlyDictionary<int, WellRegistrationWaterUseType>(All.ToDictionary(x => x.WellRegistrationWaterUseTypeID));
        }

        /// <summary>
        /// Protected constructor only for use in instantiating the set of static lookup values that match database
        /// </summary>
        protected WellRegistrationWaterUseType(int wellRegistrationWaterUseTypeID, string wellRegistrationWaterUseTypeName, string wellRegistrationWaterUseTypeDisplayName)
        {
            WellRegistrationWaterUseTypeID = wellRegistrationWaterUseTypeID;
            WellRegistrationWaterUseTypeName = wellRegistrationWaterUseTypeName;
            WellRegistrationWaterUseTypeDisplayName = wellRegistrationWaterUseTypeDisplayName;
        }

        [Key]
        public int WellRegistrationWaterUseTypeID { get; private set; }
        public string WellRegistrationWaterUseTypeName { get; private set; }
        public string WellRegistrationWaterUseTypeDisplayName { get; private set; }
        [NotMapped]
        public int PrimaryKey { get { return WellRegistrationWaterUseTypeID; } }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public bool Equals(WellRegistrationWaterUseType other)
        {
            if (other == null)
            {
                return false;
            }
            return other.WellRegistrationWaterUseTypeID == WellRegistrationWaterUseTypeID;
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override bool Equals(object obj)
        {
            return Equals(obj as WellRegistrationWaterUseType);
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override int GetHashCode()
        {
            return WellRegistrationWaterUseTypeID;
        }

        public static bool operator ==(WellRegistrationWaterUseType left, WellRegistrationWaterUseType right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(WellRegistrationWaterUseType left, WellRegistrationWaterUseType right)
        {
            return !Equals(left, right);
        }

        public WellRegistrationWaterUseTypeEnum ToEnum => (WellRegistrationWaterUseTypeEnum)GetHashCode();

        public static WellRegistrationWaterUseType ToType(int enumValue)
        {
            return ToType((WellRegistrationWaterUseTypeEnum)enumValue);
        }

        public static WellRegistrationWaterUseType ToType(WellRegistrationWaterUseTypeEnum enumValue)
        {
            switch (enumValue)
            {
                case WellRegistrationWaterUseTypeEnum.Agricultural:
                    return Agricultural;
                case WellRegistrationWaterUseTypeEnum.Domestic:
                    return Domestic;
                case WellRegistrationWaterUseTypeEnum.Other:
                    return Other;
                case WellRegistrationWaterUseTypeEnum.PrivateMunicipal:
                    return PrivateMunicipal;
                case WellRegistrationWaterUseTypeEnum.PublicMunicipal:
                    return PublicMunicipal;
                case WellRegistrationWaterUseTypeEnum.StockWatering:
                    return StockWatering;
                default:
                    throw new ArgumentException("Unable to map Enum: {enumValue}");
            }
        }
    }

    public enum WellRegistrationWaterUseTypeEnum
    {
        Agricultural = 1,
        StockWatering = 2,
        Domestic = 3,
        PublicMunicipal = 4,
        PrivateMunicipal = 5,
        Other = 6
    }

    public partial class WellRegistrationWaterUseTypeAgricultural : WellRegistrationWaterUseType
    {
        private WellRegistrationWaterUseTypeAgricultural(int wellRegistrationWaterUseTypeID, string wellRegistrationWaterUseTypeName, string wellRegistrationWaterUseTypeDisplayName) : base(wellRegistrationWaterUseTypeID, wellRegistrationWaterUseTypeName, wellRegistrationWaterUseTypeDisplayName) {}
        public static readonly WellRegistrationWaterUseTypeAgricultural Instance = new WellRegistrationWaterUseTypeAgricultural(1, @"Agricultural", @"Agricultural");
    }

    public partial class WellRegistrationWaterUseTypeStockWatering : WellRegistrationWaterUseType
    {
        private WellRegistrationWaterUseTypeStockWatering(int wellRegistrationWaterUseTypeID, string wellRegistrationWaterUseTypeName, string wellRegistrationWaterUseTypeDisplayName) : base(wellRegistrationWaterUseTypeID, wellRegistrationWaterUseTypeName, wellRegistrationWaterUseTypeDisplayName) {}
        public static readonly WellRegistrationWaterUseTypeStockWatering Instance = new WellRegistrationWaterUseTypeStockWatering(2, @"StockWatering", @"Stock Watering");
    }

    public partial class WellRegistrationWaterUseTypeDomestic : WellRegistrationWaterUseType
    {
        private WellRegistrationWaterUseTypeDomestic(int wellRegistrationWaterUseTypeID, string wellRegistrationWaterUseTypeName, string wellRegistrationWaterUseTypeDisplayName) : base(wellRegistrationWaterUseTypeID, wellRegistrationWaterUseTypeName, wellRegistrationWaterUseTypeDisplayName) {}
        public static readonly WellRegistrationWaterUseTypeDomestic Instance = new WellRegistrationWaterUseTypeDomestic(3, @"Domestic", @"Domestic");
    }

    public partial class WellRegistrationWaterUseTypePublicMunicipal : WellRegistrationWaterUseType
    {
        private WellRegistrationWaterUseTypePublicMunicipal(int wellRegistrationWaterUseTypeID, string wellRegistrationWaterUseTypeName, string wellRegistrationWaterUseTypeDisplayName) : base(wellRegistrationWaterUseTypeID, wellRegistrationWaterUseTypeName, wellRegistrationWaterUseTypeDisplayName) {}
        public static readonly WellRegistrationWaterUseTypePublicMunicipal Instance = new WellRegistrationWaterUseTypePublicMunicipal(4, @"PublicMunicipal", @"Public Municipal");
    }

    public partial class WellRegistrationWaterUseTypePrivateMunicipal : WellRegistrationWaterUseType
    {
        private WellRegistrationWaterUseTypePrivateMunicipal(int wellRegistrationWaterUseTypeID, string wellRegistrationWaterUseTypeName, string wellRegistrationWaterUseTypeDisplayName) : base(wellRegistrationWaterUseTypeID, wellRegistrationWaterUseTypeName, wellRegistrationWaterUseTypeDisplayName) {}
        public static readonly WellRegistrationWaterUseTypePrivateMunicipal Instance = new WellRegistrationWaterUseTypePrivateMunicipal(5, @"PrivateMunicipal", @"Private Municipal");
    }

    public partial class WellRegistrationWaterUseTypeOther : WellRegistrationWaterUseType
    {
        private WellRegistrationWaterUseTypeOther(int wellRegistrationWaterUseTypeID, string wellRegistrationWaterUseTypeName, string wellRegistrationWaterUseTypeDisplayName) : base(wellRegistrationWaterUseTypeID, wellRegistrationWaterUseTypeName, wellRegistrationWaterUseTypeDisplayName) {}
        public static readonly WellRegistrationWaterUseTypeOther Instance = new WellRegistrationWaterUseTypeOther(6, @"Other", @"Other");
    }
}