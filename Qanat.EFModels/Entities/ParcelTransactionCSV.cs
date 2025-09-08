using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using System.Globalization;

namespace Qanat.EFModels.Entities;

public class ParcelTransactionCSV
{
    public string UsageLocationName { get; set; }
    public decimal? Quantity { get; set; }
    public string Comment { get; set; }
}

public sealed class ParcelTransactionCSVMap : ClassMap<ParcelTransactionCSV>
{
    public ParcelTransactionCSVMap(string usageLocationColumnName, string quantityColumnName, string commentColumnName)
    {
        Map(m => m.UsageLocationName).Name(usageLocationColumnName);
        Map(m => m.Quantity).Name(quantityColumnName).TypeConverter<ScientificDecimalConverter>();
        if (!string.IsNullOrEmpty(commentColumnName))
        {
            Map(m => m.Comment).Name(commentColumnName);
        }
    }
}
public class ScientificDecimalConverter : DefaultTypeConverter
{
    public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return null;
        }

        if (decimal.TryParse(text, NumberStyles.Number, CultureInfo.InvariantCulture, out var decValue))
        {
            return decValue;
        }

        if (double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out var doubleValue))
        {
            return Convert.ToDecimal(doubleValue);
        }

        throw new TypeConverterException(this, memberMapData, text, row.Context, $"Cannot convert '{text}' to decimal.");
    }
}