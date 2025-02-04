//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[WaterMeasurementSelfReportStatus]
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Qanat.Models.DataTransferObjects;


namespace Qanat.EFModels.Entities
{
    public abstract partial class WaterMeasurementSelfReportStatus : IHavePrimaryKey
    {
        public static readonly WaterMeasurementSelfReportStatusDraft Draft = WaterMeasurementSelfReportStatusDraft.Instance;
        public static readonly WaterMeasurementSelfReportStatusSubmitted Submitted = WaterMeasurementSelfReportStatusSubmitted.Instance;
        public static readonly WaterMeasurementSelfReportStatusApproved Approved = WaterMeasurementSelfReportStatusApproved.Instance;
        public static readonly WaterMeasurementSelfReportStatusReturned Returned = WaterMeasurementSelfReportStatusReturned.Instance;

        public static readonly List<WaterMeasurementSelfReportStatus> All;
        public static readonly List<WaterMeasurementSelfReportStatusSimpleDto> AllAsSimpleDto;
        public static readonly ReadOnlyDictionary<int, WaterMeasurementSelfReportStatus> AllLookupDictionary;
        public static readonly ReadOnlyDictionary<int, WaterMeasurementSelfReportStatusSimpleDto> AllAsSimpleDtoLookupDictionary;

        /// <summary>
        /// Static type constructor to coordinate static initialization order
        /// </summary>
        static WaterMeasurementSelfReportStatus()
        {
            All = new List<WaterMeasurementSelfReportStatus> { Draft, Submitted, Approved, Returned };
            AllAsSimpleDto = new List<WaterMeasurementSelfReportStatusSimpleDto> { Draft.AsSimpleDto(), Submitted.AsSimpleDto(), Approved.AsSimpleDto(), Returned.AsSimpleDto() };
            AllLookupDictionary = new ReadOnlyDictionary<int, WaterMeasurementSelfReportStatus>(All.ToDictionary(x => x.WaterMeasurementSelfReportStatusID));
            AllAsSimpleDtoLookupDictionary = new ReadOnlyDictionary<int, WaterMeasurementSelfReportStatusSimpleDto>(AllAsSimpleDto.ToDictionary(x => x.WaterMeasurementSelfReportStatusID));
        }

        /// <summary>
        /// Protected constructor only for use in instantiating the set of static lookup values that match database
        /// </summary>
        protected WaterMeasurementSelfReportStatus(int waterMeasurementSelfReportStatusID, string waterMeasurementSelfReportStatusName, string waterMeasurementSelfReportStatusDisplayName)
        {
            WaterMeasurementSelfReportStatusID = waterMeasurementSelfReportStatusID;
            WaterMeasurementSelfReportStatusName = waterMeasurementSelfReportStatusName;
            WaterMeasurementSelfReportStatusDisplayName = waterMeasurementSelfReportStatusDisplayName;
        }

        [Key]
        public int WaterMeasurementSelfReportStatusID { get; private set; }
        public string WaterMeasurementSelfReportStatusName { get; private set; }
        public string WaterMeasurementSelfReportStatusDisplayName { get; private set; }
        [NotMapped]
        public int PrimaryKey { get { return WaterMeasurementSelfReportStatusID; } }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public bool Equals(WaterMeasurementSelfReportStatus other)
        {
            if (other == null)
            {
                return false;
            }
            return other.WaterMeasurementSelfReportStatusID == WaterMeasurementSelfReportStatusID;
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override bool Equals(object obj)
        {
            return Equals(obj as WaterMeasurementSelfReportStatus);
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override int GetHashCode()
        {
            return WaterMeasurementSelfReportStatusID;
        }

        public static bool operator ==(WaterMeasurementSelfReportStatus left, WaterMeasurementSelfReportStatus right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(WaterMeasurementSelfReportStatus left, WaterMeasurementSelfReportStatus right)
        {
            return !Equals(left, right);
        }

        public WaterMeasurementSelfReportStatusEnum ToEnum => (WaterMeasurementSelfReportStatusEnum)GetHashCode();

        public static WaterMeasurementSelfReportStatus ToType(int enumValue)
        {
            return ToType((WaterMeasurementSelfReportStatusEnum)enumValue);
        }

        public static WaterMeasurementSelfReportStatus ToType(WaterMeasurementSelfReportStatusEnum enumValue)
        {
            switch (enumValue)
            {
                case WaterMeasurementSelfReportStatusEnum.Approved:
                    return Approved;
                case WaterMeasurementSelfReportStatusEnum.Draft:
                    return Draft;
                case WaterMeasurementSelfReportStatusEnum.Returned:
                    return Returned;
                case WaterMeasurementSelfReportStatusEnum.Submitted:
                    return Submitted;
                default:
                    throw new ArgumentException("Unable to map Enum: {enumValue}");
            }
        }
    }

    public enum WaterMeasurementSelfReportStatusEnum
    {
        Draft = 1,
        Submitted = 2,
        Approved = 3,
        Returned = 4
    }

    public partial class WaterMeasurementSelfReportStatusDraft : WaterMeasurementSelfReportStatus
    {
        private WaterMeasurementSelfReportStatusDraft(int waterMeasurementSelfReportStatusID, string waterMeasurementSelfReportStatusName, string waterMeasurementSelfReportStatusDisplayName) : base(waterMeasurementSelfReportStatusID, waterMeasurementSelfReportStatusName, waterMeasurementSelfReportStatusDisplayName) {}
        public static readonly WaterMeasurementSelfReportStatusDraft Instance = new WaterMeasurementSelfReportStatusDraft(1, @"Draft", @"Draft");
    }

    public partial class WaterMeasurementSelfReportStatusSubmitted : WaterMeasurementSelfReportStatus
    {
        private WaterMeasurementSelfReportStatusSubmitted(int waterMeasurementSelfReportStatusID, string waterMeasurementSelfReportStatusName, string waterMeasurementSelfReportStatusDisplayName) : base(waterMeasurementSelfReportStatusID, waterMeasurementSelfReportStatusName, waterMeasurementSelfReportStatusDisplayName) {}
        public static readonly WaterMeasurementSelfReportStatusSubmitted Instance = new WaterMeasurementSelfReportStatusSubmitted(2, @"Submitted", @"Submitted");
    }

    public partial class WaterMeasurementSelfReportStatusApproved : WaterMeasurementSelfReportStatus
    {
        private WaterMeasurementSelfReportStatusApproved(int waterMeasurementSelfReportStatusID, string waterMeasurementSelfReportStatusName, string waterMeasurementSelfReportStatusDisplayName) : base(waterMeasurementSelfReportStatusID, waterMeasurementSelfReportStatusName, waterMeasurementSelfReportStatusDisplayName) {}
        public static readonly WaterMeasurementSelfReportStatusApproved Instance = new WaterMeasurementSelfReportStatusApproved(3, @"Approved", @"Approved");
    }

    public partial class WaterMeasurementSelfReportStatusReturned : WaterMeasurementSelfReportStatus
    {
        private WaterMeasurementSelfReportStatusReturned(int waterMeasurementSelfReportStatusID, string waterMeasurementSelfReportStatusName, string waterMeasurementSelfReportStatusDisplayName) : base(waterMeasurementSelfReportStatusID, waterMeasurementSelfReportStatusName, waterMeasurementSelfReportStatusDisplayName) {}
        public static readonly WaterMeasurementSelfReportStatusReturned Instance = new WaterMeasurementSelfReportStatusReturned(4, @"Returned", @"Returned");
    }
}