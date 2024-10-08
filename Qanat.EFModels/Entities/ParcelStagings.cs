using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Features;
using Qanat.Common.GeoSpatial;
using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities;

public static class ParcelStagings
{
    public static ParcelUpdateExpectedResultsDto GetExpectedResultsDto(QanatDbContext dbContext, int geographyID)
    {
        var fParcelStagingChanges = dbContext.fParcelStagingChanges(geographyID);

        var expectedChanges = new ParcelUpdateExpectedResultsDto()
        {
            NumParcelsInGdb = fParcelStagingChanges.Count(),
            NumParcelsUnchanged = fParcelStagingChanges.Count(x => !x.HasGeometryChange && !x.HasOwnerNameChange && !x.HasOwnerAddressChange),
            NumParcelsToBeInactivated = fParcelStagingChanges.Count(x => x.ParcelStatusID == (int)ParcelStatusEnum.Inactive),
            NumParcelsWithUpdatedGeometries = fParcelStagingChanges.Count(x => x.HasGeometryChange),
            NumParcelsAdded = fParcelStagingChanges.Count(x => x.IsNew),
            NumParcelsWithOwnerOrAddressChange = fParcelStagingChanges.Count(x => x.HasOwnerNameChange || x.HasOwnerAddressChange)
        };
        expectedChanges.NumParcelsToBeUpdated = expectedChanges.NumParcelsInGdb - expectedChanges.NumParcelsUnchanged;

        return expectedChanges;
    }
}

public class ParcelUpdateExpectedResultsDto
{
    public int NumParcelsInGdb { get; set; }
    public int NumParcelsAdded { get; set; }
    public int NumParcelsUnchanged { get; set; }
    public int NumParcelsToBeInactivated { get; set; }
    public int NumParcelsToBeUpdated { get; set; }
    public int NumParcelsWithUpdatedGeometries { get; set; }
    public int NumParcelsWithOwnerOrAddressChange { get; set; }
}