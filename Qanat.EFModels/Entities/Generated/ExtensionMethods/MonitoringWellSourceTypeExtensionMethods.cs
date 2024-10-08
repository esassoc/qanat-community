//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[MonitoringWellSourceType]

using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities
{
    public static partial class MonitoringWellSourceTypeExtensionMethods
    {
        public static MonitoringWellSourceTypeSimpleDto AsSimpleDto(this MonitoringWellSourceType monitoringWellSourceType)
        {
            var dto = new MonitoringWellSourceTypeSimpleDto()
            {
                MonitoringWellSourceTypeID = monitoringWellSourceType.MonitoringWellSourceTypeID,
                MonitoringWellSourceTypeName = monitoringWellSourceType.MonitoringWellSourceTypeName,
                MonitoringWellSourceTypeDisplayName = monitoringWellSourceType.MonitoringWellSourceTypeDisplayName
            };
            return dto;
        }
    }
}