using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using Qanat.EFModels.Entities;
using Qanat.Models.DataTransferObjects;

namespace Qanat.API.Helpers.WaterMeasurementsXL
{
    public class WaterMeasurementsXL
    {
        public static async Task<byte[]> CreateWaterMeasurementWBForGeographyAndYear(QanatDbContext dbContext, int geographyID, int year)
        {
            // this means we are looking at it from a geography's reporting period (year)'s perspective, so we want water measurements for given reporting period for all parcels
            var reportingPeriod = await ReportingPeriods.GetByGeographyIDAndYearAsync(dbContext, geographyID, year);

            var waterMeasurements = await dbContext.WaterMeasurements.AsNoTracking()
                .Include(x => x.WaterMeasurementType)
                .Include(x => x.UsageLocation).ThenInclude(x => x.Parcel)
                .Where(x => x.GeographyID == geographyID && x.ReportedDate >= reportingPeriod.StartDate && x.ReportedDate <= reportingPeriod.EndDate)
                .ToListAsync();
                
            var waterMeasurementLookup = waterMeasurements.ToLookup(x => x.WaterMeasurementTypeID);

            var waterAccountParcels = await dbContext.WaterAccountParcels.AsNoTracking()
                .Include(x => x.WaterAccount)
                .Where(x => x.GeographyID == geographyID && x.ReportingPeriodID == reportingPeriod.ReportingPeriodID)
                .ToListAsync();

            return await CreateWaterMeasurementWBImpl(dbContext, geographyID, waterMeasurementLookup, waterAccountParcels, [reportingPeriod]);
        }

        public static async Task<byte[]> CreateWaterMeasurementWBForGeographyAndParcel(QanatDbContext dbContext, int geographyID, int parcelID)
        {
            // this means we are looking at it from a parcel's perspective, so we want water measurements for all reporting periods for a parcel
            var reportingPeriods = await ReportingPeriods.ListByGeographyIDAsync(dbContext, geographyID);

            var waterMeasurements = dbContext.WaterMeasurements.AsNoTracking()
                .Include(x => x.WaterMeasurementType)
                .Include(x => x.UsageLocation).ThenInclude(x => x.Parcel)
                .Where(x => x.GeographyID == geographyID && x.UsageLocation.ParcelID == parcelID)
                .ToLookup(x => x.WaterMeasurementTypeID);

            var waterAccountParcels = dbContext.WaterAccountParcels.AsNoTracking()
                .Include(x => x.WaterAccount)
                .Where(x => x.GeographyID == geographyID && x.ParcelID == parcelID).ToList();

            return await CreateWaterMeasurementWBImpl(dbContext, geographyID, waterMeasurements, waterAccountParcels, reportingPeriods);
        }

        private static async Task<byte[]> CreateWaterMeasurementWBImpl(QanatDbContext dbContext, int geographyID, ILookup<int?, WaterMeasurement> waterMeasurements, IEnumerable<WaterAccountParcel> waterAccountParcels, List<ReportingPeriodDto> reportingPeriods)
        {
            var waterAccountParcelLookup = waterAccountParcels.ToLookup(x => x.ParcelID);
            var waterTypesForGeography = dbContext.WaterMeasurementTypes.AsNoTracking().Where(x => x.GeographyID == geographyID).ToList();

            var workbook = new XLWorkbook();
            foreach (var waterType in waterTypesForGeography)
            {
                var rowNumber = 1;
                var sheetName = waterType.ShortName;

                var workSheet = workbook.Worksheets.Add(sheetName);
                workSheet.Cell(rowNumber, 1).Value = "Usage Location Name (APN or Field ID)";
                workSheet.Cell(rowNumber, 2).Value = "APN";
                workSheet.Cell(rowNumber, 3).Value = "Usage Location Area (ac)";
                workSheet.Cell(rowNumber, 4).Value = "Reporting Period";
                workSheet.Cell(rowNumber, 5).Value = "Water Account #";
                workSheet.Cell(rowNumber, 6).Value = "Parcel Status";

                var waterMeasurementsForWaterMeasurementType = waterMeasurements[waterType.WaterMeasurementTypeID].ToList();
                var usageHeaders = new List<string>();
                var reportingPeriod = reportingPeriods.First();
                var startDate = reportingPeriod.StartDate;
                while (startDate <= reportingPeriod.EndDate)
                {
                    usageHeaders.Add(GetUsageHeader(startDate));
                    startDate = startDate.AddMonths(1);
                }

                var column = 7;
                foreach (var usageHeader in usageHeaders)
                {
                    workSheet.Cell(rowNumber, column).Value = usageHeader;
                    workSheet.Cell(rowNumber, column + 12).Value = $"{usageHeader} Comments";
                    column++;
                }

                var usageLocationGroups = waterMeasurementsForWaterMeasurementType.ToLookup(x => $"{x.UsageLocation.Name}").OrderBy(x => x.Key);
                foreach (var usageLocationGroup in usageLocationGroups)
                {
                    foreach (var reportingPeriodDto in reportingPeriods)
                    {
                        var waterMeasurementsForReportingPeriod = usageLocationGroup.Where(x => x.ReportedDate >= reportingPeriodDto.StartDate && x.ReportedDate <= reportingPeriodDto.EndDate).ToList();
                        if (waterMeasurementsForReportingPeriod.Any())
                        {
                            rowNumber++;
                            var first = waterMeasurementsForReportingPeriod.First();
                            workSheet.Cell(rowNumber, 1).Value = first.UsageLocation.Name;
                            workSheet.Cell(rowNumber, 2).Value = first.UsageLocation.Parcel.ParcelNumber;
                            workSheet.Cell(rowNumber, 3).Value = first.UsageLocation.Parcel.ParcelArea;
                            workSheet.Cell(rowNumber, 4).Value = reportingPeriodDto.EndDate.Year;
                            var waterAccountParcelsForUsageLocation =
                                waterAccountParcelLookup[first.UsageLocation.ParcelID].ToList();
                            if (waterAccountParcelsForUsageLocation.Any())
                            {
                                var waterAccountParcelDto = waterAccountParcelsForUsageLocation.SingleOrDefault(x =>
                                    x.ReportingPeriodID == reportingPeriodDto.ReportingPeriodID);
                                if (waterAccountParcelDto != null)
                                {
                                    workSheet.Cell(rowNumber, 5).Value =
                                        waterAccountParcelDto.WaterAccount?.WaterAccountNumber;
                                }
                                else
                                {
                                    workSheet.Cell(rowNumber, 5).Value = string.Empty;
                                }
                            }
                            else
                            {
                                workSheet.Cell(rowNumber, 5).Value = string.Empty;
                            }

                            workSheet.Cell(rowNumber, 6).Value =
                                first.UsageLocation.Parcel.ParcelStatus.ParcelStatusDisplayName;
                            GetParcelUsage(waterMeasurementsForReportingPeriod, usageHeaders, workSheet, rowNumber);
                        }
                    }
                }
            }

            await using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        private static void GetParcelUsage(List<WaterMeasurement> usageLocationGroup, List<string> usageHeaders, IXLWorksheet workSheet, int rowNumber)
        {
            var lookup = usageLocationGroup.ToLookup(x => GetUsageHeader(x.ReportedDate));
            var columnNumber = 7;
            foreach (var usages in usageHeaders.Select(header => lookup[header].ToList()))
            {
                if (usages.Any())
                {
                    workSheet.Cell(rowNumber, columnNumber).Value = usages.Sum(x => x.ReportedValueInAcreFeet);
                    workSheet.Cell(rowNumber, columnNumber + 12).Value =
                        string.Join("; ", usages.Select(x => x.Comment));
                }
                else
                {
                    workSheet.Cell(rowNumber, columnNumber).Value = string.Empty;
                    workSheet.Cell(rowNumber, columnNumber + 12).Value = string.Empty;
                }

                columnNumber++;
            }
        }

        private static string GetUsageHeader(DateTime reportedDate)
        {
            return $"{reportedDate:MMM}_AF";
        }
    }
}