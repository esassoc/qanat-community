using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities
{
    public static class SelfReportStatusExtensionMethods
    {
        public static SelfReportStatusSimpleDto AsSimpleDto(this SelfReportStatus selfReportStatus)
        {
            var dto = new SelfReportStatusSimpleDto()
            {
                SelfReportStatusID = selfReportStatus.SelfReportStatusID,
                SelfReportStatusName = selfReportStatus.SelfReportStatusName,
                SelfReportStatusDisplayName = selfReportStatus.SelfReportStatusDisplayName
            };
            return dto;
        }
    }
}