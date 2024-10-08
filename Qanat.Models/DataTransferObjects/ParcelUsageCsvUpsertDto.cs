using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Qanat.Models.DataTransferObjects;

public class WaterMeasurementCsvUpsertDto
{
    [Required]
    public IFormFile UploadedFile { get; set; }

    [Required]
    //[RegularExpression(@"^\d{4}\-\d{1,2}\-\d{1,2}$",
    //    ErrorMessage = "Effective Date must be entered in YYYY-MM-DD format.")]
    public string? EffectiveDate { get; set; }

    [Display(Name = "Water Use Type")]
    [Required]
    public int? WaterMeasurementTypeID { get; set; }

    [Display(Name = "Unit Type")]
    [Required]
    public int? UnitTypeID { get; set; }

    [Required]
    public string APNColumnName { get; set; }

    [Required]
    public string QuantityColumnName { get; set; }
    public string CommentColumnName { get; set; }
}

public class CsvUpsertDto
{
    [Required]
    public IFormFile UploadedFile { get; set; }
}


public class WaterMeasurementRasterUploadDto
{
    [Required]
    public IFormFile UploadedFile { get; set; }

    [Display(Name = "Unit Type")]
    [Required]
    public int? UnitTypeID { get; set; }

    [Display(Name = "Water Use Type")]
    [Required]
    public int? WaterMeasurementTypeID { get; set; }

    [Required]
    public string? EffectiveDate { get; set; }
}