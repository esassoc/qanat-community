using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Qanat.EFModels.Entities;

[Table("Geography")]
[Index("GeographyConfigurationID", Name = "AK_Geography_GeographyConfigurationID", IsUnique = true)]
[Index("GeographyDisplayName", Name = "AK_Geography_GeographyDisplayName", IsUnique = true)]
[Index("GeographyName", Name = "AK_Geography_GeographyName", IsUnique = true)]
public partial class Geography
{
    [Key]
    public int GeographyID { get; set; }

    public int GeographyConfigurationID { get; set; }

    [Required]
    [StringLength(50)]
    [Unicode(false)]
    public string GeographyName { get; set; }

    [Required]
    [StringLength(100)]
    [Unicode(false)]
    public string GeographyDisplayName { get; set; }

    [StringLength(500)]
    [Unicode(false)]
    public string GeographyDescription { get; set; }

    [Required]
    [StringLength(100)]
    [Unicode(false)]
    public string APNRegexPattern { get; set; }

    [Required]
    [StringLength(50)]
    [Unicode(false)]
    public string APNRegexPatternDisplay { get; set; }

    public int? GSACanonicalID { get; set; }

    [StringLength(9)]
    [Unicode(false)]
    public string Color { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string ContactEmail { get; set; }

    [StringLength(30)]
    [Unicode(false)]
    public string ContactPhoneNumber { get; set; }

    [StringLength(500)]
    [Unicode(false)]
    public string ContactAddressLine1 { get; set; }

    [StringLength(500)]
    [Unicode(false)]
    public string ContactAddressLine2 { get; set; }

    [Required]
    [StringLength(200)]
    [Unicode(false)]
    public string LandownerDashboardSupplyLabel { get; set; }

    [Required]
    [StringLength(200)]
    [Unicode(false)]
    public string LandownerDashboardUsageLabel { get; set; }

    public int CoordinateSystem { get; set; }

    public int AreaToAcresConversionFactor { get; set; }

    public bool IsOpenETActive { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string OpenETShapeFilePath { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string OpenETRasterTimeseriesMultipolygonColumnToUseAsIdentifier { get; set; }

    public int? SourceOfRecordWaterMeasurementTypeID { get; set; }

    [StringLength(500)]
    [Unicode(false)]
    public string SourceOfRecordExplanation { get; set; }

    public bool ShowSupplyOnWaterBudgetComponent { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string WaterBudgetSlotAHeader { get; set; }

    public int? WaterBudgetSlotAWaterMeasurementTypeID { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string WaterBudgetSlotBHeader { get; set; }

    public int? WaterBudgetSlotBWaterMeasurementTypeID { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string WaterBudgetSlotCHeader { get; set; }

    public int? WaterBudgetSlotCWaterMeasurementTypeID { get; set; }

    public bool FeeCalculatorEnabled { get; set; }

    public bool AllowWaterMeasurementSelfReporting { get; set; }

    public bool AllowFallowSelfReporting { get; set; }

    public bool AllowCoverCropSelfReporting { get; set; }

    public bool AllowLandownersToRequestAccountChanges { get; set; }

    public bool IsDemoGeography { get; set; }

    [InverseProperty("Geography")]
    public virtual ICollection<AllocationPlan> AllocationPlans { get; set; } = new List<AllocationPlan>();

    [InverseProperty("Geography")]
    public virtual ICollection<CustomAttribute> CustomAttributes { get; set; } = new List<CustomAttribute>();

    [InverseProperty("Geography")]
    public virtual ICollection<CustomRichText> CustomRichTexts { get; set; } = new List<CustomRichText>();

    [InverseProperty("Geography")]
    public virtual ICollection<ExternalMapLayer> ExternalMapLayers { get; set; } = new List<ExternalMapLayer>();

    [InverseProperty("Geography")]
    public virtual GeographyAllocationPlanConfiguration GeographyAllocationPlanConfiguration { get; set; }

    [InverseProperty("Geography")]
    public virtual GeographyBoundary GeographyBoundary { get; set; }

    [ForeignKey("GeographyConfigurationID")]
    [InverseProperty("Geography")]
    public virtual GeographyConfiguration GeographyConfiguration { get; set; }

    [InverseProperty("Geography")]
    public virtual ICollection<GeographyUser> GeographyUsers { get; set; } = new List<GeographyUser>();

    [InverseProperty("Geography")]
    public virtual ICollection<IrrigationMethod> IrrigationMethods { get; set; } = new List<IrrigationMethod>();

    [InverseProperty("Geography")]
    public virtual ICollection<MeterReadingMonthlyInterpolation> MeterReadingMonthlyInterpolations { get; set; } = new List<MeterReadingMonthlyInterpolation>();

    [InverseProperty("Geography")]
    public virtual ICollection<MeterReading> MeterReadings { get; set; } = new List<MeterReading>();

    [InverseProperty("Geography")]
    public virtual ICollection<Meter> Meters { get; set; } = new List<Meter>();

    [InverseProperty("Geography")]
    public virtual ICollection<MonitoringWellMeasurement> MonitoringWellMeasurements { get; set; } = new List<MonitoringWellMeasurement>();

    [InverseProperty("Geography")]
    public virtual ICollection<MonitoringWell> MonitoringWells { get; set; } = new List<MonitoringWell>();

    [InverseProperty("Geography")]
    public virtual ICollection<OpenETSync> OpenETSyncs { get; set; } = new List<OpenETSync>();

    [InverseProperty("Geography")]
    public virtual ICollection<ParcelGeometry> ParcelGeometries { get; set; } = new List<ParcelGeometry>();

    [InverseProperty("Geography")]
    public virtual ICollection<ParcelHistory> ParcelHistories { get; set; } = new List<ParcelHistory>();

    [InverseProperty("Geography")]
    public virtual ICollection<ParcelStaging> ParcelStagings { get; set; } = new List<ParcelStaging>();

    [InverseProperty("Geography")]
    public virtual ICollection<ParcelSupply> ParcelSupplies { get; set; } = new List<ParcelSupply>();

    [InverseProperty("Geography")]
    public virtual ICollection<ParcelWaterAccountHistory> ParcelWaterAccountHistories { get; set; } = new List<ParcelWaterAccountHistory>();

    [InverseProperty("Geography")]
    public virtual ICollection<Parcel> Parcels { get; set; } = new List<Parcel>();

    [InverseProperty("Geography")]
    public virtual ICollection<ReferenceWell> ReferenceWells { get; set; } = new List<ReferenceWell>();

    [InverseProperty("Geography")]
    public virtual ICollection<ReportingPeriod> ReportingPeriods { get; set; } = new List<ReportingPeriod>();

    [ForeignKey("SourceOfRecordWaterMeasurementTypeID")]
    [InverseProperty("GeographySourceOfRecordWaterMeasurementTypes")]
    public virtual WaterMeasurementType SourceOfRecordWaterMeasurementType { get; set; }

    [InverseProperty("Geography")]
    public virtual ICollection<StatementBatch> StatementBatches { get; set; } = new List<StatementBatch>();

    [InverseProperty("Geography")]
    public virtual ICollection<StatementTemplate> StatementTemplates { get; set; } = new List<StatementTemplate>();

    [InverseProperty("Geography")]
    public virtual ICollection<SupportTicket> SupportTickets { get; set; } = new List<SupportTicket>();

    [InverseProperty("Geography")]
    public virtual ICollection<UploadedGdb> UploadedGdbs { get; set; } = new List<UploadedGdb>();

    [InverseProperty("Geography")]
    public virtual ICollection<UploadedWellGdb> UploadedWellGdbs { get; set; } = new List<UploadedWellGdb>();

    [InverseProperty("Geography")]
    public virtual ICollection<UsageLocationHistory> UsageLocationHistories { get; set; } = new List<UsageLocationHistory>();

    [InverseProperty("Geography")]
    public virtual ICollection<UsageLocationParcelHistory> UsageLocationParcelHistories { get; set; } = new List<UsageLocationParcelHistory>();

    [InverseProperty("Geography")]
    public virtual ICollection<UsageLocationType> UsageLocationTypes { get; set; } = new List<UsageLocationType>();

    [InverseProperty("Geography")]
    public virtual ICollection<UsageLocation> UsageLocations { get; set; } = new List<UsageLocation>();

    [InverseProperty("Geography")]
    public virtual ICollection<WaterAccountContact> WaterAccountContacts { get; set; } = new List<WaterAccountContact>();

    [InverseProperty("Geography")]
    public virtual ICollection<WaterAccountCoverCropStatus> WaterAccountCoverCropStatuses { get; set; } = new List<WaterAccountCoverCropStatus>();

    [InverseProperty("Geography")]
    public virtual ICollection<WaterAccountFallowStatus> WaterAccountFallowStatuses { get; set; } = new List<WaterAccountFallowStatus>();

    [InverseProperty("Geography")]
    public virtual ICollection<WaterAccountParcel> WaterAccountParcels { get; set; } = new List<WaterAccountParcel>();

    [InverseProperty("Geography")]
    public virtual ICollection<WaterAccountReconciliation> WaterAccountReconciliations { get; set; } = new List<WaterAccountReconciliation>();

    [InverseProperty("Geography")]
    public virtual ICollection<WaterAccount> WaterAccounts { get; set; } = new List<WaterAccount>();

    [ForeignKey("WaterBudgetSlotAWaterMeasurementTypeID")]
    [InverseProperty("GeographyWaterBudgetSlotAWaterMeasurementTypes")]
    public virtual WaterMeasurementType WaterBudgetSlotAWaterMeasurementType { get; set; }

    [ForeignKey("WaterBudgetSlotBWaterMeasurementTypeID")]
    [InverseProperty("GeographyWaterBudgetSlotBWaterMeasurementTypes")]
    public virtual WaterMeasurementType WaterBudgetSlotBWaterMeasurementType { get; set; }

    [ForeignKey("WaterBudgetSlotCWaterMeasurementTypeID")]
    [InverseProperty("GeographyWaterBudgetSlotCWaterMeasurementTypes")]
    public virtual WaterMeasurementType WaterBudgetSlotCWaterMeasurementType { get; set; }

    [InverseProperty("Geography")]
    public virtual ICollection<WaterMeasurementSelfReport> WaterMeasurementSelfReports { get; set; } = new List<WaterMeasurementSelfReport>();

    [InverseProperty("Geography")]
    public virtual ICollection<WaterMeasurementTypeDependency> WaterMeasurementTypeDependencies { get; set; } = new List<WaterMeasurementTypeDependency>();

    [InverseProperty("Geography")]
    public virtual ICollection<WaterMeasurementType> WaterMeasurementTypes { get; set; } = new List<WaterMeasurementType>();

    [InverseProperty("Geography")]
    public virtual ICollection<WaterMeasurement> WaterMeasurements { get; set; } = new List<WaterMeasurement>();

    [InverseProperty("Geography")]
    public virtual ICollection<WaterType> WaterTypes { get; set; } = new List<WaterType>();

    [InverseProperty("Geography")]
    public virtual ICollection<WellRegistration> WellRegistrations { get; set; } = new List<WellRegistration>();

    [InverseProperty("Geography")]
    public virtual ICollection<WellType> WellTypes { get; set; } = new List<WellType>();

    [InverseProperty("Geography")]
    public virtual ICollection<Well> Wells { get; set; } = new List<Well>();

    [InverseProperty("Geography")]
    public virtual ICollection<ZoneGroup> ZoneGroups { get; set; } = new List<ZoneGroup>();
}
