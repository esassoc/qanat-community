using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities
{
    public static class FrequentlyAskedQuestionFaqDisplayLocationTypeExtensionMethods
    {
        public static FrequentlyAskedQuestionFaqDisplayLocationTypeSimpleDto AsSimpleDto(this FrequentlyAskedQuestionFaqDisplayLocationType frequentlyAskedQuestionFaqDisplayLocationType)
        {
            var dto = new FrequentlyAskedQuestionFaqDisplayLocationTypeSimpleDto()
            {
                FrequentlyAskedQuestionFaqDisplayLocationTypeID = frequentlyAskedQuestionFaqDisplayLocationType.FrequentlyAskedQuestionFaqDisplayLocationTypeID,
                FrequentlyAskedQuestionID = frequentlyAskedQuestionFaqDisplayLocationType.FrequentlyAskedQuestionID,
                FaqDisplayLocationTypeID = frequentlyAskedQuestionFaqDisplayLocationType.FaqDisplayLocationTypeID,
                SortOrder = frequentlyAskedQuestionFaqDisplayLocationType.SortOrder
            };
            return dto;
        }
    }
}