//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[UnitType]
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Qanat.Models.DataTransferObjects;


namespace Qanat.EFModels.Entities
{
    public abstract partial class UnitType : IHavePrimaryKey
    {
        public static readonly UnitTypeInches Inches = UnitTypeInches.Instance;
        public static readonly UnitTypeMillimeters Millimeters = UnitTypeMillimeters.Instance;

        public static readonly List<UnitType> All;
        public static readonly List<UnitTypeSimpleDto> AllAsSimpleDto;
        public static readonly ReadOnlyDictionary<int, UnitType> AllLookupDictionary;
        public static readonly ReadOnlyDictionary<int, UnitTypeSimpleDto> AllAsSimpleDtoLookupDictionary;

        /// <summary>
        /// Static type constructor to coordinate static initialization order
        /// </summary>
        static UnitType()
        {
            All = new List<UnitType> { Inches, Millimeters };
            AllAsSimpleDto = new List<UnitTypeSimpleDto> { Inches.AsSimpleDto(), Millimeters.AsSimpleDto() };
            AllLookupDictionary = new ReadOnlyDictionary<int, UnitType>(All.ToDictionary(x => x.UnitTypeID));
            AllAsSimpleDtoLookupDictionary = new ReadOnlyDictionary<int, UnitTypeSimpleDto>(AllAsSimpleDto.ToDictionary(x => x.UnitTypeID));
        }

        /// <summary>
        /// Protected constructor only for use in instantiating the set of static lookup values that match database
        /// </summary>
        protected UnitType(int unitTypeID, string unitTypeName, string unitTypeDisplayName, string unitTypeAbbreviation)
        {
            UnitTypeID = unitTypeID;
            UnitTypeName = unitTypeName;
            UnitTypeDisplayName = unitTypeDisplayName;
            UnitTypeAbbreviation = unitTypeAbbreviation;
        }

        [Key]
        public int UnitTypeID { get; private set; }
        public string UnitTypeName { get; private set; }
        public string UnitTypeDisplayName { get; private set; }
        public string UnitTypeAbbreviation { get; private set; }
        [NotMapped]
        public int PrimaryKey { get { return UnitTypeID; } }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public bool Equals(UnitType other)
        {
            if (other == null)
            {
                return false;
            }
            return other.UnitTypeID == UnitTypeID;
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override bool Equals(object obj)
        {
            return Equals(obj as UnitType);
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override int GetHashCode()
        {
            return UnitTypeID;
        }

        public static bool operator ==(UnitType left, UnitType right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(UnitType left, UnitType right)
        {
            return !Equals(left, right);
        }

        public UnitTypeEnum ToEnum => (UnitTypeEnum)GetHashCode();

        public static UnitType ToType(int enumValue)
        {
            return ToType((UnitTypeEnum)enumValue);
        }

        public static UnitType ToType(UnitTypeEnum enumValue)
        {
            switch (enumValue)
            {
                case UnitTypeEnum.Inches:
                    return Inches;
                case UnitTypeEnum.Millimeters:
                    return Millimeters;
                default:
                    throw new ArgumentException("Unable to map Enum: {enumValue}");
            }
        }
    }

    public enum UnitTypeEnum
    {
        Inches = 1,
        Millimeters = 2
    }

    public partial class UnitTypeInches : UnitType
    {
        private UnitTypeInches(int unitTypeID, string unitTypeName, string unitTypeDisplayName, string unitTypeAbbreviation) : base(unitTypeID, unitTypeName, unitTypeDisplayName, unitTypeAbbreviation) {}
        public static readonly UnitTypeInches Instance = new UnitTypeInches(1, @"Inches", @"inches", @"in");
    }

    public partial class UnitTypeMillimeters : UnitType
    {
        private UnitTypeMillimeters(int unitTypeID, string unitTypeName, string unitTypeDisplayName, string unitTypeAbbreviation) : base(unitTypeID, unitTypeName, unitTypeDisplayName, unitTypeAbbreviation) {}
        public static readonly UnitTypeMillimeters Instance = new UnitTypeMillimeters(2, @"Millimeters", @"millimeters", @"mm");
    }
}