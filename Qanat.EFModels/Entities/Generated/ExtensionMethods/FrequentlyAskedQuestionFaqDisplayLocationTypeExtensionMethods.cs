//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[FrequentlyAskedQuestionFaqDisplayLocationType]

using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities
{
    public static partial class FrequentlyAskedQuestionFaqDisplayLocationTypeExtensionMethods
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