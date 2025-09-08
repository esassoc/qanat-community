using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Qanat.EFModels.Entities;
using Qanat.Swagger.Entities;
using Qanat.Swagger.Filters;
using Qanat.Swagger.Models;
using System.Collections.Generic;
using System.Linq;

namespace Qanat.Swagger.Controllers;

[Authorize]
[ApiController]
[Tags("Water Accounts")]
public class WaterAccountController(QanatDbContext dbContext, ILogger<WaterAccountController> logger) : ControllerBase
{
    [EndpointSummary("List by Geography")]
    [EndpointDescription("List all water accounts for a specified geography")]
    [NoAccessBlock]
    [GeographyAccess]
    [HttpGet("geographies/{geographyID}/water-accounts")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Produces("application/json")]
    public ActionResult<List<WaterAccountConsumerDto>> ListWaterAccounts([FromRoute] int geographyID)
    {
        var userID = HttpContext.User.GetUserID()!;
        var callingUser = Users.GetByUserID(dbContext, userID.Value);

        List<int> waterAccountIDs;

        if (UserPermissions.UserIsSystemAdmin(callingUser) || UserPermissions.UserIsGeographyManager(callingUser, geographyID))
        {
            waterAccountIDs = dbContext.WaterAccounts.AsNoTracking()
                .Where(x => x.GeographyID == geographyID).Select(x => x.WaterAccountID).ToList();
        }
        else
        {
            var associatedWaterAccountIDs = UserPermissions.ListAssociatedWaterAccountsByGeographyIDAndUser(dbContext, geographyID, callingUser);

            waterAccountIDs = dbContext.WaterAccounts.AsNoTracking()
                .Where(x => x.GeographyID == geographyID && associatedWaterAccountIDs.Contains(x.WaterAccountID))
                .Select(x => x.WaterAccountID)
                .ToList();
        }

        var waterAccountConsumerDtos = dbContext.WaterAccounts.AsNoTracking()
            .Include(x => x.WaterAccountContact)
            .Include(x => x.WaterAccountUsers)
            .Where(x => waterAccountIDs.Contains(x.WaterAccountID))
            .Select(x => new WaterAccountConsumerDto()
            {
                WaterAccountID = x.WaterAccountID,
                WaterAccountNumber = x.WaterAccountNumber,
                WaterAccountName = x.WaterAccountName,
                Notes = x.Notes,
                WaterAccountPIN = x.WaterAccountPIN,
                WaterAccountPINLastUsed = x.WaterAccountUsers.Any() ? x.WaterAccountUsers.Max(x => x.ClaimDate) : null,
                WaterAccountContactName = x.WaterAccountContact.ContactName,
                ContactEmail = x.WaterAccountContact.ContactEmail,
                ContactPhoneNumber = x.WaterAccountContact.ContactPhoneNumber,
                FullAddress = x.WaterAccountContact.FullAddress,
                GeographyID = x.GeographyID
            }).ToList();

        return Ok(waterAccountConsumerDtos);
    }
}