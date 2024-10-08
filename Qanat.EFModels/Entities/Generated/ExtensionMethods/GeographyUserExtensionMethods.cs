//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[GeographyUser]

using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities
{
    public static partial class GeographyUserExtensionMethods
    {
        public static GeographyUserSimpleDto AsSimpleDto(this GeographyUser geographyUser)
        {
            var dto = new GeographyUserSimpleDto()
            {
                GeographyUserID = geographyUser.GeographyUserID,
                GeographyID = geographyUser.GeographyID,
                UserID = geographyUser.UserID,
                GeographyRoleID = geographyUser.GeographyRoleID
            };
            return dto;
        }
    }
}