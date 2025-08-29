//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[StatementTemplateType]
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Qanat.Models.DataTransferObjects;


namespace Qanat.EFModels.Entities
{
    public abstract partial class StatementTemplateType : IHavePrimaryKey
    {
        public static readonly StatementTemplateTypeUsageStatement UsageStatement = StatementTemplateTypeUsageStatement.Instance;

        public static readonly List<StatementTemplateType> All;
        public static readonly ReadOnlyDictionary<int, StatementTemplateType> AllLookupDictionary;

        /// <summary>
        /// Static type constructor to coordinate static initialization order
        /// </summary>
        static StatementTemplateType()
        {
            All = new List<StatementTemplateType> { UsageStatement };
            AllLookupDictionary = new ReadOnlyDictionary<int, StatementTemplateType>(All.ToDictionary(x => x.StatementTemplateTypeID));
        }

        /// <summary>
        /// Protected constructor only for use in instantiating the set of static lookup values that match database
        /// </summary>
        protected StatementTemplateType(int statementTemplateTypeID, string statementTemplateTypeName, string statementTemplateTypeDisplayName, string customFieldDefaultParagraphs, string customLabelDefaults)
        {
            StatementTemplateTypeID = statementTemplateTypeID;
            StatementTemplateTypeName = statementTemplateTypeName;
            StatementTemplateTypeDisplayName = statementTemplateTypeDisplayName;
            CustomFieldDefaultParagraphs = customFieldDefaultParagraphs;
            CustomLabelDefaults = customLabelDefaults;
        }

        [Key]
        public int StatementTemplateTypeID { get; private set; }
        public string StatementTemplateTypeName { get; private set; }
        public string StatementTemplateTypeDisplayName { get; private set; }
        public string CustomFieldDefaultParagraphs { get; private set; }
        public string CustomLabelDefaults { get; private set; }
        [NotMapped]
        public int PrimaryKey { get { return StatementTemplateTypeID; } }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public bool Equals(StatementTemplateType other)
        {
            if (other == null)
            {
                return false;
            }
            return other.StatementTemplateTypeID == StatementTemplateTypeID;
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override bool Equals(object obj)
        {
            return Equals(obj as StatementTemplateType);
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override int GetHashCode()
        {
            return StatementTemplateTypeID;
        }

        public static bool operator ==(StatementTemplateType left, StatementTemplateType right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(StatementTemplateType left, StatementTemplateType right)
        {
            return !Equals(left, right);
        }

        public StatementTemplateTypeEnum ToEnum => (StatementTemplateTypeEnum)GetHashCode();

        public static StatementTemplateType ToType(int enumValue)
        {
            return ToType((StatementTemplateTypeEnum)enumValue);
        }

        public static StatementTemplateType ToType(StatementTemplateTypeEnum enumValue)
        {
            switch (enumValue)
            {
                case StatementTemplateTypeEnum.UsageStatement:
                    return UsageStatement;
                default:
                    throw new ArgumentException("Unable to map Enum: {enumValue}");
            }
        }
    }

    public enum StatementTemplateTypeEnum
    {
        UsageStatement = 1
    }

    public partial class StatementTemplateTypeUsageStatement : StatementTemplateType
    {
        private StatementTemplateTypeUsageStatement(int statementTemplateTypeID, string statementTemplateTypeName, string statementTemplateTypeDisplayName, string customFieldDefaultParagraphs, string customLabelDefaults) : base(statementTemplateTypeID, statementTemplateTypeName, statementTemplateTypeDisplayName, customFieldDefaultParagraphs, customLabelDefaults) {}
        public static readonly StatementTemplateTypeUsageStatement Instance = new StatementTemplateTypeUsageStatement(1, @"UsageStatement", @"Usage Statement", @"{ ""Page 1: Additional Information"": 1, ""Page 1: About This Usage Statement"": 5, ""Page 2: Additional Information"": 1, ""Page 2: Have Questions?"": 1}", @"{ ""Balance"": ""Balance"" }");
    }
}