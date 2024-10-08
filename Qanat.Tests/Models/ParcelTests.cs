using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Qanat.EFModels.Entities;

namespace Qanat.Tests.Models;

[TestClass]
public class ParcelTests
{
    [TestMethod]
    public void InactivateParcelTest()
    {
        var reportingPeriod = new ReportingPeriod()
        {
            StartMonth = 1
        };

        var currentDate = new DateTime(2023, 8, 23);
        var startAtBeginningOfYear = reportingPeriod.StartOfReportingPeriodForCurrentYear(currentDate);

        Assert.AreEqual(new DateTime(currentDate.Year, 1, 1), startAtBeginningOfYear);
    }
}