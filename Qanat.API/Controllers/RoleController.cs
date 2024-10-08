using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Qanat.API.Services;
using Qanat.API.Services.Authorization;
using Qanat.EFModels.Entities;
using Qanat.Models.DataTransferObjects;
using Qanat.Models.Security;

namespace Qanat.API.Controllers
{
    [ApiController]
    [RightsChecker]
    public class RoleController : SitkaController<RoleController>
    {
        public RoleController(QanatDbContext dbContext, ILogger<RoleController> logger, IOptions<QanatConfiguration> qanatConfiguration)
            : base(dbContext, logger, qanatConfiguration)
        {
        }

        [HttpGet("roles")]
        [WithRolePermission(PermissionEnum.UserRights, RightsEnum.Read)]  // Roles are just a lookup table so they don't have their own rights
        public ActionResult<List<RoleDto>> GetAllRoles()
        {
            var roleDtos = Roles.List();
            return Ok(roleDtos);
        }
    }
}
