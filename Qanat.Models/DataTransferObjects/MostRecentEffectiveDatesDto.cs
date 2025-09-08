namespace Qanat.Models.DataTransferObjects;

public class MostRecentEffectiveDatesDto
{
    public DateTime? MostRecentSupplyEffectiveDate { get; set; }
    public DateTime? MostRecentUsageEffectiveDate { get; set; }
    public DateTime? MostRecentEffectiveDate => MostRecentSupplyEffectiveDate > MostRecentUsageEffectiveDate ? MostRecentSupplyEffectiveDate : MostRecentUsageEffectiveDate;
}