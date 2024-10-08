using System;
using System.Collections.Generic;

namespace Qanat.EFModels.Entities;

public static partial class ReportingPeriodExtensionMethods
{
    public static DateTime StartOfReportingPeriodForYear(this ReportingPeriod reportingPeriod, int year)
    {
        return new DateTime(reportingPeriod.StartMonth == 1 ? year : year - 1, reportingPeriod.StartMonth, 1);
    }

    public static DateTime StartOfReportingPeriodForCurrentYear(this ReportingPeriod reportingPeriod, DateTime? currentDateOverride = null)
    {
        var currentDate = currentDateOverride ?? DateTime.UtcNow;

        if (reportingPeriod.StartMonth == 1)
        {
            return new DateTime(currentDate.Year, 1, 1);
        }

        var cutOffDate = new DateTime(currentDate.Year, reportingPeriod.StartMonth, 1);
        if (currentDate >= cutOffDate)
        {
            return new DateTime(currentDate.AddYears(1).Year, reportingPeriod.StartMonth, 1);
        }

        return new DateTime(currentDate.Year, reportingPeriod.StartMonth, 1);
    }


}