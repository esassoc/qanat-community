using Microsoft.EntityFrameworkCore;
using Qanat.Models.DataTransferObjects;
using Schemoto.SchemaNamespace;
using System.Text.Json;

namespace Qanat.EFModels.Entities;

public static class WellTypes
{
    public static async Task<WellTypeDto> GetAsync(QanatDbContext dbContext, int geographyID, int wellTypeID)
    {
        var wellType = await dbContext.WellTypes
            .SingleAsync(wt => wt.GeographyID == geographyID && wt.WellTypeID == wellTypeID);

        var wellTypeDto = new WellTypeDto
        {
            WellTypeID = wellType.WellTypeID,
            Name = wellType.Name,
            SchemotoSchema = !string.IsNullOrEmpty(wellType.SchemotoSchema)
                ? JsonSerializer.Deserialize<Schema>(wellType.SchemotoSchema)
                : null
        };

        return wellTypeDto;
    }
}
