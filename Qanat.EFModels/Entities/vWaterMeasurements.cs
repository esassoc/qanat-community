using Microsoft.EntityFrameworkCore;
using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities;

public static class vWaterMeasurements
{
    public static async Task<List<WaterMeasurementQualityAssuranceDto>> ListByGeographyIDReportingPeriodIDWaterMeasurementTypeIDAndMonths(QanatDbContext dbContext, int geographyID, int reportingPeriodID, int waterMeasurementTypeID, List<int> months)
    {
        var waterMeasurements = await dbContext.vWaterMeasurements.AsNoTracking()
            .Where(x => x.GeographyID == geographyID && x.ReportingPeriodID == reportingPeriodID && x.WaterMeasurementTypeID == waterMeasurementTypeID && months.Contains(x.ReportedDate.Month))
            .ToListAsync();

        var grouped = waterMeasurements
            .GroupBy(x => new { x.UsageLocationID, x.UsageLocationName, x.UsageLocationArea, x.UsageLocationTypeID, x.UsageLocationTypeName, x.ParcelID, x.ParcelNumber, x.WaterAccountID, x.WaterAccountNumber, x.WaterAccountName, x.CoverCropStatus, x.FallowStatus })
            .Select(g => new WaterMeasurementQualityAssuranceDto
            {
                UsageLocationID = g.Key.UsageLocationID,
                UsageLocationName = g.Key.UsageLocationName,
                UsageLocationArea = g.Key.UsageLocationArea,
                UsageLocationTypeID = g.Key.UsageLocationTypeID,
                UsageLocationTypeName = g.Key.UsageLocationTypeName,
                ParcelID = g.Key.ParcelID,
                ParcelNumber = g.Key.ParcelNumber,
                WaterAccountID = g.Key.WaterAccountID,
                WaterAccountNumberAndName = g.Key.WaterAccountName == null ? $"#{g.Key.WaterAccountNumber}" : $"#{g.Key.WaterAccountNumber} ({g.Key.WaterAccountName})",
                CoverCropStatus = g.Key.CoverCropStatus,
                FallowStatus = g.Key.FallowStatus,
                SummedValueInFeet = g.Sum(x => x.ReportedValueInFeet),
                SummedValueInAcreFeet = g.Sum(x => x.ReportedValueInAcreFeet),
                ReportedValueInFeetByMonth = g.GroupBy(x => x.ReportedDate.Month).ToDictionary(mg => mg.Key, mg => mg.Sum(x => x.ReportedValueInFeet)),
                PercentileBucket = 0
            })
            .ToList();

        var sorted = grouped.OrderBy(x => x.SummedValueInFeet).ToList();
        var values = sorted.Select(x => x.SummedValueInFeet).ToList();

        if (values.Any())
        {
            var percentiles = new[] { 0.166, 0.333, 0.5, 0.666, 0.833 }; //MK 6/25/2025: Hard coded to correspond to front end buckets for map choropleth. Could data drive this eventually. 
            var thresholds = percentiles.Select(p => values[(int)Math.Floor(p * (values.Count - 1))]).ToList();
            foreach (var dto in sorted)
            {
                var value = dto.SummedValueInFeet;
                var bucket = thresholds.TakeWhile(t => value > t).Count();
                dto.PercentileBucket = bucket; // 0 through 5
            }
        }

        // Add in any Usage Location that did not have measurements for the given Reporting Period and Water Measurement Type.
        var usageLocationIDsWithData = new HashSet<int>(grouped.Select(g => g.UsageLocationID));
        var usageLocations = await dbContext.UsageLocations.AsNoTracking()
            .Include(ul => ul.UsageLocationType)
            .Include(ul => ul.Parcel).ThenInclude(p => p.WaterAccountParcels).ThenInclude(wap => wap.WaterAccount).ThenInclude(x => x.WaterAccountCoverCropStatuses)
            .Include(ul => ul.Parcel).ThenInclude(p => p.WaterAccountParcels).ThenInclude(wap => wap.WaterAccount).ThenInclude(x => x.WaterAccountFallowStatuses)
            .Where(x => x.GeographyID == geographyID && x.ReportingPeriodID == reportingPeriodID && !usageLocationIDsWithData.Contains(x.UsageLocationID))
            .ToListAsync();

        foreach (var usageLocation in usageLocations)
        {
            var waterAccount = usageLocation.Parcel.WaterAccountParcels
                .Where(x => x.ParcelID == usageLocation.ParcelID && x.ReportingPeriodID == usageLocation.ReportingPeriodID)
                .Select(x => x.WaterAccount)
                .FirstOrDefault();

            var coverCropSelfReport = waterAccount?.WaterAccountCoverCropStatuses
                .FirstOrDefault(x => x.ReportingPeriodID == usageLocation.ReportingPeriodID);

            var fallowSelfReport = waterAccount?.WaterAccountFallowStatuses
                .FirstOrDefault(x => x.ReportingPeriodID == usageLocation.ReportingPeriodID);

            var dto = new WaterMeasurementQualityAssuranceDto
            {
                UsageLocationID = usageLocation.UsageLocationID,
                UsageLocationName = usageLocation.Name,
                UsageLocationArea = usageLocation.Area,
                UsageLocationTypeID = usageLocation.UsageLocationTypeID!.Value,
                UsageLocationTypeName = usageLocation.UsageLocationType!.Name,
                ParcelID = usageLocation.ParcelID,
                ParcelNumber = usageLocation.Parcel?.ParcelNumber,
                WaterAccountID = waterAccount?.WaterAccountID,
                WaterAccountNumberAndName = waterAccount != null
                    ? waterAccount.WaterAccountName == null ? $"#{waterAccount.WaterAccountNumber}" : $"#{waterAccount.WaterAccountNumber} ({waterAccount.WaterAccountName})"
                    : string.Empty,
                SummedValueInFeet = null,
                SummedValueInAcreFeet = null,
                ReportedValueInFeetByMonth = new Dictionary<int, decimal>(),
                PercentileBucket = null,
                CoverCropStatus = coverCropSelfReport?.SelfReportStatus?.SelfReportStatusDisplayName,
                FallowStatus = fallowSelfReport?.SelfReportStatus?.SelfReportStatusDisplayName,
            };

            sorted.Add(dto);
        }

        return sorted;
    }
}
