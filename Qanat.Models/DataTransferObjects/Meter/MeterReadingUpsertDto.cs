using System.ComponentModel.DataAnnotations;

namespace Qanat.Models.DataTransferObjects;

public class MeterReadingUpsertDto
{
    [Required]
    public int? MeterReadingUnitTypeID { get; set; }

    [Required(ErrorMessage = "The Date field is required.")]
    public DateTime? ReadingDate { get; set; }

    [Required(ErrorMessage = "The Time field is required.")]
    public string ReadingTime { get; set; }

    [Required(ErrorMessage = "The Previous Reading field is required.")]
    public decimal? PreviousReading { get; set; }

    [Required(ErrorMessage = "The Current Reading field is required.")]
    public decimal? CurrentReading { get; set; }

    [MaxLength(5, ErrorMessage = "Reader Initials cannot be longer than 5 characters.")]
    public string ReaderInitials { get; set; }

    public string Comment { get; set; }
}
