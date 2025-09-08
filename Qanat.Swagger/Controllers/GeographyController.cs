using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Qanat.EFModels.Entities;
using Qanat.Models.DataTransferObjects;
using Qanat.Swagger.Filters;
using Qanat.Swagger.Models;
using System.Collections.Generic;
using System.Linq;
using Qanat.Swagger.Entities;

namespace Qanat.Swagger.Controllers;

[Authorize]
[ApiController]
[Tags("Geographies")]
public class GeographyController : ControllerBase
{
    private readonly QanatDbContext dbContext;
    private readonly ILogger<GeographyController> logger;

    public GeographyController(QanatDbContext dbContext, ILogger<GeographyController> logger)
    {
        this.dbContext = dbContext;
        this.logger = logger;
    }

    [EndpointSummary("List")]
    [EndpointDescription("List all available geographies")]
    [NoAccessBlock]
    [HttpGet("geographies")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [Produces("application/json")]
    public ActionResult<List<GeographyConsumerDto>> ListGeographies()
    {
        var userID = HttpContext.User.GetUserID()!;
        var callingUser = Users.GetByUserID(dbContext, userID.Value);

        var geographies = ListGeographiesForUser(callingUser);
        return Ok(geographies);
    }

    private List<GeographyConsumerDto> ListGeographiesForUser(UserDto callingUser)
    {
        List<int> geographyIDs;

        if (UserPermissions.UserIsSystemAdmin(callingUser))
        {
            geographyIDs = dbContext.Geographies.AsNoTracking().Select(x => x.GeographyID).ToList();
        }
        else
        {
            var managedGeographyIDs = callingUser.GeographyFlags
                .Where(x => x.Value[Flag.HasManagerDashboard.FlagName])
                .Select(x => x.Key).ToList();

            var associatedWaterAccountGeographyIDs = dbContext.WaterAccounts.AsNoTracking()
                .Include(x => x.WaterAccountUsers)
                .Where(x => x.WaterAccountUsers.Any(y => y.UserID == callingUser.UserID))
                .Select(x => x.GeographyID)
                .Distinct()
                .ToList();

            geographyIDs = managedGeographyIDs.Union(associatedWaterAccountGeographyIDs).Distinct().ToList();
        }

        var geographyConsumerDtos = dbContext.Geographies.AsNoTracking()
            .Where(x => geographyIDs.Contains(x.GeographyID))
            .Select(x => new GeographyConsumerDto
            {
                GeographyID = x.GeographyID,
                GeographyName = x.GeographyName,
                GeographyDisplayName = x.GeographyDisplayName
            }).ToList();

        return geographyConsumerDtos;
    }
}