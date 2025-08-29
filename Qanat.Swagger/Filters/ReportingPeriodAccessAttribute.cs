using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Qanat.EFModels.Entities;
using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Qanat.Swagger.Filters
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class ReportingPeriodAccessAttribute(
        string geographyIDParamName = "geographyID",
        string reportingPeriodIDParamName = "reportingPeriodID")
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
            if (!context.ActionArguments.TryGetValue(reportingPeriodIDParamName, out var reportingPeriodIDObj) || reportingPeriodIDObj is not int reportingPeriodID)
            {
                context.Result = new BadRequestObjectResult($"Missing or invalid '{reportingPeriodIDParamName}' parameter.");
                return;
            }

            var dbContext = context.HttpContext.RequestServices.GetService(typeof(QanatDbContext)) as QanatDbContext;
            if (dbContext == null)
            {
                context.Result = new NotFoundResult();
                return;
            }

            var reportingPeriodExists = await dbContext.ReportingPeriods
                .AsNoTracking()
                .AnyAsync(x => x.ReportingPeriodID == reportingPeriodID && x.GeographyID == geographyID);
            if (!reportingPeriodExists)
            {
                context.Result = new NotFoundResult();
                return;
            }

            await next();
        }
    }
}
