using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Qanat.Models.DataTransferObjects;
using Qanat.Models.Security;

namespace Qanat.EFModels.Entities;

public static partial class UserExtensionMethods
{
    public static UserDto AsUserDto(this User user)
    {
        var userDto = new UserDto()
        {
            UserID = user.UserID,
            UserGuid = user.UserGuid,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Phone = user.Phone,
            Role = user.Role.AsRoleDto(),
            CreateDate = user.CreateDate,
            UpdateDate = user.UpdateDate,
            LastActivityDate = user.LastActivityDate,
            IsActive = user.IsActive,
            ReceiveSupportEmails = user.ReceiveSupportEmails,
            LoginName = user.LoginName,
            Company = user.Company,
            ImpersonatedUserGuid = user.ImpersonatedUserGuid,
            IsClientUser = user.IsClientUser,
            GETRunCustomerID = user.GETRunCustomerID,
            GETRunUserID = user.GETRunUserID,
            FullName = user.FullName,
            GeographyUser = user.GeographyUsers.Where(x => x.UserID == user.UserID).Select(x => x.AsSimpleDto()).ToList(),
            GeographyRights = user.GeographyUsers.Select(x => new { x.GeographyID, Role = x.GeographyRole })
                .ToDictionary(x => x.GeographyID, x => x.Role.AsGeographyRights()),
            GeographyFlags = user.GeographyUsers.Select(x => new { x.GeographyID, Role = x.GeographyRole })
                .ToDictionary(x => x.GeographyID, x => x.Role.AsGeographyFlags()),
            WaterAccountRights = user.WaterAccountUsers.Select(x => new { x.WaterAccountID, Role = x.WaterAccountRole })
                .ToDictionary(x => x.WaterAccountID, x => x.Role.AsWaterAccountRights()),
            WaterAccountFlags = user.WaterAccountUsers.Select(x => new { x.WaterAccountID, Role = x.WaterAccountRole })
                .ToDictionary(x => x.WaterAccountID, x => x.Role.AsWaterAccountFlags()),
            NumberOfWaterAccounts = user.WaterAccountUsers.Count
        };

        return userDto;
    }

    public static UserWithFullNameDto AsUserWithFullNameDto(this User user)
    {
        var dto = new UserWithFullNameDto()
        {
            UserID = user.UserID,
            UserGuid = user.UserGuid,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Phone = user.Phone,
            RoleID = user.RoleID,
            CreateDate = user.CreateDate,
            UpdateDate = user.UpdateDate,
            LastActivityDate = user.LastActivityDate,
            IsActive = user.IsActive,
            ReceiveSupportEmails = user.ReceiveSupportEmails,
            LoginName = user.LoginName,
            Company = user.Company,
            ImpersonatedUserGuid = user.ImpersonatedUserGuid,
            IsClientUser = user.IsClientUser,
            FullName = user.FullName
        };
        return dto;
    }
    
}