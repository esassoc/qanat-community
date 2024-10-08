//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[UsageEntityGeometry]

using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities
{
    public static partial class UsageEntityGeometryExtensionMethods
    {
        public static UsageEntityGeometrySimpleDto AsSimpleDto(this UsageEntityGeometry usageEntityGeometry)
        {
            var dto = new UsageEntityGeometrySimpleDto()
            {
                UsageEntityID = usageEntityGeometry.UsageEntityID
            };
            return dto;
        }
    }
}