using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Qanat.EFModels.Entities;
using System;
using System.Threading.Tasks;

namespace Qanat.Swagger.Filters
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class ReportingPeriodAccessAttribute(
        string geographyIDParamName = "geographyID",
        string yearParamName = "year")
        : Attribute, IAsyncActionFilter, IOrderedFilter
    {
        public int Order { get; set; } = 2; // Should run after GeographyAccess

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.ActionArguments.TryGetValue(geographyIDParamName, out var geographyIDObj) || geographyIDObj is not int geographyID)
            {
                context.Result = new BadRequestObjectResult($"Missing or invalid '{geographyIDParamName}' parameter.");
                return;
            }
            if (!context.ActionArguments.TryGetValue(yearParamName, out var yearObj) || yearObj is not int year)
            {
                context.Result = new BadRequestObjectResult($"Missing or invalid '{yearParamName}' parameter.");
                return;
            }

            var dbContext = context.HttpContext.RequestServices.GetService(typeof(QanatDbContext)) as QanatDbContext;
            if (dbContext == null)
            {
                context.Result = new NotFoundResult();
                return;
            }

            var reportingPeriod = await dbContext.ReportingPeriods
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.GeographyID == geographyID && x.EndDate.Year == year);
            if (reportingPeriod == null)
            {
                context.Result = new NotFoundResult();
                return;
            }

            await next();
        }
    }
}