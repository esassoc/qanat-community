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


        public static MostRecentEffectiveDatesDto GetMostRecentEffectiveDatesByWaterAccount(
            QanatDbContext dbContext, int waterAccountID, int year)
        {
            var waterAccountIDParam = new SqlParameter("waterAccountID", waterAccountID);
            var yearParam = new SqlParameter("year", year);
            var waterAccountMostRecentEffectiveDates = dbContext.WaterAccountMostRecentEffectiveDate
                .FromSqlRaw($"EXECUTE dbo.pWaterAccountMostRecentEffectiveDate @waterAccountID, @year",
                    waterAccountIDParam, yearParam).ToList();
            var mostRecentEffectiveDateDto = new MostRecentEffectiveDatesDto();
            var waterAccountMostRecentEffectiveDate = waterAccountMostRecentEffectiveDates
                .FirstOrDefault();

            if (waterAccountMostRecentEffectiveDate != null)
            {
                mostRecentEffectiveDateDto.MostRecentSupplyEffectiveDate = waterAccountMostRecentEffectiveDate.MostRecentSupplyEffectiveDate;
                mostRecentEffectiveDateDto.MostRecentUsageEffectiveDate = waterAccountMostRecentEffectiveDate.MostRecentUsageEffectiveDate;
            }

            return mostRecentEffectiveDateDto;
        }
    }
}