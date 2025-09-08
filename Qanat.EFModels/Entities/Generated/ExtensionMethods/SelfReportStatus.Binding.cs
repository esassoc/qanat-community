//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[SelfReportStatus]
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Qanat.Models.DataTransferObjects;


namespace Qanat.EFModels.Entities
{
    public abstract partial class SelfReportStatus : IHavePrimaryKey
    {
        public static readonly SelfReportStatusDraft Draft = SelfReportStatusDraft.Instance;
        public static readonly SelfReportStatusSubmitted Submitted = SelfReportStatusSubmitted.Instance;
        public static readonly SelfReportStatusApproved Approved = SelfReportStatusApproved.Instance;
        public static readonly SelfReportStatusReturned Returned = SelfReportStatusReturned.Instance;

        public static readonly List<SelfReportStatus> All;
        public static readonly ReadOnlyDictionary<int, SelfReportStatus> AllLookupDictionary;

        /// <summary>
        /// Static type constructor to coordinate static initialization order
        /// </summary>
        static SelfReportStatus()
        {
            All = new List<SelfReportStatus> { Draft, Submitted, Approved, Returned };
            AllLookupDictionary = new ReadOnlyDictionary<int, SelfReportStatus>(All.ToDictionary(x => x.SelfReportStatusID));
        }

        /// <summary>
        /// Protected constructor only for use in instantiating the set of static lookup values that match database
        /// </summary>
        protected SelfReportStatus(int selfReportStatusID, string selfReportStatusName, string selfReportStatusDisplayName)
        {
            SelfReportStatusID = selfReportStatusID;
            SelfReportStatusName = selfReportStatusName;
            SelfReportStatusDisplayName = selfReportStatusDisplayName;
        }

        [Key]
        public int SelfReportStatusID { get; private set; }
        public string SelfReportStatusName { get; private set; }
        public string SelfReportStatusDisplayName { get; private set; }
        [NotMapped]
        public int PrimaryKey { get { return SelfReportStatusID; } }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public bool Equals(SelfReportStatus other)
        {
            if (other == null)
            {
                return false;
            }
            return other.SelfReportStatusID == SelfReportStatusID;
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override bool Equals(object obj)
        {
            return Equals(obj as SelfReportStatus);
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override int GetHashCode()
        {
            return SelfReportStatusID;
        }

        public static bool operator ==(SelfReportStatus left, SelfReportStatus right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(SelfReportStatus left, SelfReportStatus right)
        {
            return !Equals(left, right);
        }

        public SelfReportStatusEnum ToEnum => (SelfReportStatusEnum)GetHashCode();

        public static SelfReportStatus ToType(int enumValue)
        {
            return ToType((SelfReportStatusEnum)enumValue);
        }

        public static SelfReportStatus ToType(SelfReportStatusEnum enumValue)
        {
            switch (enumValue)
            {
                case SelfReportStatusEnum.Approved:
                    return Approved;
                case SelfReportStatusEnum.Draft:
                    return Draft;
                case SelfReportStatusEnum.Returned:
                    return Returned;
                case SelfReportStatusEnum.Submitted:
                    return Submitted;
                default:
                    throw new ArgumentException("Unable to map Enum: {enumValue}");
            }
        }
    }

    public enum SelfReportStatusEnum
    {
        Draft = 1,
        Submitted = 2,
        Approved = 3,
        Returned = 4
    }

    public partial class SelfReportStatusDraft : SelfReportStatus
    {
        private SelfReportStatusDraft(int selfReportStatusID, string selfReportStatusName, string selfReportStatusDisplayName) : base(selfReportStatusID, selfReportStatusName, selfReportStatusDisplayName) {}
        public static readonly SelfReportStatusDraft Instance = new SelfReportStatusDraft(1, @"Draft", @"Draft");
    }

    public partial class SelfReportStatusSubmitted : SelfReportStatus
    {
        private SelfReportStatusSubmitted(int selfReportStatusID, string selfReportStatusName, string selfReportStatusDisplayName) : base(selfReportStatusID, selfReportStatusName, selfReportStatusDisplayName) {}
        public static readonly SelfReportStatusSubmitted Instance = new SelfReportStatusSubmitted(2, @"Submitted", @"Submitted");
    }

    public partial class SelfReportStatusApproved : SelfReportStatus
    {
        private SelfReportStatusApproved(int selfReportStatusID, string selfReportStatusName, string selfReportStatusDisplayName) : base(selfReportStatusID, selfReportStatusName, selfReportStatusDisplayName) {}
        public static readonly SelfReportStatusApproved Instance = new SelfReportStatusApproved(3, @"Approved", @"Approved");
    }

    public partial class SelfReportStatusReturned : SelfReportStatus
    {
        private SelfReportStatusReturned(int selfReportStatusID, string selfReportStatusName, string selfReportStatusDisplayName) : base(selfReportStatusID, selfReportStatusName, selfReportStatusDisplayName) {}
        public static readonly SelfReportStatusReturned Instance = new SelfReportStatusReturned(4, @"Returned", @"Returned");
    }
}