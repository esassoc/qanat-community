using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Qanat.API.Services;
using Qanat.API.Services.Attributes;
using Qanat.API.Services.Authorization;
using Qanat.EFModels.Entities;
using Qanat.Models.DataTransferObjects;
using Qanat.Models.Security;

namespace Qanat.API.Controllers;

[ApiController]
[RightsChecker]
[Route("geographies/{geographyID}/water-account-contacts")]
public class WaterAccountContactByGeographyController(QanatDbContext dbContext, ILogger<WaterAccountContactByGeographyController> logger,
        IOptions<QanatConfiguration> qanatConfiguration, MapboxService mapboxService, [FromServices] UserDto callingUser)
    : SitkaController<WaterAccountContactByGeographyController>(dbContext, logger, qanatConfiguration)
{
    [HttpPost]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [WithGeographyRolePermission(PermissionEnum.WaterAccountRights, RightsEnum.Update)]
    public async Task<ActionResult<WaterAccountContactDto>> Create([FromRoute] int geographyID, [FromBody] WaterAccountContactUpsertDto waterAccountContactUpsertDto)
    {
        var waterAccountContact = await WaterAccountContacts.Create(_dbContext, geographyID, waterAccountContactUpsertDto);

        if (waterAccountContactUpsertDto.WaterAccountID.HasValue)
        {
            var errors = WaterAccounts.ValidateUpdateWaterAccountContact(_dbContext, waterAccountContactUpsertDto.WaterAccountID.Value, waterAccountContact.WaterAccountContactID);
            if (!errors.Any())
            {
                WaterAccounts.UpdateWaterAccountContact(_dbContext, waterAccountContactUpsertDto.WaterAccountID.Value, waterAccountContact.WaterAccountContactID);
            }
        }

        return Ok(waterAccountContact);
    }

    [HttpGet]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [WithGeographyRolePermission(PermissionEnum.WaterAccountRights, RightsEnum.Read)]
    public async Task<ActionResult<List<WaterAccountContactDto>>> ListByGeographyID([FromRoute] int geographyID)
    {
        var waterAccountContactDtos = await WaterAccountContacts.ListByGeographyIDAsDto(_dbContext, geographyID);
        return Ok(waterAccountContactDtos);
    }

    [HttpPut("validate-addresses")]
    [WithGeographyRolePermission(PermissionEnum.WaterAccountRights, RightsEnum.Delete)]
    public async Task<ActionResult<MapboxBulkResponseDto>> BatchValidateAddresses([FromRoute] int geographyID, [FromBody] BatchValidateWaterAccountContactAddressRequestDto requestDto)
    {
        MapboxBulkResponseDto mapboxBulkResponseDto;
        try
        {
            mapboxBulkResponseDto = await mapboxService.BatchValidateWaterAccountContactAddresses(geographyID, requestDto.WaterAccountContactIDs);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = "An error occurred while validating addresses." });
        }

        return Ok(mapboxBulkResponseDto);
    }

    [HttpPut("validate-address")]
    [EntityNotFound(typeof(WaterAccountContact), "waterAccountContactID")]
    [WithGeographyRolePermission(PermissionEnum.WaterAccountRights, RightsEnum.Update)]
    public async Task<ActionResult<MapboxResponseDto>> ValidateAddress([FromRoute] int geographyID, [FromBody] WaterAccountContactUpsertDto waterAccountContactUpsertDto)
    {
        var mapboxResponseDto = await mapboxService.ValidateSingleAddressAsync(waterAccountContactUpsertDto.Address, waterAccountContactUpsertDto.SecondaryAddress, waterAccountContactUpsertDto.City, waterAccountContactUpsertDto.State, waterAccountContactUpsertDto.ZipCode);

        return Ok(mapboxResponseDto);
    }
}