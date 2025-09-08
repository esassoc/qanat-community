//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[GETActionStatus]
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Qanat.Models.DataTransferObjects;


namespace Qanat.EFModels.Entities
{
    public abstract partial class GETActionStatus : IHavePrimaryKey
    {
        public static readonly GETActionStatusCreated Created = GETActionStatusCreated.Instance;
        public static readonly GETActionStatusGETIntegrationFailure GETIntegrationFailure = GETActionStatusGETIntegrationFailure.Instance;
        public static readonly GETActionStatusCreatedInGET CreatedInGET = GETActionStatusCreatedInGET.Instance;
        public static readonly GETActionStatusQueued Queued = GETActionStatusQueued.Instance;
        public static readonly GETActionStatusProcessing Processing = GETActionStatusProcessing.Instance;
        public static readonly GETActionStatusComplete Complete = GETActionStatusComplete.Instance;
        public static readonly GETActionStatusSystemError SystemError = GETActionStatusSystemError.Instance;
        public static readonly GETActionStatusInvalidOutput InvalidOutput = GETActionStatusInvalidOutput.Instance;
        public static readonly GETActionStatusInvalidInput InvalidInput = GETActionStatusInvalidInput.Instance;
        public static readonly GETActionStatusHasDryCells HasDryCells = GETActionStatusHasDryCells.Instance;
        public static readonly GETActionStatusAnalysisFailed AnalysisFailed = GETActionStatusAnalysisFailed.Instance;
        public static readonly GETActionStatusAnalysisSuccess AnalysisSuccess = GETActionStatusAnalysisSuccess.Instance;
        public static readonly GETActionStatusProcesingInputs ProcesingInputs = GETActionStatusProcesingInputs.Instance;
        public static readonly GETActionStatusRunningAnalysis RunningAnalysis = GETActionStatusRunningAnalysis.Instance;

        public static readonly List<GETActionStatus> All;
        public static readonly List<GETActionStatusSimpleDto> AllAsSimpleDto;
        public static readonly ReadOnlyDictionary<int, GETActionStatus> AllLookupDictionary;
        public static readonly ReadOnlyDictionary<int, GETActionStatusSimpleDto> AllAsSimpleDtoLookupDictionary;

        /// <summary>
        /// Static type constructor to coordinate static initialization order
        /// </summary>
        static GETActionStatus()
        {
            All = new List<GETActionStatus> { Created, GETIntegrationFailure, CreatedInGET, Queued, Processing, Complete, SystemError, InvalidOutput, InvalidInput, HasDryCells, AnalysisFailed, AnalysisSuccess, ProcesingInputs, RunningAnalysis };
            AllAsSimpleDto = new List<GETActionStatusSimpleDto> { Created.AsSimpleDto(), GETIntegrationFailure.AsSimpleDto(), CreatedInGET.AsSimpleDto(), Queued.AsSimpleDto(), Processing.AsSimpleDto(), Complete.AsSimpleDto(), SystemError.AsSimpleDto(), InvalidOutput.AsSimpleDto(), InvalidInput.AsSimpleDto(), HasDryCells.AsSimpleDto(), AnalysisFailed.AsSimpleDto(), AnalysisSuccess.AsSimpleDto(), ProcesingInputs.AsSimpleDto(), RunningAnalysis.AsSimpleDto() };
            AllLookupDictionary = new ReadOnlyDictionary<int, GETActionStatus>(All.ToDictionary(x => x.GETActionStatusID));
            AllAsSimpleDtoLookupDictionary = new ReadOnlyDictionary<int, GETActionStatusSimpleDto>(AllAsSimpleDto.ToDictionary(x => x.GETActionStatusID));
        }

        /// <summary>
        /// Protected constructor only for use in instantiating the set of static lookup values that match database
        /// </summary>
        protected GETActionStatus(int gETActionStatusID, string gETActionStatusName, string gETActionStatusDisplayName, int? gETRunStatusID, bool isTerminal)
        {
            GETActionStatusID = gETActionStatusID;
            GETActionStatusName = gETActionStatusName;
            GETActionStatusDisplayName = gETActionStatusDisplayName;
            GETRunStatusID = gETRunStatusID;
            IsTerminal = isTerminal;
        }

        [Key]
        public int GETActionStatusID { get; private set; }
        public string GETActionStatusName { get; private set; }
        public string GETActionStatusDisplayName { get; private set; }
        public int? GETRunStatusID { get; private set; }
        public bool IsTerminal { get; private set; }
        [NotMapped]
        public int PrimaryKey { get { return GETActionStatusID; } }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public bool Equals(GETActionStatus other)
        {
            if (other == null)
            {
                return false;
            }
            return other.GETActionStatusID == GETActionStatusID;
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override bool Equals(object obj)
        {
            return Equals(obj as GETActionStatus);
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override int GetHashCode()
        {
            return GETActionStatusID;
        }

        public static bool operator ==(GETActionStatus left, GETActionStatus right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(GETActionStatus left, GETActionStatus right)
        {
            return !Equals(left, right);
        }

        public GETActionStatusEnum ToEnum => (GETActionStatusEnum)GetHashCode();

        public static GETActionStatus ToType(int enumValue)
        {
            return ToType((GETActionStatusEnum)enumValue);
        }

        public static GETActionStatus ToType(GETActionStatusEnum enumValue)
        {
            switch (enumValue)
            {
                case GETActionStatusEnum.AnalysisFailed:
                    return AnalysisFailed;
                case GETActionStatusEnum.AnalysisSuccess:
                    return AnalysisSuccess;
                case GETActionStatusEnum.Complete:
                    return Complete;
                case GETActionStatusEnum.Created:
                    return Created;
                case GETActionStatusEnum.CreatedInGET:
                    return CreatedInGET;
                case GETActionStatusEnum.GETIntegrationFailure:
                    return GETIntegrationFailure;
                case GETActionStatusEnum.HasDryCells:
                    return HasDryCells;
                case GETActionStatusEnum.InvalidInput:
                    return InvalidInput;
                case GETActionStatusEnum.InvalidOutput:
                    return InvalidOutput;
                case GETActionStatusEnum.ProcesingInputs:
                    return ProcesingInputs;
                case GETActionStatusEnum.Processing:
                    return Processing;
                case GETActionStatusEnum.Queued:
                    return Queued;
                case GETActionStatusEnum.RunningAnalysis:
                    return RunningAnalysis;
                case GETActionStatusEnum.SystemError:
                    return SystemError;
                default:
                    throw new ArgumentException("Unable to map Enum: {enumValue}");
            }
        }
    }

    public enum GETActionStatusEnum
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

    public partial class GETActionStatusCreated : GETActionStatus
    {
        private GETActionStatusCreated(int gETActionStatusID, string gETActionStatusName, string gETActionStatusDisplayName, int? gETRunStatusID, bool isTerminal) : base(gETActionStatusID, gETActionStatusName, gETActionStatusDisplayName, gETRunStatusID, isTerminal) {}
        public static readonly GETActionStatusCreated Instance = new GETActionStatusCreated(1, @"Created", @"Created", null, false);
    }

    public partial class GETActionStatusGETIntegrationFailure : GETActionStatus
    {
        private GETActionStatusGETIntegrationFailure(int gETActionStatusID, string gETActionStatusName, string gETActionStatusDisplayName, int? gETRunStatusID, bool isTerminal) : base(gETActionStatusID, gETActionStatusName, gETActionStatusDisplayName, gETRunStatusID, isTerminal) {}
        public static readonly GETActionStatusGETIntegrationFailure Instance = new GETActionStatusGETIntegrationFailure(2, @"GETIntegrationFailure", @"GET Integration Failure", null, false);
    }

    public partial class GETActionStatusCreatedInGET : GETActionStatus
    {
        private GETActionStatusCreatedInGET(int gETActionStatusID, string gETActionStatusName, string gETActionStatusDisplayName, int? gETRunStatusID, bool isTerminal) : base(gETActionStatusID, gETActionStatusName, gETActionStatusDisplayName, gETRunStatusID, isTerminal) {}
        public static readonly GETActionStatusCreatedInGET Instance = new GETActionStatusCreatedInGET(3, @"CreatedInGET", @"Created in GET", 0, false);
    }

    public partial class GETActionStatusQueued : GETActionStatus
    {
        private GETActionStatusQueued(int gETActionStatusID, string gETActionStatusName, string gETActionStatusDisplayName, int? gETRunStatusID, bool isTerminal) : base(gETActionStatusID, gETActionStatusName, gETActionStatusDisplayName, gETRunStatusID, isTerminal) {}
        public static readonly GETActionStatusQueued Instance = new GETActionStatusQueued(4, @"Queued", @"Queued", 1, false);
    }

    public partial class GETActionStatusProcessing : GETActionStatus
    {
        private GETActionStatusProcessing(int gETActionStatusID, string gETActionStatusName, string gETActionStatusDisplayName, int? gETRunStatusID, bool isTerminal) : base(gETActionStatusID, gETActionStatusName, gETActionStatusDisplayName, gETRunStatusID, isTerminal) {}
        public static readonly GETActionStatusProcessing Instance = new GETActionStatusProcessing(5, @"Processing", @"Processing", 2, false);
    }

    public partial class GETActionStatusComplete : GETActionStatus
    {
        private GETActionStatusComplete(int gETActionStatusID, string gETActionStatusName, string gETActionStatusDisplayName, int? gETRunStatusID, bool isTerminal) : base(gETActionStatusID, gETActionStatusName, gETActionStatusDisplayName, gETRunStatusID, isTerminal) {}
        public static readonly GETActionStatusComplete Instance = new GETActionStatusComplete(6, @"Complete", @"Complete", 3, true);
    }

    public partial class GETActionStatusSystemError : GETActionStatus
    {
        private GETActionStatusSystemError(int gETActionStatusID, string gETActionStatusName, string gETActionStatusDisplayName, int? gETRunStatusID, bool isTerminal) : base(gETActionStatusID, gETActionStatusName, gETActionStatusDisplayName, gETRunStatusID, isTerminal) {}
        public static readonly GETActionStatusSystemError Instance = new GETActionStatusSystemError(7, @"SystemError", @"System Error", 4, true);
    }

    public partial class GETActionStatusInvalidOutput : GETActionStatus
    {
        private GETActionStatusInvalidOutput(int gETActionStatusID, string gETActionStatusName, string gETActionStatusDisplayName, int? gETRunStatusID, bool isTerminal) : base(gETActionStatusID, gETActionStatusName, gETActionStatusDisplayName, gETRunStatusID, isTerminal) {}
        public static readonly GETActionStatusInvalidOutput Instance = new GETActionStatusInvalidOutput(8, @"InvalidOutput", @"Invalid Output", 5, true);
    }

    public partial class GETActionStatusInvalidInput : GETActionStatus
    {
        private GETActionStatusInvalidInput(int gETActionStatusID, string gETActionStatusName, string gETActionStatusDisplayName, int? gETRunStatusID, bool isTerminal) : base(gETActionStatusID, gETActionStatusName, gETActionStatusDisplayName, gETRunStatusID, isTerminal) {}
        public static readonly GETActionStatusInvalidInput Instance = new GETActionStatusInvalidInput(9, @"InvalidInput", @"Invalid Input", 6, true);
    }

    public partial class GETActionStatusHasDryCells : GETActionStatus
    {
        private GETActionStatusHasDryCells(int gETActionStatusID, string gETActionStatusName, string gETActionStatusDisplayName, int? gETRunStatusID, bool isTerminal) : base(gETActionStatusID, gETActionStatusName, gETActionStatusDisplayName, gETRunStatusID, isTerminal) {}
        public static readonly GETActionStatusHasDryCells Instance = new GETActionStatusHasDryCells(10, @"HasDryCells", @"Completed with Dry Cells", 7, true);
    }

    public partial class GETActionStatusAnalysisFailed : GETActionStatus
    {
        private GETActionStatusAnalysisFailed(int gETActionStatusID, string gETActionStatusName, string gETActionStatusDisplayName, int? gETRunStatusID, bool isTerminal) : base(gETActionStatusID, gETActionStatusName, gETActionStatusDisplayName, gETRunStatusID, isTerminal) {}
        public static readonly GETActionStatusAnalysisFailed Instance = new GETActionStatusAnalysisFailed(11, @"AnalysisFailed", @"Analysis Failed", 8, true);
    }

    public partial class GETActionStatusAnalysisSuccess : GETActionStatus
    {
        private GETActionStatusAnalysisSuccess(int gETActionStatusID, string gETActionStatusName, string gETActionStatusDisplayName, int? gETRunStatusID, bool isTerminal) : base(gETActionStatusID, gETActionStatusName, gETActionStatusDisplayName, gETRunStatusID, isTerminal) {}
        public static readonly GETActionStatusAnalysisSuccess Instance = new GETActionStatusAnalysisSuccess(12, @"AnalysisSuccess", @"Analysis Succeeded", 9, false);
    }

    public partial class GETActionStatusProcesingInputs : GETActionStatus
    {
        private GETActionStatusProcesingInputs(int gETActionStatusID, string gETActionStatusName, string gETActionStatusDisplayName, int? gETRunStatusID, bool isTerminal) : base(gETActionStatusID, gETActionStatusName, gETActionStatusDisplayName, gETRunStatusID, isTerminal) {}
        public static readonly GETActionStatusProcesingInputs Instance = new GETActionStatusProcesingInputs(13, @"ProcesingInputs", @"Processing Inputs", 10, false);
    }

    public partial class GETActionStatusRunningAnalysis : GETActionStatus
    {
        private GETActionStatusRunningAnalysis(int gETActionStatusID, string gETActionStatusName, string gETActionStatusDisplayName, int? gETRunStatusID, bool isTerminal) : base(gETActionStatusID, gETActionStatusName, gETActionStatusDisplayName, gETRunStatusID, isTerminal) {}
        public static readonly GETActionStatusRunningAnalysis Instance = new GETActionStatusRunningAnalysis(14, @"RunningAnalysis", @"Running Analysis", 11, false);
    }
}