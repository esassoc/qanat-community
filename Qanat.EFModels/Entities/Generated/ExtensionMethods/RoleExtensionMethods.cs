//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[Role]

using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities
{
    public static partial class RoleExtensionMethods
    {
        public static RoleSimpleDto AsSimpleDto(this Role role)
        {
            var dto = new RoleSimpleDto()
            {
                RoleID = role.RoleID,
                RoleName = role.RoleName,
                RoleDisplayName = role.RoleDisplayName,
                RoleDescription = role.RoleDescription,
                SortOrder = role.SortOrder,
                Rights = role.Rights,
                Flags = role.Flags
            };
            return dto;
        }
    }
}