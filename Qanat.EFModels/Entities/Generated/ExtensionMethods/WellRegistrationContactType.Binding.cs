//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[WellRegistrationContactType]
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Qanat.Models.DataTransferObjects;


namespace Qanat.EFModels.Entities
{
    public abstract partial class WellRegistrationContactType : IHavePrimaryKey
    {
        public static readonly WellRegistrationContactTypeLandowner Landowner = WellRegistrationContactTypeLandowner.Instance;
        public static readonly WellRegistrationContactTypeOwnerOperator OwnerOperator = WellRegistrationContactTypeOwnerOperator.Instance;

        public static readonly List<WellRegistrationContactType> All;
        public static readonly ReadOnlyDictionary<int, WellRegistrationContactType> AllLookupDictionary;

        /// <summary>
        /// Static type constructor to coordinate static initialization order
        /// </summary>
        static WellRegistrationContactType()
        {
            All = new List<WellRegistrationContactType> { Landowner, OwnerOperator };
            AllLookupDictionary = new ReadOnlyDictionary<int, WellRegistrationContactType>(All.ToDictionary(x => x.WellRegistrationContactTypeID));
        }

        /// <summary>
        /// Protected constructor only for use in instantiating the set of static lookup values that match database
        /// </summary>
        protected WellRegistrationContactType(int wellRegistrationContactTypeID, string wellRegistrationContactTypeName, string wellRegistrationContactTypeDisplayName)
        {
            WellRegistrationContactTypeID = wellRegistrationContactTypeID;
            WellRegistrationContactTypeName = wellRegistrationContactTypeName;
            WellRegistrationContactTypeDisplayName = wellRegistrationContactTypeDisplayName;
        }

        [Key]
        public int WellRegistrationContactTypeID { get; private set; }
        public string WellRegistrationContactTypeName { get; private set; }
        public string WellRegistrationContactTypeDisplayName { get; private set; }
        [NotMapped]
        public int PrimaryKey { get { return WellRegistrationContactTypeID; } }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public bool Equals(WellRegistrationContactType other)
        {
            if (other == null)
            {
                return false;
            }
            return other.WellRegistrationContactTypeID == WellRegistrationContactTypeID;
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override bool Equals(object obj)
        {
            return Equals(obj as WellRegistrationContactType);
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override int GetHashCode()
        {
            return WellRegistrationContactTypeID;
        }

        public static bool operator ==(WellRegistrationContactType left, WellRegistrationContactType right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(WellRegistrationContactType left, WellRegistrationContactType right)
        {
            return !Equals(left, right);
        }

        public WellRegistrationContactTypeEnum ToEnum => (WellRegistrationContactTypeEnum)GetHashCode();

        public static WellRegistrationContactType ToType(int enumValue)
        {
            return ToType((WellRegistrationContactTypeEnum)enumValue);
        }

        public static WellRegistrationContactType ToType(WellRegistrationContactTypeEnum enumValue)
        {
            switch (enumValue)
            {
                case WellRegistrationContactTypeEnum.Landowner:
                    return Landowner;
                case WellRegistrationContactTypeEnum.OwnerOperator:
                    return OwnerOperator;
                default:
                    throw new ArgumentException("Unable to map Enum: {enumValue}");
            }
        }
    }

    public enum WellRegistrationContactTypeEnum
    {
        Landowner = 1,
        OwnerOperator = 2
    }

    public partial class WellRegistrationContactTypeLandowner : WellRegistrationContactType
    {
        private WellRegistrationContactTypeLandowner(int wellRegistrationContactTypeID, string wellRegistrationContactTypeName, string wellRegistrationContactTypeDisplayName) : base(wellRegistrationContactTypeID, wellRegistrationContactTypeName, wellRegistrationContactTypeDisplayName) {}
        public static readonly WellRegistrationContactTypeLandowner Instance = new WellRegistrationContactTypeLandowner(1, @"Landowner", @"Landowner");
    }

    public partial class WellRegistrationContactTypeOwnerOperator : WellRegistrationContactType
    {
        private WellRegistrationContactTypeOwnerOperator(int wellRegistrationContactTypeID, string wellRegistrationContactTypeName, string wellRegistrationContactTypeDisplayName) : base(wellRegistrationContactTypeID, wellRegistrationContactTypeName, wellRegistrationContactTypeDisplayName) {}
        public static readonly WellRegistrationContactTypeOwnerOperator Instance = new WellRegistrationContactTypeOwnerOperator(2, @"OwnerOperator", @"OwnerOperator");
    }
}