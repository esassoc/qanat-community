//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[CustomAttributeType]
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Qanat.Models.DataTransferObjects;


namespace Qanat.EFModels.Entities
{
    public abstract partial class CustomAttributeType : IHavePrimaryKey
    {
        public static readonly CustomAttributeTypeWaterAccount WaterAccount = CustomAttributeTypeWaterAccount.Instance;
        public static readonly CustomAttributeTypeParcel Parcel = CustomAttributeTypeParcel.Instance;

        public static readonly List<CustomAttributeType> All;
        public static readonly ReadOnlyDictionary<int, CustomAttributeType> AllLookupDictionary;

        /// <summary>
        /// Static type constructor to coordinate static initialization order
        /// </summary>
        static CustomAttributeType()
        {
            All = new List<CustomAttributeType> { WaterAccount, Parcel };
            AllLookupDictionary = new ReadOnlyDictionary<int, CustomAttributeType>(All.ToDictionary(x => x.CustomAttributeTypeID));
        }

        /// <summary>
        /// Protected constructor only for use in instantiating the set of static lookup values that match database
        /// </summary>
        protected CustomAttributeType(int customAttributeTypeID, string customAttributeTypeName, string customAttributeTypeDisplayName)
        {
            CustomAttributeTypeID = customAttributeTypeID;
            CustomAttributeTypeName = customAttributeTypeName;
            CustomAttributeTypeDisplayName = customAttributeTypeDisplayName;
        }

        [Key]
        public int CustomAttributeTypeID { get; private set; }
        public string CustomAttributeTypeName { get; private set; }
        public string CustomAttributeTypeDisplayName { get; private set; }
        [NotMapped]
        public int PrimaryKey { get { return CustomAttributeTypeID; } }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public bool Equals(CustomAttributeType other)
        {
            if (other == null)
            {
                return false;
            }
            return other.CustomAttributeTypeID == CustomAttributeTypeID;
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override bool Equals(object obj)
        {
            return Equals(obj as CustomAttributeType);
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override int GetHashCode()
        {
            return CustomAttributeTypeID;
        }

        public static bool operator ==(CustomAttributeType left, CustomAttributeType right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(CustomAttributeType left, CustomAttributeType right)
        {
            return !Equals(left, right);
        }

        public CustomAttributeTypeEnum ToEnum => (CustomAttributeTypeEnum)GetHashCode();

        public static CustomAttributeType ToType(int enumValue)
        {
            return ToType((CustomAttributeTypeEnum)enumValue);
        }

        public static CustomAttributeType ToType(CustomAttributeTypeEnum enumValue)
        {
            switch (enumValue)
            {
                case CustomAttributeTypeEnum.Parcel:
                    return Parcel;
                case CustomAttributeTypeEnum.WaterAccount:
                    return WaterAccount;
                default:
                    throw new ArgumentException("Unable to map Enum: {enumValue}");
            }
        }
    }

    public enum CustomAttributeTypeEnum
    {
        WaterAccount = 1,
        Parcel = 2
    }

    public partial class CustomAttributeTypeWaterAccount : CustomAttributeType
    {
        private CustomAttributeTypeWaterAccount(int customAttributeTypeID, string customAttributeTypeName, string customAttributeTypeDisplayName) : base(customAttributeTypeID, customAttributeTypeName, customAttributeTypeDisplayName) {}
        public static readonly CustomAttributeTypeWaterAccount Instance = new CustomAttributeTypeWaterAccount(1, @"WaterAccount", @"Water Account");
    }

    public partial class CustomAttributeTypeParcel : CustomAttributeType
    {
        private CustomAttributeTypeParcel(int customAttributeTypeID, string customAttributeTypeName, string customAttributeTypeDisplayName) : base(customAttributeTypeID, customAttributeTypeName, customAttributeTypeDisplayName) {}
        public static readonly CustomAttributeTypeParcel Instance = new CustomAttributeTypeParcel(2, @"Parcel", @"Parcel");
    }
}