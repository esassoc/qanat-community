using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities;

public static class ReportingPeriodExtensionMethods
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
            IsDefault = reportingPeriod.IsDefault,

            CoverCropSelfReportStartDate = reportingPeriod.CoverCropSelfReportStartDate,
            CoverCropSelfReportEndDate = reportingPeriod.CoverCropSelfReportEndDate,
            CoverCropSelfReportReadyForAccountHolders = reportingPeriod.CoverCropSelfReportReadyForAccountHolders,

            FallowSelfReportStartDate = reportingPeriod.FallowSelfReportStartDate,
            FallowSelfReportEndDate = reportingPeriod.FallowSelfReportEndDate,
            FallowSelfReportReadyForAccountHolders = reportingPeriod.FallowSelfReportReadyForAccountHolders,

            CreateDate = reportingPeriod.CreateDate,
            CreateUserID = reportingPeriod.CreateUserID,
            UpdateDate = reportingPeriod.UpdateDate,
            UpdateUserID = reportingPeriod.UpdateUserID
        };
        return dto;
    }

    public static ReportingPeriodDto AsDto(this ReportingPeriod reportingPeriod)
    {
        var reportingPeriodDto = new ReportingPeriodDto()
        {
            ReportingPeriodID = reportingPeriod.ReportingPeriodID,
            Name = reportingPeriod.Name,
            StartDate = reportingPeriod.StartDate,
            EndDate = reportingPeriod.EndDate,
            ReadyForAccountHolders = reportingPeriod.ReadyForAccountHolders,
            IsDefault = reportingPeriod.IsDefault,

            CoverCropSelfReportStartDate = reportingPeriod.CoverCropSelfReportStartDate,
            CoverCropSelfReportEndDate = reportingPeriod.CoverCropSelfReportEndDate,
            CoverCropSelfReportReadyForAccountHolders = reportingPeriod.CoverCropSelfReportReadyForAccountHolders,

            FallowSelfReportStartDate = reportingPeriod.FallowSelfReportStartDate,
            FallowSelfReportEndDate = reportingPeriod.FallowSelfReportEndDate,
            FallowSelfReportReadyForAccountHolders = reportingPeriod.FallowSelfReportReadyForAccountHolders,

            CreateDate = reportingPeriod.CreateDate,
            CreateUser = reportingPeriod.CreateUser.AsSimpleDto(),
            UpdateDate = reportingPeriod.UpdateDate,
            UpdateUser = reportingPeriod.UpdateUser?.AsSimpleDto()
        };

        return reportingPeriodDto;
    }

    public static List<DateTime> GetLastDayOfEachMonth(this ReportingPeriod reportingPeriod)
    {
        var result = new List<DateTime>();

        var currentDate = new DateTime(reportingPeriod.StartDate.Year, reportingPeriod.StartDate.Month, 1);
        while (currentDate <= reportingPeriod.EndDate)
        {
            var lastDayOfMonth = new DateTime(currentDate.Year, currentDate.Month, DateTime.DaysInMonth(currentDate.Year, currentDate.Month));
            if (lastDayOfMonth <= reportingPeriod.EndDate)
            {
                result.Add(lastDayOfMonth);
            }

            currentDate = currentDate.AddMonths(1);
        }

        return result;
    } 
}