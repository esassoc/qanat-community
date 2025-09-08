using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities;

public static partial class ReferenceWellExtensionMethods
{
    public static ReferenceWellMapMarkerDto AsReferenceWellMapMarkerDto(this ReferenceWell referenceWell)
    {
        var dto = new ReferenceWellMapMarkerDto()
        {
            ReferenceWellID = referenceWell.ReferenceWellID,
            GeographyID = referenceWell.GeographyID,
            CountyWellPermitNo = referenceWell.CountyWellPermitNo,
            StateWCRNumber = referenceWell.StateWCRNumber,
            Latitude = referenceWell.LocationPoint4326.Coordinate.Y,
            Longitude = referenceWell.LocationPoint4326.Coordinate.X,
        };
        return dto;
    }
}