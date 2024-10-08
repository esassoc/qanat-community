using Microsoft.EntityFrameworkCore;
using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities;

public static class ReportingPeriods
{
    public static List<ReportingPeriod> List(QanatDbContext dbContext)
    {
        return dbContext.ReportingPeriods.AsNoTracking().ToList();
    }

    public static ReportingPeriod GetByGeographyID(QanatDbContext dbContext, int geographyID)
    {
        return dbContext.ReportingPeriods.AsNoTracking()
            .SingleOrDefault(x => x.GeographyID == geographyID);
    }

    public static ReportingPeriodSimpleDto GetReportingPeriodForGeographyAsSimpleDto(QanatDbContext dbContext, int geographyID)
    {
        return GetByGeographyID(dbContext, geographyID)?.AsSimpleDto();
    }

    public static ReportingPeriod GetByID(QanatDbContext dbContext, int reportingPeriodID)
    {
        return dbContext.ReportingPeriods.AsNoTracking().SingleOrDefault(x => x.ReportingPeriodID == reportingPeriodID);
    }

    public static MostRecentEffectiveDates GetMostRecentEffectiveDates(QanatDbContext dbContext, int reportingPeriodID, int year)
    {
        var reportingPeriod = GetByID(dbContext, reportingPeriodID);
        var geographyID = reportingPeriod.GeographyID;

        var fReportingPeriod = GetByGeographyIDAndYear(dbContext, geographyID, year);
        var startDate = fReportingPeriod.StartDate;
        var endDate = fReportingPeriod.EndDate;
        return GetMostRecentEffectiveDates(dbContext, geographyID, startDate, endDate);
    }

    public static MostRecentEffectiveDates GetMostRecentEffectiveDates(QanatDbContext dbContext, int geographyID, DateTime startDate, DateTime endDate)
    {
        var mostRecentEffectiveDatesDto = new MostRecentEffectiveDates();
        var parcelSupplies = dbContext.ParcelSupplies.AsNoTracking()
            .Where(x => x.EffectiveDate >= startDate && x.EffectiveDate <= endDate && x.GeographyID == geographyID)
            .ToList();
        var waterMeasurementSourceOfRecords = dbContext.vWaterMeasurementSourceOfRecords.AsNoTracking()
            .Where(x => x.ReportedDate >= startDate && x.ReportedDate <= endDate && x.GeographyID == geographyID);
        mostRecentEffectiveDatesDto.MostRecentSupplyEffectiveDate = parcelSupplies.Any()
            ? parcelSupplies.Max(x => x.EffectiveDate).ToShortDateString()
            : null;
        mostRecentEffectiveDatesDto.MostRecentUsageEffectiveDate = waterMeasurementSourceOfRecords.Any()
            ? waterMeasurementSourceOfRecords.Max(x => x.ReportedDate).ToShortDateString()
            : null;
        return mostRecentEffectiveDatesDto;
    }

    public static fReportingPeriod GetByGeographyIDAndYear(QanatDbContext dbContext, int geographyID, int year)
    {
        return dbContext.fReportingPeriod(year).AsNoTracking()
            .SingleOrDefault(x => x.GeographyID == geographyID);
    }
}