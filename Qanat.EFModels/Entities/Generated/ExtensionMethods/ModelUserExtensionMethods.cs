//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ModelUser]

using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities
{
    public static partial class ModelUserExtensionMethods
    {
        public static ModelUserSimpleDto AsSimpleDto(this ModelUser modelUser)
        {
            var dto = new ModelUserSimpleDto()
            {
                ModelUserID = modelUser.ModelUserID,
                ModelID = modelUser.ModelID,
                UserID = modelUser.UserID
            };
            return dto;
        }
    }
}