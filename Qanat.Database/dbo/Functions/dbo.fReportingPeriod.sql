CREATE FUNCTION dbo.fReportingPeriod(@year int)
RETURNS TABLE 
AS
RETURN
(
    select g.GeographyID, g.GeographyName, g.GeographyDisplayName, rp.ReportingPeriodID, rp.ReportingPeriodName, StartMonth, 
		dateadd(year, (case when StartMonth = 1 then 0 else -1 end), cast(concat(StartMonth, '/1/', @year) as DateTime)) as StartDate,
		dateadd(second, -1, dateadd(year, (case when StartMonth = 1 then 1 else 0 end), cast(concat(StartMonth, '/1/', @year) as DateTime))) as EndDate
    from dbo.ReportingPeriod rp
    join dbo.[Geography] g on rp.GeographyID = g.GeographyID
)

GO
