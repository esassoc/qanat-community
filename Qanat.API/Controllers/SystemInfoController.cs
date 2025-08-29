using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Qanat.API.Services;
using Qanat.API.Services.Attributes;
using Qanat.API.Services.Authorization;
using Qanat.EFModels.Entities;
using Qanat.Models.DataTransferObjects;
using System;

namespace Qanat.API.Controllers
{
    [ApiController]
    [RightsChecker]
    public class SystemInfoController(QanatDbContext dbContext, ILogger<SystemInfoController> logger, IOptions<QanatConfiguration> qanatConfiguration)
        : SitkaController<SystemInfoController>(dbContext, logger, qanatConfiguration)
    {
        [HttpGet("/", Name = "GetSystemInfo")]
        [AllowAnonymous]
        [LogIgnore]
        public ActionResult<SystemInfoDto> GetSystemInfo([FromServices] IWebHostEnvironment environment)
        {
            var systemInfo = new SystemInfoDto
            {
                Environment = environment.EnvironmentName,
                CurrentTimeUTC = DateTime.UtcNow.ToString("o"),
                PodName = _qanatConfiguration.HostName
            };

            return Ok(systemInfo);
        }
    }
}