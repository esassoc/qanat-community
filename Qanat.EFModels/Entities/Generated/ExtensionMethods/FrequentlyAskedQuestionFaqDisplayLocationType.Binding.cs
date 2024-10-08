//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[FrequentlyAskedQuestionFaqDisplayLocationType]
namespace Qanat.EFModels.Entities
{
    public partial class FrequentlyAskedQuestionFaqDisplayLocationType
    {
        public int PrimaryKey => FrequentlyAskedQuestionFaqDisplayLocationTypeID;
        public FaqDisplayLocationType FaqDisplayLocationType => FaqDisplayLocationType.AllLookupDictionary[FaqDisplayLocationTypeID];

        public static class FieldLengths
        {

        }
    }
}