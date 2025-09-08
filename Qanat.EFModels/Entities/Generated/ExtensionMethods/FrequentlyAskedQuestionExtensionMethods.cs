//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[FrequentlyAskedQuestion]

using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities
{
    public static partial class FrequentlyAskedQuestionExtensionMethods
    {
        public static FrequentlyAskedQuestionSimpleDto AsSimpleDto(this FrequentlyAskedQuestion frequentlyAskedQuestion)
        {
            var dto = new FrequentlyAskedQuestionSimpleDto()
            {
                FrequentlyAskedQuestionID = frequentlyAskedQuestion.FrequentlyAskedQuestionID,
                QuestionText = frequentlyAskedQuestion.QuestionText,
                AnswerText = frequentlyAskedQuestion.AnswerText
            };
            return dto;
        }
    }
}