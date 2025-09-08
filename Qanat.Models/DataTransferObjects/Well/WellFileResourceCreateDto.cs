using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Qanat.Models.DataTransferObjects;

public class WellFileResourceCreateDto : WellFileResourceUpdateDto
{
    [Required]
    public IFormFile File { get; set; }
}

public class WellFileResourceUpdateDto
{
    [StringLength(200, ErrorMessage = "File description cannot exceed 200 characters")]
    public string FileDescription { get; set; }
}