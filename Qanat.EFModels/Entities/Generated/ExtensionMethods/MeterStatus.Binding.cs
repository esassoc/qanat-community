//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[MeterStatus]
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Qanat.Models.DataTransferObjects;


namespace Qanat.EFModels.Entities
{
    public abstract partial class MeterStatus : IHavePrimaryKey
    {
        public static readonly MeterStatusActive Active = MeterStatusActive.Instance;
        public static readonly MeterStatusBroken Broken = MeterStatusBroken.Instance;
        public static readonly MeterStatusRetired Retired = MeterStatusRetired.Instance;

        public static readonly List<MeterStatus> All;
        public static readonly List<MeterStatusSimpleDto> AllAsSimpleDto;
        public static readonly ReadOnlyDictionary<int, MeterStatus> AllLookupDictionary;
        public static readonly ReadOnlyDictionary<int, MeterStatusSimpleDto> AllAsSimpleDtoLookupDictionary;

        /// <summary>
        /// Static type constructor to coordinate static initialization order
        /// </summary>
        static MeterStatus()
        {
            All = new List<MeterStatus> { Active, Broken, Retired };
            AllAsSimpleDto = new List<MeterStatusSimpleDto> { Active.AsSimpleDto(), Broken.AsSimpleDto(), Retired.AsSimpleDto() };
            AllLookupDictionary = new ReadOnlyDictionary<int, MeterStatus>(All.ToDictionary(x => x.MeterStatusID));
            AllAsSimpleDtoLookupDictionary = new ReadOnlyDictionary<int, MeterStatusSimpleDto>(AllAsSimpleDto.ToDictionary(x => x.MeterStatusID));
        }

        /// <summary>
        /// Protected constructor only for use in instantiating the set of static lookup values that match database
        /// </summary>
        protected MeterStatus(int meterStatusID, string meterStatusName, string meterStatusDisplayName)
        {
            MeterStatusID = meterStatusID;
            MeterStatusName = meterStatusName;
            MeterStatusDisplayName = meterStatusDisplayName;
        }

        [Key]
        public int MeterStatusID { get; private set; }
        public string MeterStatusName { get; private set; }
        public string MeterStatusDisplayName { get; private set; }
        [NotMapped]
        public int PrimaryKey { get { return MeterStatusID; } }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public bool Equals(MeterStatus other)
        {
            if (other == null)
            {
                return false;
            }
            return other.MeterStatusID == MeterStatusID;
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override bool Equals(object obj)
        {
            return Equals(obj as MeterStatus);
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override int GetHashCode()
        {
            return MeterStatusID;
        }

        public static bool operator ==(MeterStatus left, MeterStatus right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(MeterStatus left, MeterStatus right)
        {
            return !Equals(left, right);
        }

        public MeterStatusEnum ToEnum => (MeterStatusEnum)GetHashCode();

        public static MeterStatus ToType(int enumValue)
        {
            return ToType((MeterStatusEnum)enumValue);
        }

        public static MeterStatus ToType(MeterStatusEnum enumValue)
        {
            switch (enumValue)
            {
                case MeterStatusEnum.Active:
                    return Active;
                case MeterStatusEnum.Broken:
                    return Broken;
                case MeterStatusEnum.Retired:
                    return Retired;
                default:
                    throw new ArgumentException("Unable to map Enum: {enumValue}");
            }
        }
    }

    public enum MeterStatusEnum
    {
        Active = 1,
        Broken = 2,
        Retired = 3
    }

    public partial class MeterStatusActive : MeterStatus
    {
        private MeterStatusActive(int meterStatusID, string meterStatusName, string meterStatusDisplayName) : base(meterStatusID, meterStatusName, meterStatusDisplayName) {}
        public static readonly MeterStatusActive Instance = new MeterStatusActive(1, @"Active", @"Active");
    }

    public partial class MeterStatusBroken : MeterStatus
    {
        private MeterStatusBroken(int meterStatusID, string meterStatusName, string meterStatusDisplayName) : base(meterStatusID, meterStatusName, meterStatusDisplayName) {}
        public static readonly MeterStatusBroken Instance = new MeterStatusBroken(2, @"Broken", @"Broken");
    }

    public partial class MeterStatusRetired : MeterStatus
    {
        private MeterStatusRetired(int meterStatusID, string meterStatusName, string meterStatusDisplayName) : base(meterStatusID, meterStatusName, meterStatusDisplayName) {}
        public static readonly MeterStatusRetired Instance = new MeterStatusRetired(3, @"Retired", @"Retired");
    }
}