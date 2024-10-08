using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities
{
    [Keyless]
    public class WaterAccountMostRecentEffectiveDate
    {
        public WaterAccountMostRecentEffectiveDate()
        {
        }
        public DateTime? MostRecentSupplyEffectiveDate { get; set; }
        public DateTime? MostRecentUsageEffectiveDate { get; set; }


        public static MostRecentEffectiveDates GetMostRecentEffectiveDatesByAccount(
            QanatDbContext dbContext, int geographyID, int waterAccountID, int year)
        {
            var geographyIDParam = new SqlParameter("geographyID", geographyID);
            var waterAccountIDParam = new SqlParameter("waterAccountID", waterAccountID);
            var yearParam = new SqlParameter("year", year);
            var waterAccountMostRecentEffectiveDates = dbContext.WaterAccountMostRecentEffectiveDate
                .FromSqlRaw($"EXECUTE dbo.pWaterAccountMostRecentEffectiveDate @geographyID, @waterAccountID, @year",
                    geographyIDParam, waterAccountIDParam, yearParam).ToList();
            var mostRecentEffectiveDateDto = new MostRecentEffectiveDates();
            var waterAccountMostRecentEffectiveDate = waterAccountMostRecentEffectiveDates
                .FirstOrDefault();

            if (waterAccountMostRecentEffectiveDate != null)
            {
                mostRecentEffectiveDateDto.MostRecentSupplyEffectiveDate = waterAccountMostRecentEffectiveDate.MostRecentSupplyEffectiveDate?.ToShortDateString();
                mostRecentEffectiveDateDto.MostRecentUsageEffectiveDate = waterAccountMostRecentEffectiveDate.MostRecentUsageEffectiveDate?.ToShortDateString();
            }

            return mostRecentEffectiveDateDto;
        }
    }
}