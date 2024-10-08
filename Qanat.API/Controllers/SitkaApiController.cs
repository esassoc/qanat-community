using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Qanat.EFModels.Entities;
using System.Linq;

namespace Qanat.API.Controllers;

public abstract class SitkaApiController<T> : ControllerBase
{
    protected readonly QanatDbContext _dbContext;
    protected readonly ILogger<T> _logger;

    protected SitkaApiController(QanatDbContext dbContext, ILogger<T> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }
}