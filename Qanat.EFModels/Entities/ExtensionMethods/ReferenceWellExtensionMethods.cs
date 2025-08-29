using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities;

public static class ReferenceWellExtensionMethods
{
    public static ReferenceWellSimpleDto AsSimpleDto(this ReferenceWell referenceWell)
    {
        var dto = new ReferenceWellSimpleDto()
        {
            ReferenceWellID = referenceWell.ReferenceWellID,
            GeographyID = referenceWell.GeographyID,
            WellName = referenceWell.WellName,
            CountyWellPermitNo = referenceWell.CountyWellPermitNo,
            WellDepth = referenceWell.WellDepth,
            StateWCRNumber = referenceWell.StateWCRNumber,
            DateDrilled = referenceWell.DateDrilled
        };
        return dto;
    }

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