using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Qanat.EFModels.Entities;
using Qanat.Swagger.Entities;

namespace Qanat.Swagger.Filters;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class ParcelAccessAttribute(string geographyIDParamName = "geographyID", string parcelIDParamName = "parcelID") : Attribute, IAsyncActionFilter, IOrderedFilter
{
    public int Order { get; set; } = 1; // Runs after NoAccessBlock

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (!context.ActionArguments.TryGetValue(geographyIDParamName, out var geographyIDObj) || geographyIDObj is not int geographyID)
        {
            context.Result = new BadRequestObjectResult($"Missing or invalid '{geographyIDParamName}' parameter.");
            return;
        }
        if (!context.ActionArguments.TryGetValue(parcelIDParamName, out var parcelIDObj) || parcelIDObj is not int parcelID)
        {
            context.Result = new BadRequestObjectResult($"Missing or invalid '{parcelIDParamName}' parameter.");
            return;
        }

        var dbContext = context.HttpContext.RequestServices.GetService(typeof(QanatDbContext)) as QanatDbContext;

        var parcel = await dbContext.Parcels.FindAsync(parcelID);
        if (parcel == null || parcel.GeographyID != geographyID)
        {
            context.Result = new NotFoundResult();
            return;
        }

        var userID = context.HttpContext.User.GetUserID();
        if (userID == null || dbContext == null)
        {
            context.Result = new NotFoundResult();
            return;
        }
        var user = await dbContext.Users.FindAsync(userID);
        if (user == null)
        {
            context.Result = new NotFoundResult();
            return;
        }
        var userDto = Users.GetByUserID(dbContext, user.UserID);

        // Permissions check
        var isSystemAdmin = UserPermissions.UserIsSystemAdmin(userDto);
        var isGeographyManager = UserPermissions.UserIsGeographyManager(userDto, geographyID);
        var associatedWaterAccounts = UserPermissions.ListAssociatedWaterAccountsByGeographyIDAndUser(dbContext, geographyID, userDto);

        if (isSystemAdmin || isGeographyManager || (parcel.WaterAccountID.HasValue && associatedWaterAccounts.Contains(parcel.WaterAccountID.Value)))
        {
            await next();
            return;
        }

        context.Result = new NotFoundResult();
    }
}