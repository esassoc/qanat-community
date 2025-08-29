using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities;

[Keyless]
public class WaterAccountForUsageStatement
{
    public int WaterAccountID { get; set; }
    public int WaterAccountNumber { get; set; }
    public string ContactName { get; set; }
    public string Address { get; set; }
    public string SecondaryAddress { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string ZipCode { get; set; }
    public string WaterAccountPIN { get; set; }
    public string ParcelIDs { get; set; }
    public string ParcelNumbers { get; set; }
    public double ParcelArea { get; set; }
    public double UsageLocationArea { get; set; }
    public string ZoneName { get; set; }
    public DateTime ReportingPeriodStartDate { get; set; }
    public DateTime ReportingPeriodEndDate { get; set; }
    public int GeographyID { get; set; }
    public string GeographyDisplayName { get; set; }
    public string GeographyContactEmail { get; set; }
    public string GeographyContactPhoneNumber { get; set; }
    public string GeographyAddressLine1 { get; set; }
    public string GeographyAddressLine2 { get; set; }

    public static List<UsageStatementWaterAccountDto> ListByStatementBatchID(QanatDbContext dbContext, int statementBatchID)
    {
        var statementBatchIDParam = new SqlParameter("statementBatchID", statementBatchID);

        var waterAccountForUsageStatement = dbContext.WaterAccountForUsageStatement.FromSqlRaw($"EXECUTE dbo.pWaterAccountsForUsageStatementsByStatementBatch @statementBatchID", statementBatchIDParam).ToList();
        var usageStatementWaterAccountDtos = ListWaterAccountForUsageStatementAsUsageStatementWaterAccountDtos(waterAccountForUsageStatement);

        return usageStatementWaterAccountDtos;
    }

    public static UsageStatementWaterAccountDto GetByWaterAccountIDandReportingPeriodID(QanatDbContext dbContext, int waterAccountID, int reportingPeriodID)
    {
        var waterAccountIDParam = new SqlParameter("waterAccountID", waterAccountID);
        var reportingPeriodIDParam = new SqlParameter("reportingPeriodID", reportingPeriodID);

        var waterAccountForUsageStatement = dbContext.WaterAccountForUsageStatement.FromSqlRaw($"EXECUTE dbo.pWaterAccountForUsageStatementByID @waterAccountID, @reportingPeriodID", waterAccountIDParam, reportingPeriodIDParam).ToList();
        var usageStatementWaterAccountDto = ListWaterAccountForUsageStatementAsUsageStatementWaterAccountDtos(waterAccountForUsageStatement).FirstOrDefault();

        return usageStatementWaterAccountDto;
    }

    private static List<UsageStatementWaterAccountDto> ListWaterAccountForUsageStatementAsUsageStatementWaterAccountDtos(List<WaterAccountForUsageStatement> waterAccountsForUsageStatement)
    {
        var usageStatementWaterAccountDtos = waterAccountsForUsageStatement.Select(x => new UsageStatementWaterAccountDto()
        {
            WaterAccountID = x.WaterAccountID,
            WaterAccountNumber = x.WaterAccountNumber,
            ContactName = x.ContactName,
            ContactAddress = x.Address,
            ContactSecondaryAddress = x.SecondaryAddress,
            ContactCity = x.City,
            ContactState = x.State,
            ContactZipCode = x.ZipCode,
            WaterAccountPIN = x.WaterAccountPIN,
            ParcelIDs = x.ParcelIDs.Split(',').Select(int.Parse).ToList(),
            ParcelNumbers = x.ParcelNumbers.Split(',').ToList(),
            ParcelArea = x.ParcelArea,
            UsageArea = x.UsageLocationArea,
            ZoneName = x.ZoneName,
            ReportingPeriodStartDate = x.ReportingPeriodStartDate,
            ReportingPeriodEndDate = x.ReportingPeriodEndDate,
            GeographyID = x.GeographyID,
            GeographyDisplayName = x.GeographyDisplayName,
            GeographyEmail = x.GeographyContactEmail,
            GeographyPhoneNumber = x.GeographyContactPhoneNumber,
            GeographyAddressLine1 = x.GeographyAddressLine1,
            GeographyAddressLine2 = x.GeographyAddressLine2
        }).ToList();

        return usageStatementWaterAccountDtos;
    }
}