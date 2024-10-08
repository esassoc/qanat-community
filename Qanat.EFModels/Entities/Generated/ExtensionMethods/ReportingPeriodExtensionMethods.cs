//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ReportingPeriod]

using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities
{
    public static partial class ReportingPeriodExtensionMethods
    {
        public static ReportingPeriodSimpleDto AsSimpleDto(this ReportingPeriod reportingPeriod)
        {
            var dto = new ReportingPeriodSimpleDto()
            {
                ReportingPeriodID = reportingPeriod.ReportingPeriodID,
                GeographyID = reportingPeriod.GeographyID,
                ReportingPeriodName = reportingPeriod.ReportingPeriodName,
                StartMonth = reportingPeriod.StartMonth,
                Interval = reportingPeriod.Interval
            };
            return dto;
        }
    }
}