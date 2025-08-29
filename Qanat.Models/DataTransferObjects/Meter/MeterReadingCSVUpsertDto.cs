using CsvHelper.Configuration;
using Microsoft.AspNetCore.Http;

namespace Qanat.Models.DataTransferObjects;

public class MeterReadingCSVFileUploadDto
{
    public IFormFile CSVFile { get; set; }
}

public class MeterReadingCSVUpsertDto
{
    public string SerialNumber { get; set; }
    public DateTime? Date { get; set; }
    public string Time { get; set; }
    public string ReaderInitials { get; set; }
    public decimal? PreviousReading { get; set; }
    public decimal? CurrentReading { get; set; }
    public string UnitType { get; set; }
    public string Comment { get; set; }

    public bool IsBlank()
    {
        var isBlank = string.IsNullOrWhiteSpace(SerialNumber)
                   && !Date.HasValue
                   && string.IsNullOrWhiteSpace(Time)
                   && string.IsNullOrWhiteSpace(ReaderInitials)
                   && !PreviousReading.HasValue
                   && !CurrentReading.HasValue
                   && string.IsNullOrWhiteSpace(UnitType)
                   && string.IsNullOrWhiteSpace(Comment);

        return isBlank;
    }
}

public class MeterReadingCSVParseResult
{
    public List<MeterReadingCSVUpsertDto> Records { get; set; } = [];
    public List<ErrorMessage> Errors { get; set; } = [];
}

public sealed class MeterReadingUpsertDtoCSVMap : ClassMap<MeterReadingCSVUpsertDto>
{
    public static string SerialNumber = "Serial Number";
    public static string Date = "Date";
    public static string Time = "Time";
    public static string ReaderInitials = "Reader Initials";
    public static string PreviousReading = "Previous Reading";
    public static string CurrentReading = "Current Reading";
    public static string UnitType = "Unit Type";
    public static string Comment = "Comment";

    public static string[] Headers =
    [
        SerialNumber,
        Date,
        Time,
        ReaderInitials,
        PreviousReading,
        CurrentReading,
        UnitType,
        Comment
    ];

    public MeterReadingUpsertDtoCSVMap()
    {
        Map(m => m.SerialNumber).Name(SerialNumber);
        Map(m => m.Date).Name(Date);
        Map(m => m.Time).Name(Time);
        Map(m => m.ReaderInitials).Name(ReaderInitials);
        Map(m => m.PreviousReading).Name(PreviousReading);
        Map(m => m.CurrentReading).Name(CurrentReading);
        Map(m => m.UnitType).Name(UnitType);
        Map(m => m.Comment).Name(Comment);
    }
}