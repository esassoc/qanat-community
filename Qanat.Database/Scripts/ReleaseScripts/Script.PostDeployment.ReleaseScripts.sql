/*
Post-Deployment Script
--------------------------------------------------------------------------------------
This file is generated on every build, DO NOT modify.
--------------------------------------------------------------------------------------
*/

PRINT N'Qanat.Database - Script.PostDeployment.ReleaseScripts.sql';
GO

:r ".\0110 - repopulate GeographyAllocationPlanConfiguration and related tables.sql"
GO
:r ".\0111 - Add Allocation Plan rte.sql"
GO
:r ".\0112 - water use types for miugsa.sql"
GO
:r ".\0113 - add link RTEs.sql"
GO
:r ".\0114 - update MSGSA openet.sql"
GO
:r ".\0115 - update WaterMeasurementTypes.sql"
GO
:r ".\0116 - update ParcelOwnershipHistory records.sql"
GO
:r ".\0117 - usage entity table seeding.sql"
GO
:r ".\0118 - populate water account request changes RTEs.sql"
GO
:r ".\0119 - update zone accent colors.sql"
GO
:r ".\0120 - repopulate ParcelHistory table.sql"
GO
:r ".\0122 - delete account names for msgsa and ETSGSA.sql"
GO
:r ".\0123 - update ETSGSA WaterMeasurementTypes.sql"
GO
:r ".\0124 - update MIUGSA WaterMeasurementTypes.sql"
GO
:r ".\0125 - update ShowToLandowner bits for MIUGSA WaterMeasurementTypes.sql"
GO
:r ".\0126 - clear MIUGSA ledger data.sql"
GO
:r ".\0127 - populating missing WaterMeasurement UsageEntityAreas.sql"
GO
:r ".\0128 - update Zone precip multipliers.sql"
GO
:r ".\0129 - repopulate ParcelSupply table.sql"
GO
:r ".\0130 - Backfill WaterAccountUser and GeographyUser for MIUGSA.sql"
GO
:r ".\0131 - Add Extracted Groundwater Adjustment MeasurementType for MIUGSA.sql"
GO
:r ".\0132 - Backfill Extracted Groundwater Adjustments for MIUGSA.sql"
GO
:r ".\0133 - Set up water calculation dependencies.sql"
GO
:r ".\0134 - Add RTEs for Scenario Models.sql"
GO
:r ".\0135 - Missed ETSGSA Dependency.sql"
GO
:r ".\0136 - Add Raster Upload Guidance custom rich text.sql"
GO
:r ".\0137 - Update WaterTypeColor values.sql"
GO
:r ".\0138 - Update ETSGSA ShowSupplyOnWaterBudgetComponent.sql"
GO
:r ".\0139 - Update ETSGSA FeeCalculatorEnabled.sql"
GO
:r ".\0140 - Add lorem ipsum for Fee Calculator Your data tab.sql"
GO
:r ".\0141 - Update Yolo WaterMeasurementTypes.sql"
GO
:r ".\0142 - Remove unneeded calculation dependency.sql"
GO
:r ".\0143 - Mark ParcelHistories midchain as reviewed.sql"
GO
:r ".\0144 - Configure ETSGSA self reporting.sql"
GO
:r ".\0145 - Backfill missing parcel histories.sql"
GO
:r ".\0146 - Set parcel acreage to 4 decimal places.sql"
GO
:r ".\0147 - Updates to ETSGSA surface water measurement types.sql"
GO
:r ".\0148 - Prefill ModelUsers and ScenarioPlannerRole for users with existing actions.sql"
GO
:r ".\0149 - Rename GETAction to ScenarioRun.sql"
GO
:r ".\0150 - Add initial ReportingPeriods.sql"
GO

