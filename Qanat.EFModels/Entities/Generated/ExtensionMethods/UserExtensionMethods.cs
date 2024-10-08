//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[User]

using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities
{
    public static partial class UserExtensionMethods
    {
        public static UserSimpleDto AsSimpleDto(this User user)
        {
            var dto = new UserSimpleDto()
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
                GETRunCustomerID = user.GETRunCustomerID,
                GETRunUserID = user.GETRunUserID
            };
            return dto;
        }
    }
}