using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Qanat.API.Services;
using Qanat.API.Services.Attributes;
using Qanat.API.Services.Authorization;
using Qanat.Common.Services.GDAL;
using Qanat.EFModels.Entities;
using Qanat.Models.DataTransferObjects;
using Qanat.Models.Security;
using System.Threading.Tasks;

namespace Qanat.API.Controllers;

[ApiController]
[RightsChecker]
[Route("geographies/{geographyID}/well-types")]
public class WellTypeController : SitkaController<WellTypeController>
{
    public WellTypeController(QanatDbContext dbContext, ILogger<WellTypeController> logger, IOptions<QanatConfiguration> qanatConfiguration, FileService fileService, GDALAPIService gdalApiService, HierarchyContext hierarchyContext)
        : base(dbContext, logger, qanatConfiguration)
    {
    }

    [HttpGet("{wellTypeID}")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [EntityNotFound(typeof(WellType), "wellTypeID")]
    [WithGeographyRolePermission(PermissionEnum.WellRights, RightsEnum.Read)]
    public async Task<ActionResult<WellTypeDto>> Get([FromRoute] int geographyID, [FromRoute] int wellTypeID)
    {
        var wellType = await WellTypes.GetAsync(_dbContext, geographyID, wellTypeID);
        return Ok(wellType);
    }
}