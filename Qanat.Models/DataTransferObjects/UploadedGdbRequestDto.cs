using Microsoft.AspNetCore.Http;

namespace Qanat.Models.DataTransferObjects;

public class UploadedGdbRequestDto
{
    public IFormFile File { get; set; }
}