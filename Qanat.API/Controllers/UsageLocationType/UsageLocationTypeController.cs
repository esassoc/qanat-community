using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Qanat.API.Services;
using Qanat.API.Services.Attributes;
using Qanat.API.Services.Authorization;
using Qanat.EFModels.Entities;
using Qanat.Models.DataTransferObjects;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Qanat.API.Controllers;

[ApiController]
[RightsChecker]
[Route("geographies/{geographyID}/usage-location-types")]
public class UsageLocationTypeController(QanatDbContext dbContext, ILogger<UsageLocationTypeController> logger, IOptions<QanatConfiguration> qanatConfiguration, UserDto callingUser)
    : SitkaController<UsageLocationTypeController>(dbContext, logger, qanatConfiguration)
{

    [HttpPost]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [WithGeographyRoleFlag(FlagEnum.HasManagerDashboard)]
    public async Task<ActionResult<List<UsageLocationTypeDto>>> Create([FromRoute] int geographyID, [FromBody] UsageLocationTypeUpsertDto usageLocationTypeUpsertDto)
    {
        var errors = await UsageLocationTypes.ValidateCreateAsync(dbContext, geographyID, usageLocationTypeUpsertDto);
        errors.ForEach(x => ModelState.AddModelError(x.Type, x.Message));
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var updatedUsageLocationTypeDtos = await UsageLocationTypes.CreateAsync(dbContext, geographyID, usageLocationTypeUpsertDto, callingUser);
        return Ok(updatedUsageLocationTypeDtos);
    }

    [HttpGet]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [WithRoleFlag(FlagEnum.CanClaimWaterAccounts)]
    public async Task<ActionResult<List<UsageLocationTypeDto>>> List([FromRoute] int geographyID)
    {
        var usageLocationTypeDtos = await UsageLocationTypes.ListAsync(dbContext, geographyID);
        return Ok(usageLocationTypeDtos);
    }

    [HttpPut]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [WithGeographyRoleFlag(FlagEnum.HasManagerDashboard)]
    public async Task<ActionResult<List<UsageLocationTypeDto>>> Update([FromRoute] int geographyID, [FromBody] List<UsageLocationTypeUpsertDto> usageLocationTypeUpsertDtos)
    {
        var errors = await UsageLocationTypes.ValidateUpdateAsync(dbContext, geographyID, usageLocationTypeUpsertDtos);
        errors.ForEach(x => ModelState.AddModelError(x.Type, x.Message));
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var updatedUsageLocationTypeDtos = await UsageLocationTypes.UpdateAsync(dbContext, geographyID, usageLocationTypeUpsertDtos, callingUser);
        return Ok(updatedUsageLocationTypeDtos);
    }

    [HttpPut("{usageLocationTypeID}/update-cover-crop-metadata")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [EntityNotFound(typeof(UsageLocationType), "usageLocationTypeID")]
    [WithGeographyRoleFlag(FlagEnum.HasManagerDashboard)]
    public async Task<ActionResult<UsageLocationTypeSimpleDto>> UpdateCoverCropMetadata([FromRoute] int geographyID, [FromRoute] int usageLocationTypeID, [FromBody] UsageLocationTypeUpdateCoverCropMetadataDto updateCoverCropMetadataDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var updatedUsageLocationTypeDtos = await UsageLocationTypes.UpdateCoverCropMetadataAsync(dbContext, geographyID, usageLocationTypeID, updateCoverCropMetadataDto, callingUser);
        return Ok(updatedUsageLocationTypeDtos);
    }

    [HttpPut("{usageLocationTypeID}/update-fallow-metadata")]
    [EntityNotFound(typeof(Geography), "geographyID")]  
    [EntityNotFound(typeof(UsageLocationType), "usageLocationTypeID")]
    [WithGeographyRoleFlag(FlagEnum.HasManagerDashboard)]
    public async Task<ActionResult<UsageLocationTypeSimpleDto>> UpdateFallowMetadata([FromRoute] int geographyID, [FromRoute] int usageLocationTypeID, [FromBody] UsageLocationTypeUpdateFallowMetadataDto updateFallowMetadataDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var updatedUsageLocationTypeDtos = await UsageLocationTypes.UpdateFallowMetadataAsync(dbContext, geographyID, usageLocationTypeID, updateFallowMetadataDto, callingUser);
        return Ok(updatedUsageLocationTypeDtos);
    }

    [HttpDelete("{usageLocationTypeID}")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [EntityNotFound(typeof(UsageLocationType), "usageLocationTypeID")]
    [WithGeographyRoleFlag(FlagEnum.HasManagerDashboard)]
    public async Task<ActionResult> Delete([FromRoute] int geographyID, [FromRoute] int usageLocationTypeID)
    {
        var errors = await UsageLocationTypes.ValidateDeleteAsync(dbContext, geographyID, usageLocationTypeID);
        errors.ForEach(x => ModelState.AddModelError(x.Type, x.Message));
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        await UsageLocationTypes.DeleteAsync(dbContext, geographyID, usageLocationTypeID);
        return NoContent();
    }
}