//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ModelBoundary]

using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities
{
    public static partial class ModelBoundaryExtensionMethods
    {
        public static ModelBoundarySimpleDto AsSimpleDto(this ModelBoundary modelBoundary)
        {
            var dto = new ModelBoundarySimpleDto()
            {
                ModelBoundaryID = modelBoundary.ModelBoundaryID,
                ModelID = modelBoundary.ModelID
            };
            return dto;
        }
    }
}