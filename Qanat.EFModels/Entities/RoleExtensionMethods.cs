using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities;

public static partial class RoleExtensionMethods
{
    public static RoleDto AsRoleDto(this Role role)
    {
        var roleDto = new RoleDto()
        {
            RoleID = role.RoleID,
            RoleName = role.RoleName,
            RoleDisplayName = role.RoleDisplayName,
            RoleDescription = role.RoleDescription,
            SortOrder = role.SortOrder,
            Rights = role.Rights,
            Flags = role.Flags
        };
        return roleDto;
    }
}