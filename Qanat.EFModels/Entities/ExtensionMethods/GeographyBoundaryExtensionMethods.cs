using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities
{
    public static class GeographyBoundaryExtensionMethods
    {
        public static GeographyBoundarySimpleDto AsSimpleDto(this GeographyBoundary geographyBoundary)
        {
            var dto = new GeographyBoundarySimpleDto()
            {
                GeographyBoundaryID = geographyBoundary.GeographyBoundaryID,
                GeographyID = geographyBoundary.GeographyID,
                GSABoundaryLastUpdated = geographyBoundary.GSABoundaryLastUpdated
            };
            return dto;
        }
    }
}