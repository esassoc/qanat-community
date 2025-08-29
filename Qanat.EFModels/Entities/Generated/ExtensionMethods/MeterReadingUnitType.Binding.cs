//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[MeterReadingUnitType]
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Qanat.Models.DataTransferObjects;


namespace Qanat.EFModels.Entities
{
    public abstract partial class MeterReadingUnitType : IHavePrimaryKey
    {
        public static readonly MeterReadingUnitTypeAcreFeet AcreFeet = MeterReadingUnitTypeAcreFeet.Instance;
        public static readonly MeterReadingUnitTypeGallons Gallons = MeterReadingUnitTypeGallons.Instance;

        public static readonly List<MeterReadingUnitType> All;
        public static readonly ReadOnlyDictionary<int, MeterReadingUnitType> AllLookupDictionary;

        /// <summary>
        /// Static type constructor to coordinate static initialization order
        /// </summary>
        static MeterReadingUnitType()
        {
            All = new List<MeterReadingUnitType> { AcreFeet, Gallons };
            AllLookupDictionary = new ReadOnlyDictionary<int, MeterReadingUnitType>(All.ToDictionary(x => x.MeterReadingUnitTypeID));
        }

        /// <summary>
        /// Protected constructor only for use in instantiating the set of static lookup values that match database
        /// </summary>
        protected MeterReadingUnitType(int meterReadingUnitTypeID, string meterReadingUnitTypeName, string meterReadingUnitTypeDisplayName, string meterReadingUnitTypeAbbreviation, string meterReadingUnitTypeAlternateDisplayName)
        {
            MeterReadingUnitTypeID = meterReadingUnitTypeID;
            MeterReadingUnitTypeName = meterReadingUnitTypeName;
            MeterReadingUnitTypeDisplayName = meterReadingUnitTypeDisplayName;
            MeterReadingUnitTypeAbbreviation = meterReadingUnitTypeAbbreviation;
            MeterReadingUnitTypeAlternateDisplayName = meterReadingUnitTypeAlternateDisplayName;
        }

        [Key]
        public int MeterReadingUnitTypeID { get; private set; }
        public string MeterReadingUnitTypeName { get; private set; }
        public string MeterReadingUnitTypeDisplayName { get; private set; }
        public string MeterReadingUnitTypeAbbreviation { get; private set; }
        public string MeterReadingUnitTypeAlternateDisplayName { get; private set; }
        [NotMapped]
        public int PrimaryKey { get { return MeterReadingUnitTypeID; } }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public bool Equals(MeterReadingUnitType other)
        {
            if (other == null)
            {
                return false;
            }
            return other.MeterReadingUnitTypeID == MeterReadingUnitTypeID;
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override bool Equals(object obj)
        {
            return Equals(obj as MeterReadingUnitType);
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override int GetHashCode()
        {
            return MeterReadingUnitTypeID;
        }

        public static bool operator ==(MeterReadingUnitType left, MeterReadingUnitType right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(MeterReadingUnitType left, MeterReadingUnitType right)
        {
            return !Equals(left, right);
        }

        public MeterReadingUnitTypeEnum ToEnum => (MeterReadingUnitTypeEnum)GetHashCode();

        public static MeterReadingUnitType ToType(int enumValue)
        {
            return ToType((MeterReadingUnitTypeEnum)enumValue);
        }

        public static MeterReadingUnitType ToType(MeterReadingUnitTypeEnum enumValue)
        {
            switch (enumValue)
            {
                case MeterReadingUnitTypeEnum.AcreFeet:
                    return AcreFeet;
                case MeterReadingUnitTypeEnum.Gallons:
                    return Gallons;
                default:
                    throw new ArgumentException("Unable to map Enum: {enumValue}");
            }
        }
    }

    public enum MeterReadingUnitTypeEnum
    {
        AcreFeet = 1,
        Gallons = 2
    }

    public partial class MeterReadingUnitTypeAcreFeet : MeterReadingUnitType
    {
        private MeterReadingUnitTypeAcreFeet(int meterReadingUnitTypeID, string meterReadingUnitTypeName, string meterReadingUnitTypeDisplayName, string meterReadingUnitTypeAbbreviation, string meterReadingUnitTypeAlternateDisplayName) : base(meterReadingUnitTypeID, meterReadingUnitTypeName, meterReadingUnitTypeDisplayName, meterReadingUnitTypeAbbreviation, meterReadingUnitTypeAlternateDisplayName) {}
        public static readonly MeterReadingUnitTypeAcreFeet Instance = new MeterReadingUnitTypeAcreFeet(1, @"AcreFeet", @"acre-feet", @"ac-ft", @"AF");
    }

    public partial class MeterReadingUnitTypeGallons : MeterReadingUnitType
    {
        private MeterReadingUnitTypeGallons(int meterReadingUnitTypeID, string meterReadingUnitTypeName, string meterReadingUnitTypeDisplayName, string meterReadingUnitTypeAbbreviation, string meterReadingUnitTypeAlternateDisplayName) : base(meterReadingUnitTypeID, meterReadingUnitTypeName, meterReadingUnitTypeDisplayName, meterReadingUnitTypeAbbreviation, meterReadingUnitTypeAlternateDisplayName) {}
        public static readonly MeterReadingUnitTypeGallons Instance = new MeterReadingUnitTypeGallons(2, @"Gallons", @"gallons", @"gal", null);
    }
}