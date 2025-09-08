using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Logging;
using NetTopologySuite.Features;
using Qanat.Common.GeoSpatial;
using Qanat.EFModels.Entities;
using Qanat.Models.DataTransferObjects;

namespace Qanat.Swagger.Controllers;

[ApiKey]
[ApiController]
public class QanatApiController : SitkaApiController<QanatApiController>
{
    public QanatApiController(QanatDbContext dbContext, ILogger<QanatApiController> logger) : base(dbContext, logger)
    {
    }

    /// <summary>
    /// Get all geographies
    /// </summary>
    /// <returns></returns>
    [HttpGet("geographies")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/json")]
    public ActionResult<List<GeographySimpleDto>> ListGeographies()
    {
        var geographySimpleDtos = Geographies.ListAsSimpleDto(_dbContext);
        return Ok(geographySimpleDtos);
    }

    /// <summary>
    /// Get all parcel numbers for a specified geography
    /// </summary>
    /// <param name="geographyID"></param>
    /// <returns></returns>
    [HttpGet("geographies/{geographyID}/parcels")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/json")]
    public ActionResult<List<ParcelDisplayDto>> ListParcels([FromRoute] int geographyID)
    {
        var geography = Geographies.GetByID(_dbContext, geographyID);
        if (geography == null)
        {
            return NotFound($"Geography with ID {geographyID} does not exist!");
        }
        var parcelDisplayDtos = Parcels.ListAsDisplayDto(_dbContext, geographyID);
        return Ok(parcelDisplayDtos);
    }

    /// <summary>
    /// Get details for a specified parcel number, including geometry
    /// </summary>
    /// <param name="geographyID"></param>
    /// <param name="parcelNumber">Minimum 3 characters to search</param>
    /// <returns></returns>
    [HttpGet("geographies/{geographyID}/parcels/{parcelNumber}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/json")]
    public ActionResult<List<ParcelWithGeometryDto>> GetParcelByGeographyAndParcelNumber([FromRoute] int geographyID, [FromRoute] string parcelNumber)
    {
        var geography = Geographies.GetByID(_dbContext, geographyID);
        if (geography == null)
        {
            return NotFound($"Geography with ID {geographyID} does not exist!");
        }

        if (parcelNumber.Length < 3)
        {
            return BadRequest($"Please enter at least 3 characters for the Parcel Number!");
        }
        var parcelWithGeometryDtos = Parcels.GetByGeographyAndParcelNumberAsFeatureCollection(_dbContext, geographyID, parcelNumber);
        if (parcelWithGeometryDtos == null)
        {
            return NotFound();
        }

        return Ok(parcelWithGeometryDtos);
    }

    /// <summary>
    /// Get all wells for a specified geography
    /// </summary>
    /// <param name="geographyID"></param>
    /// <returns></returns>
    [HttpGet("geographies/{geographyID}/wells")] // todo: wells or well registrations?
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/json")]
    public ActionResult<List<WellMinimalDto>> ListWells([FromRoute] int geographyID)
    {
        var wellMinimalDtos = Wells.ListByGeographyIDAsMinimalDto(_dbContext, geographyID);

        return wellMinimalDtos;
    }

    private IActionResult DisplayFile(string fileName, Stream stream)
    {
        var contentDisposition = new System.Net.Mime.ContentDisposition
        {
            FileName = fileName,
            Inline = false
        };
        Response.Headers.Add("Content-Disposition", contentDisposition.ToString());

        var provider = new FileExtensionContentTypeProvider();
        if (!provider.TryGetContentType(fileName, out var contentType))
        {
            contentType = "application/octet-stream";
        }

        return File(stream, contentType);
    }

}