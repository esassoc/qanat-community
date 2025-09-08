using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Qanat.API.Services;
using Qanat.API.Services.Authorization;
using Qanat.EFModels.Entities;
using Qanat.Models.DataTransferObjects;
using Qanat.Models.Security;
using System.Collections.Generic;

namespace Qanat.API.Controllers
{
    [ApiController]
    [RightsChecker]
    public class CustomRichTextController(
        QanatDbContext dbContext,
        ILogger<CustomRichTextController> logger,
        IOptions<QanatConfiguration> qanatConfiguration)
        : SitkaController<CustomRichTextController>(dbContext, logger, qanatConfiguration)
    {
        [HttpGet("field-definitions")]
        [WithRolePermission(PermissionEnum.FieldDefinitionRights, RightsEnum.Read)]
        public ActionResult<List<CustomRichTextDto>> ListFieldDefinitions()
        {
            var customRichTextDtos = CustomRichText.ListFieldDefinitions(_dbContext);
            return Ok(customRichTextDtos);
        }

        [HttpPut("custom-rich-texts/{customRichTextTypeID}")]
        [WithRolePermission(PermissionEnum.CustomRichTextRights, RightsEnum.Update)]
        public ActionResult<CustomRichTextDto> UpdateCustomRichText([FromRoute] int customRichTextTypeID, [FromBody] CustomRichTextSimpleDto customRichTextUpdateDto)
        {
            var updatedCustomRichTextDto =
                CustomRichText.UpdateCustomRichText(_dbContext, customRichTextTypeID, customRichTextUpdateDto, customRichTextUpdateDto.GeographyID);
            return Ok(updatedCustomRichTextDto);
        }
    }
}
