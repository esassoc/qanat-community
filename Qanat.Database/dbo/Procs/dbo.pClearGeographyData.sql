create procedure dbo.pClearGeographyData
(
    @geographyID int
)
as

begin
	-- WARNING: This script should not be called anywhere in the platform
	-- If you're SURE you want to run this script, comment out this line:
	return


	delete from dbo.UploadedGdb where GeographyID = @geographyID


	-- delete records associated with geography WellRegistrations
	select WellRegistrationID into #wellRegistrationIDsToDelete
	from dbo.WellRegistration
	where GeographyID = @geographyID

	delete wrc from dbo.WellRegistrationContact wrc
	join #wellRegistrationIDsToDelete wrd on wrc.WellRegistrationID = wrd.WellRegistrationID

	delete wrfr from dbo.WellRegistrationFileResource wrfr
	join #wellRegistrationIDsToDelete wrd on wrfr.WellRegistrationID = wrd.WellRegistrationID

	delete wrip from dbo.WellRegistrationIrrigatedParcel wrip 
	join #wellRegistrationIDsToDelete wrd on wrip.WellRegistrationID = wrd.WellRegistrationID

	delete wrm from dbo.WellRegistrationMetadatum wrm
	join #wellRegistrationIDsToDelete wrd on wrm.WellRegistrationID = wrd.WellRegistrationID
	
	delete wrwu from dbo.WellRegistrationWaterUse  wrwu
	join #wellRegistrationIDsToDelete wrd on wrwu.WellRegistrationID = wrd.WellRegistrationID


	-- delete geography WellRegistrations
	delete from dbo.WellRegistration where GeographyID = @geographyID


	-- delete geography WaterMeasurements
	select ParcelNumber into #parcelNumbersToDelete
	from dbo.Parcel
	where GeographyID = @geographyID

	delete from dbo.WaterMeasurement where ParcelNumber in (
		select * from #parcelNumbersToDelete
	)


	-- delete records associated with geography Parcels
	select ParcelID into #parcelIDsToDelete
	from dbo.Parcel
	where GeographyID = @geographyID

	delete pz from dbo.ParcelZone pz
	join #parcelIDsToDelete pd on pz.ParcelID = pd.ParcelID

	delete pca from dbo.ParcelCustomAttribute pca
	join #parcelIDsToDelete pd on pca.ParcelID = pd.ParcelID

	delete wip from dbo.WellIrrigatedParcel wip
	join #parcelIDsToDelete pd on wip.ParcelID = pd.ParcelID

	delete from dbo.ParcelGeometry where GeographyID = @geographyID
	delete from dbo.ParcelLedger where GeographyID = @geographyID
	delete from dbo.ParcelOwnershipHistory where GeographyID = @geographyID
	delete from dbo.WaterAccountParcel where GeographyID = @geographyID


	-- delete geography Wells
	delete from dbo.Well where GeographyID = @geographyID


	-- delete geography Parcels
	delete from dbo.Parcel where GeographyID = @geographyID


	-- delete geography Meters and associated records
	delete wm
	from dbo.WellMeter wm 
		join dbo.Meter m on wm.MeterID = m.MeterID
	where m.GeographyID = @geographyID

	delete from dbo.Meter where GeographyID = @geographyID


	-- delete geography ExternalMapLayers
	delete from dbo.ExternalMapLayer where GeographyID = @geographyID

	-- delete geography AllocationPlans and associated records
	delete app
	from AllocationPlanPeriod app
	join dbo.AllocationPlan ap on app.AllocationPlanID = ap.AllocationPlanID
	where ap.GeographyID = @geographyID

	delete from dbo.AllocationPlan where GeographyID = @geographyID
	delete from dbo.GeographyAllocationPlanConfiguration where GeographyID = @geographyID


	-- delete geography WaterTypes
	delete from dbo.WaterType where GeographyID = @geographyID


	-- delete geography Zones and ZoneGroups
	delete z
	from dbo.Zone z
		join dbo.ZoneGroup zg on z.ZoneGroupID = zg.ZoneGroupID
	where zg.GeographyID = @geographyID

	delete from dbo.ZoneGroup where GeographyID = @geographyID


	-- delete geography OpenETSyncHistories
	delete oesh
	from dbo.OpenETSyncHistory oesh
		join dbo.OpenETSync oes on oesh.OpenETSyncID = oes.OpenETSyncID
	where oes.GeographyID = @geographyID


	-- null out FinalizeDate for geography OpenETSyncs
	update dbo.OpenETSync
	set FinalizeDate = null
	where GeographyID = @geographyID


	-- delete records associated with geography WaterAccounts
	select WaterAccountID into #waterAccountIDsToDelete
	from dbo.WaterAccount
	where GeographyID = @geographyID

	delete waca from dbo.WaterAccountCustomAttribute waca
	join #waterAccountIDsToDelete wad on waca.WaterAccountID = wad.WaterAccountID

	delete war from dbo.WaterAccountReconciliation war
	join #waterAccountIDsToDelete wad on war.WaterAccountID = wad.WaterAccountID

	delete wau from dbo.WaterAccountUser wau
	join #waterAccountIDsToDelete wad on wau.WaterAccountID = wad.WaterAccountID

	delete waus from dbo.WaterAccountUserStaging waus
	join #waterAccountIDsToDelete wad on waus.WaterAccountID = wad.WaterAccountID


	-- delete geography WaterAccounts
	delete from dbo.WaterAccount where GeographyID = @geographyID

end