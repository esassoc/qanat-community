//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ScenarioRunOutputFileType]
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Qanat.Models.DataTransferObjects;


namespace Qanat.EFModels.Entities
{
    public abstract partial class ScenarioRunOutputFileType : IHavePrimaryKey
    {
        public static readonly ScenarioRunOutputFileTypeGroundWaterBudget GroundWaterBudget = ScenarioRunOutputFileTypeGroundWaterBudget.Instance;
        public static readonly ScenarioRunOutputFileTypeTimeSeriesData TimeSeriesData = ScenarioRunOutputFileTypeTimeSeriesData.Instance;
        public static readonly ScenarioRunOutputFileTypeWaterBudget WaterBudget = ScenarioRunOutputFileTypeWaterBudget.Instance;
        public static readonly ScenarioRunOutputFileTypePointsofInterest PointsofInterest = ScenarioRunOutputFileTypePointsofInterest.Instance;

        public static readonly List<ScenarioRunOutputFileType> All;
        public static readonly List<ScenarioRunOutputFileTypeSimpleDto> AllAsSimpleDto;
        public static readonly ReadOnlyDictionary<int, ScenarioRunOutputFileType> AllLookupDictionary;
        public static readonly ReadOnlyDictionary<int, ScenarioRunOutputFileTypeSimpleDto> AllAsSimpleDtoLookupDictionary;

        /// <summary>
        /// Static type constructor to coordinate static initialization order
        /// </summary>
        static ScenarioRunOutputFileType()
        {
            All = new List<ScenarioRunOutputFileType> { GroundWaterBudget, TimeSeriesData, WaterBudget, PointsofInterest };
            AllAsSimpleDto = new List<ScenarioRunOutputFileTypeSimpleDto> { GroundWaterBudget.AsSimpleDto(), TimeSeriesData.AsSimpleDto(), WaterBudget.AsSimpleDto(), PointsofInterest.AsSimpleDto() };
            AllLookupDictionary = new ReadOnlyDictionary<int, ScenarioRunOutputFileType>(All.ToDictionary(x => x.ScenarioRunOutputFileTypeID));
            AllAsSimpleDtoLookupDictionary = new ReadOnlyDictionary<int, ScenarioRunOutputFileTypeSimpleDto>(AllAsSimpleDto.ToDictionary(x => x.ScenarioRunOutputFileTypeID));
        }

        /// <summary>
        /// Protected constructor only for use in instantiating the set of static lookup values that match database
        /// </summary>
        protected ScenarioRunOutputFileType(int scenarioRunOutputFileTypeID, string scenarioRunOutputFileTypeName, string scenarioRunOutputFileTypeExtension)
        {
            ScenarioRunOutputFileTypeID = scenarioRunOutputFileTypeID;
            ScenarioRunOutputFileTypeName = scenarioRunOutputFileTypeName;
            ScenarioRunOutputFileTypeExtension = scenarioRunOutputFileTypeExtension;
        }

        [Key]
        public int ScenarioRunOutputFileTypeID { get; private set; }
        public string ScenarioRunOutputFileTypeName { get; private set; }
        public string ScenarioRunOutputFileTypeExtension { get; private set; }
        [NotMapped]
        public int PrimaryKey { get { return ScenarioRunOutputFileTypeID; } }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public bool Equals(ScenarioRunOutputFileType other)
        {
            if (other == null)
            {
                return false;
            }
            return other.ScenarioRunOutputFileTypeID == ScenarioRunOutputFileTypeID;
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override bool Equals(object obj)
        {
            return Equals(obj as ScenarioRunOutputFileType);
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override int GetHashCode()
        {
            return ScenarioRunOutputFileTypeID;
        }

        public static bool operator ==(ScenarioRunOutputFileType left, ScenarioRunOutputFileType right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ScenarioRunOutputFileType left, ScenarioRunOutputFileType right)
        {
            return !Equals(left, right);
        }

        public ScenarioRunOutputFileTypeEnum ToEnum => (ScenarioRunOutputFileTypeEnum)GetHashCode();

        public static ScenarioRunOutputFileType ToType(int enumValue)
        {
            return ToType((ScenarioRunOutputFileTypeEnum)enumValue);
        }

        public static ScenarioRunOutputFileType ToType(ScenarioRunOutputFileTypeEnum enumValue)
        {
            switch (enumValue)
            {
                case ScenarioRunOutputFileTypeEnum.GroundWaterBudget:
                    return GroundWaterBudget;
                case ScenarioRunOutputFileTypeEnum.PointsofInterest:
                    return PointsofInterest;
                case ScenarioRunOutputFileTypeEnum.TimeSeriesData:
                    return TimeSeriesData;
                case ScenarioRunOutputFileTypeEnum.WaterBudget:
                    return WaterBudget;
                default:
                    throw new ArgumentException("Unable to map Enum: {enumValue}");
            }
        }
    }

    public enum ScenarioRunOutputFileTypeEnum
    {
        GroundWaterBudget = 1,
        TimeSeriesData = 2,
        WaterBudget = 3,
        PointsofInterest = 4
    }

    public partial class ScenarioRunOutputFileTypeGroundWaterBudget : ScenarioRunOutputFileType
    {
        private ScenarioRunOutputFileTypeGroundWaterBudget(int scenarioRunOutputFileTypeID, string scenarioRunOutputFileTypeName, string scenarioRunOutputFileTypeExtension) : base(scenarioRunOutputFileTypeID, scenarioRunOutputFileTypeName, scenarioRunOutputFileTypeExtension) {}
        public static readonly ScenarioRunOutputFileTypeGroundWaterBudget Instance = new ScenarioRunOutputFileTypeGroundWaterBudget(1, @"GroundWaterBudget", @".json");
    }

    public partial class ScenarioRunOutputFileTypeTimeSeriesData : ScenarioRunOutputFileType
    {
        private ScenarioRunOutputFileTypeTimeSeriesData(int scenarioRunOutputFileTypeID, string scenarioRunOutputFileTypeName, string scenarioRunOutputFileTypeExtension) : base(scenarioRunOutputFileTypeID, scenarioRunOutputFileTypeName, scenarioRunOutputFileTypeExtension) {}
        public static readonly ScenarioRunOutputFileTypeTimeSeriesData Instance = new ScenarioRunOutputFileTypeTimeSeriesData(2, @"TimeSeriesData", @".json");
    }

    public partial class ScenarioRunOutputFileTypeWaterBudget : ScenarioRunOutputFileType
    {
        private ScenarioRunOutputFileTypeWaterBudget(int scenarioRunOutputFileTypeID, string scenarioRunOutputFileTypeName, string scenarioRunOutputFileTypeExtension) : base(scenarioRunOutputFileTypeID, scenarioRunOutputFileTypeName, scenarioRunOutputFileTypeExtension) {}
        public static readonly ScenarioRunOutputFileTypeWaterBudget Instance = new ScenarioRunOutputFileTypeWaterBudget(3, @"Water Budget", @".json");
    }

    public partial class ScenarioRunOutputFileTypePointsofInterest : ScenarioRunOutputFileType
    {
        private ScenarioRunOutputFileTypePointsofInterest(int scenarioRunOutputFileTypeID, string scenarioRunOutputFileTypeName, string scenarioRunOutputFileTypeExtension) : base(scenarioRunOutputFileTypeID, scenarioRunOutputFileTypeName, scenarioRunOutputFileTypeExtension) {}
        public static readonly ScenarioRunOutputFileTypePointsofInterest Instance = new ScenarioRunOutputFileTypePointsofInterest(4, @"Points of Interest", @".json");
    }
}