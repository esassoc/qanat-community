using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Qanat.API.Services;
using Qanat.API.Services.Authorization;
using Qanat.EFModels.Entities;
using Qanat.Models.DataTransferObjects;
using Qanat.Models.Helpers;
using System;
using System.Linq;

namespace Qanat.API.Controllers
{
    [ApiController]
    [RightsChecker]
    public class UserClaimsController : SitkaController<UserClaimsController>
    {
        public UserClaimsController(QanatDbContext dbContext, ILogger<UserClaimsController> logger, IOptions<QanatConfiguration> qanatConfiguration)
            : base(dbContext, logger, qanatConfiguration)
        {
        }

        [HttpGet("user-claims/{globalID}")]
        [UserClaimsAuthorization]
        public ActionResult<UserDto> GetByGlobalID([FromRoute] string globalID)
        {
            var isValidGuid = Guid.TryParse(globalID, out var globalIDAsGuid);
            if (!isValidGuid)
            {
                return BadRequest();
            }

            var userDto = Users.GetByUserGuid(_dbContext, globalIDAsGuid);
            if (userDto == null)
            {
                var notFoundMessage = $"User with GUID {globalIDAsGuid} does not exist!";
                _logger.LogError(notFoundMessage);
                return NotFound(notFoundMessage);
            }

            userDto = ImpersonationService.RetrieveImpersonatedUserIfImpersonating(_dbContext, userDto);

            return Ok(userDto);
        }

        [HttpPost("user-claims")]
        [Authorize]
        public ActionResult<UserDto> PostUserClaims([FromServices] HttpContext httpContext)
        {
            var claimsPrincipal = httpContext.User;
            if (!claimsPrincipal.Claims.Any())  // Updating user based on claims does not work when there are no claims
            {
                return BadRequest();
            }

            UserDto claimsUserDto = null;
            var isClient = claimsPrincipal.Claims.Any(c => c.Type == ClaimsConstants.IsClient);
            if (isClient)  // Not appropriate to actually update client user based on claims
            {
                var clientID = claimsPrincipal.Claims.SingleOrDefault(c => c.Type == ClaimsConstants.ClientID)?.Value;
                if (!string.IsNullOrEmpty(clientID))
                {
                    var globalID = Guid.Parse(clientID);
                    claimsUserDto = Users.GetByUserGuid(_dbContext, globalID);
                }
                return Ok(claimsUserDto);
            }

            var subClaim = claimsPrincipal.Claims.SingleOrDefault(c => c.Type == ClaimsConstants.Sub)?.Value;
            if (!string.IsNullOrEmpty(subClaim))
            {
                var globalID = Guid.Parse(subClaim);
                claimsUserDto = Users.GetByUserGuid(_dbContext, globalID);
            }

            var updatedUserDto = Users.UpdateClaims(_dbContext, claimsUserDto?.UserID, claimsPrincipal);

            return Ok(updatedUserDto);
        }
    }
}