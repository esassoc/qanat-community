//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[WaterAccountReconciliation]

using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities
{
    public static partial class WaterAccountReconciliationExtensionMethods
    {
        public static WaterAccountReconciliationSimpleDto AsSimpleDto(this WaterAccountReconciliation waterAccountReconciliation)
        {
            var dto = new WaterAccountReconciliationSimpleDto()
            {
                WaterAccountReconciliationID = waterAccountReconciliation.WaterAccountReconciliationID,
                GeographyID = waterAccountReconciliation.GeographyID,
                ParcelID = waterAccountReconciliation.ParcelID,
                WaterAccountID = waterAccountReconciliation.WaterAccountID
            };
            return dto;
        }
    }
}