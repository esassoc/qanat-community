using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Qanat.API.Services;
using Qanat.API.Services.Attributes;
using Qanat.API.Services.Authorization;
using Qanat.EFModels.Entities;
using Qanat.Models.DataTransferObjects;
using Qanat.Models.Security;
using System.Collections.Generic;
using System.Threading.Tasks;
using Qanat.API.Models.ExtensionMethods;

namespace Qanat.API.Controllers;

[ApiController]
[RightsChecker]
[Route("geographies/{geographyID}/parcels/{parcelID}/usage-locations")]
public class UsageLocationController(QanatDbContext dbContext, ILogger<UsageLocationController> logger, IOptions<QanatConfiguration> qanatConfiguration, UserDto callingUser)
    : SitkaController<UsageLocationController>(dbContext, logger, qanatConfiguration)
{
    [HttpGet]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [EntityNotFound(typeof(Parcel), "parcelID")]
    [WithWaterAccountRolePermission(PermissionEnum.UsageLocationRights, RightsEnum.Read)]
    public async Task<ActionResult<List<UsageLocationDto>>> List([FromRoute] int geographyID, [FromRoute] int parcelID)
    {
        var usageLocationDtos = await UsageLocations.ListByParcelAsync(_dbContext, geographyID, parcelID, callingUser.IsAdminOrWaterManager(geographyID));
        return Ok(usageLocationDtos);
    }

    [HttpGet("{usageLocationID}")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [EntityNotFound(typeof(Parcel), "parcelID")]
    [EntityNotFound(typeof(UsageLocation), "usageLocationID")]
    [WithWaterAccountRolePermission(PermissionEnum.UsageLocationRights, RightsEnum.Read)]
    public async Task<ActionResult<UsageLocationDto>> Get([FromRoute] int geographyID, [FromRoute] int parcelID, [FromRoute] int usageLocationID)
    {
        var usageLocationDto = await UsageLocations.GetAsync(_dbContext, geographyID, parcelID, usageLocationID);
        return Ok(usageLocationDto);
    }

    [HttpPut("{usageLocationID}/cover-crop")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [EntityNotFound(typeof(Parcel), "parcelID")]
    [EntityNotFound(typeof(UsageLocation), "usageLocationID")]
    [WithRoleFlag(FlagEnum.CanClaimWaterAccounts)]
    [WithGeographyRoleFlag(FlagEnum.HasManagerDashboard)]
    public async Task<ActionResult<UsageLocationDto>> UpdateCoverCrop([FromRoute] int geographyID, [FromRoute] int parcelID, [FromRoute] int usageLocationID, [FromBody] UsageLocationUpdateCoverCropDto updateCoverCropDto)
    {
        var errorMessages = await UsageLocations.ValidateUpdateCoverCropAsync(_dbContext, geographyID, parcelID, usageLocationID, updateCoverCropDto);
        errorMessages.ForEach(em =>
        {
            ModelState.AddModelError(em.Type, em.Message);
        });

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var updatedUsageLocationDto = await UsageLocations.UpdateCoverCropAsync(_dbContext, geographyID, parcelID, usageLocationID, updateCoverCropDto, callingUser);
        return Ok(updatedUsageLocationDto);
    }

    [HttpPut("{usageLocationID}/fallowing")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [EntityNotFound(typeof(Parcel), "parcelID")]
    [EntityNotFound(typeof(UsageLocation), "usageLocationID")]
    [WithRoleFlag(FlagEnum.CanClaimWaterAccounts)]
    [WithGeographyRoleFlag(FlagEnum.HasManagerDashboard)]
    public async Task<ActionResult<UsageLocationDto>> UpdateFallowing([FromRoute] int geographyID, [FromRoute] int parcelID, [FromRoute] int usageLocationID, [FromBody] UsageLocationUpdateFallowingDto updateFallowingDto)
    {
        var errorMessages = await UsageLocations.ValidateUpdateFallowingAsync(_dbContext, geographyID, parcelID, usageLocationID, updateFallowingDto);
        errorMessages.ForEach(em =>
        {
            ModelState.AddModelError(em.Type, em.Message);
        });

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var updatedUsageLocationDto = await UsageLocations.UpdateFallowingAsync(_dbContext, geographyID, parcelID, usageLocationID, updateFallowingDto, callingUser);
        return Ok(updatedUsageLocationDto);
    }

    [HttpPut("migrate-usage-locations")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [EntityNotFound(typeof(Parcel), "parcelID")]
    [WithGeographyRoleFlag(FlagEnum.HasManagerDashboard)]
    public async Task<ActionResult<List<UsageLocationDto>>> MigrateUsageLocations([FromRoute] int geographyID, [FromRoute] int parcelID, [FromBody] UsageLocationMigrationDto usageLocationMigrationDto)
    {
        var errorMessages = await UsageLocations.ValidateMigrateUsageLocationsAsync(_dbContext, geographyID, parcelID, usageLocationMigrationDto);
        errorMessages.ForEach(em =>
        {
            ModelState.AddModelError(em.Type, em.Message);
        });

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var usageLocationDtos = await UsageLocations.MigrateUsageLocationsAsync(_dbContext, geographyID, parcelID, usageLocationMigrationDto, callingUser);
        return Ok(usageLocationDtos);
    }
}