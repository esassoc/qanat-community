//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ScenarioRunStatus]
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Qanat.Models.DataTransferObjects;


namespace Qanat.EFModels.Entities
{
    public abstract partial class ScenarioRunStatus : IHavePrimaryKey
    {
        public static readonly ScenarioRunStatusCreated Created = ScenarioRunStatusCreated.Instance;
        public static readonly ScenarioRunStatusGETIntegrationFailure GETIntegrationFailure = ScenarioRunStatusGETIntegrationFailure.Instance;
        public static readonly ScenarioRunStatusCreatedInGET CreatedInGET = ScenarioRunStatusCreatedInGET.Instance;
        public static readonly ScenarioRunStatusQueued Queued = ScenarioRunStatusQueued.Instance;
        public static readonly ScenarioRunStatusProcessing Processing = ScenarioRunStatusProcessing.Instance;
        public static readonly ScenarioRunStatusComplete Complete = ScenarioRunStatusComplete.Instance;
        public static readonly ScenarioRunStatusSystemError SystemError = ScenarioRunStatusSystemError.Instance;
        public static readonly ScenarioRunStatusInvalidOutput InvalidOutput = ScenarioRunStatusInvalidOutput.Instance;
        public static readonly ScenarioRunStatusInvalidInput InvalidInput = ScenarioRunStatusInvalidInput.Instance;
        public static readonly ScenarioRunStatusHasDryCells HasDryCells = ScenarioRunStatusHasDryCells.Instance;
        public static readonly ScenarioRunStatusAnalysisFailed AnalysisFailed = ScenarioRunStatusAnalysisFailed.Instance;
        public static readonly ScenarioRunStatusAnalysisSuccess AnalysisSuccess = ScenarioRunStatusAnalysisSuccess.Instance;
        public static readonly ScenarioRunStatusProcesingInputs ProcesingInputs = ScenarioRunStatusProcesingInputs.Instance;
        public static readonly ScenarioRunStatusRunningAnalysis RunningAnalysis = ScenarioRunStatusRunningAnalysis.Instance;

        public static readonly List<ScenarioRunStatus> All;
        public static readonly ReadOnlyDictionary<int, ScenarioRunStatus> AllLookupDictionary;

        /// <summary>
        /// Static type constructor to coordinate static initialization order
        /// </summary>
        static ScenarioRunStatus()
        {
            All = new List<ScenarioRunStatus> { Created, GETIntegrationFailure, CreatedInGET, Queued, Processing, Complete, SystemError, InvalidOutput, InvalidInput, HasDryCells, AnalysisFailed, AnalysisSuccess, ProcesingInputs, RunningAnalysis };
            AllLookupDictionary = new ReadOnlyDictionary<int, ScenarioRunStatus>(All.ToDictionary(x => x.ScenarioRunStatusID));
        }

        /// <summary>
        /// Protected constructor only for use in instantiating the set of static lookup values that match database
        /// </summary>
        protected ScenarioRunStatus(int scenarioRunStatusID, string scenarioRunStatusName, string scenarioRunStatusDisplayName, int? gETRunStatusID, bool isTerminal)
        {
            ScenarioRunStatusID = scenarioRunStatusID;
            ScenarioRunStatusName = scenarioRunStatusName;
            ScenarioRunStatusDisplayName = scenarioRunStatusDisplayName;
            GETRunStatusID = gETRunStatusID;
            IsTerminal = isTerminal;
        }

        [Key]
        public int ScenarioRunStatusID { get; private set; }
        public string ScenarioRunStatusName { get; private set; }
        public string ScenarioRunStatusDisplayName { get; private set; }
        public int? GETRunStatusID { get; private set; }
        public bool IsTerminal { get; private set; }
        [NotMapped]
        public int PrimaryKey { get { return ScenarioRunStatusID; } }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public bool Equals(ScenarioRunStatus other)
        {
            if (other == null)
            {
                return false;
            }
            return other.ScenarioRunStatusID == ScenarioRunStatusID;
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override bool Equals(object obj)
        {
            return Equals(obj as ScenarioRunStatus);
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override int GetHashCode()
        {
            return ScenarioRunStatusID;
        }

        public static bool operator ==(ScenarioRunStatus left, ScenarioRunStatus right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ScenarioRunStatus left, ScenarioRunStatus right)
        {
            return !Equals(left, right);
        }

        public ScenarioRunStatusEnum ToEnum => (ScenarioRunStatusEnum)GetHashCode();

        public static ScenarioRunStatus ToType(int enumValue)
        {
            return ToType((ScenarioRunStatusEnum)enumValue);
        }

        public static ScenarioRunStatus ToType(ScenarioRunStatusEnum enumValue)
        {
            switch (enumValue)
            {
                case ScenarioRunStatusEnum.AnalysisFailed:
                    return AnalysisFailed;
                case ScenarioRunStatusEnum.AnalysisSuccess:
                    return AnalysisSuccess;
                case ScenarioRunStatusEnum.Complete:
                    return Complete;
                case ScenarioRunStatusEnum.Created:
                    return Created;
                case ScenarioRunStatusEnum.CreatedInGET:
                    return CreatedInGET;
                case ScenarioRunStatusEnum.GETIntegrationFailure:
                    return GETIntegrationFailure;
                case ScenarioRunStatusEnum.HasDryCells:
                    return HasDryCells;
                case ScenarioRunStatusEnum.InvalidInput:
                    return InvalidInput;
                case ScenarioRunStatusEnum.InvalidOutput:
                    return InvalidOutput;
                case ScenarioRunStatusEnum.ProcesingInputs:
                    return ProcesingInputs;
                case ScenarioRunStatusEnum.Processing:
                    return Processing;
                case ScenarioRunStatusEnum.Queued:
                    return Queued;
                case ScenarioRunStatusEnum.RunningAnalysis:
                    return RunningAnalysis;
                case ScenarioRunStatusEnum.SystemError:
                    return SystemError;
                default:
                    throw new ArgumentException("Unable to map Enum: {enumValue}");
            }
        }
    }

    public enum ScenarioRunStatusEnum
    {
        Created = 1,
        GETIntegrationFailure = 2,
        CreatedInGET = 3,
        Queued = 4,
        Processing = 5,
        Complete = 6,
        SystemError = 7,
        InvalidOutput = 8,
        InvalidInput = 9,
        HasDryCells = 10,
        AnalysisFailed = 11,
        AnalysisSuccess = 12,
        ProcesingInputs = 13,
        RunningAnalysis = 14
    }

    public partial class ScenarioRunStatusCreated : ScenarioRunStatus
    {
        private ScenarioRunStatusCreated(int scenarioRunStatusID, string scenarioRunStatusName, string scenarioRunStatusDisplayName, int? gETRunStatusID, bool isTerminal) : base(scenarioRunStatusID, scenarioRunStatusName, scenarioRunStatusDisplayName, gETRunStatusID, isTerminal) {}
        public static readonly ScenarioRunStatusCreated Instance = new ScenarioRunStatusCreated(1, @"Created", @"Created", null, false);
    }

    public partial class ScenarioRunStatusGETIntegrationFailure : ScenarioRunStatus
    {
        private ScenarioRunStatusGETIntegrationFailure(int scenarioRunStatusID, string scenarioRunStatusName, string scenarioRunStatusDisplayName, int? gETRunStatusID, bool isTerminal) : base(scenarioRunStatusID, scenarioRunStatusName, scenarioRunStatusDisplayName, gETRunStatusID, isTerminal) {}
        public static readonly ScenarioRunStatusGETIntegrationFailure Instance = new ScenarioRunStatusGETIntegrationFailure(2, @"GETIntegrationFailure", @"GET Integration Failure", null, false);
    }

    public partial class ScenarioRunStatusCreatedInGET : ScenarioRunStatus
    {
        private ScenarioRunStatusCreatedInGET(int scenarioRunStatusID, string scenarioRunStatusName, string scenarioRunStatusDisplayName, int? gETRunStatusID, bool isTerminal) : base(scenarioRunStatusID, scenarioRunStatusName, scenarioRunStatusDisplayName, gETRunStatusID, isTerminal) {}
        public static readonly ScenarioRunStatusCreatedInGET Instance = new ScenarioRunStatusCreatedInGET(3, @"CreatedInGET", @"Created in GET", 0, false);
    }

    public partial class ScenarioRunStatusQueued : ScenarioRunStatus
    {
        private ScenarioRunStatusQueued(int scenarioRunStatusID, string scenarioRunStatusName, string scenarioRunStatusDisplayName, int? gETRunStatusID, bool isTerminal) : base(scenarioRunStatusID, scenarioRunStatusName, scenarioRunStatusDisplayName, gETRunStatusID, isTerminal) {}
        public static readonly ScenarioRunStatusQueued Instance = new ScenarioRunStatusQueued(4, @"Queued", @"Queued", 1, false);
    }

    public partial class ScenarioRunStatusProcessing : ScenarioRunStatus
    {
        private ScenarioRunStatusProcessing(int scenarioRunStatusID, string scenarioRunStatusName, string scenarioRunStatusDisplayName, int? gETRunStatusID, bool isTerminal) : base(scenarioRunStatusID, scenarioRunStatusName, scenarioRunStatusDisplayName, gETRunStatusID, isTerminal) {}
        public static readonly ScenarioRunStatusProcessing Instance = new ScenarioRunStatusProcessing(5, @"Processing", @"Processing", 2, false);
    }

    public partial class ScenarioRunStatusComplete : ScenarioRunStatus
    {
        private ScenarioRunStatusComplete(int scenarioRunStatusID, string scenarioRunStatusName, string scenarioRunStatusDisplayName, int? gETRunStatusID, bool isTerminal) : base(scenarioRunStatusID, scenarioRunStatusName, scenarioRunStatusDisplayName, gETRunStatusID, isTerminal) {}
        public static readonly ScenarioRunStatusComplete Instance = new ScenarioRunStatusComplete(6, @"Complete", @"Complete", 3, true);
    }

    public partial class ScenarioRunStatusSystemError : ScenarioRunStatus
    {
        private ScenarioRunStatusSystemError(int scenarioRunStatusID, string scenarioRunStatusName, string scenarioRunStatusDisplayName, int? gETRunStatusID, bool isTerminal) : base(scenarioRunStatusID, scenarioRunStatusName, scenarioRunStatusDisplayName, gETRunStatusID, isTerminal) {}
        public static readonly ScenarioRunStatusSystemError Instance = new ScenarioRunStatusSystemError(7, @"SystemError", @"System Error", 4, true);
    }

    public partial class ScenarioRunStatusInvalidOutput : ScenarioRunStatus
    {
        private ScenarioRunStatusInvalidOutput(int scenarioRunStatusID, string scenarioRunStatusName, string scenarioRunStatusDisplayName, int? gETRunStatusID, bool isTerminal) : base(scenarioRunStatusID, scenarioRunStatusName, scenarioRunStatusDisplayName, gETRunStatusID, isTerminal) {}
        public static readonly ScenarioRunStatusInvalidOutput Instance = new ScenarioRunStatusInvalidOutput(8, @"InvalidOutput", @"Invalid Output", 5, true);
    }

    public partial class ScenarioRunStatusInvalidInput : ScenarioRunStatus
    {
        private ScenarioRunStatusInvalidInput(int scenarioRunStatusID, string scenarioRunStatusName, string scenarioRunStatusDisplayName, int? gETRunStatusID, bool isTerminal) : base(scenarioRunStatusID, scenarioRunStatusName, scenarioRunStatusDisplayName, gETRunStatusID, isTerminal) {}
        public static readonly ScenarioRunStatusInvalidInput Instance = new ScenarioRunStatusInvalidInput(9, @"InvalidInput", @"Invalid Input", 6, true);
    }

    public partial class ScenarioRunStatusHasDryCells : ScenarioRunStatus
    {
        private ScenarioRunStatusHasDryCells(int scenarioRunStatusID, string scenarioRunStatusName, string scenarioRunStatusDisplayName, int? gETRunStatusID, bool isTerminal) : base(scenarioRunStatusID, scenarioRunStatusName, scenarioRunStatusDisplayName, gETRunStatusID, isTerminal) {}
        public static readonly ScenarioRunStatusHasDryCells Instance = new ScenarioRunStatusHasDryCells(10, @"HasDryCells", @"Completed with Dry Cells", 7, true);
    }

    public partial class ScenarioRunStatusAnalysisFailed : ScenarioRunStatus
    {
        private ScenarioRunStatusAnalysisFailed(int scenarioRunStatusID, string scenarioRunStatusName, string scenarioRunStatusDisplayName, int? gETRunStatusID, bool isTerminal) : base(scenarioRunStatusID, scenarioRunStatusName, scenarioRunStatusDisplayName, gETRunStatusID, isTerminal) {}
        public static readonly ScenarioRunStatusAnalysisFailed Instance = new ScenarioRunStatusAnalysisFailed(11, @"AnalysisFailed", @"Analysis Failed", 8, true);
    }

    public partial class ScenarioRunStatusAnalysisSuccess : ScenarioRunStatus
    {
        private ScenarioRunStatusAnalysisSuccess(int scenarioRunStatusID, string scenarioRunStatusName, string scenarioRunStatusDisplayName, int? gETRunStatusID, bool isTerminal) : base(scenarioRunStatusID, scenarioRunStatusName, scenarioRunStatusDisplayName, gETRunStatusID, isTerminal) {}
        public static readonly ScenarioRunStatusAnalysisSuccess Instance = new ScenarioRunStatusAnalysisSuccess(12, @"AnalysisSuccess", @"Analysis Succeeded", 9, false);
    }

    public partial class ScenarioRunStatusProcesingInputs : ScenarioRunStatus
    {
        private ScenarioRunStatusProcesingInputs(int scenarioRunStatusID, string scenarioRunStatusName, string scenarioRunStatusDisplayName, int? gETRunStatusID, bool isTerminal) : base(scenarioRunStatusID, scenarioRunStatusName, scenarioRunStatusDisplayName, gETRunStatusID, isTerminal) {}
        public static readonly ScenarioRunStatusProcesingInputs Instance = new ScenarioRunStatusProcesingInputs(13, @"ProcesingInputs", @"Processing Inputs", 10, false);
    }

    public partial class ScenarioRunStatusRunningAnalysis : ScenarioRunStatus
    {
        private ScenarioRunStatusRunningAnalysis(int scenarioRunStatusID, string scenarioRunStatusName, string scenarioRunStatusDisplayName, int? gETRunStatusID, bool isTerminal) : base(scenarioRunStatusID, scenarioRunStatusName, scenarioRunStatusDisplayName, gETRunStatusID, isTerminal) {}
        public static readonly ScenarioRunStatusRunningAnalysis Instance = new ScenarioRunStatusRunningAnalysis(14, @"RunningAnalysis", @"Running Analysis", 11, false);
    }
}