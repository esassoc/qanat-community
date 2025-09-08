//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[GeographyBoundary]

using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities
{
    public static partial class GeographyBoundaryExtensionMethods
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