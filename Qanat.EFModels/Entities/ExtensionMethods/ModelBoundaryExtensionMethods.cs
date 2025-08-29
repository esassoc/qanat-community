using Qanat.Common.GeoSpatial;
using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities;

public static class ModelBoundaryExtensionMethods
{
    public static ModelBoundaryDto AsModelBoundaryDto(this ModelBoundary modelBoundary)
    {
        return new ModelBoundaryDto
        {
            ModelBoundaryID = modelBoundary.ModelBoundaryID,
            ModelID = modelBoundary.ModelID,
            GeoJson = modelBoundary.ModelBoundaryGeometry.ToGeoJSON(),
        };
    }
}