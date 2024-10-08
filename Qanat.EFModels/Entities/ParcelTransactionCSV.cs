using CsvHelper.Configuration;

namespace Qanat.EFModels.Entities;

public class ParcelTransactionCSV
{
    public string UsageEntityName { get; set; }
    public decimal? Quantity { get; set; }
    public string Comment { get; set; }
}

public sealed class ParcelTransactionCSVMap : ClassMap<ParcelTransactionCSV>
{
    public ParcelTransactionCSVMap(string usageEntityColumnName, string quantityColumnName, string commentColumnName)
    {
        Map(m => m.UsageEntityName).Name(usageEntityColumnName);
        Map(m => m.Quantity).Name(quantityColumnName);
        if (!string.IsNullOrEmpty(commentColumnName))
        {
            Map(m => m.Comment).Name(commentColumnName);
        }
    }
}