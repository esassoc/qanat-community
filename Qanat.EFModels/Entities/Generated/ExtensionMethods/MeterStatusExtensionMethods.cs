//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[MeterStatus]

using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities
{
    public static partial class MeterStatusExtensionMethods
    {
        public static MeterStatusSimpleDto AsSimpleDto(this MeterStatus meterStatus)
        {
            var dto = new MeterStatusSimpleDto()
            {
                MeterStatusID = meterStatus.MeterStatusID,
                MeterStatusName = meterStatus.MeterStatusName,
                MeterStatusDisplayName = meterStatus.MeterStatusDisplayName
            };
            return dto;
        }
    }
}