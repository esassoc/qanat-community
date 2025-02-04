namespace Qanat.Models.DataTransferObjects;

public class FeeStructureDto(int id, string name)
{
    public int FeeStructureID { get; set; } = id;
    public string Name { get; set; } = name;
    public string Description { get; set; }
    public List<FeeCategoryDto> Categories { get; set; } = [];
}

public static class FeeStructuresDtos
{
    public static List<FeeStructureDto> ETSGSA_FeeStructures =
    [
        new(1, "2025 (Phase 1)")
        {
            Description = "Proposed GW Use Fee Structure as of 9/16/2024 - 10% Reduction Target (2025)",
            Categories = 
            [
                new FeeCategoryDto()
                {
                    Name = "Category 0",
                    Description = "Within Estimated Native Sustainable Yield",
                    ThresholdFloor = 0,
                    ThresholdCeiling = 0.5m,
                    AllocationMultiplier = 0.5m,
                    TotalFeePerAcreFoot = null
                },
                new FeeCategoryDto()
                {
                    Name = "Category 1",
                    Description = "Within Intended Long-Term Additional Sustainable Yield Once Sustainable Thresholds are Met",
                    ThresholdFloor = 0.5m,
                    ThresholdCeiling = 1.1m,
                    AllocationMultiplier = .6m,
                    TotalFeePerAcreFoot = 57.81m
                },
                new FeeCategoryDto()
                {
                    Name = "Category 2",
                    Description = "Consumed GW Above Estimated Sustainable Yield",
                    ThresholdFloor = 1.1m,
                    ThresholdCeiling = 1.6m,
                    AllocationMultiplier = .5m,
                    TotalFeePerAcreFoot = 138.61m
                },
                new FeeCategoryDto()
                {
                    Name = "Category 3",
                    Description = "Above Use Reduction Target",
                    ThresholdFloor = 1.6m,
                    ThresholdCeiling = null,
                    AllocationMultiplier = null,
                    TotalFeePerAcreFoot = 138.61m
                },
            ]
        },
        new(2, "2026-27 (Phase 2)")
        {
            Description = "Proposed GW Use Fee Structure as of 9/16/2024 - 10% Reduction Target (2026-27)",
            Categories =
            [
                new FeeCategoryDto()
                {
                    Name = "Category 0",
                    Description = "Within Estimated Native Sustainable Yield",
                    ThresholdFloor = 0,
                    ThresholdCeiling = 0.5m,
                    AllocationMultiplier = 0.5m,
                    TotalFeePerAcreFoot = null
                },
                new FeeCategoryDto()
                {
                    Name = "Category 1",
                    Description = "Within Intended Long-Term Additional Sustainable Yield Once Sustainable Thresholds are Met",
                    ThresholdFloor = 0.5m,
                    ThresholdCeiling = 1.1m,
                    AllocationMultiplier = .6m,
                    TotalFeePerAcreFoot = 57.81m
                },
                new FeeCategoryDto()
                {
                    Name = "Category 2",
                    Description = "Consumed GW Above Estimated Sustainable Yield",
                    ThresholdFloor = 1.1m,
                    ThresholdCeiling = 1.6m,
                    AllocationMultiplier = .5m,
                    TotalFeePerAcreFoot = 138.61m
                },
                new FeeCategoryDto()
                {
                    Name = "Category 3",
                    Description = "Above Use Reduction Target",
                    ThresholdFloor = 1.6m,
                    ThresholdCeiling = null,
                    AllocationMultiplier = null,
                    TotalFeePerAcreFoot = 316.94m
                },
            ]
        },
        //new(3, "2028-30")
        //{
        //    Description = "Proposed GW Use Fee Structure as of 9/16/2024 - 20% Reduction Target (2028-30)",
        //    Categories =
        //    [
        //        new FeeCategoryDto()
        //        {
        //            Name = "Category 0",
        //            Description = "Within Estimated Native Sustainable Yield",
        //            ThresholdFloor = 0,
        //            ThresholdCeiling = 0.5m,
        //            AllocationMultiplier = 0.5m,
        //            TotalFeePerAcreFoot = null
        //        },
        //        new FeeCategoryDto()
        //        {
        //            Name = "Category 1",
        //            Description = "Within Intended Long-Term Additional Sustainable Yield Once Sustainable Thresholds are Met",
        //            ThresholdFloor = 0.5m,
        //            ThresholdCeiling = 1.0m,
        //            AllocationMultiplier = .6m,
        //            TotalFeePerAcreFoot = 57.81m
        //        },
        //        new FeeCategoryDto()
        //        {
        //            Name = "Category 2",
        //            Description = "Consumed GW Above Estimated Sustainable Yield",
        //            ThresholdFloor = 1.0m,
        //            ThresholdCeiling = 1.3m,
        //            AllocationMultiplier = .5m,
        //            TotalFeePerAcreFoot = 138.61m
        //        },
        //        new FeeCategoryDto()
        //        {
        //            Name = "Category 3",
        //            Description = "Above Use Reduction Target",
        //            ThresholdFloor = 1.3m,
        //            ThresholdCeiling = null,
        //            AllocationMultiplier = null,
        //            TotalFeePerAcreFoot = 316.94m
        //        },
        //    ]
        //},
    ];
}

public class FeeCategoryDto
{
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal ThresholdFloor { get; set; }
    public decimal? ThresholdCeiling { get; set; }
    public decimal? AllocationMultiplier { get; set; }
    public decimal? TotalFeePerAcreFoot { get; set; }
}