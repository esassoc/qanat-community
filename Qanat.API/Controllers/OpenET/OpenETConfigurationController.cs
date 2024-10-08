using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Qanat.API.Services;
using Qanat.API.Services.Attributes;
using Qanat.API.Services.Authorization;
using Qanat.API.Services.OpenET;
using Qanat.EFModels.Entities;
using Qanat.Models.DataTransferObjects;
using Qanat.Models.Security;

namespace Qanat.API.Controllers;

[ApiController]
[RightsChecker]
public class OpenETConfigurationController(QanatDbContext dbContext, ILogger<OpenETSyncController> logger, IOptions<QanatConfiguration> qanatConfiguration, OpenETSyncService openETSyncService, IBackgroundJobClient backgroundJobClient)
    : SitkaController<OpenETSyncController>(dbContext, logger, qanatConfiguration)
{
    [HttpGet("geographies/{geographyID}/open-et-configuration")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [WithGeographyRolePermission(PermissionEnum.ParcelRights, RightsEnum.Read)]
    public ActionResult<bool> IsOpenETActiveForGeography([FromRoute] int geographyID)
    {
        var isOpenETActive = Geographies.GetByID(_dbContext, geographyID).IsOpenETActive;
        return Ok(isOpenETActive);
    }

    [HttpPut("geographies/{geographyID}/open-et-configuration")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [WithGeographyRolePermission(PermissionEnum.ParcelRights, RightsEnum.Update)]
    public ActionResult ConfigureOpenETForGeography([FromRoute] int geographyID, [FromBody] OpenETConfigurationDto openETConfigurationDto)
    {
        if (openETConfigurationDto.IsOpenETActive && string.IsNullOrWhiteSpace(openETConfigurationDto.OpenETShapefilePath))
        {
            return BadRequest("Please enter the path to the OpenET Shape File.");
        }

        Geographies.UpdateIsOpenETConfiguration(_dbContext, geographyID, openETConfigurationDto);
        WaterMeasurementTypes.UpdateIsActiveByGeographyIDAndMeasurementTypeNames(_dbContext, geographyID, WaterMeasurementTypes.OpenETWaterMeasurementTypeNames, openETConfigurationDto.IsOpenETActive);

        return Ok();
    }
}