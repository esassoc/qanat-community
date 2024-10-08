using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Qanat.API.Services;
using Qanat.API.Services.Attributes;
using Qanat.API.Services.Authorization;
using Qanat.EFModels.Entities;
using Qanat.Models.DataTransferObjects;
using Qanat.Models.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using Qanat.Common.GeoSpatial;

namespace Qanat.API.Controllers;

[ApiController]
[RightsChecker]
public class WaterAccountUserController : SitkaController<WaterAccountUserController>
{
    protected readonly SitkaSmtpClientService _sitkaSmtpClientService;

    public WaterAccountUserController(QanatDbContext dbContext, 
        ILogger<WaterAccountUserController> logger, 
        IOptions<QanatConfiguration> qanatConfiguration,
        SitkaSmtpClientService sitkaSmtpClientService) : base(dbContext, logger, qanatConfiguration)
    {
        _sitkaSmtpClientService = sitkaSmtpClientService;
    }

    [HttpGet("geographies/{geographyID}/waterAccountPINs")]
    [WithRoleFlag(FlagEnum.CanClaimWaterAccounts)]
    public ActionResult<List<OnboardWaterAccountPINDto>> GetUserWaterAccountPINsForGeography([FromRoute] int geographyID)
    {
        var user = UserContext.GetUserFromHttpContext(_dbContext, HttpContext);
        var onboardWaterAccountPINDtos = WaterAccountUserExtensionMethods.GetOnboardWaterAccountPINDtos(_dbContext, geographyID, user.UserID);

        return Ok(onboardWaterAccountPINDtos);
    }

    [HttpGet("water-accounts/{waterAccountID}/users")]
    [EntityNotFound(typeof(WaterAccount), "waterAccountID")]
    [WithWaterAccountRolePermission(PermissionEnum.WaterAccountRights, RightsEnum.Read)]
    public ActionResult<List<WaterAccountUserMinimalDto>> GetAllUsersForWaterAccount([FromRoute] int waterAccountID)
    {
        var users = _dbContext.WaterAccountUsers.Include(x => x.User)
            .Include(x => x.WaterAccount)
            .ThenInclude(x => x.Geography)
            .Where(x => x.WaterAccountID == waterAccountID)
            .OrderBy(x => x.WaterAccountRoleID).ThenBy(x => x.ClaimDate)
            .Select(x => x.AsWaterAccountUserMinimalDto()).ToList();

        return users;
    }

    [HttpGet("/user/{userID}/water-accounts")]
    [WithRolePermission(PermissionEnum.UserRights, RightsEnum.Read)]
    public ActionResult<List<WaterAccountUserMinimalDto>> GetUserWaterAccounts([FromRoute] int userID)
    {
        var dtos = WaterAccountUsers.GetWaterAccountUsersForUserID(_dbContext, userID);
        return dtos;
    }

    [HttpPut("/user/{userID}/water-accounts")]
    [WithRolePermission(PermissionEnum.UserRights, RightsEnum.Update)]
    public ActionResult<List<WaterAccountUserMinimalDto>> UpdateUserWaterAccounts([FromRoute] int userID, [FromBody] List<WaterAccountUserMinimalDto> waterAccountUserSimpleDtos)
    {
        var waterAccountUsers = WaterAccountUsers.UpdateUserWaterAccounts(_dbContext, userID, waterAccountUserSimpleDtos);
        return waterAccountUsers;
    }

    [HttpGet("geographies/{geographyID}/water-account")]
    [WithRoleFlag(FlagEnum.CanClaimWaterAccounts)]
    public ActionResult<List<OnboardingWaterAccountDto>> GetUserAccountsForGeography([FromRoute] int geographyID)
    {
        var user = UserContext.GetUserFromHttpContext(_dbContext, HttpContext);
        var accountDtos = _dbContext.WaterAccountUserStagings.AsNoTracking()
            .Include(x => x.WaterAccount)
                .ThenInclude(x => x.Parcels)
                    .ThenInclude(x => x.ParcelGeometry)
            .Where(x => x.WaterAccount.GeographyID == geographyID && x.UserID == user.UserID).ToList();

        var onboardingWaterAccountDtos = accountDtos
            .Select(waterAccountUserStaging => new OnboardingWaterAccountDto()
            {
                WaterAccountID = waterAccountUserStaging.WaterAccount.WaterAccountID,
                WaterAccountName = waterAccountUserStaging.WaterAccount.WaterAccountName,
                WaterAccountNumber = waterAccountUserStaging.WaterAccount.WaterAccountNumber,
                WaterAccountPIN = waterAccountUserStaging.WaterAccount.WaterAccountPIN,
                ParcelNumbers = waterAccountUserStaging.WaterAccount.Parcels?.Select(x => x.ParcelNumber).ToList(),
                ParcelGeoJson = waterAccountUserStaging.WaterAccount.Parcels?.Select(x => x.ParcelGeometry.Geometry4326.ToGeoJSON()).ToList()
            })
            .ToList();

        return Ok(onboardingWaterAccountDtos);
    }

    [HttpPost("geographies/{geographyID}/water-account/waterAccountPIN/{waterAccountPIN}")]
    [WithRoleFlag(FlagEnum.CanClaimWaterAccounts)]
    public ActionResult SetAccountToUserStaging([FromRoute] int geographyID, string waterAccountPIN)
    {
        var user = UserContext.GetUserFromHttpContext(_dbContext, HttpContext);

        var waterAccount = _dbContext.WaterAccounts.AsNoTracking()
            .SingleOrDefault(x => x.GeographyID == geographyID && x.WaterAccountPIN == waterAccountPIN);
        if (waterAccount == null)
        {
            return BadRequest(("Water Account PIN not found within current geography."));
        }

        var waterAccountID = waterAccount.WaterAccountID;
        var accountUser = _dbContext.WaterAccountUserStagings.SingleOrDefault(x => x.UserID == user.UserID && x.WaterAccountID == waterAccountID);
        if (accountUser == null)
        {
            accountUser = new WaterAccountUserStaging()
            {
                WaterAccountID = waterAccountID,
                UserID = user.UserID,
                ClaimDate = DateTime.UtcNow
            };
            _dbContext.WaterAccountUserStagings.Add(accountUser);
            _dbContext.SaveChanges();
        }

        return Ok();
    }

    [HttpPost("geographies/{geographyID}/water-accounts/claim")]
    [WithRoleFlag(FlagEnum.CanClaimWaterAccounts)]
    public ActionResult UpdateWaterAccountUsers([FromRoute] int geographyID, [FromBody] List<OnboardingWaterAccountDto> onboardingWaterAccountDtos)
    {
        var user = UserContext.GetUserFromHttpContext(_dbContext, HttpContext);

        var errors = WaterAccountUsers.ValidateClaimWaterAccounts(_dbContext, user.UserID, onboardingWaterAccountDtos);
        errors.ForEach(vm => { ModelState.AddModelError(vm.Type, vm.Message); });
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        WaterAccountUsers.ClaimWaterAccounts(_dbContext, user.UserID, onboardingWaterAccountDtos);
        
        return Ok();
    }

    [HttpGet("water-account-roles")]
    [WithRoleFlag(FlagEnum.CanClaimWaterAccounts)]
    public ActionResult<List<WaterAccountRoleSimpleDto>> GetWaterAccountRoles()
    {
        return Ok(WaterAccountRole.AllAsSimpleDto);
    }

    [HttpPost("water-accounts/{waterAccountID}/inviting-user/{invitingUserID}")]
    [EntityNotFound(typeof(WaterAccount), "waterAccountID")]
    [WithWaterAccountRolePermission(PermissionEnum.WaterAccountUserRights, RightsEnum.Create)]
    public ActionResult<WaterAccountUserMinimalDto> AddUserOnWaterAccountByEmail([FromRoute] int waterAccountID, int invitingUserID, [FromBody] AddUserByEmailDto addUserByEmailDto)
    {
        var errors = WaterAccountUsers.ValidateAddUserData(_dbContext, addUserByEmailDto);
        var emailErrors = WaterAccountUsers.ValidateEmail(addUserByEmailDto.Email);
        errors.ForEach(vm => { ModelState.AddModelError(vm.Type, vm.Message); });
        emailErrors.ForEach(vm => { ModelState.AddModelError(vm.Type, vm.Message); });
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var user = _dbContext.Users.SingleOrDefault(x => x.Email == addUserByEmailDto.Email.Replace(" ", string.Empty));
        var invitingUser = _dbContext.Users.Single(x => x.UserID == invitingUserID);
        var waterAccount = _dbContext.WaterAccounts.Single(x => x.WaterAccountID == waterAccountID);
        if (user != null && user.Role != Role.PendingLogin)
        {
            var waterAccountUser =
                _dbContext.WaterAccountUsers.SingleOrDefault(x =>
                    x.WaterAccountID == waterAccountID && x.UserID == user.UserID);

            if (waterAccountUser == null)
            {
                var mailMessageActiveUser = WaterAccountUsers.AddUserToWaterAccount(_dbContext, waterAccountID, addUserByEmailDto,
                    user, invitingUser);
                _sitkaSmtpClientService.Send(mailMessageActiveUser);

                return _dbContext.WaterAccountUsers.Include(x=>x.WaterAccount).Single(x =>
                   x.WaterAccountID == waterAccountID && x.UserID == user.UserID).AsWaterAccountUserMinimalDto();
            }
            else
            {
                return BadRequest("User is already added on this water account.");
            }
        }

        if (user == null)
        {
            var pendingUser = new User()
            {
                Email = addUserByEmailDto.Email,
                FirstName = addUserByEmailDto.Email,
                LastName = addUserByEmailDto.Email,
                CreateDate = DateTime.UtcNow,
                RoleID = (int)RoleEnum.PendingLogin,
                IsActive = false,
                ReceiveSupportEmails = false,
            };
            _dbContext.Users.Add(pendingUser);
            _dbContext.SaveChanges();
            user = pendingUser;
        }
        
        var pendingWaterAccountUser = new WaterAccountUser()
        {
            WaterAccountID = waterAccountID,
            UserID = user.UserID,
            WaterAccountRoleID = addUserByEmailDto.WaterAccountRoleID,
            ClaimDate = DateTime.UtcNow
        };
        _dbContext.WaterAccountUsers.Add(pendingWaterAccountUser);

        GeographyUsers.AddGeographyNormalUserIfAbsent(_dbContext, user.UserID, waterAccount.GeographyID);
        _dbContext.SaveChanges();

        var mailMessage = new MailMessage
        {
            Subject = $"Water Account Invitation in the Groundwater Accounting Platform",
            Body = $"Hello,<br /><br />" +
                   $"{invitingUser.FullName} has added you to their {waterAccount.WaterAccountName} Water Account in the Groundwater Accounting Platform. <br /><br />" +
                   $"If this is your first time on the platform, you will need to sign up for a user account. You can do that here: <br />" +
                   $"https://groundwateraccounting.org. <br /><br />" +
                   $"Be sure to sign up with the email address {invitingUser.FullName} invited you with ({addUserByEmailDto.Email}) so you can be automatically associated with this Water Account. <br /><br />" +
                   $"If you think you have been added to this Water Account in error, please contact {invitingUser.FullName} at {invitingUser.Email}.<br /><br />" +
                   $"Thank you,<br />The Groundwater Accounting Platform Team",
            IsBodyHtml = true
        };

        mailMessage.To.Add(new MailAddress(addUserByEmailDto.Email));
        _sitkaSmtpClientService.Send(mailMessage);
        return pendingWaterAccountUser.AsWaterAccountUserMinimalDto();
    }

    [HttpPut("water-accounts/{waterAccountID}/user/{userID}")]
    [EntityNotFound(typeof(WaterAccount), "waterAccountID")]
    [WithWaterAccountRolePermission(PermissionEnum.WaterAccountUserRights, RightsEnum.Update)]
    public ActionResult UpdateWaterAccountUser([FromRoute] int waterAccountID, int userID, [FromBody] WaterAccountUserMinimalDto waterAccountUserDto)
    {
        var waterAccountUser = _dbContext.WaterAccountUsers.Single(x => x.WaterAccountID == waterAccountID && x.UserID == userID);
        waterAccountUser.WaterAccountRoleID = waterAccountUserDto.WaterAccountRoleID;
        _dbContext.SaveChanges();
        return Ok();

    }

    [HttpPost("water-accounts/{waterAccountID}/inviting-user/{invitingUserID}/resend")]
    [EntityNotFound(typeof(WaterAccount), "waterAccountID")]
    [WithWaterAccountRolePermission(PermissionEnum.WaterAccountUserRights, RightsEnum.Update)]
    public ActionResult ResendInvitationToPendingUser([FromRoute] int waterAccountID, int invitingUserID,
        [FromBody] WaterAccountUserMinimalDto waterAccountUserMinimalDto)
    {
        var invitingUser = _dbContext.Users.Single(x => x.UserID == invitingUserID);
        var waterAccount = _dbContext.WaterAccounts.Single(x => x.WaterAccountID == waterAccountID);
        var inviteeEmail = _dbContext.Users.Single(x => x.UserID == waterAccountUserMinimalDto.UserID).Email;
        var mailMessage = new MailMessage
        {
            Subject = $"Water Account Invitation in the Groundwater Accounting Platform",
            Body = $"Hello,<br /><br />" +
                   $"{invitingUser.FullName} has added you to their {waterAccount.WaterAccountName} Water Account in the Groundwater Accounting Platform. <br /><br />" +
                   $"If this is your first time on the platform, you will need to sign up for a user account. You can do that here: <br />" +
                   $"https://groundwateraccounting.org. <br /><br />" +
                   $"Be sure to sign up with the email address {invitingUser.FullName} invited you with ({inviteeEmail}) so you can be automatically associated with this Water Account. <br /><br />" +
                   $"If you think you have been added to this Water Account in error, please contact {invitingUser.FullName} at {invitingUser.Email}.<br /><br />" +
                   $"Thank you,<br />The Groundwater Accounting Platform Team",
            IsBodyHtml = true
        };

        mailMessage.To.Add(new MailAddress(waterAccountUserMinimalDto.UserEmail));
        _sitkaSmtpClientService.Send(mailMessage);

        WaterAccountUsers.UpdatePendingDate(_dbContext, waterAccountUserMinimalDto.UserEmail, waterAccountID);
        return Ok();
    }

    [HttpDelete("water-accounts/{waterAccountID}/user/{waterAccountUserID}")]
    [EntityNotFound(typeof(WaterAccount), "waterAccountID")]
    [WithWaterAccountRolePermission(PermissionEnum.WaterAccountUserRights, RightsEnum.Delete)]
    public ActionResult RemoveUserFromWaterAccount([FromRoute] int waterAccountID, [FromRoute] int waterAccountUserID)
    {
        WaterAccountUsers.RemoveUserFromWaterAccount(_dbContext, waterAccountUserID);
        return Ok();
    }

}