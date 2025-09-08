//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[OpenETRasterCalculationResultType]
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Qanat.Models.DataTransferObjects;


namespace Qanat.EFModels.Entities
{
    public abstract partial class OpenETRasterCalculationResultType : IHavePrimaryKey
    {
        public static readonly OpenETRasterCalculationResultTypeNotStarted NotStarted = OpenETRasterCalculationResultTypeNotStarted.Instance;
        public static readonly OpenETRasterCalculationResultTypeInProgress InProgress = OpenETRasterCalculationResultTypeInProgress.Instance;
        public static readonly OpenETRasterCalculationResultTypeSucceeded Succeeded = OpenETRasterCalculationResultTypeSucceeded.Instance;
        public static readonly OpenETRasterCalculationResultTypeFailed Failed = OpenETRasterCalculationResultTypeFailed.Instance;

        public static readonly List<OpenETRasterCalculationResultType> All;
        public static readonly ReadOnlyDictionary<int, OpenETRasterCalculationResultType> AllLookupDictionary;

        /// <summary>
        /// Static type constructor to coordinate static initialization order
        /// </summary>
        static OpenETRasterCalculationResultType()
        {
            All = new List<OpenETRasterCalculationResultType> { NotStarted, InProgress, Succeeded, Failed };
            AllLookupDictionary = new ReadOnlyDictionary<int, OpenETRasterCalculationResultType>(All.ToDictionary(x => x.OpenETRasterCalculationResultTypeID));
        }

        /// <summary>
        /// Protected constructor only for use in instantiating the set of static lookup values that match database
        /// </summary>
        protected OpenETRasterCalculationResultType(int openETRasterCalculationResultTypeID, string openETRasterCalculationResultTypeName, string openETRasterCalculationResultTypeDisplayName)
        {
            OpenETRasterCalculationResultTypeID = openETRasterCalculationResultTypeID;
            OpenETRasterCalculationResultTypeName = openETRasterCalculationResultTypeName;
            OpenETRasterCalculationResultTypeDisplayName = openETRasterCalculationResultTypeDisplayName;
        }

        [Key]
        public int OpenETRasterCalculationResultTypeID { get; private set; }
        public string OpenETRasterCalculationResultTypeName { get; private set; }
        public string OpenETRasterCalculationResultTypeDisplayName { get; private set; }
        [NotMapped]
        public int PrimaryKey { get { return OpenETRasterCalculationResultTypeID; } }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public bool Equals(OpenETRasterCalculationResultType other)
        {
            if (other == null)
            {
                return false;
            }
            return other.OpenETRasterCalculationResultTypeID == OpenETRasterCalculationResultTypeID;
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override bool Equals(object obj)
        {
            return Equals(obj as OpenETRasterCalculationResultType);
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override int GetHashCode()
        {
            return OpenETRasterCalculationResultTypeID;
        }

        public static bool operator ==(OpenETRasterCalculationResultType left, OpenETRasterCalculationResultType right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(OpenETRasterCalculationResultType left, OpenETRasterCalculationResultType right)
        {
            return !Equals(left, right);
        }

        public OpenETRasterCalculationResultTypeEnum ToEnum => (OpenETRasterCalculationResultTypeEnum)GetHashCode();

        public static OpenETRasterCalculationResultType ToType(int enumValue)
        {
            return ToType((OpenETRasterCalculationResultTypeEnum)enumValue);
        }

        public static OpenETRasterCalculationResultType ToType(OpenETRasterCalculationResultTypeEnum enumValue)
        {
            switch (enumValue)
            {
                case OpenETRasterCalculationResultTypeEnum.Failed:
                    return Failed;
                case OpenETRasterCalculationResultTypeEnum.InProgress:
                    return InProgress;
                case OpenETRasterCalculationResultTypeEnum.NotStarted:
                    return NotStarted;
                case OpenETRasterCalculationResultTypeEnum.Succeeded:
                    return Succeeded;
                default:
                    throw new ArgumentException("Unable to map Enum: {enumValue}");
            }
        }
    }

    public enum OpenETRasterCalculationResultTypeEnum
    {
        NotStarted = 1,
        InProgress = 2,
        Succeeded = 3,
        Failed = 4
    }

    public partial class OpenETRasterCalculationResultTypeNotStarted : OpenETRasterCalculationResultType
    {
        private OpenETRasterCalculationResultTypeNotStarted(int openETRasterCalculationResultTypeID, string openETRasterCalculationResultTypeName, string openETRasterCalculationResultTypeDisplayName) : base(openETRasterCalculationResultTypeID, openETRasterCalculationResultTypeName, openETRasterCalculationResultTypeDisplayName) {}
        public static readonly OpenETRasterCalculationResultTypeNotStarted Instance = new OpenETRasterCalculationResultTypeNotStarted(1, @"NotStarted", @"Not Started");
    }

    public partial class OpenETRasterCalculationResultTypeInProgress : OpenETRasterCalculationResultType
    {
        private OpenETRasterCalculationResultTypeInProgress(int openETRasterCalculationResultTypeID, string openETRasterCalculationResultTypeName, string openETRasterCalculationResultTypeDisplayName) : base(openETRasterCalculationResultTypeID, openETRasterCalculationResultTypeName, openETRasterCalculationResultTypeDisplayName) {}
        public static readonly OpenETRasterCalculationResultTypeInProgress Instance = new OpenETRasterCalculationResultTypeInProgress(2, @"InProgress", @"In Progress");
    }

    public partial class OpenETRasterCalculationResultTypeSucceeded : OpenETRasterCalculationResultType
    {
        private OpenETRasterCalculationResultTypeSucceeded(int openETRasterCalculationResultTypeID, string openETRasterCalculationResultTypeName, string openETRasterCalculationResultTypeDisplayName) : base(openETRasterCalculationResultTypeID, openETRasterCalculationResultTypeName, openETRasterCalculationResultTypeDisplayName) {}
        public static readonly OpenETRasterCalculationResultTypeSucceeded Instance = new OpenETRasterCalculationResultTypeSucceeded(3, @"Succeeded", @"Succeeded");
    }

    public partial class OpenETRasterCalculationResultTypeFailed : OpenETRasterCalculationResultType
    {
        private OpenETRasterCalculationResultTypeFailed(int openETRasterCalculationResultTypeID, string openETRasterCalculationResultTypeName, string openETRasterCalculationResultTypeDisplayName) : base(openETRasterCalculationResultTypeID, openETRasterCalculationResultTypeName, openETRasterCalculationResultTypeDisplayName) {}
        public static readonly OpenETRasterCalculationResultTypeFailed Instance = new OpenETRasterCalculationResultTypeFailed(4, @"Failed", @"Failed");
    }
}