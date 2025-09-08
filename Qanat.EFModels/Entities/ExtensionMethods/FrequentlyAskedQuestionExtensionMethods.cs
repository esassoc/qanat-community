using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities;

public static class FrequentlyAskedQuestionExtensionMethods
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

    public static FrequentlyAskedQuestionGridDto AsGridDto(this FrequentlyAskedQuestion frequentlyAskedQuestion)
    {
        var dto = new FrequentlyAskedQuestionGridDto()
        {
            FrequentlyAskedQuestionID = frequentlyAskedQuestion.FrequentlyAskedQuestionID,
            QuestionText = frequentlyAskedQuestion.QuestionText,
            AnswerText = frequentlyAskedQuestion.AnswerText,
            FaqDisplayLocations = frequentlyAskedQuestion.FrequentlyAskedQuestionFaqDisplayLocationTypes.Select(x => x.AsSimpleDto()).ToList()
        };
        return dto;
    }

    public static FrequentlyAskedQuestionLocationDisplayDto AsLocationDisplayDto(
        this FrequentlyAskedQuestion frequentlyAskedQuestion, int faqDisplayLocationTypeID)
    {
        var dto = new FrequentlyAskedQuestionLocationDisplayDto()
        {
            FrequentlyAskedQuestionID = frequentlyAskedQuestion.FrequentlyAskedQuestionID,
            QuestionText = frequentlyAskedQuestion.QuestionText,
            AnswerText = frequentlyAskedQuestion.AnswerText,
            SortOrder = frequentlyAskedQuestion.FrequentlyAskedQuestionFaqDisplayLocationTypes.SingleOrDefault(x => x.FaqDisplayLocationTypeID == faqDisplayLocationTypeID).SortOrder
        };
        return dto;
    }
}