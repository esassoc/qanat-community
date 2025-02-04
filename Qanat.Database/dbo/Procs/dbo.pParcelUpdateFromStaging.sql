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

    -- set removed parcels to inactive and remove the water account association; we might have to revisit this if the effective year is less than the wap effective year
    update wap
    set wap.EndYear = case when @effectiveYear < wap.EffectiveYear then wap.EffectiveYear + 1 else @effectiveYear end
    from dbo.Parcel p
    join #parcelChanges pc on p.ParcelNumber = pc.ParcelNumber and p.GeographyID = pc.GeographyID
    join dbo.WaterAccountParcel wap on p.ParcelID = wap.ParcelID and p.WaterAccountID = wap.WaterAccountID
    where p.GeographyID = @geographyID and pc.ParcelStatusID = 2 and p.WaterAccountID is not null and wap.EndYear is null

    update p
    set 
        ParcelStatusID = pc.ParcelStatusID,
        WaterAccountID = null
    from dbo.Parcel p
    join #parcelChanges pc on p.ParcelNumber = pc.ParcelNumber and p.GeographyID = pc.GeographyID
    where p.GeographyID = @geographyID and pc.ParcelStatusID = 2


    -- log changes to ParcelOwnerHistory table; any change to OwnerName, OwnerAddress, or ParcelArea will be logged
    insert into dbo.ParcelHistory(GeographyID, ParcelID, EffectiveYear, UpdateDate, UpdateUserID, OwnerName, OwnerAddress, ParcelArea, ParcelStatusID, IsReviewed, WaterAccountID)
	select @geographyID, p.ParcelID, @effectiveYear, GETUTCDATE(), @uploadUserID, p.OwnerName, p.OwnerAddress, p.ParcelArea, p.ParcelStatusID,
		case when (
		LTRIM(RTRIM(isnull(p.OwnerName, ''))) != LTRIM(RTRIM(isnull(poh.OwnerName, '')))
        or LTRIM(RTRIM(isnull(p.OwnerAddress, ''))) != LTRIM(RTRIM(isnull(poh.OwnerAddress, '')))
        or cast(isnull(p.ParcelArea, 0) as decimal(10,2)) != isnull(poh.ParcelArea, 0)
		or isnull(p.ParcelStatusID, 0) != isnull(poh.ParcelStatusID, 0)
		) then 0 else 1 end as IsReviewed,
        p.WaterAccountID
	from dbo.Parcel p
	left join 
    (
        select ParcelID, UpdateDate, OwnerName, OwnerAddress, ParcelArea, ParcelStatusID, rank() over (partition by ParcelID order by UpdateDate desc) as Ranking
        from dbo.ParcelHistory
        where GeographyID = @geographyID
    ) poh on p.ParcelID = poh.ParcelID and poh.Ranking = 1
	where p.GeographyID = @geographyID
	
end