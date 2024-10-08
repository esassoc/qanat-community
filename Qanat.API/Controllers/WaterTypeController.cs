using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Qanat.API.Services;
using Qanat.API.Services.Attributes;
using Qanat.API.Services.Authorization;
using Qanat.Common.Util;
using Qanat.EFModels.Entities;
using Qanat.Models.DataTransferObjects;
using Qanat.Models.Security;
using System.Collections.Generic;
using System.Linq;

namespace Qanat.API.Controllers;

[ApiController]
[RightsChecker]
public class WaterTypeController : SitkaController<WaterTypeController>
{
    public WaterTypeController(QanatDbContext dbContext, ILogger<WaterTypeController> logger,
        IOptions<QanatConfiguration> qanatConfiguration)
        : base(dbContext, logger, qanatConfiguration)
    {
    }

    [HttpGet("geographies/{geographyID}/water-types/")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [WithGeographyRolePermission(PermissionEnum.WaterTypeRights, RightsEnum.Read)]
    public ActionResult<List<WaterTypeSimpleDto>> GetWaterTypes([FromRoute] int geographyID)
    {
        var waterTypeDtos = WaterTypes.ListWaterTypesAsSimpleDto(_dbContext, geographyID);
        return Ok(waterTypeDtos);
    }

    [HttpGet("geographies/{geographyID}/water-types/active")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [WithGeographyRolePermission(PermissionEnum.WaterTypeRights, RightsEnum.Read)]
    public ActionResult<List<WaterTypeSimpleDto>> GetActiveWaterTypes([FromRoute] int geographyID)
    {
        var waterTypeDtos = _dbContext.WaterTypes.Where(x => x.IsActive == true && x.GeographyID == geographyID)
            .OrderBy(x => x.SortOrder)
            .Select(x => x.AsSimpleDto()).ToList();
        return Ok(waterTypeDtos);
    }

    [HttpPost("geographies/{geographyID}/water-type/update")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [WithGeographyRolePermission(PermissionEnum.WaterTypeRights, RightsEnum.Update)]
    public ActionResult<List<WaterTypeSimpleDto>> UpdateWaterTypes([FromBody] WaterTypeSimpleDto[] waterTypeDtos, [FromRoute] int geographyID)
    {
        var updatedWaterTypes = waterTypeDtos.Select(x => new WaterType()
        {
            WaterTypeID = x.WaterTypeID,
            WaterTypeName = x.WaterTypeName,
            WaterTypeSlug = x.WaterTypeName.Replace(" ", "-")?.ToLower(),
            GeographyID = geographyID,
            IsAppliedProportionally = x.IsAppliedProportionally,
            IsSourcedFromApi = x.IsSourcedFromApi,
            WaterTypeDefinition = x.WaterTypeDefinition,
            SortOrder = x.SortOrder,
            IsActive = x.IsActive
        }).ToList();

        var allInDatabase = _dbContext.WaterTypes;
        var existingWaterTypes = allInDatabase.Where(x => x.GeographyID == geographyID).ToList();

        existingWaterTypes.Merge(updatedWaterTypes, allInDatabase,
            (x, y) => _dbContext.Entry(x).Property(e => e.WaterTypeID).CurrentValue 
                == _dbContext.Entry(y).Property(e => e.WaterTypeID).CurrentValue,
            (x, y) =>
            {
                x.WaterTypeName = y.WaterTypeName;
                x.GeographyID = y.GeographyID;
                x.IsAppliedProportionally = y.IsAppliedProportionally;
                x.WaterTypeDefinition = y.WaterTypeDefinition;
                x.WaterTypeSlug = y.WaterTypeSlug;
                x.IsSourcedFromApi = y.IsSourcedFromApi;
                x.SortOrder = y.SortOrder;
                x.IsActive = y.IsActive;
            });

        _dbContext.SaveChanges();

        return Ok(WaterTypes.ListWaterTypesAsSimpleDto(_dbContext, geographyID));
    }
}