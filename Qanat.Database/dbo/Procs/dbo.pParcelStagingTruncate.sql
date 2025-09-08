create procedure dbo.pParcelStagingTruncate
with execute as owner
as

begin
	TRUNCATE TABLE dbo.ParcelStaging
	TRUNCATE TABLE dbo.[WaterAccountReconciliation]
end