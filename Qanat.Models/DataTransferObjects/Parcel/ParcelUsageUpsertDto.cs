using System.ComponentModel.DataAnnotations;

namespace Qanat.Models.DataTransferObjects;

public class WaterMeasurementCsvResponseDto
{
    public int TransactionCount { get; set; }
    public List<string> UnmatchedParcelNumbers { get; set; }

    public WaterMeasurementCsvResponseDto(int transactionCount, List<string> unmatchedParcelNumbers)
    {
        TransactionCount = transactionCount;
        UnmatchedParcelNumbers = unmatchedParcelNumbers;
    }
}

public class WaterMeasurementUpsertDto
{
    [Display(Name = "APN")]
    [Required]
    public List<int> ParcelIDs { get; set; }

    [Display(Name = "Effective Date")]
    [Required]
    [RegularExpression(@"^\d{4}\-\d{1,2}\-\d{1,2}$",
        ErrorMessage = "Effective Date must be entered in YYYY-MM-DD format.")]
    public string? EffectiveDate { get; set; }

    [Display(Name = "Unit Type")]
    [Required]
    public int? UnitTypeID { get; set; }

    [Display(Name = "Transaction Amount")]
    [Required]
    public decimal? TransactionAmount { get; set; }

    public string UserComment { get; set; }
}