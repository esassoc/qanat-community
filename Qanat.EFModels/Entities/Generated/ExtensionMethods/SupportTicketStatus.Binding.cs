//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[SupportTicketStatus]
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Qanat.Models.DataTransferObjects;


namespace Qanat.EFModels.Entities
{
    public abstract partial class SupportTicketStatus : IHavePrimaryKey
    {
        public static readonly SupportTicketStatusUnassigned Unassigned = SupportTicketStatusUnassigned.Instance;
        public static readonly SupportTicketStatusAssigned Assigned = SupportTicketStatusAssigned.Instance;
        public static readonly SupportTicketStatusClosed Closed = SupportTicketStatusClosed.Instance;

        public static readonly List<SupportTicketStatus> All;
        public static readonly List<SupportTicketStatusSimpleDto> AllAsSimpleDto;
        public static readonly ReadOnlyDictionary<int, SupportTicketStatus> AllLookupDictionary;
        public static readonly ReadOnlyDictionary<int, SupportTicketStatusSimpleDto> AllAsSimpleDtoLookupDictionary;

        /// <summary>
        /// Static type constructor to coordinate static initialization order
        /// </summary>
        static SupportTicketStatus()
        {
            All = new List<SupportTicketStatus> { Unassigned, Assigned, Closed };
            AllAsSimpleDto = new List<SupportTicketStatusSimpleDto> { Unassigned.AsSimpleDto(), Assigned.AsSimpleDto(), Closed.AsSimpleDto() };
            AllLookupDictionary = new ReadOnlyDictionary<int, SupportTicketStatus>(All.ToDictionary(x => x.SupportTicketStatusID));
            AllAsSimpleDtoLookupDictionary = new ReadOnlyDictionary<int, SupportTicketStatusSimpleDto>(AllAsSimpleDto.ToDictionary(x => x.SupportTicketStatusID));
        }

        /// <summary>
        /// Protected constructor only for use in instantiating the set of static lookup values that match database
        /// </summary>
        protected SupportTicketStatus(int supportTicketStatusID, string supportTicketStatusName, string supportTicketStatusDisplayName)
        {
            SupportTicketStatusID = supportTicketStatusID;
            SupportTicketStatusName = supportTicketStatusName;
            SupportTicketStatusDisplayName = supportTicketStatusDisplayName;
        }

        [Key]
        public int SupportTicketStatusID { get; private set; }
        public string SupportTicketStatusName { get; private set; }
        public string SupportTicketStatusDisplayName { get; private set; }
        [NotMapped]
        public int PrimaryKey { get { return SupportTicketStatusID; } }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public bool Equals(SupportTicketStatus other)
        {
            if (other == null)
            {
                return false;
            }
            return other.SupportTicketStatusID == SupportTicketStatusID;
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override bool Equals(object obj)
        {
            return Equals(obj as SupportTicketStatus);
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override int GetHashCode()
        {
            return SupportTicketStatusID;
        }

        public static bool operator ==(SupportTicketStatus left, SupportTicketStatus right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(SupportTicketStatus left, SupportTicketStatus right)
        {
            return !Equals(left, right);
        }

        public SupportTicketStatusEnum ToEnum => (SupportTicketStatusEnum)GetHashCode();

        public static SupportTicketStatus ToType(int enumValue)
        {
            return ToType((SupportTicketStatusEnum)enumValue);
        }

        public static SupportTicketStatus ToType(SupportTicketStatusEnum enumValue)
        {
            switch (enumValue)
            {
                case SupportTicketStatusEnum.Assigned:
                    return Assigned;
                case SupportTicketStatusEnum.Closed:
                    return Closed;
                case SupportTicketStatusEnum.Unassigned:
                    return Unassigned;
                default:
                    throw new ArgumentException("Unable to map Enum: {enumValue}");
            }
        }
    }

    public enum SupportTicketStatusEnum
    {
        Unassigned = 1,
        Assigned = 2,
        Closed = 3
    }

    public partial class SupportTicketStatusUnassigned : SupportTicketStatus
    {
        private SupportTicketStatusUnassigned(int supportTicketStatusID, string supportTicketStatusName, string supportTicketStatusDisplayName) : base(supportTicketStatusID, supportTicketStatusName, supportTicketStatusDisplayName) {}
        public static readonly SupportTicketStatusUnassigned Instance = new SupportTicketStatusUnassigned(1, @"Unassigned", @"Unassigned");
    }

    public partial class SupportTicketStatusAssigned : SupportTicketStatus
    {
        private SupportTicketStatusAssigned(int supportTicketStatusID, string supportTicketStatusName, string supportTicketStatusDisplayName) : base(supportTicketStatusID, supportTicketStatusName, supportTicketStatusDisplayName) {}
        public static readonly SupportTicketStatusAssigned Instance = new SupportTicketStatusAssigned(2, @"Assigned", @"Assigned");
    }

    public partial class SupportTicketStatusClosed : SupportTicketStatus
    {
        private SupportTicketStatusClosed(int supportTicketStatusID, string supportTicketStatusName, string supportTicketStatusDisplayName) : base(supportTicketStatusID, supportTicketStatusName, supportTicketStatusDisplayName) {}
        public static readonly SupportTicketStatusClosed Instance = new SupportTicketStatusClosed(3, @"Closed", @"Closed");
    }
}