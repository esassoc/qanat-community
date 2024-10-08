using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Qanat.API.Services;
using Qanat.API.Services.Attributes;
using Qanat.API.Services.Authorization;
using Qanat.EFModels.Entities;
using Qanat.Models.DataTransferObjects;
using Qanat.Models.DataTransferObjects.User;
using Qanat.Models.Security;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Qanat.API.Controllers
{
    [ApiController]
    [RightsChecker]
    public class UserController : SitkaController<UserController>
    {
        public UserController(QanatDbContext dbContext, ILogger<UserController> logger, IOptions<QanatConfiguration> qanatConfiguration)
            : base(dbContext, logger, qanatConfiguration)
        {
        }

        [HttpGet("users")]
        [WithRolePermission(PermissionEnum.UserRights, RightsEnum.Read)]
        public ActionResult<IEnumerable<UserDto>> List()
        {
            var userDtos = Users.List(_dbContext);
            return Ok(userDtos);
        }

        [HttpGet("users/unassigned-report")]
        [WithRolePermission(PermissionEnum.UserRights, RightsEnum.Read)]
        public ActionResult<UnassignedUserReportDto> GetUnassignedUserReport()
        {
            var report = new UnassignedUserReportDto
            { Count = _dbContext.Users.Count(x => x.RoleID == (int)RoleEnum.NoAccess && x.IsActive) };
            return Ok(report);
        }

        [HttpGet("users/{userID}")]
        [WithRolePermission(PermissionEnum.UserRights, RightsEnum.Read)]
        public ActionResult<UserDto> GetByUserID([FromRoute] int userID)
        {
            var userDto = Users.GetByUserID(_dbContext, userID);
            return RequireNotNullThrowNotFound(userDto, "User", userID);
        }

        [HttpGet("pending-users")]
        [WithRolePermission(PermissionEnum.UserRights, RightsEnum.Read)]
        public ActionResult<List<UserDto>> GetAllPendingUsers()
        {
            var pendingUsersDtos = _dbContext.Users.Include(x => x.WaterAccountUsers).AsNoTracking()
                .Where(x => x.RoleID == (int)RoleEnum.PendingLogin)
                .Select(x => x.AsUserDto()).ToList();
            return Ok(pendingUsersDtos);
        }

        [HttpPut("users/{userID}")]
        [EntityNotFound(typeof(User), "userID")]
        [WithRolePermission(PermissionEnum.UserRights, RightsEnum.Update)]
        public async Task<ActionResult<UserDto>> UpdateUser([FromRoute] int userID, [FromBody] UserUpsertDto userUpsertDto)
        {
            var validationMessages =
                Users.ValidateUpdate(_dbContext, userUpsertDto, userID);
            validationMessages.ForEach(vm => { ModelState.AddModelError(vm.Type, vm.Message); });

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var role = Role.AllLookupDictionary[userUpsertDto.RoleID.GetValueOrDefault()];
            if (role == null)
            {
                return BadRequest($"Could not find a System Role with the ID {userUpsertDto.RoleID}");
            }

            var updatedUserDto = await Users.UpdateUserEntity(_dbContext, userID, userUpsertDto);

            return Ok(updatedUserDto);
        }

        [HttpGet("users/normal-users")]
        [WithRolePermission(PermissionEnum.UserRights, RightsEnum.Read)]
        public ActionResult<IEnumerable<UserDetailedDto>> ListNormalUsers()
        {
            var userDtos = Users.ListByRole(_dbContext,
                new List<int> { (int)RoleEnum.Normal })
                .OrderBy(x => x.LastName);

            return Ok(userDtos);
        }

        [HttpGet("user/geography-summary")]
        [AuthenticatedWithUser]
        public ActionResult<List<UserGeographySummaryDto>> GetGeographySummaryForUser()
        {
            var user = UserContext.GetUserFromHttpContext(_dbContext, HttpContext);
            var userGeographySummaryDtos = Users.ListUserGeographySummariesByUserID(_dbContext, user.UserID);

            return Ok(userGeographySummaryDtos);
        }

        [HttpGet("geographies/{geographyID}/landing-page")]
        [EntityNotFound(typeof(Geography), "geographyID")]
        [WithRoleFlag(FlagEnum.CanClaimWaterAccounts)]
        public ActionResult<GeographyLandingPageDto> GetNumberOfWellsAndParcelsRegisteredToUser([FromRoute] int geographyID)
        {
            var user = UserContext.GetUserFromHttpContext(_dbContext, HttpContext);
            var wellRegistrationCount = Users.GetNumberOfWellRegistrationsForUser(_dbContext, user.UserID, geographyID);
            var waterAccountCount = Users.GetNumberOfWaterAccountForUser(_dbContext, user.UserID, geographyID);
            return Ok(new GeographyLandingPageDto()
            {
                NumberOfWaterAccounts = waterAccountCount,
                NumberOfWellRegistrations = wellRegistrationCount
            });
        }

        [HttpGet("user/well-registrations")]
        [WithRoleFlag(FlagEnum.CanRegisterWells)]
        public ActionResult<List<WellRegistrationMinimalDto>> ListWellRegistrationsForCurrentUser()
        {
            var user = UserContext.GetUserFromHttpContext(_dbContext, HttpContext);
            var wellRegistrationMinimalDtos = WellRegistrations.ListByUserAsWellRegistrationUserDetailDto(_dbContext, user.UserID);
            return Ok(wellRegistrationMinimalDtos);
        }

        //MK 7/18/2024 - I waffled on adding this to this controller or the well registration controller. Might be a case where we'd want a UserWellRegistrationController or something similar?
        [HttpGet("users/{userID}/well-registrations")]
        [EntityNotFound(typeof(User), "userID")]
        [WithRolePermission(PermissionEnum.UserRights, RightsEnum.Read)]
        public ActionResult<List<WellRegistrationMinimalDto>> ListWellRegistrationsForUser([FromRoute] int userID)
        {
            var wellRegistrationMinimalDtos = WellRegistrations.ListByUserAsWellRegistrationUserDetailDto(_dbContext, userID);
            return Ok(wellRegistrationMinimalDtos);
        }

        [HttpGet("geographies/{geographyID}/users/{userID}/water-accounts")]
        [EntityNotFound(typeof(Geography), "geographyID")]
        [WithRoleFlag(FlagEnum.CanClaimWaterAccounts)]
        public ActionResult ListWaterAccountsOwnedByCurrentUser([FromRoute] int geographyID, [FromRoute] int userID)
        {
            var waterAccountDtos = WaterAccounts.ListByGeographyIDAndUserIDAsWaterAccountRequestChangesDto(_dbContext, geographyID, userID);

            return Ok(waterAccountDtos);
        }

        [HttpPut("geographies/{geographyID}/users/{userID}/water-accounts")]
        [EntityNotFound(typeof(Geography), "geographyID")]
        [WithRoleFlag(FlagEnum.CanClaimWaterAccounts)]
        public async Task<ActionResult> UpdateWaterAccountsOwnedByCurrentUser([FromRoute] int geographyID, [FromRoute] int userID, [FromBody] WaterAccountParcelsRequestChangesDto requestDto)
        {
            var errors = WaterAccounts.ValidateRequestedWaterAccountChanges(_dbContext, geographyID, userID, requestDto);
            errors.ForEach(vm => { ModelState.AddModelError(vm.Type, vm.Message); });
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await WaterAccounts.ApplyRequestedWaterAccountChanges(_dbContext, geographyID, userID, requestDto);
            return Ok();
        }
    }
}

