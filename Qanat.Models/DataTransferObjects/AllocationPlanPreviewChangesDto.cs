namespace Qanat.Models.DataTransferObjects;

public class AllocationPlanPreviewChangesDto
{
    public string AllocationPlanDisplayName { get; set; }
    public bool ToDelete { get; set; }
    public int TotalPeriodsCount { get; set; }
    public int PeriodsToDeleteCount { get; set; }
}