using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Qanat.Models.DataTransferObjects;
public class ParcelSupplyCsvUpsertDto
{
    [Required]
    public IFormFile UploadedFile { get; set; }

    [Required]
    [RegularExpression(@"^\d{4}\-\d{1,2}\-\d{1,2}$",
        ErrorMessage = "Effective Date must be entered in YYYY-MM-DD format.")]
    public string? EffectiveDate { get; set; }

    [Display(Name = "Supply Type")]
    [Required]
    public int? WaterTypeID { get; set; }
}