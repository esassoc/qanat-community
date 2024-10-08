//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[WellStatus]
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Qanat.Models.DataTransferObjects;


namespace Qanat.EFModels.Entities
{
    public abstract partial class WellStatus : IHavePrimaryKey
    {
        public static readonly WellStatusOperational Operational = WellStatusOperational.Instance;
        public static readonly WellStatusNonOperational NonOperational = WellStatusNonOperational.Instance;

        public static readonly List<WellStatus> All;
        public static readonly List<WellStatusSimpleDto> AllAsSimpleDto;
        public static readonly ReadOnlyDictionary<int, WellStatus> AllLookupDictionary;
        public static readonly ReadOnlyDictionary<int, WellStatusSimpleDto> AllAsSimpleDtoLookupDictionary;

        /// <summary>
        /// Static type constructor to coordinate static initialization order
        /// </summary>
        static WellStatus()
        {
            All = new List<WellStatus> { Operational, NonOperational };
            AllAsSimpleDto = new List<WellStatusSimpleDto> { Operational.AsSimpleDto(), NonOperational.AsSimpleDto() };
            AllLookupDictionary = new ReadOnlyDictionary<int, WellStatus>(All.ToDictionary(x => x.WellStatusID));
            AllAsSimpleDtoLookupDictionary = new ReadOnlyDictionary<int, WellStatusSimpleDto>(AllAsSimpleDto.ToDictionary(x => x.WellStatusID));
        }

        /// <summary>
        /// Protected constructor only for use in instantiating the set of static lookup values that match database
        /// </summary>
        protected WellStatus(int wellStatusID, string wellStatusName, string wellStatusDisplayName)
        {
            WellStatusID = wellStatusID;
            WellStatusName = wellStatusName;
            WellStatusDisplayName = wellStatusDisplayName;
        }

        [Key]
        public int WellStatusID { get; private set; }
        public string WellStatusName { get; private set; }
        public string WellStatusDisplayName { get; private set; }
        [NotMapped]
        public int PrimaryKey { get { return WellStatusID; } }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public bool Equals(WellStatus other)
        {
            if (other == null)
            {
                return false;
            }
            return other.WellStatusID == WellStatusID;
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override bool Equals(object obj)
        {
            return Equals(obj as WellStatus);
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override int GetHashCode()
        {
            return WellStatusID;
        }

        public static bool operator ==(WellStatus left, WellStatus right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(WellStatus left, WellStatus right)
        {
            return !Equals(left, right);
        }

        public WellStatusEnum ToEnum => (WellStatusEnum)GetHashCode();

        public static WellStatus ToType(int enumValue)
        {
            return ToType((WellStatusEnum)enumValue);
        }

        public static WellStatus ToType(WellStatusEnum enumValue)
        {
            switch (enumValue)
            {
                case WellStatusEnum.NonOperational:
                    return NonOperational;
                case WellStatusEnum.Operational:
                    return Operational;
                default:
                    throw new ArgumentException("Unable to map Enum: {enumValue}");
            }
        }
    }

    public enum WellStatusEnum
    {
        Operational = 1,
        NonOperational = 2
    }

    public partial class WellStatusOperational : WellStatus
    {
        private WellStatusOperational(int wellStatusID, string wellStatusName, string wellStatusDisplayName) : base(wellStatusID, wellStatusName, wellStatusDisplayName) {}
        public static readonly WellStatusOperational Instance = new WellStatusOperational(1, @"Operational", @"Operational");
    }

    public partial class WellStatusNonOperational : WellStatus
    {
        private WellStatusNonOperational(int wellStatusID, string wellStatusName, string wellStatusDisplayName) : base(wellStatusID, wellStatusName, wellStatusDisplayName) {}
        public static readonly WellStatusNonOperational Instance = new WellStatusNonOperational(2, @"NonOperational", @"Non-Operational");
    }
}