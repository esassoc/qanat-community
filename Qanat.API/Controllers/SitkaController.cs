using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Qanat.API.Services;
using Qanat.EFModels.Entities;

namespace Qanat.API.Controllers
{
#if DEBUG
    [ApiExplorerSettings(IgnoreApi = false)]
    #else
        [ApiExplorerSettings(IgnoreApi = true)]
    #endif
    
    // MCS: I feel strongly that all endpoints should require that the user be authenticated at a bare minimum. Moreover, adding this check to the parent controller class
    // helps ensure no endpoint allows unauthenticated access.
    // However, some Qanat apps display custom rich texts on the homepage before the user is logged in, so I have to comment this out.
    // Strongly consider uncommenting this attribute.
    //[Authorize]
    public abstract class SitkaController<T> : ControllerBase
    {
        protected readonly QanatDbContext _dbContext;
        protected readonly ILogger<T> _logger;
        protected readonly QanatConfiguration _qanatConfiguration;

        protected SitkaController(QanatDbContext dbContext, ILogger<T> logger, IOptions<QanatConfiguration> qanatConfiguration)
        {
            _dbContext = dbContext;
            _logger = logger;
            _qanatConfiguration = qanatConfiguration.Value;
        }

        protected ActionResult RequireNotNullThrowNotFound(object theObject, string objectType, object objectID)
        {
            return ThrowNotFound(theObject, objectType, objectID, out var actionResult) ? actionResult : Ok(theObject);
        }

        protected bool ThrowNotFound(object theObject, string objectType, object objectID, out ActionResult actionResult)
        {
            if (theObject == null)
            {
                var notFoundMessage = $"{objectType} with ID {objectID} does not exist!";
                _logger.LogError(notFoundMessage);
                {
                    actionResult = NotFound(notFoundMessage);
                    return true;
                }
            }

            actionResult = null;
            return false;
        }
    }
}