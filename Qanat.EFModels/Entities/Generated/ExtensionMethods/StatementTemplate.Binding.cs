//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[StatementTemplate]
namespace Qanat.EFModels.Entities
{
    public partial class StatementTemplate
    {
        public int PrimaryKey => StatementTemplateID;
        public StatementTemplateType StatementTemplateType => StatementTemplateType.AllLookupDictionary[StatementTemplateTypeID];

        public static class FieldLengths
        {
            public const int TemplateTitle = 100;
        }
    }
}