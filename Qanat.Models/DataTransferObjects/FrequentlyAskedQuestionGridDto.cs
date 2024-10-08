namespace Qanat.Models.DataTransferObjects;

public class FrequentlyAskedQuestionGridDto
{
    public int FrequentlyAskedQuestionID { get; set; }
    public string QuestionText { get; set; }
    public string AnswerText { get; set; }
    public List<FrequentlyAskedQuestionFaqDisplayLocationTypeSimpleDto> FaqDisplayLocations { get; set; }
}