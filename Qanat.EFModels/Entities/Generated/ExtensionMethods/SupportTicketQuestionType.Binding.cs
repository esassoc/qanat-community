//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[SupportTicketQuestionType]
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Qanat.Models.DataTransferObjects;


namespace Qanat.EFModels.Entities
{
    public abstract partial class SupportTicketQuestionType : IHavePrimaryKey
    {
        public static readonly SupportTicketQuestionTypeAccessingData AccessingData = SupportTicketQuestionTypeAccessingData.Instance;
        public static readonly SupportTicketQuestionTypePolicyQuestion PolicyQuestion = SupportTicketQuestionTypePolicyQuestion.Instance;
        public static readonly SupportTicketQuestionTypeInterpretingDataQuestion InterpretingDataQuestion = SupportTicketQuestionTypeInterpretingDataQuestion.Instance;
        public static readonly SupportTicketQuestionTypeLogInQuestion LogInQuestion = SupportTicketQuestionTypeLogInQuestion.Instance;
        public static readonly SupportTicketQuestionTypeBug Bug = SupportTicketQuestionTypeBug.Instance;
        public static readonly SupportTicketQuestionTypeOther Other = SupportTicketQuestionTypeOther.Instance;

        public static readonly List<SupportTicketQuestionType> All;
        public static readonly ReadOnlyDictionary<int, SupportTicketQuestionType> AllLookupDictionary;

        /// <summary>
        /// Static type constructor to coordinate static initialization order
        /// </summary>
        static SupportTicketQuestionType()
        {
            All = new List<SupportTicketQuestionType> { AccessingData, PolicyQuestion, InterpretingDataQuestion, LogInQuestion, Bug, Other };
            AllLookupDictionary = new ReadOnlyDictionary<int, SupportTicketQuestionType>(All.ToDictionary(x => x.SupportTicketQuestionTypeID));
        }

        /// <summary>
        /// Protected constructor only for use in instantiating the set of static lookup values that match database
        /// </summary>
        protected SupportTicketQuestionType(int supportTicketQuestionTypeID, string supportTicketQuestionTypeName, string supportTicketQuestionTypeDisplayName)
        {
            SupportTicketQuestionTypeID = supportTicketQuestionTypeID;
            SupportTicketQuestionTypeName = supportTicketQuestionTypeName;
            SupportTicketQuestionTypeDisplayName = supportTicketQuestionTypeDisplayName;
        }

        [Key]
        public int SupportTicketQuestionTypeID { get; private set; }
        public string SupportTicketQuestionTypeName { get; private set; }
        public string SupportTicketQuestionTypeDisplayName { get; private set; }
        [NotMapped]
        public int PrimaryKey { get { return SupportTicketQuestionTypeID; } }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public bool Equals(SupportTicketQuestionType other)
        {
            if (other == null)
            {
                return false;
            }
            return other.SupportTicketQuestionTypeID == SupportTicketQuestionTypeID;
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override bool Equals(object obj)
        {
            return Equals(obj as SupportTicketQuestionType);
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override int GetHashCode()
        {
            return SupportTicketQuestionTypeID;
        }

        public static bool operator ==(SupportTicketQuestionType left, SupportTicketQuestionType right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(SupportTicketQuestionType left, SupportTicketQuestionType right)
        {
            return !Equals(left, right);
        }

        public SupportTicketQuestionTypeEnum ToEnum => (SupportTicketQuestionTypeEnum)GetHashCode();

        public static SupportTicketQuestionType ToType(int enumValue)
        {
            return ToType((SupportTicketQuestionTypeEnum)enumValue);
        }

        public static SupportTicketQuestionType ToType(SupportTicketQuestionTypeEnum enumValue)
        {
            switch (enumValue)
            {
                case SupportTicketQuestionTypeEnum.AccessingData:
                    return AccessingData;
                case SupportTicketQuestionTypeEnum.Bug:
                    return Bug;
                case SupportTicketQuestionTypeEnum.InterpretingDataQuestion:
                    return InterpretingDataQuestion;
                case SupportTicketQuestionTypeEnum.LogInQuestion:
                    return LogInQuestion;
                case SupportTicketQuestionTypeEnum.Other:
                    return Other;
                case SupportTicketQuestionTypeEnum.PolicyQuestion:
                    return PolicyQuestion;
                default:
                    throw new ArgumentException("Unable to map Enum: {enumValue}");
            }
        }
    }

    public enum SupportTicketQuestionTypeEnum
    {
        AccessingData = 1,
        PolicyQuestion = 2,
        InterpretingDataQuestion = 3,
        LogInQuestion = 4,
        Bug = 5,
        Other = 6
    }

    public partial class SupportTicketQuestionTypeAccessingData : SupportTicketQuestionType
    {
        private SupportTicketQuestionTypeAccessingData(int supportTicketQuestionTypeID, string supportTicketQuestionTypeName, string supportTicketQuestionTypeDisplayName) : base(supportTicketQuestionTypeID, supportTicketQuestionTypeName, supportTicketQuestionTypeDisplayName) {}
        public static readonly SupportTicketQuestionTypeAccessingData Instance = new SupportTicketQuestionTypeAccessingData(1, @"AccessingData", @"I need help accessing my Water Account / Parcel / Usage Data");
    }

    public partial class SupportTicketQuestionTypePolicyQuestion : SupportTicketQuestionType
    {
        private SupportTicketQuestionTypePolicyQuestion(int supportTicketQuestionTypeID, string supportTicketQuestionTypeName, string supportTicketQuestionTypeDisplayName) : base(supportTicketQuestionTypeID, supportTicketQuestionTypeName, supportTicketQuestionTypeDisplayName) {}
        public static readonly SupportTicketQuestionTypePolicyQuestion Instance = new SupportTicketQuestionTypePolicyQuestion(2, @"PolicyQuestion", @"I have a question about policies, rules, fees, etc");
    }

    public partial class SupportTicketQuestionTypeInterpretingDataQuestion : SupportTicketQuestionType
    {
        private SupportTicketQuestionTypeInterpretingDataQuestion(int supportTicketQuestionTypeID, string supportTicketQuestionTypeName, string supportTicketQuestionTypeDisplayName) : base(supportTicketQuestionTypeID, supportTicketQuestionTypeName, supportTicketQuestionTypeDisplayName) {}
        public static readonly SupportTicketQuestionTypeInterpretingDataQuestion Instance = new SupportTicketQuestionTypeInterpretingDataQuestion(3, @"InterpretingDataQuestion", @"I need help interpreting my water usage or allocations");
    }

    public partial class SupportTicketQuestionTypeLogInQuestion : SupportTicketQuestionType
    {
        private SupportTicketQuestionTypeLogInQuestion(int supportTicketQuestionTypeID, string supportTicketQuestionTypeName, string supportTicketQuestionTypeDisplayName) : base(supportTicketQuestionTypeID, supportTicketQuestionTypeName, supportTicketQuestionTypeDisplayName) {}
        public static readonly SupportTicketQuestionTypeLogInQuestion Instance = new SupportTicketQuestionTypeLogInQuestion(4, @"LogInQuestion", @"I can't log in or my account isn't configured");
    }

    public partial class SupportTicketQuestionTypeBug : SupportTicketQuestionType
    {
        private SupportTicketQuestionTypeBug(int supportTicketQuestionTypeID, string supportTicketQuestionTypeName, string supportTicketQuestionTypeDisplayName) : base(supportTicketQuestionTypeID, supportTicketQuestionTypeName, supportTicketQuestionTypeDisplayName) {}
        public static readonly SupportTicketQuestionTypeBug Instance = new SupportTicketQuestionTypeBug(5, @"Bug", @"I ran into a bug or problem with the system");
    }

    public partial class SupportTicketQuestionTypeOther : SupportTicketQuestionType
    {
        private SupportTicketQuestionTypeOther(int supportTicketQuestionTypeID, string supportTicketQuestionTypeName, string supportTicketQuestionTypeDisplayName) : base(supportTicketQuestionTypeID, supportTicketQuestionTypeName, supportTicketQuestionTypeDisplayName) {}
        public static readonly SupportTicketQuestionTypeOther Instance = new SupportTicketQuestionTypeOther(6, @"Other", @"Other");
    }
}