using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Qanat.Models.DataTransferObjects;

public class ZoneGroupCsvUpsertDto
{
    [Required]
    public IFormFile UploadedFile { get; set; }

    [Required]
    public string APNColumnName { get; set; }

    [Required]
    public string ZoneColumnName { get; set; }
}