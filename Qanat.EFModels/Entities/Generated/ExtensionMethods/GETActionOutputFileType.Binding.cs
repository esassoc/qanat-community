//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[GETActionOutputFileType]
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Qanat.Models.DataTransferObjects;


namespace Qanat.EFModels.Entities
{
    public abstract partial class GETActionOutputFileType : IHavePrimaryKey
    {
        public static readonly GETActionOutputFileTypeGroundWaterBudget GroundWaterBudget = GETActionOutputFileTypeGroundWaterBudget.Instance;
        public static readonly GETActionOutputFileTypeTimeSeriesData TimeSeriesData = GETActionOutputFileTypeTimeSeriesData.Instance;
        public static readonly GETActionOutputFileTypeWaterBudget WaterBudget = GETActionOutputFileTypeWaterBudget.Instance;
        public static readonly GETActionOutputFileTypePointsofInterest PointsofInterest = GETActionOutputFileTypePointsofInterest.Instance;

        public static readonly List<GETActionOutputFileType> All;
        public static readonly List<GETActionOutputFileTypeSimpleDto> AllAsSimpleDto;
        public static readonly ReadOnlyDictionary<int, GETActionOutputFileType> AllLookupDictionary;
        public static readonly ReadOnlyDictionary<int, GETActionOutputFileTypeSimpleDto> AllAsSimpleDtoLookupDictionary;

        /// <summary>
        /// Static type constructor to coordinate static initialization order
        /// </summary>
        static GETActionOutputFileType()
        {
            All = new List<GETActionOutputFileType> { GroundWaterBudget, TimeSeriesData, WaterBudget, PointsofInterest };
            AllAsSimpleDto = new List<GETActionOutputFileTypeSimpleDto> { GroundWaterBudget.AsSimpleDto(), TimeSeriesData.AsSimpleDto(), WaterBudget.AsSimpleDto(), PointsofInterest.AsSimpleDto() };
            AllLookupDictionary = new ReadOnlyDictionary<int, GETActionOutputFileType>(All.ToDictionary(x => x.GETActionOutputFileTypeID));
            AllAsSimpleDtoLookupDictionary = new ReadOnlyDictionary<int, GETActionOutputFileTypeSimpleDto>(AllAsSimpleDto.ToDictionary(x => x.GETActionOutputFileTypeID));
        }

        /// <summary>
        /// Protected constructor only for use in instantiating the set of static lookup values that match database
        /// </summary>
        protected GETActionOutputFileType(int gETActionOutputFileTypeID, string gETActionOutputFileTypeName, string gETActionOutputFileTypeExtension)
        {
            GETActionOutputFileTypeID = gETActionOutputFileTypeID;
            GETActionOutputFileTypeName = gETActionOutputFileTypeName;
            GETActionOutputFileTypeExtension = gETActionOutputFileTypeExtension;
        }

        [Key]
        public int GETActionOutputFileTypeID { get; private set; }
        public string GETActionOutputFileTypeName { get; private set; }
        public string GETActionOutputFileTypeExtension { get; private set; }
        [NotMapped]
        public int PrimaryKey { get { return GETActionOutputFileTypeID; } }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public bool Equals(GETActionOutputFileType other)
        {
            if (other == null)
            {
                return false;
            }
            return other.GETActionOutputFileTypeID == GETActionOutputFileTypeID;
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override bool Equals(object obj)
        {
            return Equals(obj as GETActionOutputFileType);
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override int GetHashCode()
        {
            return GETActionOutputFileTypeID;
        }

        public static bool operator ==(GETActionOutputFileType left, GETActionOutputFileType right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(GETActionOutputFileType left, GETActionOutputFileType right)
        {
            return !Equals(left, right);
        }

        public GETActionOutputFileTypeEnum ToEnum => (GETActionOutputFileTypeEnum)GetHashCode();

        public static GETActionOutputFileType ToType(int enumValue)
        {
            return ToType((GETActionOutputFileTypeEnum)enumValue);
        }

        public static GETActionOutputFileType ToType(GETActionOutputFileTypeEnum enumValue)
        {
            switch (enumValue)
            {
                case GETActionOutputFileTypeEnum.GroundWaterBudget:
                    return GroundWaterBudget;
                case GETActionOutputFileTypeEnum.PointsofInterest:
                    return PointsofInterest;
                case GETActionOutputFileTypeEnum.TimeSeriesData:
                    return TimeSeriesData;
                case GETActionOutputFileTypeEnum.WaterBudget:
                    return WaterBudget;
                default:
                    throw new ArgumentException("Unable to map Enum: {enumValue}");
            }
        }
    }

    public enum GETActionOutputFileTypeEnum
    {
        GroundWaterBudget = 1,
        TimeSeriesData = 2,
        WaterBudget = 3,
        PointsofInterest = 4
    }

    public partial class GETActionOutputFileTypeGroundWaterBudget : GETActionOutputFileType
    {
        private GETActionOutputFileTypeGroundWaterBudget(int gETActionOutputFileTypeID, string gETActionOutputFileTypeName, string gETActionOutputFileTypeExtension) : base(gETActionOutputFileTypeID, gETActionOutputFileTypeName, gETActionOutputFileTypeExtension) {}
        public static readonly GETActionOutputFileTypeGroundWaterBudget Instance = new GETActionOutputFileTypeGroundWaterBudget(1, @"GroundWaterBudget", @".json");
    }

    public partial class GETActionOutputFileTypeTimeSeriesData : GETActionOutputFileType
    {
        private GETActionOutputFileTypeTimeSeriesData(int gETActionOutputFileTypeID, string gETActionOutputFileTypeName, string gETActionOutputFileTypeExtension) : base(gETActionOutputFileTypeID, gETActionOutputFileTypeName, gETActionOutputFileTypeExtension) {}
        public static readonly GETActionOutputFileTypeTimeSeriesData Instance = new GETActionOutputFileTypeTimeSeriesData(2, @"TimeSeriesData", @".json");
    }

    public partial class GETActionOutputFileTypeWaterBudget : GETActionOutputFileType
    {
        private GETActionOutputFileTypeWaterBudget(int gETActionOutputFileTypeID, string gETActionOutputFileTypeName, string gETActionOutputFileTypeExtension) : base(gETActionOutputFileTypeID, gETActionOutputFileTypeName, gETActionOutputFileTypeExtension) {}
        public static readonly GETActionOutputFileTypeWaterBudget Instance = new GETActionOutputFileTypeWaterBudget(3, @"Water Budget", @".json");
    }

    public partial class GETActionOutputFileTypePointsofInterest : GETActionOutputFileType
    {
        private GETActionOutputFileTypePointsofInterest(int gETActionOutputFileTypeID, string gETActionOutputFileTypeName, string gETActionOutputFileTypeExtension) : base(gETActionOutputFileTypeID, gETActionOutputFileTypeName, gETActionOutputFileTypeExtension) {}
        public static readonly GETActionOutputFileTypePointsofInterest Instance = new GETActionOutputFileTypePointsofInterest(4, @"Points of Interest", @".json");
    }
}