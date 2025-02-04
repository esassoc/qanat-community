using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Qanat.Common.GeoSpatial;

namespace Qanat.EFModels.Entities
{
    public partial class QanatDbContext
    {
        public QanatDbContext(string connectionString) : this(GetOptions(connectionString))
        {
        }

        private static DbContextOptions<QanatDbContext> GetOptions(string connectionString)
        {
            var optionsBuilder = new DbContextOptionsBuilder<QanatDbContext>();
            optionsBuilder.UseSqlServer(connectionString, x =>
            {
                x.CommandTimeout((int)TimeSpan.FromMinutes(3).TotalSeconds);
                x.UseNetTopologySuite();
            });
            return optionsBuilder.Options;
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDbFunction(typeof(QanatDbContext)
                    .GetMethod(nameof(fParcelStagingChanges), new[] { typeof(int) }))
                .HasName("fParcelStagingChanges");

            modelBuilder.HasDbFunction(typeof(QanatDbContext)
                    .GetMethod(nameof(fWaterAccountUser), new[] { typeof(int) }))
                .HasName("fWaterAccountUser");

            modelBuilder.HasDbFunction(typeof(QanatDbContext)
                    .GetMethod(nameof(fReportingPeriod), new[] { typeof(int) }))
                .HasName("fReportingPeriod");

            modelBuilder.HasDbFunction(typeof(QanatDbContext)
                    .GetMethod(nameof(fWaterAccountParcelByGeographyAndYear), new[] { typeof(int), typeof(int) }))
                .HasName("fWaterAccountParcelByGeographyAndYear");

            modelBuilder.HasDbFunction(typeof(QanatDbContext)
                    .GetMethod(nameof(fWaterAccountParcelByWaterAccountAndYear), new[] { typeof(int), typeof(int) }))
                .HasName("fWaterAccountParcelByWaterAccountAndYear");

            modelBuilder
                .Entity<WaterAccountSuggestion>()
                .Property(e => e.Parcels)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, GeoJsonSerializer.DefaultSerializerOptions),
                    v => JsonSerializer.Deserialize<List<ParcelIDAndNumber>>(v, GeoJsonSerializer.DefaultSerializerOptions)
                    );

            modelBuilder.Entity<vParcelDetailed>(entity =>
            {
                entity.ToView("vParcelDetailed");
            });

            modelBuilder
                .Entity<vParcelDetailed>()
                .OwnsMany(x => x.WellsOnParcel, x => x.ToJson());

            modelBuilder
                .Entity<vParcelDetailed>()
                .OwnsMany(x => x.IrrigatedByWells, x => x.ToJson());
        }

        public virtual DbSet<vParcelDetailed> vParcelDetaileds { get; set; }
        public virtual DbSet<ParcelWaterSupplyAndUsage> ParcelWaterSupplyAndUsages { get; set; }
        public virtual DbSet<WaterTypeSupply> WaterTypeSupplies { get; set; }
        public virtual DbSet<WaterTypeMonthlySupply> WaterTypeMonthlySupplies { get; set; }
        public virtual DbSet<WaterAccountMostRecentEffectiveDate> WaterAccountMostRecentEffectiveDate { get; set; }
        public virtual DbSet<WaterAccountBudgetReportByGeographyAndYear> WaterAccountBudgetReportByGeographyAndDateRanges { get; set; }
        public virtual DbSet<MonthlyUsageSummary> MonthlyUsageSummary { get; set; }
        public virtual DbSet<WaterAccountSuggestion> WaterAccountSuggestions { get; set; }
        public virtual DbSet<ZoneGroupMonthlyUsage> ZoneGroupMonthlyUsage { get; set; }
        public IQueryable<fParcelStagingChanges> fParcelStagingChanges(int geographyID) => FromExpression(() => fParcelStagingChanges(geographyID));
        public IQueryable<fWaterAccountUser> fWaterAccountUser(int userID) => FromExpression(() => fWaterAccountUser(userID));
        public IQueryable<fReportingPeriod> fReportingPeriod(int year) => FromExpression(() => fReportingPeriod(year));
        public IQueryable<fWaterAccountParcel> fWaterAccountParcelByGeographyAndYear(int geographyID, int year) => FromExpression(() => fWaterAccountParcelByGeographyAndYear(geographyID, year));
        public IQueryable<fWaterAccountParcel> fWaterAccountParcelByWaterAccountAndYear(int waterAccountID, int year) => FromExpression(() => fWaterAccountParcelByWaterAccountAndYear(waterAccountID, year));
    }
}