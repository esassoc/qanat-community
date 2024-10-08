using System.Collections.Generic;
using Qanat.API.Services;
using Qanat.EFModels.Entities;
using Qanat.Models.DataTransferObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Qanat.API.Services.Authorization;

namespace Qanat.API.Controllers
{
    [ApiController]
    [RightsChecker]

    public class MenuItemController : SitkaController<MenuItemController>
    {
        public MenuItemController(QanatDbContext dbContext, ILogger<MenuItemController> logger, IOptions<QanatConfiguration> qanatConfiguration)
            : base(dbContext, logger, qanatConfiguration)
        {
        }

        [HttpGet("menuItems")]
        [AuthenticatedWithUser]  // MCS: Menu Items are just a lookup table, so they don't have their own rights
        public ActionResult<IEnumerable<MenuItemSimpleDto>> GetMenuItems()
        {
            var menuItemsDto = MenuItems.List();
            return Ok(menuItemsDto);
        }
    }
}