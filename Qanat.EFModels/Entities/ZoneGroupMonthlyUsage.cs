using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities;

[Keyless]
public class ZoneGroupMonthlyUsage
{
    public string ZoneName { get; set; }
    public string ZoneColor { get; set; }
    public DateTime EffectiveDate { get; set; }
    public decimal? TotalMonthlyUsage { get; set; }
    public double? TotalMonthlyUsageDepth { get; set; }

    public static List<ZoneGroupMonthlyUsageDto> ListByZoneGroupID(QanatDbContext dbContext,
        int geographyID, int zoneGroupID)
    {
        var geographyIDParam = new SqlParameter("geographyID", geographyID);
        var zoneGroupIDParam = new SqlParameter("zoneGroupID", zoneGroupID);

        var monthlyUsages = dbContext.ZoneGroupMonthlyUsage.FromSqlRaw($"EXECUTE dbo.pZoneGroupMonthlyUsage @geographyID, @zoneGroupID", geographyIDParam, zoneGroupIDParam).ToList();
        
        return monthlyUsages.Select(x => new ZoneGroupMonthlyUsageDto()
        {
            ZoneName = x.ZoneName,
            ZoneColor = x.ZoneColor,
            EffectiveDate = x.EffectiveDate,
            UsageAmount = x.TotalMonthlyUsage,
            UsageAmountDepth = (decimal?)x.TotalMonthlyUsageDepth
        }).ToList();
    }
}