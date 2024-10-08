using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Qanat.EFModels.Entities;
using Qanat.Models.DataTransferObjects;
using Qanat.Swagger.Controllers;
using System;

namespace Qanat.API.Controllers
{
    [ApiController]
    public class SystemInfoController : SitkaApiController<SystemInfoController>
    {
        public SystemInfoController(QanatDbContext dbContext, ILogger<SystemInfoController> logger)
            : base(dbContext, logger)
        {
        }

        [HttpGet("/", Name = "GetSystemInfo")]  // MCS: the pattern seems to be to allow anonymous access to this endpoint
        [AllowAnonymous]
        public IActionResult GetSystemInfo([FromServices] IWebHostEnvironment environment)
        {
            SystemInfoDto systemInfo = new SystemInfoDto
            {
                Environment = environment.EnvironmentName,
                CurrentTimeUTC = DateTime.UtcNow.ToString("o")
            };

            return Ok(systemInfo);
        }

    }
}