create procedure dbo.pParcelUpdateFromStaging
(
    @geographyID int,
    @effectiveYear int,
    @uploadUserID int
)
as

begin

    -- merge parcelstaging into parcel
    -- write changes in owner and address to parcel owner history table

    declare @squareFeetToAcresDivisor int
    select @squareFeetToAcresDivisor = AreaToAcresConversionFactor from dbo.[Geography] where GeographyID = @geographyID

    --insert new parcels into parcel table
    select @geographyID as GeographyID,
		   ParcelNumber,
           NewGeometry,
           NewGeometry4326,
           NewOwnerName, 
           NewOwnerAddress,
		   NewAcres,
           ParcelStatusID,
		   case 
				when NewGeometry is null or NewAcres is null then null 
				when NewAcres is not null then NewAcres
				else round(NewGeometry.STArea() / @squareFeetToAcresDivisor, 14) 
		   end as ParcelArea,
           HasGeometryChange,
		   HasAcresChange,
           IsNew
    into #parcelChanges
	from dbo.fParcelStagingChanges(@geographyID)

	insert into dbo.Parcel (GeographyID, ParcelNumber, ParcelArea, ParcelStatusID, OwnerName, OwnerAddress)
    select GeographyID, ParcelNumber, ParcelArea, ParcelStatusID,  LTRIM(RTRIM(NewOwnerName)),  LTRIM(RTRIM(NewOwnerAddress))
    from #parcelChanges
    where IsNew = 1

    insert into dbo.ParcelGeometry(GeographyID, ParcelID, GeometryNative, Geometry4326)
    select np.GeographyID, p.ParcelID, np.NewGeometry, np.NewGeometry4326
    from #parcelChanges np
    join dbo.Parcel p on np.ParcelNumber = p.ParcelNumber and np.GeographyID = p.GeographyID
    where np.IsNew = 1

	-- update existing parcels
    update p
    set 
        ParcelArea = pc.ParcelArea,
        ParcelStatusID = pc.ParcelStatusID,
        OwnerName = LTRIM(RTRIM(pc.NewOwnerName)),
        OwnerAddress =  LTRIM(RTRIM(pc.NewOwnerAddress))

    from dbo.Parcel p
    join #parcelChanges pc on p.ParcelNumber = pc.ParcelNumber and p.GeographyID = pc.GeographyID
    where p.GeographyID = @geographyID and pc.ParcelStatusID != 2 -- only update if they are not inactive

    update pg
    set 
        GeometryNative = pc.NewGeometry,
        Geometry4326 = pc.NewGeometry4326
    from dbo.Parcel p
    join dbo.ParcelGeometry pg on p.ParcelID = pg.ParcelID
    join #parcelChanges pc on p.ParcelNumber = pc.ParcelNumber and p.GeographyID = pc.GeographyID
    where p.GeographyID = @geographyID and pc.ParcelStatusID != 2 -- only update if they are not inactive

    -- remove the water account associations
	SELECT WAP.WaterAccountParcelID, p.ParcelID, WA.WaterAccountID, WA.WaterAccountNumber, WA.WaterAccountName, RP.ReportingPeriodID
	INTO #WaterAccountParcelsToDelete
    from dbo.Parcel p
    join #parcelChanges			pc on p.ParcelNumber = pc.ParcelNumber and p.GeographyID = pc.GeographyID
    join dbo.WaterAccountParcel wap on p.ParcelID = wap.ParcelID and p.WaterAccountID = wap.WaterAccountID
	join dbo.WaterAccount		wa on wa.WaterAccountID = wap.WaterAccountID
	join dbo.ReportingPeriod	rp on rp.ReportingPeriodID = wap.ReportingPeriodID
    where p.GeographyID = @geographyID and pc.ParcelStatusID = 2 and p.WaterAccountID is not null 
	AND YEAR(rp.EndDate) = @effectiveYear

	INSERT INTO dbo.ParcelWaterAccountHistory(GeographyID, ParcelID, ReportingPeriodID, FromWaterAccountID, FromWaterAccountNumber, FromWaterAccountName, Reason, CreateUserID, CreateDate)
	SELECT @geographyID, WAPTD.ParcelID, WAPTD.ReportingPeriodID, WAPTD.WaterAccountID, WAPTD.WaterAccountNumber, WAPTD.WaterAccountName, 'Removed from parcel upload.', @uploadUserID, GETUTCDATE()
	FROM #WaterAccountParcelsToDelete WAPTD

	DELETE WAP FROM dbo.WaterAccountParcel WAP WHERE WAP.WaterAccountParcelID IN (SELECT WAPTD.WaterAccountParcelID FROM #WaterAccountParcelsToDelete WAPTD);

	-- set parcel to Inactive
	update p
	set 
		ParcelStatusID = pc.ParcelStatusID,
		WaterAccountID = null
	from dbo.Parcel p
	join #parcelChanges pc on p.ParcelNumber = pc.ParcelNumber and p.GeographyID = pc.GeographyID
	where p.GeographyID = @geographyID and pc.ParcelStatusID = 2

    -- log changes to ParcelOwnerHistory table; any change to OwnerName, OwnerAddress, or ParcelArea will be logged
    insert into dbo.ParcelHistory(GeographyID, ParcelID, UpdateDate, UpdateUserID, OwnerName, OwnerAddress, ParcelArea, ParcelStatusID, IsReviewed)
	select @geographyID, p.ParcelID, GETUTCDATE(), @uploadUserID, p.OwnerName, p.OwnerAddress, p.ParcelArea, p.ParcelStatusID,
		case when (
		LTRIM(RTRIM(isnull(p.OwnerName, ''))) != LTRIM(RTRIM(isnull(poh.OwnerName, '')))
        or LTRIM(RTRIM(isnull(p.OwnerAddress, ''))) != LTRIM(RTRIM(isnull(poh.OwnerAddress, '')))
        or cast(isnull(p.ParcelArea, 0) as decimal(10,2)) != isnull(poh.ParcelArea, 0)
		or isnull(p.ParcelStatusID, 0) != isnull(poh.ParcelStatusID, 0)
		) then 0 else 1 end as IsReviewed
	from dbo.Parcel p
	left join 
    (
        select ParcelID, UpdateDate, OwnerName, OwnerAddress, ParcelArea, ParcelStatusID, rank() over (partition by ParcelID order by UpdateDate desc) as Ranking
        from dbo.ParcelHistory
        where GeographyID = @geographyID
    ) poh on p.ParcelID = poh.ParcelID and poh.Ranking = 1
	where p.GeographyID = @geographyID
	

    --update geography's bounding box 
    
    declare @gsaboundary geometry
    declare @extent geometry
    select @gsaboundary = geometry::UnionAggregate(pg.Geometry4326)
    from Parcel p
    join ParcelGeometry pg on p.ParcelID = pg.ParcelID
    where p.GeographyID = @geographyID

    set @extent = @gsaboundary.STEnvelope().STBuffer(.03).STEnvelope()
    update dbo.GeographyBoundary
    set BoundingBox = @extent, GSABoundary = @gsaboundary
    where GeographyID = @geographyID
end