using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Qanat.Models.DataTransferObjects;
using Scalar.AspNetCore;
using System;

namespace Qanat.Swagger.Controllers;

[ApiController]
[ExcludeFromApiReference]
public class SystemInfoController : ControllerBase
{
    [HttpGet("/", Name = "GetSystemInfo")]
    [AllowAnonymous]
    public IActionResult GetSystemInfo([FromServices] IWebHostEnvironment environment)
    {
        var systemInfo = new SystemInfoDto
        {
            Environment = environment.EnvironmentName,
            CurrentTimeUTC = DateTime.UtcNow.ToString("o")
        };

        return Ok(systemInfo);
    }
}