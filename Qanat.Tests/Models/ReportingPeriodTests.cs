using Microsoft.VisualStudio.TestTools.UnitTesting;
using Qanat.EFModels.Entities;
using System;

namespace Qanat.Tests.Models;

[TestClass]
public class ReportingPeriodTests
{

    [TestMethod]
    public void StartOfReportingPeriodThatStartsOnJanuaryForCurrentYearTest()
    {
        var reportingPeriod = new ReportingPeriod()
        {
            StartMonth = 1
        };

        var currentDate = new DateTime(2023, 8, 23);
        var startAtBeginningOfYear = reportingPeriod.StartOfReportingPeriodForCurrentYear(currentDate);
        
        Assert.AreEqual(new DateTime(currentDate.Year, 1, 1),startAtBeginningOfYear);
    }

    [TestMethod]
    public void StartOfReportingPeriodThatStartsInOctoberForCurrentYearTest()
    {
        var reportingPeriod = new ReportingPeriod()
        {
            StartMonth = 10
        };

        var currentDate = new DateTime(2023, 8, 23);
        var startDateForReportingPeriod = reportingPeriod.StartOfReportingPeriodForCurrentYear(currentDate);
        
        Assert.AreEqual(new DateTime(currentDate.Year, 10, 1), startDateForReportingPeriod);
    }

    [TestMethod]
    public void StartOfReportingPeriodThatStartsInMarchForCurrentYearTest()
    {
        var reportingPeriod = new ReportingPeriod()
        {
            StartMonth = 3
        };

        var currentDate = new DateTime(2023, 8, 23);
        var startDateForReportingPeriod = reportingPeriod.StartOfReportingPeriodForCurrentYear(currentDate);
        
        Assert.AreEqual(new DateTime(currentDate.AddYears(1).Year, 3, 1), startDateForReportingPeriod);
    }

    [TestMethod]
    public void StartOfReportingPeriodFromCurrentDateOnFirstDayTest()
    {
        var reportingPeriod = new ReportingPeriod()
        {
            StartMonth = 3
        };

        var currentDate = new DateTime(2023, 3, 1);
        var startDateForReportingPeriod = reportingPeriod.StartOfReportingPeriodForCurrentYear(currentDate);

        Assert.AreEqual(new DateTime(currentDate.AddYears(1).Year, 3, 1), startDateForReportingPeriod);
    }

    [TestMethod]
    public void StartOfReportingPeriodFromDateThatEndsOnLastDayTest()
    {
        var reportingPeriod = new ReportingPeriod()
        {
            StartMonth = 3
        };

        var currentDate = new DateTime(2023, 2, 28);
        var startDateForReportingPeriod = reportingPeriod.StartOfReportingPeriodForCurrentYear(currentDate);

        Assert.AreEqual(new DateTime(currentDate.Year, 3, 1), startDateForReportingPeriod);
    }

    [TestMethod]
    public void StartOfReportingPeriodFromLeapYearDateThatEndsOnLastDayTest()
    {
        var reportingPeriod = new ReportingPeriod()
        {
            StartMonth = 3
        };

        var currentDate = new DateTime(2020, 2, 29);
        var startDateForReportingPeriod = reportingPeriod.StartOfReportingPeriodForCurrentYear(currentDate);

        Assert.AreEqual(new DateTime(currentDate.Year, 3, 1), startDateForReportingPeriod);
    }

    
}