//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ParcelStatus]
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Qanat.Models.DataTransferObjects;


namespace Qanat.EFModels.Entities
{
    public abstract partial class ParcelStatus : IHavePrimaryKey
    {
        public static readonly ParcelStatusAssigned Assigned = ParcelStatusAssigned.Instance;
        public static readonly ParcelStatusInactive Inactive = ParcelStatusInactive.Instance;
        public static readonly ParcelStatusUnassigned Unassigned = ParcelStatusUnassigned.Instance;
        public static readonly ParcelStatusExcluded Excluded = ParcelStatusExcluded.Instance;

        public static readonly List<ParcelStatus> All;
        public static readonly List<ParcelStatusSimpleDto> AllAsSimpleDto;
        public static readonly ReadOnlyDictionary<int, ParcelStatus> AllLookupDictionary;
        public static readonly ReadOnlyDictionary<int, ParcelStatusSimpleDto> AllAsSimpleDtoLookupDictionary;

        /// <summary>
        /// Static type constructor to coordinate static initialization order
        /// </summary>
        static ParcelStatus()
        {
            All = new List<ParcelStatus> { Assigned, Inactive, Unassigned, Excluded };
            AllAsSimpleDto = new List<ParcelStatusSimpleDto> { Assigned.AsSimpleDto(), Inactive.AsSimpleDto(), Unassigned.AsSimpleDto(), Excluded.AsSimpleDto() };
            AllLookupDictionary = new ReadOnlyDictionary<int, ParcelStatus>(All.ToDictionary(x => x.ParcelStatusID));
            AllAsSimpleDtoLookupDictionary = new ReadOnlyDictionary<int, ParcelStatusSimpleDto>(AllAsSimpleDto.ToDictionary(x => x.ParcelStatusID));
        }

        /// <summary>
        /// Protected constructor only for use in instantiating the set of static lookup values that match database
        /// </summary>
        protected ParcelStatus(int parcelStatusID, string parcelStatusName, string parcelStatusDisplayName)
        {
            ParcelStatusID = parcelStatusID;
            ParcelStatusName = parcelStatusName;
            ParcelStatusDisplayName = parcelStatusDisplayName;
        }

        [Key]
        public int ParcelStatusID { get; private set; }
        public string ParcelStatusName { get; private set; }
        public string ParcelStatusDisplayName { get; private set; }
        [NotMapped]
        public int PrimaryKey { get { return ParcelStatusID; } }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public bool Equals(ParcelStatus other)
        {
            if (other == null)
            {
                return false;
            }
            return other.ParcelStatusID == ParcelStatusID;
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override bool Equals(object obj)
        {
            return Equals(obj as ParcelStatus);
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override int GetHashCode()
        {
            return ParcelStatusID;
        }

        public static bool operator ==(ParcelStatus left, ParcelStatus right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ParcelStatus left, ParcelStatus right)
        {
            return !Equals(left, right);
        }

        public ParcelStatusEnum ToEnum => (ParcelStatusEnum)GetHashCode();

        public static ParcelStatus ToType(int enumValue)
        {
            return ToType((ParcelStatusEnum)enumValue);
        }

        public static ParcelStatus ToType(ParcelStatusEnum enumValue)
        {
            switch (enumValue)
            {
                case ParcelStatusEnum.Assigned:
                    return Assigned;
                case ParcelStatusEnum.Excluded:
                    return Excluded;
                case ParcelStatusEnum.Inactive:
                    return Inactive;
                case ParcelStatusEnum.Unassigned:
                    return Unassigned;
                default:
                    throw new ArgumentException("Unable to map Enum: {enumValue}");
            }
        }
    }

    public enum ParcelStatusEnum
    {
        Assigned = 1,
        Inactive = 2,
        Unassigned = 3,
        Excluded = 4
    }

    public partial class ParcelStatusAssigned : ParcelStatus
    {
        private ParcelStatusAssigned(int parcelStatusID, string parcelStatusName, string parcelStatusDisplayName) : base(parcelStatusID, parcelStatusName, parcelStatusDisplayName) {}
        public static readonly ParcelStatusAssigned Instance = new ParcelStatusAssigned(1, @"Assigned", @"Active");
    }

    public partial class ParcelStatusInactive : ParcelStatus
    {
        private ParcelStatusInactive(int parcelStatusID, string parcelStatusName, string parcelStatusDisplayName) : base(parcelStatusID, parcelStatusName, parcelStatusDisplayName) {}
        public static readonly ParcelStatusInactive Instance = new ParcelStatusInactive(2, @"Inactive", @"Inactive");
    }

    public partial class ParcelStatusUnassigned : ParcelStatus
    {
        private ParcelStatusUnassigned(int parcelStatusID, string parcelStatusName, string parcelStatusDisplayName) : base(parcelStatusID, parcelStatusName, parcelStatusDisplayName) {}
        public static readonly ParcelStatusUnassigned Instance = new ParcelStatusUnassigned(3, @"Unassigned", @"Unassigned");
    }

    public partial class ParcelStatusExcluded : ParcelStatus
    {
        private ParcelStatusExcluded(int parcelStatusID, string parcelStatusName, string parcelStatusDisplayName) : base(parcelStatusID, parcelStatusName, parcelStatusDisplayName) {}
        public static readonly ParcelStatusExcluded Instance = new ParcelStatusExcluded(4, @"Excluded", @"Excluded");
    }
}