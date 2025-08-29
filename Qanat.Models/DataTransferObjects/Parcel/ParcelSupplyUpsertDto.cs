using System.ComponentModel.DataAnnotations;

namespace Qanat.Models.DataTransferObjects;

public class ParcelSupplyUpsertDto
{
    [Display(Name = "APN")]
    [Required]
    public List<int> ParcelIDs { get; set; }

    [Display(Name = "Effective Date")]
    [Required]
    [RegularExpression(@"^\d{4}\-\d{1,2}\-\d{1,2}$",
        ErrorMessage = "Effective Date must be entered in YYYY-MM-DD format.")]
    public string? EffectiveDate { get; set; }

    [Display(Name = "Transaction Amount")]
    [Required]
    public decimal? TransactionAmount { get; set; }

    [Display(Name = "Supply Type")]
    [Required]
    public int? WaterTypeID { get; set; }

    public string UserComment { get; set; }
}