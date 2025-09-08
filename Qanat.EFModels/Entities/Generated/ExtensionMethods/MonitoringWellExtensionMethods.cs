//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[MonitoringWell]

using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities
{
    public static partial class MonitoringWellExtensionMethods
    {
        public static MonitoringWellSimpleDto AsSimpleDto(this MonitoringWell monitoringWell)
        {
            var dto = new MonitoringWellSimpleDto()
            {
                MonitoringWellID = monitoringWell.MonitoringWellID,
                GeographyID = monitoringWell.GeographyID,
                SiteCode = monitoringWell.SiteCode,
                MonitoringWellName = monitoringWell.MonitoringWellName,
                MonitoringWellSourceTypeID = monitoringWell.MonitoringWellSourceTypeID
            };
            return dto;
        }
    }
}