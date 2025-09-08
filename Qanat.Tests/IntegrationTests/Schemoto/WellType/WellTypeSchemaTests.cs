using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Schemoto.InstanceNamespace;
using Schemoto.SchemaNamespace;
using Schemoto.SchemaNamespace.DataTypes.DerivedTypes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Schemoto.SchemaNamespace.UnitOfMeasureNameSpace;

namespace Qanat.Tests.IntegrationTests.Schemoto.WellType;

[TestClass]
public class WellTypeSchemaTests
{
    [TestMethod]
    public void GenerateWellTypeSchemaForGMD3()
    {
        var wellTypeSchema = new Schema
        {
            Name = "Basic Well Type",
            CanonicalName = "basic-well-type",
            AdditionalProperties = new JsonObject(),
            RecordSetSchemata =
            [
                new RecordSetSchema
                {
                    Name = "Water Rights",
                    CanonicalName = "water-rights",
                    AdditionalProperties = new JsonObject(),
                    Description = "Water rights associated to the well.",
                    MinCount = 0,
                    MaxCount = null,
                    FieldSchemata =
                    [
                        new FieldSchema
                        {
                            Name = "Water Right Number",
                            CanonicalName = "WR_NUM",
                            DataType = new StringType()
                            {
                                Nullable = true
                            },
                        },
                        new FieldSchema
                        {
                            Name = "Priority Date",
                            CanonicalName = "PRIORITY",
                            DataType = new DateTimeType()
                            {
                                Nullable = true,
                            },
                        },
                        new FieldSchema
                        {
                            Name = "Authorized Quantity",
                            CanonicalName = "AUTH_QUANT",
                            Description = "The authorized quantity of water for the right",
                            DataType = new DecimalType
                            {
                                Precision = 8,
                                Scale = 2,
                                Nullable = false,
                                MinValue = 0,
                                UnitOfMeasure = UnitOfMeasure.AcreFeet
                            }
                        },
                        new FieldSchema
                        {
                            Name = "Additional Quantity",
                            CanonicalName = "ADD_QUANT",
                            Description = "The additional quantity of water for the right",
                            DataType = new DecimalType
                            {
                                Precision = 8,
                                Scale = 2,
                                Nullable = true,
                                MinValue = 0,
                                UnitOfMeasure = UnitOfMeasure.AcreFeet
                            }
                        },
                        new FieldSchema
                        {
                            Name = "Authorized Rate",
                            CanonicalName = "AUTH_RATE",
                            Description = "The authorized rate of water for the right",
                            DataType = new DecimalType
                            {
                                Precision = 8,
                                Scale = 2,
                                Nullable = false,
                                MinValue = 0,
                                UnitOfMeasure = UnitOfMeasure.GallonPerMinute
                            }
                        },
                        new FieldSchema
                        {
                            Name = "Additional Rate",
                            CanonicalName = "ADD_RATE",
                            Description = "The additional rate of water for the right",
                            DataType = new DecimalType
                            {
                                Precision = 8,
                                Scale = 2,
                                Nullable = false,
                                MinValue = 0,
                                UnitOfMeasure = UnitOfMeasure.GallonPerMinute
                            }
                        },
                        new FieldSchema
                        {
                            Name = "File ID",
                            CanonicalName = "FILE_ID",
                            Description = "The file ID associated with the water right",
                            DataType = new StringType
                            {
                                Nullable = true,
                            }
                        }
                    ]
                },
                new RecordSetSchema
                {
                    Name = "Special Use Area",
                    CanonicalName = "special-use-area",
                    AdditionalProperties = new JsonObject(),
                    Description = "Special usage areas associated to the well.",
                    MinCount = 0,
                    MaxCount = null,
                    FieldSchemata =
                    [
                        new FieldSchema
                        {
                            Name = "Special Use Area Code",
                            CanonicalName = "SUA_CODE",
                            Description = "",
                            DataType = new StringType
                            {
                                Nullable = false,
                                MaxLength = 3,
                            },
                        }
                    ]
                }
            ]
        };

        var errors = wellTypeSchema.ValidateSchemaErrors(new JsonObject());
        foreach (var error in errors)
        {
            Console.WriteLine(error);
        }

        Assert.AreEqual(0, errors.Count);

        var toJson = JsonSerializer.Serialize(wellTypeSchema, AssemblySteps.DefaultJsonSerializerOptions);
        Console.WriteLine($"Schema:\n{toJson}");

        var exampleInstance = new Instance()
        {
            RecordSets =
            [
                new RecordSet()
                {
                    CanonicalName = "water-rights",
                    Records =
                    [
                        new Record(Guid.NewGuid().ToString())
                        {
                            RecordInstance = new RecordInstance()
                            {
                                Fields =
                                [
                                    new FieldInstance("WR_NUM", "7193"),
                                    new FieldInstance("PRIORITY", DateTime.UtcNow.ToString("d")),
                                    new FieldInstance("AUTH_QUANT", 1000.00),
                                    new FieldInstance("ADD_QUANT", 133.00),
                                    new FieldInstance("AUTH_RATE", 1200.00),
                                    new FieldInstance("ADD_RATE", 1200.00),
                                ]
                            }
                        },
                    ]
                },
                new RecordSet()
                {
                    CanonicalName = "special-use-area",
                    Records =
                    [
                        new Record(Guid.NewGuid().ToString())
                        {
                            RecordInstance = new RecordInstance()
                            {
                                Fields =
                                [
                                    new FieldInstance("SUA_CODE", "016")
                                ]
                            }
                        },
                        new Record(Guid.NewGuid().ToString())
                        {
                            RecordInstance = new RecordInstance()
                            {
                                Fields =
                                [
                                    new FieldInstance("SUA_CODE", "025")
                                ]
                            }
                        }
                    ]
                }
            ]
        };

        exampleInstance.ValidateInstance(wellTypeSchema, new JsonObject());
        Assert.IsTrue(exampleInstance.ValidationMessages.Count == 0, string.Join(", ", exampleInstance.ValidationMessages));

        var instanceJson = JsonSerializer.Serialize(exampleInstance, AssemblySteps.DefaultJsonSerializerOptions);
        Console.WriteLine($"\nInstance:\n{instanceJson}");
    }

    [DataRow("C:\\Users\\Mikey\\Documents\\gardencitycowells3.csv")]
    [TestMethod]
    public async Task ParseGMD3WellCSV(string filePath)
    {
        Console.WriteLine("Keeping this around for example, but making it no-op for the CI.");
        //var wellType = await AssemblySteps.QanatDbContext.WellTypes.FirstOrDefaultAsync(x => x.GeographyID == 10);

        //var schemotoSchema = JsonSerializer.Deserialize<Schema>(wellType.SchemotoSchema);
        //Assert.IsNotNull(schemotoSchema);

        ////Read CSV from file path.
        //var csvContent = await File.ReadAllBytesAsync(filePath);

        //using var memoryStream = new MemoryStream(csvContent);
        //using var reader = new StreamReader(memoryStream);
        //using var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture);

        //csvReader.Context.RegisterClassMap<GMD3WellCSVMap>();

        //var csvData = csvReader.GetRecords<GMD3WellDto>().ToList();
        ////Group records by PDIV_ID, X, Y, POINT_X, POINT_Y
        //var wellRows = csvData.GroupBy(r => new
        //    {
        //        r.PDIV_ID,
        //        r.X,
        //        r.Y,
        //        r.POINT_X,
        //        r.POINT_Y
        //    })
        //    .Select(g => new
        //    {
        //        g.Key.PDIV_ID,
        //        g.Key.X,
        //        g.Key.Y,
        //        g.Key.POINT_X,
        //        g.Key.POINT_Y,
        //        Records = g.ToList()
        //    }).ToList();

        //var wellMigrationStrings = new List<string>();

        //var insertStatement = $"INSERT INTO dbo.Well([GeographyID], [WellTypeID], [WellName], [LocationPoint], [LocationPoint4326], [SchemotoInstance], [CreateDate], [ParcelIsManualOverride])\nVALUES";
        //var valueTemplate = "(10, 10, {0}, {1}, {2}, {3}, GETUTCDATE(), 0)";
        //foreach (var wellRow in wellRows)
        //{
        //    var pdivId = wellRow.PDIV_ID;
        //    var records = wellRow.Records;

        //    var wellInstance = new Instance();
        //    var WR_NUMs = records.Select(x => x.WR_NUM).Distinct();

        //    var wrRecords = new List<Record>();
        //    foreach (var wrNuM in WR_NUMs)
        //    {
        //        var wrNumRecord = records.FirstOrDefault(x => x.WR_NUM == wrNuM);
        //        wrRecords.Add(new Record(Guid.NewGuid().ToString())
        //        {
        //            RecordInstance = new RecordInstance()
        //            {
        //                Fields =
        //                [
        //                    new FieldInstance(GMD3WellCSVMap.WR_NUM, wrNuM),
        //                    new FieldInstance(GMD3WellCSVMap.PRIORITY, wrNumRecord?.PRIORITY),
        //                    new FieldInstance(GMD3WellCSVMap.AUTH_QUANT, Math.Round(wrNumRecord?.AUTH_QUANT ?? 0, 2, MidpointRounding.ToEven)),
        //                    new FieldInstance(GMD3WellCSVMap.ADD_QUANT, Math.Round(wrNumRecord?.ADD_QUANT ?? 0, 2, MidpointRounding.ToEven)),
        //                    new FieldInstance(GMD3WellCSVMap.AUTH_RATE, Math.Round(wrNumRecord ?.AUTH_RATE ?? 0, 2, MidpointRounding.ToEven)),
        //                    new FieldInstance(GMD3WellCSVMap.ADD_RATE, Math.Round(wrNumRecord ?.ADD_RATE ?? 0, 2, MidpointRounding.ToEven)),
        //                    new FieldInstance(GMD3WellCSVMap.FILE_ID, wrNumRecord?.FILE_ID)
        //                ]
        //            }
        //        });
        //    }

        //    wellInstance.RecordSets.Add(new RecordSet()
        //    {
        //        CanonicalName = "water-rights",
        //        Records = wrRecords
        //    });

        //    var SUA_CODEs = records.Select(x => x.SUA_CODE).Where(x => !string.IsNullOrEmpty(x)).Distinct();
        //    var suaRecords = new List<Record>();
        //    foreach (var suaCode in SUA_CODEs)
        //    {
        //        suaRecords.Add(new Record(Guid.NewGuid().ToString())
        //        {
        //            RecordInstance = new RecordInstance()
        //            {
        //                Fields =
        //                [
        //                    new FieldInstance(GMD3WellCSVMap.SUA_CODE, suaCode)
        //                ]
        //            }
        //        });
        //    }
        //    wellInstance.RecordSets.Add(new RecordSet()
        //    {
        //        CanonicalName = "special-use-area",
        //        Records = suaRecords
        //    });

        //    wellInstance.ValidateInstance(schemotoSchema, new JsonObject());

        //    Assert.IsTrue(wellInstance.ValidationMessages.Count == 0, $"Validation failed for PDIV_ID {pdivId}: {string.Join(", ", wellInstance.ValidationMessages)}\n");

        //    var instanceJson = JsonSerializer.Serialize(wellInstance, new JsonSerializerOptions(){WriteIndented = false});
        //    wellMigrationStrings.Add(string.Format(valueTemplate, pdivId, $"(select geometry::STGeomFromText('POINT ({wellRow.POINT_X} {wellRow.POINT_Y})', 3420))", $"(select geometry::STGeomFromText('POINT ({wellRow.X} {wellRow.Y})', 4326))", $"'{instanceJson}'"));
        //}
        
        //Console.WriteLine(insertStatement);
        //Console.WriteLine(string.Join(",\n", wellMigrationStrings));
    }
}

public class GMD3WellCSVMap : ClassMap<GMD3WellDto>
{
    public static string PDIV_ID = "PDIV_ID";
    public static string WR_NUM = "WR_NUM";
    public static string PRIORITY = "PRIORITY";
    public static string AUTH_QUANT = "AUTH_QUANT";
    public static string ADD_QUANT = "ADD_QUANT";
    public static string AUTH_RATE = "AUTH_RATE";
    public static string ADD_RATE = "ADD_RATE";
    public static string FILE_ID = "FILE_ID";
    public static string SUA_CODE = "SUA_CODE";
    public static string X = "x"; //4326 projection
    public static string Y = "y"; //4326 projection
    public static string POINT_X = "3420x"; //Projected 3420 coordinate
    public static string POINT_Y = "3420y"; //Projected 3420 coordinate

    public GMD3WellCSVMap()
    {
        Map(m => m.PDIV_ID).Name(PDIV_ID);
        Map(m => m.WR_NUM).Name(WR_NUM);
        Map(m => m.PRIORITY).Name(PRIORITY);
        Map(m => m.AUTH_QUANT).Name(AUTH_QUANT);
        Map(m => m.ADD_QUANT).Name(ADD_QUANT);
        Map(m => m.AUTH_RATE).Name(AUTH_RATE);
        Map(m => m.ADD_RATE).Name(ADD_RATE);
        Map(m => m.FILE_ID).Name(FILE_ID);
        Map(m => m.SUA_CODE).Name(SUA_CODE);
        Map(m => m.X).Name(X);
        Map(m => m.Y).Name(Y);
        Map(m => m.POINT_X).Name(POINT_X);
        Map(m => m.POINT_Y).Name(POINT_Y);
    }
}

public class GMD3WellDto
{
    public string PDIV_ID { get; set; }
    public int WR_NUM { get; set; }
    public string PRIORITY { get; set; }
    public decimal? AUTH_QUANT { get; set; }
    public decimal? ADD_QUANT { get; set; }
    public decimal? AUTH_RATE { get; set; }
    public decimal? ADD_RATE { get; set; }
    public string FILE_ID { get; set; }
    public string SUA_CODE { get; set; }
    public decimal X { get; set; }
    public decimal Y { get; set; }
    public decimal POINT_X { get; set; }
    public decimal POINT_Y { get; set; }
}