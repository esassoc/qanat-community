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
                Name = reportingPeriod.Name,
                StartDate = reportingPeriod.StartDate,
                EndDate = reportingPeriod.EndDate,
                ReadyForAccountHolders = reportingPeriod.ReadyForAccountHolders,
                CreateDate = reportingPeriod.CreateDate,
                CreateUserID = reportingPeriod.CreateUserID,
                UpdateDate = reportingPeriod.UpdateDate,
                UpdateUserID = reportingPeriod.UpdateUserID
            };
            return dto;
        }
    }
}