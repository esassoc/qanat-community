namespace Qanat.Models.DataTransferObjects;

public class FeeCalculatorYearDto(int year, string name)
{
    public int Year { get; set; } = year;
    public string Name { get; set; } = name;
}

public static class FeeCalculatorYearDtos
{
    public static List<FeeCalculatorYearDto> ETSGSA_ReportingPeriods = [
        new(2023, "2023 Reporting Period (Nov 1, 2022 - Oct 31 2023)"),
        new(2024, "2024 Reporting Period (Nov 1, 2023 - Oct 31 2024)"),
    ];
}