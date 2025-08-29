using Microsoft.EntityFrameworkCore;
using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities
{
    public static class ParcelSupplies
    {
        private static IQueryable<ParcelSupply> GetImpl(QanatDbContext dbContext)
        {
            var parcelSupplies = dbContext.ParcelSupplies
                .Include(x => x.WaterType)
                .Include(x => x.Parcel)
                .AsNoTracking();
            return parcelSupplies;
        }

        public static List<ParcelWaterSupplyBreakdownDto> GetParcelWaterSupplyBreakdownForYearAsDto(QanatDbContext dbContext, DateTime endDate)
        {
            var parcelWaterSupplyBreakdownForYear = dbContext.ParcelSupplies
                .AsNoTracking()
                .Where(x => x.EffectiveDate <= endDate &&
                            x.WaterTypeID != null)
                .ToList()
                .GroupBy(x => x.ParcelID)
                .Select(x => new ParcelWaterSupplyBreakdownDto
                {
                    ParcelID = x.Key,
                    WaterSupplyByWaterType = x.Where(y => y.WaterTypeID.HasValue)
                        .GroupBy(y => y.WaterTypeID.Value)
                        .ToDictionary(y => y.Key, y => y.Sum(z => z.TransactionAmount))
                }).ToList();
            return parcelWaterSupplyBreakdownForYear;
        }

        public static List<ParcelSupplyDetailDto> ListByParcelIDAsDetailDto(QanatDbContext dbContext, IEnumerable<int> parcelIDs)
        {
            return dbContext.ParcelSupplies
                .Include(x => x.Geography)
                .Include(x => x.Parcel)
                .Include(x => x.WaterType)
                .Where(x => parcelIDs.Contains(x.ParcelID))
                .OrderByDescending(x => x.EffectiveDate)
                .ThenByDescending(x => x.TransactionDate)
                .Select(x => x.AsDetailDto())
                .ToList();
        }

        public static List<ParcelSupplyDetailDto> ListByParcelIDAsDetailDto(QanatDbContext dbContext, int parcelID)
        {
            return ListByParcelIDAsDetailDto(dbContext, new List<int> { parcelID });
        }

        public static List<ParcelActivityDto> ListAsParcelActivityDto(QanatDbContext dbContext, List<int> parcelIDs)
        {
            var parcelSupplies = GetImpl(dbContext)
                .Where(x => parcelIDs.Contains(x.ParcelID)).ToList()
                .GroupBy(x => new { EffectiveDate = new DateTime(x.EffectiveDate.Year, x.EffectiveDate.Month, 1), x.WaterTypeID })
                .Select(x =>
                {
                    var parcels = x.DistinctBy(x => x.ParcelID).ToList();

                    return new ParcelActivityDto()
                    {
                        EffectiveDate = x.Key.EffectiveDate,
                        WaterTypeID = x.Key.WaterTypeID,
                        TransactionAmount = x.Sum(y =>   y.TransactionAmount),
                        ParcelSupplies = x.Select(y => y.AsDetailDto()).OrderBy(x => x.TransactionDate).ToList(),
                        ParcelCount = parcels.Count(),
                        ParcelArea = parcels.Sum(x => x.Parcel.ParcelArea)
                    };
                }).OrderByDescending(x => x.EffectiveDate).ThenByDescending(x => x.TransactionTypeID)
                .ToList();

            return parcelSupplies;
        }

        public static void Create(QanatDbContext dbContext, Parcel parcel, ParcelSupplyUpsertDto parcelSupplyUpsertDto, int userID)
        {
            BulkCreate(dbContext, [parcel],  parcelSupplyUpsertDto, userID, false);
        }

        public static int BulkCreate(QanatDbContext dbContext, List<Parcel> parcels, ParcelSupplyUpsertDto parcelSupplyUpsertDto, int userID, bool convertFromAcreFeetPerAcre)
        {
            var transactionDate = DateTime.UtcNow;
            var effectiveDate = DateTime.Parse(parcelSupplyUpsertDto.EffectiveDate).AddHours(8);

            var parcelSupplies = parcels.Select(x => new ParcelSupply()
            {
                ParcelID = x.ParcelID,
                TransactionDate = transactionDate,
                EffectiveDate = effectiveDate,
                TransactionAmount = convertFromAcreFeetPerAcre ? parcelSupplyUpsertDto.TransactionAmount.Value * (decimal)x.ParcelArea : parcelSupplyUpsertDto.TransactionAmount.Value,
                WaterTypeID = parcelSupplyUpsertDto.WaterTypeID,
                UserID = userID,
                UserComment = parcelSupplyUpsertDto.UserComment,
                GeographyID = x.GeographyID
            }).ToList();

            dbContext.ParcelSupplies.AddRange(parcelSupplies);
            dbContext.SaveChanges();

            return parcelSupplies.Count;
        }

        public static int CreateFromCSV(QanatDbContext dbContext, List<ParcelTransactionCSV> records, string uploadedFileName, DateTime effectiveDate, int waterTypeID, int userID, int geographyID)
        {
            var parcelNumbers = records.Select(x => x.UsageLocationName).ToList();
            var parcels = Parcels.ListByGeographyIDAndParcelNumbers(dbContext, geographyID, parcelNumbers);

            var transactionDate = DateTime.UtcNow;
            effectiveDate = effectiveDate.AddHours(8);

            var parcelSupplies = new List<ParcelSupply>();
            foreach (var record in records)
            {
                var parcel = parcels.SingleOrDefault(x => x.ParcelNumber == record.UsageLocationName);
                var parcelSupply = new ParcelSupply()
                {
                    ParcelID = parcel.ParcelID,
                    TransactionDate = transactionDate,
                    EffectiveDate = effectiveDate,
                    TransactionAmount = (decimal)record.Quantity,
                    WaterTypeID = waterTypeID,
                    UserID = userID,
                    UploadedFileName = uploadedFileName,
                    GeographyID = geographyID
                };
                parcelSupplies.Add(parcelSupply);
            }

            dbContext.ParcelSupplies.AddRange(parcelSupplies);
            dbContext.SaveChanges();

            return parcelSupplies.Count;
        }
    }
}