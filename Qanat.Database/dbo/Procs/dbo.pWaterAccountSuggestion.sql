create procedure dbo.pWaterAccountSuggestion
(
    @geographyID int
)
as

begin

	drop table if exists #parcels

	declare @zoneGroupID int = (select ZoneGroupID from GeographyAllocationPlanConfiguration where GeographyID = @geographyID)

	select p.ParcelID, p.ParcelNumber, p.ParcelStatusID, p.ParcelArea, sub.ZoneName, p.OwnerName, p.OwnerAddress
    into #parcels
    from dbo.Parcel p
	left join (
		select pz.ParcelID, z.ZoneName from dbo.ParcelZone pz
		join dbo.[Zone] z on pz.ZoneID = z.ZoneID and z.ZoneGroupID = @zoneGroupID
	) sub on p.ParcelID = sub.ParcelID
	where p.GeographyID = @geographyID and p.ParcelStatusID = 3 -- we only care about unassigned parcels

	-- #wells acts as a filter for only caring about wells that fit a certain criteria and parcels that relate to them
	drop table if exists #wells
	SELECT distinct w.WellRegistrationID
		, w.WellID as WellID
        , wc.ContactName
        , concat(StreetAddress, ', ', City, ', ', s.StatePostalCode, ' ', ZipCode) as ContactAddress
	into #wells
	FROM dbo.WellRegistration w
    join dbo.WellRegistrationContact wc on w.WellRegistrationID = wc.WellRegistrationID and wc.WellRegistrationContactTypeID = 2 -- OwnerOperator
	join dbo.[State] s on wc.StateID = s.StateID
	where w.GeographyID = @geographyID and w.ApprovalDate is not null

	-- these are all the possible well irrigated parcel relationships
	drop table if exists #wellParcelsPossible
	SELECT wip.WellRegistrationID, wip.ParcelID
	into #wellParcelsPossible 
	FROM dbo.WellRegistrationIrrigatedParcel wip
    join #parcels p on wip.ParcelID = p.ParcelID
	join #wells w on wip.WellRegistrationID = w.WellRegistrationID

	drop table if exists #wellParcelsProcess
	SELECT WellRegistrationID as WellRegistrationIDOriginal
		, WellRegistrationID
		, ParcelID
		, 0 as IsProcessed
	into #wellParcelsProcess 
	FROM #wellParcelsPossible

	declare @processingParcels bit
	set @processingParcels = 1

	while(exists (select 1 from #wellParcelsProcess where IsProcessed = 0))
	begin
		if(@processingParcels = 1)
		begin
			drop table if exists #newWellParcelsToProcess1

			SELECT wp.WellRegistrationIDOriginal
				, tw.WellRegistrationID
				, tw.ParcelID
			into #newWellParcelsToProcess1
			FROM #wellParcelsPossible tw
			join #wellParcelsProcess wp on tw.ParcelID = wp.ParcelID and wp.IsProcessed = 0

			update #wellParcelsProcess set IsProcessed = 1 

			insert into #wellParcelsProcess(WellRegistrationIDOriginal, WellRegistrationID, ParcelID, IsProcessed)
			SELECT np.WellRegistrationIDOriginal
				, np.WellRegistrationID
				, np.ParcelID
				, 0 As IsProcessed
			from #newWellParcelsToProcess1 np
			left join #wellParcelsProcess wp on np.WellRegistrationIDOriginal = wp.WellRegistrationIDOriginal and np.WellRegistrationID = wp.WellRegistrationID and np.ParcelID = wp.ParcelID
			where wp.WellRegistrationIDOriginal is null

			set @processingParcels = 0
		end
		else
		begin
			drop table if exists #newWellParcelsToProcess2

			SELECT wp.WellRegistrationIDOriginal
				, tw.WellRegistrationID
				, tw.ParcelID
			into #newWellParcelsToProcess2
			FROM #wellParcelsPossible tw
			join #wellParcelsProcess wp on tw.WellRegistrationID = wp.WellRegistrationID and wp.IsProcessed = 0

			update #wellParcelsProcess set IsProcessed = 1 

			insert into #wellParcelsProcess(WellRegistrationIDOriginal, WellRegistrationID, ParcelID, IsProcessed)
			SELECT np.WellRegistrationIDOriginal
				, np.WellRegistrationID
				, np.ParcelID
				, 0 As IsProcessed
			from #newWellParcelsToProcess2 np
			left join #wellParcelsProcess wp on np.WellRegistrationIDOriginal = wp.WellRegistrationIDOriginal and np.WellRegistrationID = wp.WellRegistrationID and np.ParcelID = wp.ParcelID
			where wp.WellRegistrationIDOriginal is null
			set @processingParcels = 1
		end

	end


	-- now that we have the full sets of parcels and wells for a given well, we need to order them by WellID; first WellID becomes the WaterAccount name for the Well WaterAccount
	drop table if exists #wellRankings
	SELECT	wp.WellRegistrationIDOriginal, wp.WellRegistrationID, wp.ParcelID, wp.IsProcessed, w.WellID, row_number() over (partition by WellRegistrationIDOriginal order by WellID) as Ranking
	into #wellRankings
	FROM	#wellParcelsProcess wp
	join	#wells w on wp.WellRegistrationID = w.WellRegistrationID
    join    #wellParcelsPossible p on wp.ParcelID = p.ParcelID

    -- start with well water accounts
    drop table if exists #wellWaterAccounts
	create table #wellWaterAccounts
	(
		WaterAccountID int not null identity(1,1) primary key,
		WaterAccountName varchar(1000) not null,
		ContactAddress varchar(1000) null,
		WellRegistrationID int not null
	)

	insert into #wellWaterAccounts(WellRegistrationID, WaterAccountName, ContactAddress)
	select t2.WellRegistrationID, t2.ContactName as WaterAccountName, t2.ContactAddress
	from #wellRankings t1
	join (
		select distinct w.WellRegistrationID, w.WellID, w.ContactName, w.ContactAddress
		from #wellRankings wr
		join #wells w on wr.WellID = w.WellID
		where wr.Ranking = 1
	) t2 on t1.WellRegistrationIDOriginal = t2.WellRegistrationID
	group by t2.WellRegistrationID, t2.WellID, t2.ContactName, t2.ContactAddress
	order by t2.WellRegistrationID, t2.WellID, t2.ContactName, t2.ContactAddress
	
	drop table if exists #wellWaterAccountParcels
	create table #wellWaterAccountParcels
	(
		WaterAccountID int not null,
		ParcelID int not null,
		unique(WaterAccountID, ParcelID)
	)

	insert into #wellWaterAccountParcels(WaterAccountID, ParcelID)
	select distinct wwa.WaterAccountID, wr.ParcelID
	from #wellWaterAccounts wwa
	join #wellRankings wr on wwa.WellRegistrationID = wr.WellRegistrationIDOriginal
	order by wwa.WaterAccountID, wr.ParcelID

	drop table if exists #wellWaterAccountWells
	create table #wellWaterAccountWells
	(
		WaterAccountID int not null,
		WellRegistrationID int not null,
        WellID int not null,
		unique(WaterAccountID, WellRegistrationID)
	)

	insert into #wellWaterAccountWells(WaterAccountID, WellRegistrationID, WellID)
	select distinct t1.WaterAccountID, t2.WellRegistrationID, t2.WellID
	from #wellWaterAccounts t1
	join #wellRankings t2 on t1.WellRegistrationID = t2.WellRegistrationIDOriginal
	order by t1.WaterAccountID, t2.WellRegistrationID

    -- the remaining Parcels that are not matched via WellIrrigatedParcel are grouped by Parcel Owner and Parcel Address
	drop table if exists #remainingParcels
    select p.ParcelID, p.OwnerName, p.OwnerAddress
    into #remainingParcels
    from #parcels p
    left join #wellWaterAccountParcels wap on p.ParcelID = wap.ParcelID
    where wap.ParcelID is null

	drop table if exists #parcelOwnerWaterAccounts
	create table #parcelOwnerWaterAccounts
	(
		WaterAccountID int not null identity(1,1) primary key,
		WaterAccountName varchar(1000) not null,
        OwnerName varchar(500) not null,
		OwnerAddress varchar(500) null
	)

	drop table if exists #parcelOwnerWaterAccountParcels
	create table #parcelOwnerWaterAccountParcels
	(
		WaterAccountID int not null,
		ParcelID int not null,
		unique(WaterAccountID, ParcelID)
	)

    insert into #parcelOwnerWaterAccounts(WaterAccountName, OwnerName, OwnerAddress)
	SELECT  OwnerName as WaterAccountName, OwnerName, OwnerAddress
	from #remainingParcels
    where len(OwnerName) > 0
	group by OwnerName, OwnerAddress
    order by OwnerName, OwnerAddress

	insert into #parcelOwnerWaterAccountParcels(WaterAccountID, ParcelID)
	select wa.WaterAccountID, p.ParcelID
	from #parcelOwnerWaterAccounts wa
	join #remainingParcels p on wa.OwnerName = p.OwnerName and wa.OwnerAddress = p.OwnerAddress
    order by p.ParcelID


    -- combine the well water accounts with parcel owner water accounts
    select concat(wwa.WaterAccountName, ' (', Round(wwpa.ParcelArea, 2),' acres)') as WaterAccountName, www.WellIDList, wwa.WaterAccountName as ContactName, wwa.ContactAddress, Round(wwpa.ParcelArea, 2) as ParcelArea, wwpz.Zones,
    wwp.Parcels
    from #wellWaterAccounts wwa
    join 
    (
        select WaterAccountID, '[' + STRING_AGG(cast(concat('{"ParcelID":', p.ParcelID, ',"ParcelNumber":"', p.ParcelNumber, '"}') as nvarchar(max)), ',') WITHIN GROUP (ORDER BY ParcelNumber) + ']' as Parcels
        from #wellWaterAccountParcels w
        join #parcels p on w.ParcelID = p.ParcelID
        group by WaterAccountID
    ) wwp on wwa.WaterAccountID = wwp.WaterAccountID
    join 
    (
        select WaterAccountID, SUM(ParcelArea) as ParcelArea
        from #wellWaterAccountParcels w
        join #parcels p on w.ParcelID = p.ParcelID
        group by WaterAccountID
    ) wwpa on wwa.WaterAccountID = wwpa.WaterAccountID
    join 
    (
        select WaterAccountID, STRING_AGG(WellID, ',') WITHIN GROUP (ORDER BY WellID) as WellIDList
        from #wellWaterAccountWells
        group by WaterAccountID
    ) www on wwa.WaterAccountID = www.WaterAccountID
    left join 
    (
        select WaterAccountID, STRING_AGG(ZoneName, ',') WITHIN GROUP (ORDER BY ZoneName) as Zones
        from (
            select distinct WaterAccountID, ZoneName
            from #wellWaterAccountParcels w
            join #parcels p on w.ParcelID = p.ParcelID
        ) a
        group by WaterAccountID
    ) wwpz on wwa.WaterAccountID = wwpz.WaterAccountID

    union

    select concat(wwa.WaterAccountName, ' (', Round(wwpa.ParcelArea, 2),' acres)') as WaterAccountName, null as WellIDList, wwa.WaterAccountName as ContactName, wwa.OwnerAddress as ContactAddress, Round(wwpa.ParcelArea, 2) as ParcelArea, wwpz.Zones,
    wwp.Parcels
    from #parcelOwnerWaterAccounts wwa
    join 
    (
        select WaterAccountID, '[' + STRING_AGG(cast(concat('{"ParcelID":', p.ParcelID, ',"ParcelNumber":"', p.ParcelNumber, '"}') as nvarchar(max)), ',') WITHIN GROUP (ORDER BY ParcelNumber) + ']' as Parcels
        from #parcelOwnerWaterAccountParcels w
        join #parcels p on w.ParcelID = p.ParcelID
        group by WaterAccountID
    ) wwp on wwa.WaterAccountID = wwp.WaterAccountID
    join 
    (
        select WaterAccountID, SUM(ParcelArea) as ParcelArea
        from #parcelOwnerWaterAccountParcels w
        join #parcels p on w.ParcelID = p.ParcelID
        group by WaterAccountID
    ) wwpa on wwa.WaterAccountID = wwpa.WaterAccountID
    left join 
    (
        select WaterAccountID, STRING_AGG(ZoneName, ',') WITHIN GROUP (ORDER BY ZoneName) as Zones
        from (
            select distinct WaterAccountID, ZoneName
            from #parcelOwnerWaterAccountParcels w
            join #parcels p on w.ParcelID = p.ParcelID
        ) a
        group by WaterAccountID
    ) wwpz on wwa.WaterAccountID = wwpz.WaterAccountID

    order by WaterAccountName 
end