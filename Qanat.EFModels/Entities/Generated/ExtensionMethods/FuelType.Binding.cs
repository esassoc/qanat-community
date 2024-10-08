//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[FuelType]
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Qanat.Models.DataTransferObjects;


namespace Qanat.EFModels.Entities
{
    public abstract partial class FuelType : IHavePrimaryKey
    {
        public static readonly FuelTypeElectric Electric = FuelTypeElectric.Instance;
        public static readonly FuelTypeDiesel Diesel = FuelTypeDiesel.Instance;
        public static readonly FuelTypeLPGas LPGas = FuelTypeLPGas.Instance;
        public static readonly FuelTypeOther Other = FuelTypeOther.Instance;

        public static readonly List<FuelType> All;
        public static readonly List<FuelTypeSimpleDto> AllAsSimpleDto;
        public static readonly ReadOnlyDictionary<int, FuelType> AllLookupDictionary;
        public static readonly ReadOnlyDictionary<int, FuelTypeSimpleDto> AllAsSimpleDtoLookupDictionary;

        /// <summary>
        /// Static type constructor to coordinate static initialization order
        /// </summary>
        static FuelType()
        {
            All = new List<FuelType> { Electric, Diesel, LPGas, Other };
            AllAsSimpleDto = new List<FuelTypeSimpleDto> { Electric.AsSimpleDto(), Diesel.AsSimpleDto(), LPGas.AsSimpleDto(), Other.AsSimpleDto() };
            AllLookupDictionary = new ReadOnlyDictionary<int, FuelType>(All.ToDictionary(x => x.FuelTypeID));
            AllAsSimpleDtoLookupDictionary = new ReadOnlyDictionary<int, FuelTypeSimpleDto>(AllAsSimpleDto.ToDictionary(x => x.FuelTypeID));
        }

        /// <summary>
        /// Protected constructor only for use in instantiating the set of static lookup values that match database
        /// </summary>
        protected FuelType(int fuelTypeID, string fuelTypeName, string fuelTypeDisplayName)
        {
            FuelTypeID = fuelTypeID;
            FuelTypeName = fuelTypeName;
            FuelTypeDisplayName = fuelTypeDisplayName;
        }

        [Key]
        public int FuelTypeID { get; private set; }
        public string FuelTypeName { get; private set; }
        public string FuelTypeDisplayName { get; private set; }
        [NotMapped]
        public int PrimaryKey { get { return FuelTypeID; } }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public bool Equals(FuelType other)
        {
            if (other == null)
            {
                return false;
            }
            return other.FuelTypeID == FuelTypeID;
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override bool Equals(object obj)
        {
            return Equals(obj as FuelType);
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override int GetHashCode()
        {
            return FuelTypeID;
        }

        public static bool operator ==(FuelType left, FuelType right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(FuelType left, FuelType right)
        {
            return !Equals(left, right);
        }

        public FuelTypeEnum ToEnum => (FuelTypeEnum)GetHashCode();

        public static FuelType ToType(int enumValue)
        {
            return ToType((FuelTypeEnum)enumValue);
        }

        public static FuelType ToType(FuelTypeEnum enumValue)
        {
            switch (enumValue)
            {
                case FuelTypeEnum.Diesel:
                    return Diesel;
                case FuelTypeEnum.Electric:
                    return Electric;
                case FuelTypeEnum.LPGas:
                    return LPGas;
                case FuelTypeEnum.Other:
                    return Other;
                default:
                    throw new ArgumentException("Unable to map Enum: {enumValue}");
            }
        }
    }

    public enum FuelTypeEnum
    {
        Electric = 1,
        Diesel = 2,
        LPGas = 3,
        Other = 4
    }

    public partial class FuelTypeElectric : FuelType
    {
        private FuelTypeElectric(int fuelTypeID, string fuelTypeName, string fuelTypeDisplayName) : base(fuelTypeID, fuelTypeName, fuelTypeDisplayName) {}
        public static readonly FuelTypeElectric Instance = new FuelTypeElectric(1, @"Electric", @"Electric");
    }

    public partial class FuelTypeDiesel : FuelType
    {
        private FuelTypeDiesel(int fuelTypeID, string fuelTypeName, string fuelTypeDisplayName) : base(fuelTypeID, fuelTypeName, fuelTypeDisplayName) {}
        public static readonly FuelTypeDiesel Instance = new FuelTypeDiesel(2, @"Diesel", @"Diesel");
    }

    public partial class FuelTypeLPGas : FuelType
    {
        private FuelTypeLPGas(int fuelTypeID, string fuelTypeName, string fuelTypeDisplayName) : base(fuelTypeID, fuelTypeName, fuelTypeDisplayName) {}
        public static readonly FuelTypeLPGas Instance = new FuelTypeLPGas(3, @"LP Gas", @"LP Gas");
    }

    public partial class FuelTypeOther : FuelType
    {
        private FuelTypeOther(int fuelTypeID, string fuelTypeName, string fuelTypeDisplayName) : base(fuelTypeID, fuelTypeName, fuelTypeDisplayName) {}
        public static readonly FuelTypeOther Instance = new FuelTypeOther(4, @"Other", @"Other");
    }
}