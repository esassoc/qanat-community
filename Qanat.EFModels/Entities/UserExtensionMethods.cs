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
            RoleID = user.Role.RoleID,
            RoleDisplayName = user.Role.RoleDisplayName,
            CreateDate = user.CreateDate,
            UpdateDate = user.UpdateDate,
            LastActivityDate = user.LastActivityDate,
            IsActive = user.IsActive,
            ReceiveSupportEmails = user.ReceiveSupportEmails,
            LoginName = user.LoginName,
            Company = user.Company,
            ImpersonatedUserGuid = user.ImpersonatedUserGuid,
            IsClientUser = user.IsClientUser,
            FullName = user.FullName,
            GeographyUser = user.GeographyUsers.Where(x => x.UserID == user.UserID).Select(x => x.AsSimpleDto()).ToList(),
            Rights = JsonConvert.DeserializeObject<Dictionary<string, Rights>>(user.Role.Rights),
            Flags = JsonConvert.DeserializeObject<Dictionary<string, bool>>(user.Role.Flags),
            GeographyRights = user.GeographyUsers.Select(x => new { x.GeographyID, Role = x.GeographyRole })
                .ToDictionary(x => x.GeographyID, x => x.Role.AsGeographyRights()),
            GeographyFlags = user.GeographyUsers.Select(x => new { x.GeographyID, Role = x.GeographyRole })
                .ToDictionary(x => x.GeographyID, x => x.Role.AsGeographyFlags()),
            WaterAccountRights = user.WaterAccountUsers.Select(x => new { x.WaterAccountID, Role = x.WaterAccountRole })
                .ToDictionary(x => x.WaterAccountID, x => x.Role.AsWaterAccountRights()),
            WaterAccountFlags = user.WaterAccountUsers.Select(x => new { x.WaterAccountID, Role = x.WaterAccountRole })
                .ToDictionary(x => x.WaterAccountID, x => x.Role.AsWaterAccountFlags()),
            NumberOfWaterAccounts = user.WaterAccountUsers.Count,

            ScenarioPlannerRoleID = user.ScenarioPlannerRoleID,
            ScenarioPlannerRoleDisplayName = user.ScenarioPlannerRole.ScenarioPlannerRoleDisplayName,
            Models = user.ModelUsers.OrderBy(x => x.Model.ModelName).Select(x => x.Model.AsSimpleDto()).ToList(),
            ScenarioPlannerRights = JsonConvert.DeserializeObject<Dictionary<string, Rights>>(user.ScenarioPlannerRole.Rights),
            GETRunCustomerID = user.GETRunCustomerID,
            GETRunUserID = user.GETRunUserID,
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