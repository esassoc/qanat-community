create view dbo.vParcelSupplyTransactionHistory
as

select ps.GeographyID, ps.EffectiveDate, ps.TransactionDate, ps.WaterTypeID, wt.WaterTypeName, ps.UploadedFileName,
case when u.LastName is not null then concat(u.FirstName, ' ', u.LastName) else null end as CreateUserFullName,
                    count(ps.ParcelID) as AffectedParcelsCount,
                    Sum(p.ParcelArea) as AffectedAcresCount,
                    Sum(ps.TransactionAmount) as TransactionVolume,
                    case when ps.UploadedFileName is null then Sum(ps.TransactionAmount) / Sum(p.ParcelArea) else null end as TransactionDepth

from dbo.ParcelSupply ps
join dbo.Parcel p on ps.ParcelID = p.ParcelID
left join dbo.[User] u on ps.UserID = u.UserID
left join dbo.WaterType wt on ps.WaterTypeID = wt.WaterTypeID
group by ps.GeographyID, ps.EffectiveDate, ps.TransactionDate, ps.WaterTypeID, wt.WaterTypeName, ps.UploadedFileName, u.FirstName, u.LastName 
having count(*) > 1 -- might remove this if we want to show single transactions

GO

/*
select * from dbo.vParcelSupplyTransactionHistory
*/