using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Qanat.API.Services;
using Qanat.API.Services.Attributes;
using Qanat.API.Services.Authorization;
using Qanat.EFModels.Entities;
using Qanat.Models.DataTransferObjects;
using Qanat.Models.DataTransferObjects.Geography;
using Qanat.Models.DataTransferObjects.User;
using Qanat.Models.Security;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Qanat.API.Controllers
{
    [ApiController]
    [RightsChecker]
    [Route("users")]
    public class UserController(QanatDbContext dbContext, ILogger<UserController> logger, IOptions<QanatConfiguration> qanatConfiguration, UserDto callingUser)
        : SitkaController<UserController>(dbContext, logger, qanatConfiguration)
    {
        [HttpGet]
        [WithRolePermission(PermissionEnum.UserRights, RightsEnum.Read)]
        public ActionResult<IEnumerable<UserDto>> List()
        {
            var userDtos = Users.List(_dbContext);
            return Ok(userDtos);
        }

        [HttpGet("{userID}")]
        [WithRolePermission(PermissionEnum.UserRights, RightsEnum.Read)]
        public ActionResult<UserDto> Get([FromRoute] int userID)
        {
            var userDto = Users.GetByUserID(_dbContext, userID);
            return RequireNotNullLogIfNotFound(userDto, "User", userID);
        }

        [HttpPut("{userID}")]
        [EntityNotFound(typeof(User), "userID")]
        [WithRolePermission(PermissionEnum.UserRights, RightsEnum.Update)]
        public async Task<ActionResult<UserDto>> Update([FromRoute] int userID, [FromBody] UserUpsertDto userUpsertDto)
        {
            var validationMessages = Users.ValidateUpdate(_dbContext, userUpsertDto, userID);
            validationMessages.ForEach(vm => { ModelState.AddModelError(vm.Type, vm.Message); });

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updatedUserDto = await Users.UpdateUserEntity(_dbContext, userID, userUpsertDto);
            return Ok(updatedUserDto);
        }

        [HttpGet("{userID}/api-key")]
        [EntityNotFound(typeof(User), "userID")]
        [WithRolePermission(PermissionEnum.UserRights, RightsEnum.Read)]
        public async Task<ActionResult<string>> GetApiKey([FromRoute] int userID)
        {
            var apiKey = await Users.GetApiKeyByUserID(_dbContext, userID);
            return Ok(apiKey);
        }

        [HttpPost("{userID}/api-key")]
        [EntityNotFound(typeof(User), "userID")]
        [WithRolePermission(PermissionEnum.UserRights, RightsEnum.Read)]
        public async Task<ActionResult<string>> GenerateNewApiKey([FromRoute] int userID)
        {
            var newApiKey = await Users.GenerateApiKeyAsync(_dbContext, userID);
            return Ok(newApiKey);
        }

        [HttpGet("pending")]
        [WithRolePermission(PermissionEnum.UserRights, RightsEnum.Read)]
        public ActionResult<List<UserDto>> ListPending()
        {
            var pendingUsersDtos = _dbContext.Users.AsNoTracking()
                .Include(x => x.WaterAccountUsers)
                .Include(x => x.ModelUsers)
                .Where(x => x.RoleID == (int)RoleEnum.PendingLogin)
                .Select(x => x.AsUserDto()).ToList();
            return Ok(pendingUsersDtos);
        }

        [HttpGet("normal")]
        [WithRolePermission(PermissionEnum.UserRights, RightsEnum.Read)]
        public ActionResult<IEnumerable<UserDto>> ListNormal()
        {
            var userDtos = Users.ListByRole(_dbContext,
                new List<int> { (int)RoleEnum.Normal })
                .OrderBy(x => x.LastName);

            return Ok(userDtos);
        }

        [HttpGet("geography-summary")]
        [AuthenticatedWithUser]
        public ActionResult<List<UserGeographySummaryDto>> GetGeographySummary()
        {
            var userGeographySummaryDtos = Users.ListUserGeographySummariesByUserID(_dbContext, callingUser.UserID);
            
            return Ok(userGeographySummaryDtos);
        }

        //MK 7/18/2024 - I waffled on adding this to this controller or the well registration controller. Might be a case where we'd want a UserWellRegistrationController or something similar?
        [HttpGet("{userID}/well-registrations")]
        [EntityNotFound(typeof(User), "userID")]
        [WithRolePermission(PermissionEnum.UserRights, RightsEnum.Read)]
        public ActionResult<List<WellRegistrationUserDetailDto>> ListWellRegistrations([FromRoute] int userID)
        {
            var wellRegistrationDtos = WellRegistrations.ListByUserAsWellRegistrationUserDetailDto(_dbContext, userID);
            return Ok(wellRegistrationDtos);
        }

        [HttpGet("{userID}/permissions")]
        [WithRolePermission(PermissionEnum.GeographyRights, RightsEnum.Read)]
        public ActionResult<List<GeographyUserDto>> GetGeographyPermissions([FromRoute] int userID)
        {
            var permissions = _dbContext.GeographyUsers
                .Include(x => x.Geography)
                .Include(x => x.User).ThenInclude(x => x.WaterAccountUsers).ThenInclude(x => x.WaterAccount)
                .Include(x => x.User).ThenInclude(x => x.WellRegistrations)
                .Where(x => x.UserID == userID)
                .OrderBy(x => x.Geography.GeographyDisplayName)
                .Select(x => x.AsGeographyUserDto())
                .ToList();

            return Ok(permissions);
        }
    }
}

