using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Qanat.EFModels.Entities;

[Table("FrequentlyAskedQuestionFaqDisplayLocationType")]
public partial class FrequentlyAskedQuestionFaqDisplayLocationType
{
    [Key]
    public int FrequentlyAskedQuestionFaqDisplayLocationTypeID { get; set; }

    public int FrequentlyAskedQuestionID { get; set; }

    public int FaqDisplayLocationTypeID { get; set; }

    public int SortOrder { get; set; }

    [ForeignKey("FrequentlyAskedQuestionID")]
    [InverseProperty("FrequentlyAskedQuestionFaqDisplayLocationTypes")]
    public virtual FrequentlyAskedQuestion FrequentlyAskedQuestion { get; set; }
}
