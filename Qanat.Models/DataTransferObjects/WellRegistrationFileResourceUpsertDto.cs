using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Qanat.Models.DataTransferObjects;

public class WellRegistrationFileResourceUpsertDto : WellRegistrationFileResourceUpdateDto
{
    [Required]
    public int WellRegistrationID { get; set; }

    [Required]
    public IFormFile File { get; set; }
}

public class WellRegistrationFileResourceUpdateDto
{
    [StringLength(200, ErrorMessage = "File description cannot exceed 200 characters")]
    public string FileDescription { get; set; }
    public int WellRegistrationFileResourceID { get; set; }
}