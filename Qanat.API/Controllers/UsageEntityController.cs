using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Qanat.API.Services;
using Qanat.API.Services.Authorization;
using Qanat.Common.Services.GDAL;
using Qanat.EFModels.Entities;
using Qanat.Models.DataTransferObjects;
using Qanat.Models.Security;
using System.Collections.Generic;
using Qanat.API.Services.Attributes;

namespace Qanat.API.Controllers;

[ApiController]
[RightsChecker]
public class UsageEntityController : SitkaController<UsageEntityController>
{
    private readonly GeographyGISBoundaryService _geographyGISBoundaryService;
    private readonly GDALAPIService _gdalApiService;
    private readonly FileService _fileService;

    public UsageEntityController(QanatDbContext dbContext, ILogger<UsageEntityController> logger,
        IOptions<QanatConfiguration> qanatConfiguration, GeographyGISBoundaryService geographyGISBoundaryService,
        GDALAPIService gdalApiService, FileService fileService)
        : base(dbContext, logger, qanatConfiguration)
    {
        _geographyGISBoundaryService = geographyGISBoundaryService;
        _gdalApiService = gdalApiService;
        _fileService = fileService;
    }

    [HttpGet("usage-entities/{usageEntityID}")]
    [WithWaterAccountRolePermission(PermissionEnum.UsageEntityRights, RightsEnum.Read)]
    public ActionResult<UsageEntityPopupDto> GetUsageEntityPopupByID([FromRoute] int usageEntityID)
    {
        var usageEntity = UsageEntities.GetPopupDtoByID(_dbContext, usageEntityID);
        return Ok(usageEntity);
    }

    [HttpGet("parcels/{parcelID}/usage-entities")]
    [EntityNotFound(typeof(Parcel), "parcelID")]
    [WithWaterAccountRolePermission(PermissionEnum.UsageEntityRights, RightsEnum.Read)]
    public ActionResult<List<UsageEntityListItemDto>> GetUsageEntityByParcelID([FromRoute] int parcelID)
    {
        var usageEntities = UsageEntities.GetListByParcelID(_dbContext, parcelID);
        return usageEntities;
    }

    [HttpGet("water-accounts/{waterAccountID}/usage-entities")]
    [EntityNotFound(typeof(Parcel), "parcelID")]
    [WithWaterAccountRolePermission(PermissionEnum.UsageEntityRights, RightsEnum.Read)]
    public ActionResult<List<UsageEntitySimpleDto>> GetUsageEntityByWaterAccountID([FromRoute] int waterAccountID)
    {
        var usageEntities = UsageEntities.ListByWaterAccountID(_dbContext, waterAccountID);
        return usageEntities;
    }
}