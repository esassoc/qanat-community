using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Qanat.Models.DataTransferObjects;

public class WaterMeasurementSelfReportFileResourceCreateDto
{
    [Required]
    public IFormFile File { get; set; }

    [StringLength(200, ErrorMessage = "File description cannot exceed 200 characters")]
    public string FileDescription { get; set; }
}

public class WaterMeasurementSelfReportFileResourceUpdateDto
{
    [StringLength(200, ErrorMessage = "File description cannot exceed 200 characters")]
    public string FileDescription { get; set; }
}
