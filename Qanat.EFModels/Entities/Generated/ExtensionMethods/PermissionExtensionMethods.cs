//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[Permission]

using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities
{
    public static partial class PermissionExtensionMethods
    {
        public static PermissionSimpleDto AsSimpleDto(this Permission permission)
        {
            var dto = new PermissionSimpleDto()
            {
                PermissionID = permission.PermissionID,
                PermissionName = permission.PermissionName,
                PermissionDisplayName = permission.PermissionDisplayName
            };
            return dto;
        }
    }
}