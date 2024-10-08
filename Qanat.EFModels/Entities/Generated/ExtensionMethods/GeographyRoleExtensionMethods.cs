//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[GeographyRole]

using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities
{
    public static partial class GeographyRoleExtensionMethods
    {
        public static GeographyRoleSimpleDto AsSimpleDto(this GeographyRole geographyRole)
        {
            var dto = new GeographyRoleSimpleDto()
            {
                GeographyRoleID = geographyRole.GeographyRoleID,
                GeographyRoleName = geographyRole.GeographyRoleName,
                GeographyRoleDisplayName = geographyRole.GeographyRoleDisplayName,
                GeographyRoleDescription = geographyRole.GeographyRoleDescription,
                SortOrder = geographyRole.SortOrder,
                Rights = geographyRole.Rights,
                Flags = geographyRole.Flags
            };
            return dto;
        }
    }
}