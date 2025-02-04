using System.Runtime.InteropServices;
using Microsoft.EntityFrameworkCore;
using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities;

public static class ReportingPeriods
{
    #region CRU

    public static async Task<List<ErrorMessage>> ValidateCreateAsync(QanatDbContext dbContext, int geographyID, ReportingPeriodUpsertDto reportingPeriodUpsertDto)
    {
        var errorMessages = new List<ErrorMessage>();

        var reportingPeriods = await dbContext.ReportingPeriods.AsNoTracking()
            .Where(x => x.GeographyID == geographyID)
            .ToListAsync();

        if (!reportingPeriodUpsertDto.StartDate.HasValue)
        {
            errorMessages.Add(new() { Type = "Start Date", Message = "Start date is required."});
            return errorMessages;
        }

        var endDate = reportingPeriodUpsertDto.StartDate.Value.AddYears(1).AddDays(-1);
        if (reportingPeriods.Any(x => x.Name == reportingPeriodUpsertDto.Name))
        {
            errorMessages.Add(new() { Type = "Name", Message = "Reporting period with this name already exists." });
        }

        //Check if the new reporting period overlaps with any existing reporting period, inclusive bounds.
        if (reportingPeriods.Any(x => reportingPeriodUpsertDto.StartDate <= x.EndDate && endDate >= x.StartDate))
        {
            errorMessages.Add(new() { Type = "Date Interval", Message = "Reporting period overlaps with an existing reporting period." });
        }

        return errorMessages;
    }

    public static async Task<ReportingPeriodDto> CreateAsync(QanatDbContext dbContext, int geographyID, ReportingPeriodUpsertDto reportingPeriodUpsertDto, UserDto callingUser)
    {
        var endDate = reportingPeriodUpsertDto.StartDate!.Value.AddYears(1).AddDays(-1);
        var reportingPeriod = new ReportingPeriod()
        {
            GeographyID = geographyID,
            Name = reportingPeriodUpsertDto.Name,
            StartDate = reportingPeriodUpsertDto.StartDate!.Value,
            EndDate = endDate,
            ReadyForAccountHolders = reportingPeriodUpsertDto.ReadyForAccountHolders,
            CreateUserID = callingUser.UserID,
            CreateDate = DateTime.UtcNow
        };

        dbContext.ReportingPeriods.Add(reportingPeriod);
        await dbContext.SaveChangesAsync();
        await dbContext.Entry(reportingPeriod).ReloadAsync();

        var newReportingPeriod = await GetAsync(dbContext, geographyID, reportingPeriod.ReportingPeriodID, callingUser);
        return newReportingPeriod;
    }

    public static async Task<List<ReportingPeriodDto>> ListByGeographyIDAsync(QanatDbContext dbContext, int geographyID, UserDto callingUser)
    {
        var userIsAdmin = callingUser.Flags[Flag.IsSystemAdmin.FlagName];
        var userIsWaterManager = userIsAdmin || callingUser.GeographyFlags[geographyID][Flag.HasManagerDashboard.FlagName]; //Admins won't have a geography flag, short circuit to true.
        var userShouldSeeAllReportingPeriods = userIsAdmin || userIsWaterManager;

        var reportingPeriods = await dbContext.ReportingPeriods.AsNoTracking()
            .Include(x => x.Geography)
            .Include(x => x.CreateUser)
            .Include(x => x.UpdateUser)
            .Where(x => x.GeographyID == geographyID && (userShouldSeeAllReportingPeriods || x.ReadyForAccountHolders))
            .OrderBy(x => x.StartDate)
            .ToListAsync();

        var reportingPeriodDtos = reportingPeriods.Select(x => x.AsDto()).ToList();
        return reportingPeriodDtos;
    }

    public static async Task<ReportingPeriodDto> GetAsync(QanatDbContext dbContext, int geographyID, int reportingPeriodID, UserDto callingUser)
    {
        var userIsAdmin = callingUser.Flags[Flag.IsSystemAdmin.FlagName];
        var userIsWaterManager = userIsAdmin || callingUser.GeographyFlags[geographyID][Flag.HasManagerDashboard.FlagName]; //Admins won't have a geography flag, short circuit to true.
        var userShouldSeeAllReportingPeriods = userIsAdmin || userIsWaterManager;

        var reportingPeriod = await dbContext.ReportingPeriods.AsNoTracking()
            .Include(x => x.Geography)
            .Include(x => x.CreateUser)
            .Include(x => x.UpdateUser)
            .SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportingPeriodID == reportingPeriodID && (userShouldSeeAllReportingPeriods || x.ReadyForAccountHolders));

        var reportingPeriodDto = reportingPeriod?.AsDto();
        return reportingPeriodDto;
    }

    public static async Task<ReportingPeriodDto> GetByGeographyIDAndYearAsync(QanatDbContext dbContext, int geographyID, int year, UserDto callingUser)
    {
        var userIsAdmin = callingUser.Flags[Flag.IsSystemAdmin.FlagName];
        var userIsWaterManager = userIsAdmin || callingUser.GeographyFlags[geographyID][Flag.HasManagerDashboard.FlagName]; //Admins won't have a geography flag, short circuit to true.
        var userShouldSeeAllReportingPeriods = userIsAdmin || userIsWaterManager;

        var reportingPeriod = await dbContext.ReportingPeriods.AsNoTracking()
            .Include(x => x.Geography)
            .Include(x => x.CreateUser)
            .Include(x => x.UpdateUser)
            .SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.EndDate.Year == year && (userShouldSeeAllReportingPeriods || x.ReadyForAccountHolders));

        var reportingPeriodDto = reportingPeriod?.AsDto();
        return reportingPeriodDto;
    }

    public static async Task<ReportingPeriodDto> GetByGeographyIDAndYearAsync(QanatDbContext dbContext, int geographyID, int year)
    {
        var reportingPeriod = await dbContext.ReportingPeriods.AsNoTracking()
            .Include(x => x.Geography)
            .Include(x => x.CreateUser)
            .Include(x => x.UpdateUser)
            .SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.EndDate.Year == year);

        var reportingPeriodDto = reportingPeriod?.AsDto();
        return reportingPeriodDto;
    }

    public static async Task<List<ErrorMessage>> ValidateUpdateAsync(QanatDbContext dbContext, int geographyID, ReportingPeriodUpsertDto reportingPeriodUpsertDto, int reportingPeriodID)
    {
        var errorMessages = new List<ErrorMessage>();

        var otherReportingPeriods = await dbContext.ReportingPeriods.AsNoTracking()
            .Where(x => x.GeographyID == geographyID && x.ReportingPeriodID != reportingPeriodID)
            .ToListAsync();

        if (!reportingPeriodUpsertDto.StartDate.HasValue)
        {
            errorMessages.Add(new() { Type = "Start Date", Message = "Start date is required." });
            return errorMessages;
        }

        if (otherReportingPeriods.Any(x => x.Name == reportingPeriodUpsertDto.Name))
        {
            errorMessages.Add(new() { Type = "Name", Message = "Reporting period with this name already exists." });
        }

        //Check if the new reporting period overlaps with any existing reporting period, inclusive bounds.
        var endDate = reportingPeriodUpsertDto.StartDate.Value.AddYears(1).AddDays(-1);
        if (otherReportingPeriods.Any(x => reportingPeriodUpsertDto.StartDate <= x.EndDate && endDate >= x.StartDate))
        {
            errorMessages.Add(new() { Type = "Date Interval", Message = "Reporting period overlaps with an existing reporting period." });
        }

        if (!reportingPeriodUpsertDto.ReadyForAccountHolders)
        {
            var geography = await dbContext.Geographies.AsNoTracking()
                .Include(x => x.ReportingPeriods)
                .SingleAsync(x => x.GeographyID == geographyID);

            if(geography.DefaultReportingPeriodID == reportingPeriodID)
            {
                errorMessages.Add(new() { Type = "Default Reporting Period", Message = "Default reporting period must be ready for account holders." });
            }
        }

        return errorMessages;
    }

    public static async Task<ReportingPeriodDto> UpdateAsync(QanatDbContext dbContext, int geographyID, int reportingPeriodID, ReportingPeriodUpsertDto reportingPeriodUpsertDto, UserDto callingUser)
    {
        var reportingPeriod = await dbContext.ReportingPeriods.SingleAsync(x => x.ReportingPeriodID == reportingPeriodID);

        var endDate = reportingPeriodUpsertDto.StartDate!.Value.AddYears(1).AddDays(-1);
        reportingPeriod.Name = reportingPeriodUpsertDto.Name;
        reportingPeriod.StartDate = reportingPeriodUpsertDto.StartDate!.Value;
        reportingPeriod.EndDate = endDate;
        reportingPeriod.ReadyForAccountHolders = reportingPeriodUpsertDto.ReadyForAccountHolders;
        reportingPeriod.UpdateUserID = callingUser.UserID;
        reportingPeriod.UpdateDate = DateTime.UtcNow;

        await dbContext.SaveChangesAsync();
        await dbContext.Entry(reportingPeriod).ReloadAsync();

        var updatedReportingPeriod = await GetAsync(dbContext, geographyID, reportingPeriodID, callingUser);
        return updatedReportingPeriod;
    }

    #endregion

    public static async Task<MostRecentEffectiveDatesDto> GetMostRecentEffectiveDatesAsync(QanatDbContext dbContext, int geographyID, int year, UserDto callingUser)
    {
        var reportingPeriodDto = await GetByGeographyIDAndYearAsync(dbContext, geographyID, year, callingUser);
        if (reportingPeriodDto == null)
        {
            //Users shouldn't see parcel supplies unless the reporting period is ready for account holders.
            return null;
        }

        var startDate = reportingPeriodDto.StartDate;
        var endDate = reportingPeriodDto.EndDate;

        var mostRecentEffectiveDatesDto = await GetMostRecentEffectiveDatesAsync(dbContext, geographyID, startDate, endDate);
        return mostRecentEffectiveDatesDto;
    }

    public static async Task<MostRecentEffectiveDatesDto> GetMostRecentEffectiveDatesAsync(QanatDbContext dbContext, int geographyID, DateTime startDate, DateTime endDate)
    {
        var mostRecentEffectiveDatesDto = new MostRecentEffectiveDatesDto();

        var parcelSupplies = await dbContext.ParcelSupplies.AsNoTracking()
            .Where(x => x.EffectiveDate >= startDate && x.EffectiveDate <= endDate && x.GeographyID == geographyID)
            .ToListAsync();

        var waterMeasurementSourceOfRecords = await dbContext.vWaterMeasurementSourceOfRecords.AsNoTracking()
            .Where(x => x.ReportedDate >= startDate && x.ReportedDate <= endDate && x.GeographyID == geographyID)
            .ToListAsync();

        mostRecentEffectiveDatesDto.MostRecentSupplyEffectiveDate = parcelSupplies.Any()
            ? parcelSupplies.Max(x => x.EffectiveDate)
            : null;

        mostRecentEffectiveDatesDto.MostRecentUsageEffectiveDate = waterMeasurementSourceOfRecords.Any()
            ? waterMeasurementSourceOfRecords.Max(x => x.ReportedDate)
            : null;

        return mostRecentEffectiveDatesDto;
    }
}