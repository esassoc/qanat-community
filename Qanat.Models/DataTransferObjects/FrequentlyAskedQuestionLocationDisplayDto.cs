namespace Qanat.Models.DataTransferObjects;

public class FrequentlyAskedQuestionLocationDisplayDto
{
    public int FrequentlyAskedQuestionID { get; set; }
    public string QuestionText { get; set; }
    public string AnswerText { get; set; }
    public int? SortOrder { get; set; }
}