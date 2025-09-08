using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Qanat.EFModels.Entities;

[Table("FrequentlyAskedQuestion")]
public partial class FrequentlyAskedQuestion
{
    [Key]
    public int FrequentlyAskedQuestionID { get; set; }

    [Required]
    [Unicode(false)]
    public string QuestionText { get; set; }

    [Required]
    [Unicode(false)]
    public string AnswerText { get; set; }

    [InverseProperty("FrequentlyAskedQuestion")]
    public virtual ICollection<FrequentlyAskedQuestionFaqDisplayLocationType> FrequentlyAskedQuestionFaqDisplayLocationTypes { get; set; } = new List<FrequentlyAskedQuestionFaqDisplayLocationType>();
}
