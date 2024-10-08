using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using Qanat.EFModels.Entities;
using Qanat.Models.DataTransferObjects;

namespace Qanat.API.WaterMeasurementsXL
{
    public class WaterMeasurementsXL
    {
        public static async Task<byte[]> CreateWaterMeasurementWBForGeography(QanatDbContext dbContext, int geographyID, List<string> usageEntityNames = null)
        {
            var workbook = new XLWorkbook();

            var waterMeasurements = dbContext.WaterMeasurements.AsNoTracking()
                .Include(x => x.WaterMeasurementType)
                .Where(x => x.GeographyID == geographyID && (usageEntityNames == null || usageEntityNames.Contains(x.UsageEntityName)))
                .ToLookup(x => x.WaterMeasurementTypeID);

            var reportingMonth = dbContext.ReportingPeriods.Single(x => x.GeographyID == geographyID).StartMonth;

            var waterAccountParcelDtos = dbContext.WaterAccountParcels.AsNoTracking()
                .Include(x => x.WaterAccount)
                .Where(x => x.GeographyID == geographyID).ToList()
                .Select(x => new WaterAccountParcelDto()
                {
                    GeographyID = x.GeographyID,
                    ParcelID = x.ParcelID,
                    EffectiveYear = x.EffectiveYear,
                    WaterAccountID = x.WaterAccountID,
                    WaterAccount = x.WaterAccount?.AsWaterAccountMinimalDto(),

                }).ToList()
                .GroupBy(x => x.ParcelID)
                .ToDictionary(x => x.Key, x => x.OrderByDescending(y => y.EffectiveYear).ToList());

            var parcelStatuses = dbContext.Parcels.Where(x => x.GeographyID == geographyID)
                .ToDictionary(x => x.PrimaryKey, x => x.ParcelStatus);

            var usageEntityDictionary = dbContext.UsageEntities.AsNoTracking()
                .Include(x => x.Parcel)
                .Where(x => x.GeographyID == geographyID && (usageEntityNames == null || usageEntityNames.Contains(x.UsageEntityName)))
                .ToDictionary(x => x.UsageEntityName, y => new
                {
                    y.UsageEntityArea,
                    y.Parcel.ParcelNumber,
                    y.Parcel.ParcelID

                });

            var waterTypesForGeography = dbContext.WaterMeasurementTypes.AsNoTracking().Where(x => x.GeographyID == geographyID).ToList();

            foreach (var waterType in waterTypesForGeography)
            {
                var rowNumber = 1;
                var sheetName = waterType.WaterMeasurementTypeName.Length > 31
                    ? waterType.WaterMeasurementTypeName[..31]
                    : waterType.WaterMeasurementTypeName;

                var workSheet = workbook.Worksheets.Add(sheetName);
                workSheet.Cell(rowNumber, 1).Value = "Usage Entity Name (APN or Field ID)";
                workSheet.Cell(rowNumber, 2).Value = "APN";
                workSheet.Cell(rowNumber, 3).Value = "Usage Entity Area (ac)";
                workSheet.Cell(rowNumber, 4).Value = "Water Account #";
                workSheet.Cell(rowNumber, 5).Value = "Effective Year";
                workSheet.Cell(rowNumber, 6).Value = "Parcel Status";

                var waterMeasurementsForWaterMeasurementType = waterMeasurements[waterType.WaterMeasurementTypeID].ToList();
                var usageHeaders = waterMeasurementsForWaterMeasurementType
                    .GroupBy(x => new { x.ReportedDate, x.WaterMeasurementType.WaterMeasurementTypeName, x.WaterMeasurementType.SortOrder })
                    .OrderByDescending(x => x.Key.ReportedDate.Year)
                    .ThenBy(x => x.Key.ReportedDate.Month)
                    .ThenBy(x => x.Key.SortOrder)
                    .Select(x => GetUsageHeader(x.Key.ReportedDate, x.Key.WaterMeasurementTypeName)).ToList();

                var column = 7;
                foreach (var usageHeader in usageHeaders)
                {
                    workSheet.Cell(rowNumber, column).Value = usageHeader;
                    workSheet.Cell(rowNumber, column+1).Value = $"{usageHeader} Comments";
                    column += 2;
                }

                var usageEntityGroups = waterMeasurementsForWaterMeasurementType.GroupBy(x => x.UsageEntityName).OrderBy(x => x.Key);

                foreach (var usageEntityGroup in usageEntityGroups)
                {
                    rowNumber++;
                    workSheet.Cell(rowNumber, 1).Value = usageEntityGroup.Key;
                    if (usageEntityDictionary.TryGetValue(usageEntityGroup.Key, out var usageEntity))
                    {
                        var waterAccountParcelDto =
                            waterAccountParcelDtos.TryGetValue(usageEntity.ParcelID, out var dto)
                                ? dto.Where(x => IsEffectiveDateInReportingPeriod(x.EffectiveYear, reportingMonth))
                                    .MaxBy(x => x.EffectiveYear)
                                : null;
                        var parcelStatus = parcelStatuses.TryGetValue(usageEntity.ParcelID, out var status) ? status : null;
                        workSheet.Cell(rowNumber, 2).Value = (usageEntity.ParcelNumber);
                        workSheet.Cell(rowNumber, 3).Value =
                            (usageEntity.UsageEntityArea);
                        if (waterAccountParcelDto != null)
                        {
                            workSheet.Cell(rowNumber, 4).Value = waterAccountParcelDto.WaterAccount?.WaterAccountNumber;
                            workSheet.Cell(rowNumber, 5).Value = waterAccountParcelDto.EffectiveYear;
                        }
                        workSheet.Cell(rowNumber, 6).Value = parcelStatus?.ParcelStatusDisplayName;
                    }
                    else
                    {
                        workSheet.Cell(rowNumber, 2).Value = (string.Empty);
                        workSheet.Cell(rowNumber, 3).Value =(string.Empty);
                        workSheet.Cell(rowNumber, 4).Value =(string.Empty);
                        workSheet.Cell(rowNumber, 5).Value =(string.Empty);
                    }

                    GetParcelUsage(usageEntityGroup, usageHeaders, workSheet, rowNumber);
                }
            }

            await using var stream = new MemoryStream();
            workbook.SaveAs(stream);

            return stream.ToArray();
        }

        private static void GetParcelUsage(IGrouping<string, WaterMeasurement> usageEntityGroup, List<string> usageHeaders, IXLWorksheet workSheet, int rowNumber)
        {
            var parcelUsageByMonthYear = usageEntityGroup
                .ToLookup(x => GetUsageHeader(x.ReportedDate, x.WaterMeasurementType.WaterMeasurementTypeName));

            var columnNumber = 7;
            foreach (var usages in usageHeaders.Select(header => parcelUsageByMonthYear[header].ToList()))
            {
                if (usages.Any())
                {
                    workSheet.Cell(rowNumber, columnNumber).Value =(usages.Sum(x => x.ReportedValueInAcreFeet));
                    workSheet.Cell(rowNumber, columnNumber+1).Value = (string.Join("; ", usages.Select(x => x.Comment)));
                }
                else
                {
                    workSheet.Cell(rowNumber, columnNumber).Value =(string.Empty);
                    workSheet.Cell(rowNumber, columnNumber+1).Value =(string.Empty);
                }
                columnNumber+=2;
            }
        }

        private static string GetUsageHeader(DateTime reportedDate, string waterUseTypeName)
        {
            return $"{reportedDate:yyyy_MMM}_{(waterUseTypeName)}_AF";
        }

        private static bool IsEffectiveDateInReportingPeriod(int effectiveDate, int month)
        {
            return DateTime.Parse($"{month}/01/{effectiveDate}") <= DateTime.UtcNow;
        }
    }
}