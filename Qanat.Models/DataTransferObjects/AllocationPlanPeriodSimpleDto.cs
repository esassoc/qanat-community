namespace Qanat.Models.DataTransferObjects
{
    public class AllocationPlanPeriodSimpleDto
    {
        public int AllocationPlanPeriodID { get; set; }
        public int AllocationPlanID { get; set; }
        public string AllocationPeriodName { get; set; }
        public decimal AllocationAcreFeetPerAcre { get; set; }
        public int NumberOfYears { get; set; }
        public int StartYear { get; set; }
        public bool EnableCarryOver { get; set; }
        public int? CarryOverNumberOfYears { get; set; }
        public decimal? CarryOverDepreciationRate { get; set; }
        public bool EnableBorrowForward { get; set; }
        public int? BorrowForwardNumberOfYears { get; set; }
    }
}