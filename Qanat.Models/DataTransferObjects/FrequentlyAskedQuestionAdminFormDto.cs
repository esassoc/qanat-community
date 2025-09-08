using System.ComponentModel.DataAnnotations;

namespace Qanat.Models.DataTransferObjects;

public class FrequentlyAskedQuestionAdminFormDto
{
    public int? FrequentlyAskedQuestionID { get; set; }

    [Required]
    public string QuestionText { get; set; }

    [Required]
    public string AnswerText { get; set; }

    public List<int> FaqDisplayLocationTypeIDs { get; set; }

    public FrequentlyAskedQuestionAdminFormDto()
    {
        FaqDisplayLocationTypeIDs = new List<int>();
    }

    public void FixNullFaqDisplayLocationTypeIDs()
    {
        // MK 6/13/2024 -- Usually setting this in the param-less constructor will fix issues involving null lists, but with the front end form binding the way it is, the front end sends up null if you don't select anything in the dropdown.
        // The param-less constructor is called, but then overwritten with null. This is not my favorite solution, but it fixes it quickly so lets move on. 
        FaqDisplayLocationTypeIDs ??= new List<int>();
    }
}