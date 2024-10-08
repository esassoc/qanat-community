CREATE FUNCTION dbo.fParcelStagingChanges(@geographyID int)
RETURNS TABLE
as return
(
    select      ParcelNumber,
                NewGeometry,
                NewGeometry4326,
		        OldGeometryText,
		        NewGeometryText,
                OldOwnerName,
                NewOwnerName, 
                OldOwnerAddress,
                NewOwnerAddress,
                ParcelStatusID,
                cast(case when isnull(NewOwnerName, '') != isnull(OldOwnerName, '') then 1 else 0 end as bit) as HasOwnerNameChange,
                cast(case when isnull(NewOwnerAddress, '') != isnull(OldOwnerAddress, '') then 1 else 0 end as bit) as HasOwnerAddressChange,
                cast(case when isnull(NewGeometryText, '') != isnull(OldGeometryText, '') then 1 else 0 end as bit) as HasGeometryChange,
                cast(case when ParcelID is null then 1 else 0 end as bit) as IsNew
    from
    (
        select  curr.ParcelID,
                coalesce(new.ParcelNumber, curr.ParcelNumber) as ParcelNumber,
                new.[Geometry] as NewGeometry,
                new.[Geometry4326] as NewGeometry4326,
		        curr.[Geometry4326].STAsText() as OldGeometryText,
		        new.[Geometry4326].STAsText() as NewGeometryText,
                curr.OwnerName as OldOwnerName,
                new.OwnerName as NewOwnerName, 
                curr.OwnerAddress as OldOwnerAddress,
                new.OwnerAddress as NewOwnerAddress,
		        case 
                    when curr.ParcelID is null then 3 -- new parcel, set to Unassigned
                    when new.ParcelStagingID is null then 2  -- no longer exists, so mark as inactive
                    else curr.ParcelStatusID end as ParcelStatusID
	    from dbo.ParcelStaging new
	    full outer join 
        (
			    select p.GeographyID, p.ParcelID, p.ParcelStatusID, p.OwnerName, p.OwnerAddress, p.ParcelNumber, pg.Geometry4326
			    from  dbo.Parcel p
			    join	dbo.ParcelGeometry pg on p.ParcelID = pg.ParcelID and p.GeographyID = pg.GeographyID
			    where p.GeographyID = @geographyID
	    ) curr on new.ParcelNumber = curr.ParcelNumber and new.GeographyID = curr.GeographyID
	    where new.GeographyID = @geographyID or curr.GeographyID = @geographyID
    ) a
)

GO

