//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[WellRegistrationStatus]
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Qanat.Models.DataTransferObjects;


namespace Qanat.EFModels.Entities
{
    public abstract partial class WellRegistrationStatus : IHavePrimaryKey
    {
        public static readonly WellRegistrationStatusDraft Draft = WellRegistrationStatusDraft.Instance;
        public static readonly WellRegistrationStatusSubmitted Submitted = WellRegistrationStatusSubmitted.Instance;
        public static readonly WellRegistrationStatusReturned Returned = WellRegistrationStatusReturned.Instance;
        public static readonly WellRegistrationStatusApproved Approved = WellRegistrationStatusApproved.Instance;

        public static readonly List<WellRegistrationStatus> All;
        public static readonly List<WellRegistrationStatusSimpleDto> AllAsSimpleDto;
        public static readonly ReadOnlyDictionary<int, WellRegistrationStatus> AllLookupDictionary;
        public static readonly ReadOnlyDictionary<int, WellRegistrationStatusSimpleDto> AllAsSimpleDtoLookupDictionary;

        /// <summary>
        /// Static type constructor to coordinate static initialization order
        /// </summary>
        static WellRegistrationStatus()
        {
            All = new List<WellRegistrationStatus> { Draft, Submitted, Returned, Approved };
            AllAsSimpleDto = new List<WellRegistrationStatusSimpleDto> { Draft.AsSimpleDto(), Submitted.AsSimpleDto(), Returned.AsSimpleDto(), Approved.AsSimpleDto() };
            AllLookupDictionary = new ReadOnlyDictionary<int, WellRegistrationStatus>(All.ToDictionary(x => x.WellRegistrationStatusID));
            AllAsSimpleDtoLookupDictionary = new ReadOnlyDictionary<int, WellRegistrationStatusSimpleDto>(AllAsSimpleDto.ToDictionary(x => x.WellRegistrationStatusID));
        }

        /// <summary>
        /// Protected constructor only for use in instantiating the set of static lookup values that match database
        /// </summary>
        protected WellRegistrationStatus(int wellRegistrationStatusID, string wellRegistrationStatusName, string wellRegistrationStatusDisplayName)
        {
            WellRegistrationStatusID = wellRegistrationStatusID;
            WellRegistrationStatusName = wellRegistrationStatusName;
            WellRegistrationStatusDisplayName = wellRegistrationStatusDisplayName;
        }

        [Key]
        public int WellRegistrationStatusID { get; private set; }
        public string WellRegistrationStatusName { get; private set; }
        public string WellRegistrationStatusDisplayName { get; private set; }
        [NotMapped]
        public int PrimaryKey { get { return WellRegistrationStatusID; } }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public bool Equals(WellRegistrationStatus other)
        {
            if (other == null)
            {
                return false;
            }
            return other.WellRegistrationStatusID == WellRegistrationStatusID;
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override bool Equals(object obj)
        {
            return Equals(obj as WellRegistrationStatus);
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override int GetHashCode()
        {
            return WellRegistrationStatusID;
        }

        public static bool operator ==(WellRegistrationStatus left, WellRegistrationStatus right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(WellRegistrationStatus left, WellRegistrationStatus right)
        {
            return !Equals(left, right);
        }

        public WellRegistrationStatusEnum ToEnum => (WellRegistrationStatusEnum)GetHashCode();

        public static WellRegistrationStatus ToType(int enumValue)
        {
            return ToType((WellRegistrationStatusEnum)enumValue);
        }

        public static WellRegistrationStatus ToType(WellRegistrationStatusEnum enumValue)
        {
            switch (enumValue)
            {
                case WellRegistrationStatusEnum.Approved:
                    return Approved;
                case WellRegistrationStatusEnum.Draft:
                    return Draft;
                case WellRegistrationStatusEnum.Returned:
                    return Returned;
                case WellRegistrationStatusEnum.Submitted:
                    return Submitted;
                default:
                    throw new ArgumentException("Unable to map Enum: {enumValue}");
            }
        }
    }

    public enum WellRegistrationStatusEnum
    {
        Draft = 1,
        Submitted = 2,
        Returned = 3,
        Approved = 4
    }

    public partial class WellRegistrationStatusDraft : WellRegistrationStatus
    {
        private WellRegistrationStatusDraft(int wellRegistrationStatusID, string wellRegistrationStatusName, string wellRegistrationStatusDisplayName) : base(wellRegistrationStatusID, wellRegistrationStatusName, wellRegistrationStatusDisplayName) {}
        public static readonly WellRegistrationStatusDraft Instance = new WellRegistrationStatusDraft(1, @"Draft", @"Draft");
    }

    public partial class WellRegistrationStatusSubmitted : WellRegistrationStatus
    {
        private WellRegistrationStatusSubmitted(int wellRegistrationStatusID, string wellRegistrationStatusName, string wellRegistrationStatusDisplayName) : base(wellRegistrationStatusID, wellRegistrationStatusName, wellRegistrationStatusDisplayName) {}
        public static readonly WellRegistrationStatusSubmitted Instance = new WellRegistrationStatusSubmitted(2, @"Submitted", @"Submitted");
    }

    public partial class WellRegistrationStatusReturned : WellRegistrationStatus
    {
        private WellRegistrationStatusReturned(int wellRegistrationStatusID, string wellRegistrationStatusName, string wellRegistrationStatusDisplayName) : base(wellRegistrationStatusID, wellRegistrationStatusName, wellRegistrationStatusDisplayName) {}
        public static readonly WellRegistrationStatusReturned Instance = new WellRegistrationStatusReturned(3, @"Returned", @"Returned");
    }

    public partial class WellRegistrationStatusApproved : WellRegistrationStatus
    {
        private WellRegistrationStatusApproved(int wellRegistrationStatusID, string wellRegistrationStatusName, string wellRegistrationStatusDisplayName) : base(wellRegistrationStatusID, wellRegistrationStatusName, wellRegistrationStatusDisplayName) {}
        public static readonly WellRegistrationStatusApproved Instance = new WellRegistrationStatusApproved(4, @"Approved", @"Approved");
    }
}