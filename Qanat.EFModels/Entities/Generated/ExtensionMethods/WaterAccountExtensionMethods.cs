//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[WaterAccount]

using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities
{
    public static partial class WaterAccountExtensionMethods
    {
        public static WaterAccountSimpleDto AsSimpleDto(this WaterAccount waterAccount)
        {
            var dto = new WaterAccountSimpleDto()
            {
                WaterAccountID = waterAccount.WaterAccountID,
                GeographyID = waterAccount.GeographyID,
                WaterAccountNumber = waterAccount.WaterAccountNumber,
                WaterAccountName = waterAccount.WaterAccountName,
                Notes = waterAccount.Notes,
                UpdateDate = waterAccount.UpdateDate,
                WaterAccountPIN = waterAccount.WaterAccountPIN,
                CreateDate = waterAccount.CreateDate,
                ContactName = waterAccount.ContactName,
                ContactAddress = waterAccount.ContactAddress
            };
            return dto;
        }
    }
}