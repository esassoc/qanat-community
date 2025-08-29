using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Qanat.EFModels.Entities;

public partial class QanatDbContext : DbContext
{
    public QanatDbContext(DbContextOptions<QanatDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AllocationPlan> AllocationPlans { get; set; }

    public virtual DbSet<AllocationPlanPeriod> AllocationPlanPeriods { get; set; }

    public virtual DbSet<CustomAttribute> CustomAttributes { get; set; }

    public virtual DbSet<CustomRichText> CustomRichTexts { get; set; }

    public virtual DbSet<ExternalMapLayer> ExternalMapLayers { get; set; }

    public virtual DbSet<FileResource> FileResources { get; set; }

    public virtual DbSet<FrequentlyAskedQuestion> FrequentlyAskedQuestions { get; set; }

    public virtual DbSet<FrequentlyAskedQuestionFaqDisplayLocationType> FrequentlyAskedQuestionFaqDisplayLocationTypes { get; set; }

    public virtual DbSet<Geography> Geographies { get; set; }

    public virtual DbSet<GeographyAllocationPlanConfiguration> GeographyAllocationPlanConfigurations { get; set; }

    public virtual DbSet<GeographyBoundary> GeographyBoundaries { get; set; }

    public virtual DbSet<GeographyConfiguration> GeographyConfigurations { get; set; }

    public virtual DbSet<GeographyUser> GeographyUsers { get; set; }

    public virtual DbSet<IrrigationMethod> IrrigationMethods { get; set; }

    public virtual DbSet<Meter> Meters { get; set; }

    public virtual DbSet<MeterReading> MeterReadings { get; set; }

    public virtual DbSet<MeterReadingMonthlyInterpolation> MeterReadingMonthlyInterpolations { get; set; }

    public virtual DbSet<ModelBoundary> ModelBoundaries { get; set; }

    public virtual DbSet<ModelScenario> ModelScenarios { get; set; }

    public virtual DbSet<ModelUser> ModelUsers { get; set; }

    public virtual DbSet<MonitoringWell> MonitoringWells { get; set; }

    public virtual DbSet<MonitoringWellMeasurement> MonitoringWellMeasurements { get; set; }

    public virtual DbSet<OpenETSync> OpenETSyncs { get; set; }

    public virtual DbSet<OpenETSyncHistory> OpenETSyncHistories { get; set; }

    public virtual DbSet<Parcel> Parcels { get; set; }

    public virtual DbSet<ParcelCustomAttribute> ParcelCustomAttributes { get; set; }

    public virtual DbSet<ParcelGeometry> ParcelGeometries { get; set; }

    public virtual DbSet<ParcelHistory> ParcelHistories { get; set; }

    public virtual DbSet<ParcelStaging> ParcelStagings { get; set; }

    public virtual DbSet<ParcelSupply> ParcelSupplies { get; set; }

    public virtual DbSet<ParcelWaterAccountHistory> ParcelWaterAccountHistories { get; set; }

    public virtual DbSet<ParcelZone> ParcelZones { get; set; }

    public virtual DbSet<ReferenceWell> ReferenceWells { get; set; }

    public virtual DbSet<ReportingPeriod> ReportingPeriods { get; set; }

    public virtual DbSet<ScenarioRun> ScenarioRuns { get; set; }

    public virtual DbSet<ScenarioRunFileResource> ScenarioRunFileResources { get; set; }

    public virtual DbSet<ScenarioRunOutputFile> ScenarioRunOutputFiles { get; set; }

    public virtual DbSet<StatementBatch> StatementBatches { get; set; }

    public virtual DbSet<StatementBatchWaterAccount> StatementBatchWaterAccounts { get; set; }

    public virtual DbSet<StatementTemplate> StatementTemplates { get; set; }

    public virtual DbSet<SupportTicket> SupportTickets { get; set; }

    public virtual DbSet<SupportTicketNote> SupportTicketNotes { get; set; }

    public virtual DbSet<UploadedGdb> UploadedGdbs { get; set; }

    public virtual DbSet<UploadedWellGdb> UploadedWellGdbs { get; set; }

    public virtual DbSet<UsageLocation> UsageLocations { get; set; }

    public virtual DbSet<UsageLocationCrop> UsageLocationCrops { get; set; }

    public virtual DbSet<UsageLocationGeometry> UsageLocationGeometries { get; set; }

    public virtual DbSet<UsageLocationHistory> UsageLocationHistories { get; set; }

    public virtual DbSet<UsageLocationParcelHistory> UsageLocationParcelHistories { get; set; }

    public virtual DbSet<UsageLocationType> UsageLocationTypes { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<WaterAccount> WaterAccounts { get; set; }

    public virtual DbSet<WaterAccountContact> WaterAccountContacts { get; set; }

    public virtual DbSet<WaterAccountCoverCropStatus> WaterAccountCoverCropStatuses { get; set; }

    public virtual DbSet<WaterAccountCustomAttribute> WaterAccountCustomAttributes { get; set; }

    public virtual DbSet<WaterAccountFallowStatus> WaterAccountFallowStatuses { get; set; }

    public virtual DbSet<WaterAccountParcel> WaterAccountParcels { get; set; }

    public virtual DbSet<WaterAccountReconciliation> WaterAccountReconciliations { get; set; }

    public virtual DbSet<WaterAccountUser> WaterAccountUsers { get; set; }

    public virtual DbSet<WaterAccountUserStaging> WaterAccountUserStagings { get; set; }

    public virtual DbSet<WaterMeasurement> WaterMeasurements { get; set; }

    public virtual DbSet<WaterMeasurementSelfReport> WaterMeasurementSelfReports { get; set; }

    public virtual DbSet<WaterMeasurementSelfReportFileResource> WaterMeasurementSelfReportFileResources { get; set; }

    public virtual DbSet<WaterMeasurementSelfReportLineItem> WaterMeasurementSelfReportLineItems { get; set; }

    public virtual DbSet<WaterMeasurementType> WaterMeasurementTypes { get; set; }

    public virtual DbSet<WaterMeasurementTypeDependency> WaterMeasurementTypeDependencies { get; set; }

    public virtual DbSet<WaterType> WaterTypes { get; set; }

    public virtual DbSet<Well> Wells { get; set; }

    public virtual DbSet<WellFileResource> WellFileResources { get; set; }

    public virtual DbSet<WellIrrigatedParcel> WellIrrigatedParcels { get; set; }

    public virtual DbSet<WellMeter> WellMeters { get; set; }

    public virtual DbSet<WellRegistration> WellRegistrations { get; set; }

    public virtual DbSet<WellRegistrationContact> WellRegistrationContacts { get; set; }

    public virtual DbSet<WellRegistrationFileResource> WellRegistrationFileResources { get; set; }

    public virtual DbSet<WellRegistrationIrrigatedParcel> WellRegistrationIrrigatedParcels { get; set; }

    public virtual DbSet<WellRegistrationMetadatum> WellRegistrationMetadata { get; set; }

    public virtual DbSet<WellRegistrationWaterUse> WellRegistrationWaterUses { get; set; }

    public virtual DbSet<WellType> WellTypes { get; set; }

    public virtual DbSet<Zone> Zones { get; set; }

    public virtual DbSet<ZoneGroup> ZoneGroups { get; set; }

    public virtual DbSet<vGeoServerAllParcel> vGeoServerAllParcels { get; set; }

    public virtual DbSet<vGeoServerAllUsageLocation> vGeoServerAllUsageLocations { get; set; }

    public virtual DbSet<vGeoServerCNRAMonitoringWell> vGeoServerCNRAMonitoringWells { get; set; }

    public virtual DbSet<vGeoServerMonitoringWell> vGeoServerMonitoringWells { get; set; }

    public virtual DbSet<vGeoServerYoloWRIDMonitoringWell> vGeoServerYoloWRIDMonitoringWells { get; set; }

    public virtual DbSet<vGeoServerZoneGroup> vGeoServerZoneGroups { get; set; }

    public virtual DbSet<vGeoserverGeographyGSABoundary> vGeoserverGeographyGSABoundaries { get; set; }

    public virtual DbSet<vMeterReading> vMeterReadings { get; set; }

    public virtual DbSet<vParcelSupplyTransactionHistory> vParcelSupplyTransactionHistories { get; set; }

    public virtual DbSet<vWaterMeasurement> vWaterMeasurements { get; set; }

    public virtual DbSet<vWaterMeasurementSourceOfRecord> vWaterMeasurementSourceOfRecords { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AllocationPlan>(entity =>
        {
            entity.HasKey(e => e.AllocationPlanID).HasName("PK_AllocationPlan_AllocationPlanID");

            entity.Property(e => e.LastUpdated).HasDefaultValueSql("(getutcdate())");

            entity.HasOne(d => d.GeographyAllocationPlanConfiguration).WithMany(p => p.AllocationPlans).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Geography).WithMany(p => p.AllocationPlans).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.WaterType).WithMany(p => p.AllocationPlans).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Zone).WithMany(p => p.AllocationPlans).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<AllocationPlanPeriod>(entity =>
        {
            entity.HasKey(e => e.AllocationPlanPeriodID).HasName("PK_AllocationPlanPeriod_AllocationPlanPeriodID");

            entity.HasOne(d => d.AllocationPlan).WithMany(p => p.AllocationPlanPeriods).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<CustomAttribute>(entity =>
        {
            entity.HasKey(e => e.CustomAttributeID).HasName("PK_CustomAttribute_CustomAttributeID");

            entity.HasOne(d => d.Geography).WithMany(p => p.CustomAttributes).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<CustomRichText>(entity =>
        {
            entity.HasKey(e => e.CustomRichTextID).HasName("PK_CustomRichText_CustomRichTextID");
        });

        modelBuilder.Entity<ExternalMapLayer>(entity =>
        {
            entity.HasKey(e => e.ExternalMapLayerID).HasName("PK_ExternalMapLayer_ExternalMapLayerID");

            entity.HasOne(d => d.Geography).WithMany(p => p.ExternalMapLayers).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<FileResource>(entity =>
        {
            entity.HasKey(e => e.FileResourceID).HasName("PK_FileResource_FileResourceID");

            entity.HasOne(d => d.CreateUser).WithMany(p => p.FileResources)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FileResource_User_CreateUserID_UserID");
        });

        modelBuilder.Entity<FrequentlyAskedQuestion>(entity =>
        {
            entity.HasKey(e => e.FrequentlyAskedQuestionID).HasName("PK_FrequentlyAskedQuestion_FrequentlyAskedQuestionID");
        });

        modelBuilder.Entity<FrequentlyAskedQuestionFaqDisplayLocationType>(entity =>
        {
            entity.HasKey(e => e.FrequentlyAskedQuestionFaqDisplayLocationTypeID).HasName("PK_FrequentlyAskedQuestionFaqDisplayLocationType_FrequentlyAskedQuestionFaqDisplayLocationTypeID");
        });

        modelBuilder.Entity<Geography>(entity =>
        {
            entity.HasKey(e => e.GeographyID).HasName("PK_Geography_GeographyID");

            entity.Property(e => e.GeographyID).ValueGeneratedNever();
            entity.Property(e => e.LandownerDashboardSupplyLabel).HasDefaultValue("Supply");
            entity.Property(e => e.LandownerDashboardUsageLabel).HasDefaultValue("Usage");
            entity.Property(e => e.ShowSupplyOnWaterBudgetComponent).HasDefaultValue(true);

            entity.HasOne(d => d.GeographyConfiguration).WithOne(p => p.Geography).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.SourceOfRecordWaterMeasurementType).WithMany(p => p.GeographySourceOfRecordWaterMeasurementTypes).HasConstraintName("FK_Geography_WaterMeasurementType_SourceOfRecordWaterMeasurementTypeID_WaterMeasurementTypeID");
        });

        modelBuilder.Entity<GeographyAllocationPlanConfiguration>(entity =>
        {
            entity.HasKey(e => e.GeographyAllocationPlanConfigurationID).HasName("PK_GeographyAllocationPlanConfiguration_GeographyAllocationPlanConfigurationID");

            entity.HasOne(d => d.Geography).WithOne(p => p.GeographyAllocationPlanConfiguration).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.ZoneGroup).WithMany(p => p.GeographyAllocationPlanConfigurations).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<GeographyBoundary>(entity =>
        {
            entity.HasKey(e => e.GeographyBoundaryID).HasName("PK_GeographyBoundary_GeographyBoundaryID");

            entity.Property(e => e.GeographyBoundaryID).ValueGeneratedNever();

            entity.HasOne(d => d.Geography).WithOne(p => p.GeographyBoundary).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<GeographyConfiguration>(entity =>
        {
            entity.HasKey(e => e.GeographyConfigurationID).HasName("PK_GeographyConfiguration_GeographyConfigurationID");

            entity.Property(e => e.GeographyConfigurationID).ValueGeneratedNever();
        });

        modelBuilder.Entity<GeographyUser>(entity =>
        {
            entity.HasKey(e => e.GeographyUserID).HasName("PK_GeographyUser_GeographyUserID");

            entity.HasOne(d => d.Geography).WithMany(p => p.GeographyUsers).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.User).WithMany(p => p.GeographyUsers).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<IrrigationMethod>(entity =>
        {
            entity.HasKey(e => e.IrrigationMethodID).HasName("PK_IrrigationMethod_IrrigationMethodID");

            entity.HasOne(d => d.Geography).WithMany(p => p.IrrigationMethods).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Meter>(entity =>
        {
            entity.HasKey(e => e.MeterID).HasName("PK_Meter_MeterID");

            entity.HasOne(d => d.Geography).WithMany(p => p.Meters).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<MeterReading>(entity =>
        {
            entity.HasKey(e => e.MeterReadingID).HasName("PK_MeterReading_MeterReadingID");

            entity.HasOne(d => d.Geography).WithMany(p => p.MeterReadings).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Meter).WithMany(p => p.MeterReadings).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Well).WithMany(p => p.MeterReadings).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<MeterReadingMonthlyInterpolation>(entity =>
        {
            entity.HasKey(e => e.MeterReadingMonthlyInterpolationID).HasName("PK_MeterReadingMonthlyInterpolation_MeterReadingMonthlyInterpolationID");

            entity.HasOne(d => d.Geography).WithMany(p => p.MeterReadingMonthlyInterpolations).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Meter).WithMany(p => p.MeterReadingMonthlyInterpolations).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Well).WithMany(p => p.MeterReadingMonthlyInterpolations).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<ModelBoundary>(entity =>
        {
            entity.HasKey(e => e.ModelBoundaryID).HasName("PK_ModelBoundary_ModelBoundaryID");
        });

        modelBuilder.Entity<ModelScenario>(entity =>
        {
            entity.HasKey(e => e.ModelScenarioID).HasName("PK_ModelScenario_ModelScenarioID");
        });

        modelBuilder.Entity<ModelUser>(entity =>
        {
            entity.HasKey(e => e.ModelUserID).HasName("PK_ModelUser_ModelUserID");

            entity.HasOne(d => d.User).WithMany(p => p.ModelUsers).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<MonitoringWell>(entity =>
        {
            entity.HasKey(e => e.MonitoringWellID).HasName("PK_MonitoringWell_MonitoringWellID");

            entity.HasOne(d => d.Geography).WithMany(p => p.MonitoringWells).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<MonitoringWellMeasurement>(entity =>
        {
            entity.HasKey(e => e.MonitoringWellMeasurementID).HasName("PK_MonitoringWellMeasurement_MonitoringWellMeasurementID");

            entity.HasOne(d => d.Geography).WithMany(p => p.MonitoringWellMeasurements).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.MonitoringWell).WithMany(p => p.MonitoringWellMeasurements).HasConstraintName("FK_MonitoringWell_MonitoringWellID");
        });

        modelBuilder.Entity<OpenETSync>(entity =>
        {
            entity.HasKey(e => e.OpenETSyncID).HasName("PK_OpenETSync_OpenETSyncID");

            entity.HasOne(d => d.Geography).WithMany(p => p.OpenETSyncs).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<OpenETSyncHistory>(entity =>
        {
            entity.HasKey(e => e.OpenETSyncHistoryID).HasName("PK_OpenETSyncHistory_OpenETSyncHistoryID");

            entity.Property(e => e.OpenETRasterCalculationResultTypeID).HasDefaultValue(1);

            entity.HasOne(d => d.OpenETSync).WithMany(p => p.OpenETSyncHistories).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Parcel>(entity =>
        {
            entity.HasKey(e => e.ParcelID).HasName("PK_Parcel_ParcelID");

            entity.HasOne(d => d.Geography).WithMany(p => p.Parcels).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<ParcelCustomAttribute>(entity =>
        {
            entity.HasKey(e => e.ParcelCustomAttributeID).HasName("PK_ParcelCustomAttribute_ParcelCustomAttributeID");

            entity.Property(e => e.CustomAttributes).HasDefaultValue("{}");

            entity.HasOne(d => d.Parcel).WithOne(p => p.ParcelCustomAttribute).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<ParcelGeometry>(entity =>
        {
            entity.HasKey(e => e.ParcelGeometryID).HasName("PK_ParcelGeometry_ParcelGeometryID");

            entity.HasOne(d => d.Geography).WithMany(p => p.ParcelGeometries).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Parcel).WithOne(p => p.ParcelGeometry).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<ParcelHistory>(entity =>
        {
            entity.HasKey(e => e.ParcelHistoryID).HasName("PK_ParcelHistory_ParcelHistoryID");

            entity.HasOne(d => d.Geography).WithMany(p => p.ParcelHistories).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Parcel).WithMany(p => p.ParcelHistories).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.UpdateUser).WithMany(p => p.ParcelHistories)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ParcelHistory_User_UpdateUserID_UserID");
        });

        modelBuilder.Entity<ParcelStaging>(entity =>
        {
            entity.HasKey(e => e.ParcelStagingID).HasName("PK_ParcelStaging_ParcelStagingID");

            entity.HasOne(d => d.Geography).WithMany(p => p.ParcelStagings).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<ParcelSupply>(entity =>
        {
            entity.HasKey(e => e.ParcelSupplyID).HasName("PK_ParcelSupply_ParcelSupplyID");

            entity.HasOne(d => d.Geography).WithMany(p => p.ParcelSupplies).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Parcel).WithMany(p => p.ParcelSupplyParcels).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.ParcelNavigation).WithMany(p => p.ParcelSupplyParcelNavigations)
                .HasPrincipalKey(p => new { p.ParcelID, p.GeographyID })
                .HasForeignKey(d => new { d.ParcelID, d.GeographyID })
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.WaterTypeNavigation).WithMany(p => p.ParcelSupplyWaterTypeNavigations)
                .HasPrincipalKey(p => new { p.WaterTypeID, p.GeographyID })
                .HasForeignKey(d => new { d.WaterTypeID, d.GeographyID });
        });

        modelBuilder.Entity<ParcelWaterAccountHistory>(entity =>
        {
            entity.HasKey(e => e.ParcelWaterAccountHistoryID).HasName("PK_ParcelWaterAccountHistory_WaterAccountParcelID");

            entity.Property(e => e.CreateDate).HasDefaultValueSql("(getutcdate())");

            entity.HasOne(d => d.CreateUser).WithMany(p => p.ParcelWaterAccountHistories).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Geography).WithMany(p => p.ParcelWaterAccountHistories).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Parcel).WithMany(p => p.ParcelWaterAccountHistories).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.ReportingPeriod).WithMany(p => p.ParcelWaterAccountHistories).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<ParcelZone>(entity =>
        {
            entity.HasKey(e => e.ParcelZoneID).HasName("PK_ParcelZone_ParcelZoneID");

            entity.HasOne(d => d.Parcel).WithMany(p => p.ParcelZones).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Zone).WithMany(p => p.ParcelZones).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<ReferenceWell>(entity =>
        {
            entity.HasKey(e => e.ReferenceWellID).HasName("PK_ReferenceWell_ReferenceWellID");

            entity.HasOne(d => d.Geography).WithMany(p => p.ReferenceWells).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<ReportingPeriod>(entity =>
        {
            entity.HasKey(e => e.ReportingPeriodID).HasName("PK_ReportingPeriod_ReportingPeriodID");

            entity.HasOne(d => d.CreateUser).WithMany(p => p.ReportingPeriodCreateUsers).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Geography).WithMany(p => p.ReportingPeriods).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<ScenarioRun>(entity =>
        {
            entity.HasKey(e => e.ScenarioRunID).HasName("PK_ScenarioRun_ScenarioRunID");

            entity.HasOne(d => d.User).WithMany(p => p.ScenarioRuns).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<ScenarioRunFileResource>(entity =>
        {
            entity.HasKey(e => e.ScenarioRunFileResourceID).HasName("PK_ScenarioRunFileResource_ScenarioRunFileResourceID");

            entity.HasOne(d => d.FileResource).WithMany(p => p.ScenarioRunFileResources).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.ScenarioRun).WithMany(p => p.ScenarioRunFileResources).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<ScenarioRunOutputFile>(entity =>
        {
            entity.HasKey(e => e.ScenarioRunOutputFileID).HasName("PK_ScenarioRunOutputFile_ScenarioRunOutputFileID");

            entity.HasOne(d => d.FileResource).WithMany(p => p.ScenarioRunOutputFiles).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.ScenarioRun).WithMany(p => p.ScenarioRunOutputFiles).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<StatementBatch>(entity =>
        {
            entity.HasKey(e => e.StatementBatchID).HasName("PK_StatementBatch_StatementBatchID");

            entity.HasOne(d => d.Geography).WithMany(p => p.StatementBatches).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.ReportingPeriod).WithMany(p => p.StatementBatches).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.StatementTemplate).WithMany(p => p.StatementBatches).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.UpdateUser).WithMany(p => p.StatementBatches)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_StatementBatch_User_UpdateUserID_UserID");
        });

        modelBuilder.Entity<StatementBatchWaterAccount>(entity =>
        {
            entity.HasKey(e => e.StatementBatchWaterAccountID).HasName("PK_StatementBatchWaterAccount_StatementBatchWaterAccountID");

            entity.HasOne(d => d.StatementBatch).WithMany(p => p.StatementBatchWaterAccounts).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.WaterAccount).WithMany(p => p.StatementBatchWaterAccounts).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<StatementTemplate>(entity =>
        {
            entity.HasKey(e => e.StatementTemplateID).HasName("PK_StatementTemplate_StatementTemplateID");

            entity.Property(e => e.CustomFieldsContent).HasDefaultValue("{}");

            entity.HasOne(d => d.Geography).WithMany(p => p.StatementTemplates).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.UpdateUser).WithMany(p => p.StatementTemplates)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_StatementTemplate_User_UpdateUserID_UserID");
        });

        modelBuilder.Entity<SupportTicket>(entity =>
        {
            entity.HasKey(e => e.SupportTicketID).HasName("PK_SupportTicket_SupportTicketID");

            entity.HasOne(d => d.Geography).WithMany(p => p.SupportTickets).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<SupportTicketNote>(entity =>
        {
            entity.HasKey(e => e.SupportTicketNoteID).HasName("PK_SupportTicketNote_SupportTicketNoteID");

            entity.HasOne(d => d.CreateUser).WithMany(p => p.SupportTicketNotes)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SupportTicketNote_User_UserID");

            entity.HasOne(d => d.SupportTicket).WithMany(p => p.SupportTicketNotes).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<UploadedGdb>(entity =>
        {
            entity.HasKey(e => e.UploadedGdbID).HasName("PK_UploadedGdb_UploadedGdbID");

            entity.HasOne(d => d.Geography).WithMany(p => p.UploadedGdbs).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.User).WithMany(p => p.UploadedGdbs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UploadedGdbID_User_UserID");
        });

        modelBuilder.Entity<UploadedWellGdb>(entity =>
        {
            entity.HasKey(e => e.UploadedWellGdbID).HasName("PK_UploadedWellGdb_UploadedWellGdbID");

            entity.Property(e => e.SRID).HasDefaultValue(2227);

            entity.HasOne(d => d.Geography).WithMany(p => p.UploadedWellGdbs).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.User).WithMany(p => p.UploadedWellGdbs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UploadedWellGdbID_User_UserID");
        });

        modelBuilder.Entity<UsageLocation>(entity =>
        {
            entity.HasKey(e => e.UsageLocationID).HasName("PK_UsageLocation_UsageLocationID");

            entity.Property(e => e.CreateDate).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.CreateUserID).HasDefaultValue(2);

            entity.HasOne(d => d.CreateUser).WithMany(p => p.UsageLocationCreateUsers).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Geography).WithMany(p => p.UsageLocations).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Parcel).WithMany(p => p.UsageLocations).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.ReportingPeriod).WithMany(p => p.UsageLocations).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<UsageLocationCrop>(entity =>
        {
            entity.HasKey(e => e.UsageLocationCropID).HasName("PK_UsageLocationCrop_UsageLocationCropID");
        });

        modelBuilder.Entity<UsageLocationGeometry>(entity =>
        {
            entity.HasKey(e => e.UsageLocationID).HasName("PK_UsageLocationGeometry_UsageLocationID");

            entity.Property(e => e.UsageLocationID).ValueGeneratedNever();
        });

        modelBuilder.Entity<UsageLocationHistory>(entity =>
        {
            entity.HasKey(e => e.UsageLocationHistoryID).HasName("PK_UsageLocationHistory_UsageLocationHistoryID");

            entity.Property(e => e.CreateDate).HasDefaultValueSql("(getutcdate())");

            entity.HasOne(d => d.CreateUser).WithMany(p => p.UsageLocationHistories).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Geography).WithMany(p => p.UsageLocationHistories).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.UsageLocation).WithMany(p => p.UsageLocationHistories).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<UsageLocationParcelHistory>(entity =>
        {
            entity.HasKey(e => e.UsageLocationParcelHistoryID).HasName("PK_UsageLocationParcelHistory_UsageLocationParcelHistoryID");

            entity.Property(e => e.CreateDate).HasDefaultValueSql("(getutcdate())");

            entity.HasOne(d => d.CreateUser).WithMany(p => p.UsageLocationParcelHistories).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Geography).WithMany(p => p.UsageLocationParcelHistories).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.ReportingPeriod).WithMany(p => p.UsageLocationParcelHistories).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.UsageLocation).WithMany(p => p.UsageLocationParcelHistories).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<UsageLocationType>(entity =>
        {
            entity.HasKey(e => e.UsageLocationTypeID).HasName("PK_UsageLocationType_UsageLocationTypeID");

            entity.HasOne(d => d.CreateUser).WithMany(p => p.UsageLocationTypeCreateUsers).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Geography).WithMany(p => p.UsageLocationTypes).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserID).HasName("PK_User_UserID");

            entity.Property(e => e.ScenarioPlannerRoleID).HasDefaultValue(1);
        });

        modelBuilder.Entity<WaterAccount>(entity =>
        {
            entity.HasKey(e => e.WaterAccountID).HasName("PK_WaterAccount_WaterAccountID");

            entity.Property(e => e.WaterAccountNumber).HasComputedColumnSql("(isnull([WaterAccountID]+(10000),(0)))", false);

            entity.HasOne(d => d.Geography).WithMany(p => p.WaterAccounts).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<WaterAccountContact>(entity =>
        {
            entity.HasKey(e => e.WaterAccountContactID).HasName("PK_WaterAccountContact_WaterAccountContactID");

            entity.Property(e => e.FullAddress).HasComputedColumnSql("(case when datalength([City])>(0) AND datalength([State])>(0) AND datalength([ZipCode])>(0) then concat([Address],case when datalength([SecondaryAddress])=(0) then '' else ', '+[SecondaryAddress] end,', ',[City],', ',[State],' ',[ZipCode]) else [Address] end)", false);
            entity.Property(e => e.PrefersPhysicalCommunication).HasDefaultValue(true);

            entity.HasOne(d => d.Geography).WithMany(p => p.WaterAccountContacts).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<WaterAccountCoverCropStatus>(entity =>
        {
            entity.HasKey(e => e.WaterAccountCoverCropStatusID).HasName("PK_WaterAccountCoverCropStatus_WaterAccountCoverCropStatusID");

            entity.Property(e => e.SelfReportStatusID).HasDefaultValue(1);

            entity.HasOne(d => d.CreateUser).WithMany(p => p.WaterAccountCoverCropStatusCreateUsers).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Geography).WithMany(p => p.WaterAccountCoverCropStatuses).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.ReportingPeriod).WithMany(p => p.WaterAccountCoverCropStatuses).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.WaterAccount).WithMany(p => p.WaterAccountCoverCropStatuses).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<WaterAccountCustomAttribute>(entity =>
        {
            entity.HasKey(e => e.WaterAccountCustomAttributeID).HasName("PK_WaterAccountCustomAttribute_WaterAccountCustomAttributeID");

            entity.Property(e => e.CustomAttributes).HasDefaultValue("{}");

            entity.HasOne(d => d.WaterAccount).WithOne(p => p.WaterAccountCustomAttribute).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<WaterAccountFallowStatus>(entity =>
        {
            entity.HasKey(e => e.WaterAccountFallowStatusID).HasName("PK_WaterAccountFallowStatus_WaterAccountFallowStatusID");

            entity.Property(e => e.SelfReportStatusID).HasDefaultValue(1);

            entity.HasOne(d => d.CreateUser).WithMany(p => p.WaterAccountFallowStatusCreateUsers).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Geography).WithMany(p => p.WaterAccountFallowStatuses).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.ReportingPeriod).WithMany(p => p.WaterAccountFallowStatuses).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.WaterAccount).WithMany(p => p.WaterAccountFallowStatuses).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<WaterAccountParcel>(entity =>
        {
            entity.HasKey(e => e.WaterAccountParcelID).HasName("PK_WaterAccountParcel_WaterAccountParcelID");

            entity.HasOne(d => d.Geography).WithMany(p => p.WaterAccountParcels).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Parcel).WithMany(p => p.WaterAccountParcels).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.ReportingPeriod).WithMany(p => p.WaterAccountParcels).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.WaterAccount).WithMany(p => p.WaterAccountParcels).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<WaterAccountReconciliation>(entity =>
        {
            entity.HasKey(e => e.WaterAccountReconciliationID).HasName("PK_WaterAccountReconciliation_WaterAccountReconciliationID");

            entity.HasOne(d => d.Geography).WithMany(p => p.WaterAccountReconciliations).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Parcel).WithMany(p => p.WaterAccountReconciliationParcels).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.WaterAccount).WithMany(p => p.WaterAccountReconciliationWaterAccounts).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.ParcelNavigation).WithMany(p => p.WaterAccountReconciliationParcelNavigations)
                .HasPrincipalKey(p => new { p.ParcelID, p.GeographyID })
                .HasForeignKey(d => new { d.ParcelID, d.GeographyID })
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.WaterAccountNavigation).WithMany(p => p.WaterAccountReconciliationWaterAccountNavigations)
                .HasPrincipalKey(p => new { p.WaterAccountID, p.GeographyID })
                .HasForeignKey(d => new { d.WaterAccountID, d.GeographyID })
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<WaterAccountUser>(entity =>
        {
            entity.HasKey(e => e.WaterAccountUserID).HasName("PK_WaterAccountUser_WaterAccountUserID");

            entity.HasOne(d => d.User).WithMany(p => p.WaterAccountUsers).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.WaterAccount).WithMany(p => p.WaterAccountUsers).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<WaterAccountUserStaging>(entity =>
        {
            entity.HasKey(e => e.WaterAccountUserStagingID).HasName("PK_WaterAccountUserStaging_WaterAccountUserStagingID");

            entity.HasOne(d => d.User).WithMany(p => p.WaterAccountUserStagings).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.WaterAccount).WithMany(p => p.WaterAccountUserStagings).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<WaterMeasurement>(entity =>
        {
            entity.HasKey(e => e.WaterMeasurementID).HasName("PK_WaterMeasurement_WaterMeasurementID");

            entity.HasOne(d => d.Geography).WithMany(p => p.WaterMeasurements).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.UsageLocation).WithMany(p => p.WaterMeasurements).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<WaterMeasurementSelfReport>(entity =>
        {
            entity.HasKey(e => e.WaterMeasurementSelfReportID).HasName("PK_WaterMeasurementSelfReport_WaterMeasurementSelfReportID");

            entity.HasOne(d => d.CreateUser).WithMany(p => p.WaterMeasurementSelfReportCreateUsers).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Geography).WithMany(p => p.WaterMeasurementSelfReports).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.ReportingPeriod).WithMany(p => p.WaterMeasurementSelfReports).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.WaterAccount).WithMany(p => p.WaterMeasurementSelfReports).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.WaterMeasurementType).WithMany(p => p.WaterMeasurementSelfReports).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<WaterMeasurementSelfReportFileResource>(entity =>
        {
            entity.HasKey(e => e.WaterMeasurementSelfReportFileResourceID).HasName("PK_WaterMeasurementSelfReportFileResource_WaterMeasurementSelfReportFileResourceID");

            entity.HasOne(d => d.FileResource).WithMany(p => p.WaterMeasurementSelfReportFileResources).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.WaterMeasurementSelfReport).WithMany(p => p.WaterMeasurementSelfReportFileResources).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<WaterMeasurementSelfReportLineItem>(entity =>
        {
            entity.HasKey(e => e.WaterMeasurementSelfReportLineItemID).HasName("PK_WaterMeasurementSelfReportLineItem_WaterMeasurementSelfReportLineItemID");

            entity.HasOne(d => d.CreateUser).WithMany(p => p.WaterMeasurementSelfReportLineItemCreateUsers).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.IrrigationMethod).WithMany(p => p.WaterMeasurementSelfReportLineItems).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Parcel).WithMany(p => p.WaterMeasurementSelfReportLineItems).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.WaterMeasurementSelfReport).WithMany(p => p.WaterMeasurementSelfReportLineItems).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<WaterMeasurementType>(entity =>
        {
            entity.HasKey(e => e.WaterMeasurementTypeID).HasName("PK_WaterMeasurementType_WaterMeasurementTypeID");

            entity.HasOne(d => d.Geography).WithMany(p => p.WaterMeasurementTypes).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<WaterMeasurementTypeDependency>(entity =>
        {
            entity.HasKey(e => e.WaterMeasurementTypeDependencyID).HasName("PK_WaterMeasurementTypeDependency_WaterMeasurementTypeDependencyID");

            entity.HasOne(d => d.DependsOnWaterMeasurementType).WithMany(p => p.WaterMeasurementTypeDependencyDependsOnWaterMeasurementTypes)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_WaterMeasurementTypeDependency_DependsOnWaterMeasurementType_DependsOnWaterMeasurementTypeID");

            entity.HasOne(d => d.Geography).WithMany(p => p.WaterMeasurementTypeDependencies).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.WaterMeasurementType).WithMany(p => p.WaterMeasurementTypeDependencyWaterMeasurementTypes).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<WaterType>(entity =>
        {
            entity.HasKey(e => e.WaterTypeID).HasName("PK_WaterType_WaterTypeID");

            entity.HasOne(d => d.Geography).WithMany(p => p.WaterTypes).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Well>(entity =>
        {
            entity.HasKey(e => e.WellID).HasName("PK_Well_WellID");

            entity.Property(e => e.WellStatusID).HasDefaultValue(1);

            entity.HasOne(d => d.Geography).WithMany(p => p.Wells).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<WellFileResource>(entity =>
        {
            entity.HasKey(e => e.WellFileResourceID).HasName("PK_WellFileResource_WellFileResourceID");

            entity.HasOne(d => d.FileResource).WithMany(p => p.WellFileResources).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Well).WithMany(p => p.WellFileResources).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<WellIrrigatedParcel>(entity =>
        {
            entity.HasKey(e => e.WellIrrigatedParcelID).HasName("PK_WellIrrigatedParcel_WellIrrigatedParcelID");

            entity.HasOne(d => d.Parcel).WithMany(p => p.WellIrrigatedParcels).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Well).WithMany(p => p.WellIrrigatedParcels).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<WellMeter>(entity =>
        {
            entity.HasKey(e => e.WellMeterID).HasName("PK_WellMeter_WellMeterID");

            entity.HasOne(d => d.Meter).WithMany(p => p.WellMeters).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Well).WithMany(p => p.WellMeters).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<WellRegistration>(entity =>
        {
            entity.HasKey(e => e.WellRegistrationID).HasName("PK_WellRegistration_WellRegistrationID");

            entity.HasOne(d => d.CreateUser).WithMany(p => p.WellRegistrations).HasConstraintName("FK_WellRegistration_User_UserID");

            entity.HasOne(d => d.Geography).WithMany(p => p.WellRegistrations).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<WellRegistrationContact>(entity =>
        {
            entity.HasKey(e => e.WellRegistrationContactID).HasName("PK_WellRegistrationContact_WellRegistrationContactID");

            entity.HasOne(d => d.WellRegistration).WithMany(p => p.WellRegistrationContacts).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<WellRegistrationFileResource>(entity =>
        {
            entity.HasKey(e => e.WellRegistrationFileResourceID).HasName("PK_WellRegistrationFileResource_WellRegistrationFileResourceID");

            entity.HasOne(d => d.FileResource).WithMany(p => p.WellRegistrationFileResources).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.WellRegistration).WithMany(p => p.WellRegistrationFileResources).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<WellRegistrationIrrigatedParcel>(entity =>
        {
            entity.HasKey(e => e.WellRegistrationIrrigatedParcelID).HasName("PK_WellRegistrationIrrigatedParcel_WellRegistrationIrrigatedParcelID");

            entity.HasOne(d => d.Parcel).WithMany(p => p.WellRegistrationIrrigatedParcels).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.WellRegistration).WithMany(p => p.WellRegistrationIrrigatedParcels).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<WellRegistrationMetadatum>(entity =>
        {
            entity.HasKey(e => e.WellRegistrationMetadatumID).HasName("PK_WellRegistrationMetadatum_WellRegistrationMetadatumID");

            entity.HasOne(d => d.WellRegistration).WithOne(p => p.WellRegistrationMetadatum).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<WellRegistrationWaterUse>(entity =>
        {
            entity.HasKey(e => e.WellRegistrationWaterUseID).HasName("PK_WellRegistrationWaterUse_WellRegistrationWaterUseID");

            entity.HasOne(d => d.WellRegistration).WithMany(p => p.WellRegistrationWaterUses).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<WellType>(entity =>
        {
            entity.HasKey(e => e.WellTypeID).HasName("PK_WellType_WellTypeID");

            entity.HasOne(d => d.CreateUser).WithMany(p => p.WellTypeCreateUsers).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Geography).WithMany(p => p.WellTypes).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Zone>(entity =>
        {
            entity.HasKey(e => e.ZoneID).HasName("PK_Zone_ZoneID");

            entity.HasOne(d => d.ZoneGroup).WithMany(p => p.Zones).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<ZoneGroup>(entity =>
        {
            entity.HasKey(e => e.ZoneGroupID).HasName("PK_ZoneGroup_ZoneGroupID");

            entity.Property(e => e.DisplayToAccountHolders).HasDefaultValue(true);

            entity.HasOne(d => d.Geography).WithMany(p => p.ZoneGroups).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<vGeoServerAllParcel>(entity =>
        {
            entity.ToView("vGeoServerAllParcels");
        });

        modelBuilder.Entity<vGeoServerAllUsageLocation>(entity =>
        {
            entity.ToView("vGeoServerAllUsageLocations");
        });

        modelBuilder.Entity<vGeoServerCNRAMonitoringWell>(entity =>
        {
            entity.ToView("vGeoServerCNRAMonitoringWells");

            entity.Property(e => e.PrimaryKey).ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<vGeoServerMonitoringWell>(entity =>
        {
            entity.ToView("vGeoServerMonitoringWells");

            entity.Property(e => e.PrimaryKey).ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<vGeoServerYoloWRIDMonitoringWell>(entity =>
        {
            entity.ToView("vGeoServerYoloWRIDMonitoringWells");

            entity.Property(e => e.PrimaryKey).ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<vGeoServerZoneGroup>(entity =>
        {
            entity.ToView("vGeoServerZoneGroups");
        });

        modelBuilder.Entity<vGeoserverGeographyGSABoundary>(entity =>
        {
            entity.ToView("vGeoserverGeographyGSABoundaries");
        });

        modelBuilder.Entity<vMeterReading>(entity =>
        {
            entity.ToView("vMeterReading");
        });

        modelBuilder.Entity<vParcelSupplyTransactionHistory>(entity =>
        {
            entity.ToView("vParcelSupplyTransactionHistory");
        });

        modelBuilder.Entity<vWaterMeasurement>(entity =>
        {
            entity.ToView("vWaterMeasurement");
        });

        modelBuilder.Entity<vWaterMeasurementSourceOfRecord>(entity =>
        {
            entity.ToView("vWaterMeasurementSourceOfRecord");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
