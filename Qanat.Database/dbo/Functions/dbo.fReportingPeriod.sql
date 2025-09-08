CREATE FUNCTION dbo.fReportingPeriod(@year int)
RETURNS TABLE 
AS
RETURN
(
    SELECT
        G.GeographyID
    ,   G.GeographyName
    ,   G.GeographyDisplayName
    ,   RP.ReportingPeriodID
    ,   RP.[Name]               as ReportingPeriodName
    ,   MONTH(RP.StartDate)     as StartMonth
    ,   RP.StartDate            as StartDate
    ,   RP.EndDate              as EndDate
    FROM dbo.ReportingPeriod    RP
    JOIN dbo.[Geography]        G on rp.GeographyID = g.GeographyID
    WHERE YEAR(RP.EndDate) = @year
)
GO
