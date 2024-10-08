CREATE FUNCTION dbo.fWaterAccountUser(@userID int)
RETURNS @waterAccounts TABLE 
(
	WaterAccountID int not null,
    GeographyID int not null
)
AS
BEGIN
    if exists(select 1 from dbo.[User] where UserID = @userID and RoleID = 1) -- SysAdmin
    begin
        insert into @waterAccounts(WaterAccountID, GeographyID)
        select WaterAccountID, GeographyID from dbo.WaterAccount
    end
    else
    begin
        insert into @waterAccounts(WaterAccountID, GeographyID)
        select wa.WaterAccountID, wa.GeographyID
        from dbo.GeographyUser gu
        join dbo.WaterAccount wa on gu.GeographyID = wa.GeographyID
        where gu.UserID = @userID and gu.GeographyRoleID = 1 -- WaterManager
        union
        select wa.WaterAccountID, wa.GeographyID
        from dbo.WaterAccountUser wau
        join dbo.WaterAccount wa on wau.WaterAccountID = wa.WaterAccountID
        where wau.UserID = @userID
    end
	
	RETURN 
END

GO

