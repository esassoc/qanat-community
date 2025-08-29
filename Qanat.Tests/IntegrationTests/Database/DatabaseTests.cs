using System;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Qanat.EFModels.Entities;
using VerifyMSTest;

namespace Qanat.Tests.IntegrationTests.Database;

[TestClass]
[UsesVerify]
public class DatabaseTests : VerifyBase
{
    private static QanatDbContext _dbContext;

    [TestInitialize]
    public void TestInitialize()
    {
        var dbCS = AssemblySteps.Configuration["sqlConnectionString"];
        _dbContext = new QanatDbContext(dbCS);
    }

    [TestCleanup]
    public async Task TestCleanup()
    {
        await _dbContext.DisposeAsync();
    }

    [TestMethod]
    [Description("This test checks for tables with more than 10000 rows that have missing indices.")]
    public async Task AssureThatThereAreIndicesForTablesWithMoreThan10000Rows()
    {
        var sqlToRun = @"
            select a.*, fks.ForeignKeyName, fks.ParentColumnName, 'IX_' + a.TableName + '_' + fks.ParentColumnName + ' on ' + a.TableSchema + '.' + a.TableName + ' (' + fks.ParentColumnName + ')' as IndexName, 'CREATE INDEX IX_' + a.TableName + '_' + fks.ParentColumnName + ' on ' + a.TableSchema + '.' + a.TableName + '(' + fks.ParentColumnName + ')' as IndexToCreate
            from
            (
                select sc.name as TableSchema, ta.name as TableName, sum(pa.rows) as TableRowCount
                from sys.tables ta
                join sys.partitions pa on pa.object_id = ta.object_id
                join sys.schemas sc on ta.schema_id = sc.schema_id
                where ta.is_ms_shipped = 0 and pa.index_id IN (1,0)
                group by sc.name,ta.name having SUM(pa.rows) > 10000
            ) a
            join
            (
                select fk.name as ForeignKeyName, pt.name as ParentTableName, pt.object_id as ParentTableObjectID, pc.name as ParentColumnName, pc.column_id as ParentColumnID--, rt.name as ReferencedTableName, rc.name as ReferencedColumnName, rc.object_id as ReferencedColumnID
                from sys.foreign_keys fk
                join sys.foreign_key_columns fkc on fk.object_id = fkc.constraint_object_id
                join sys.tables pt on fkc.parent_object_id = pt.object_id
                --join sys.tables rt on fkc.referenced_object_id = rt.object_id
                join sys.columns pc	on pc.object_id = fkc.parent_object_id and pc.column_id = fkc.parent_column_id
                --join sys.columns rc on rc.object_id = fkc.referenced_object_id and rc.column_id = fkc.referenced_column_id
            ) fks on a.TableName = fks.ParentTableName
            left join
            (
                select ind.name as IndexName, ind.index_id as IndexID, ic.index_column_id as IndexColumnID, col.name as ColumnName, col.column_id as ColumnID, t.object_id as TableObjectID
                from sys.indexes ind
                join sys.index_columns ic on ind.object_id = ic.object_id and ind.index_id = ic.index_id
                join sys.columns col on ic.object_id = col.object_id and ic.column_id = col.column_id 
                join sys.tables t on ind.object_id = t.object_id
                where ind.is_primary_key = 0 
                and ind.is_unique = 0 
                and ind.is_unique_constraint = 0
                and t.is_ms_shipped = 0
            ) inds on fks.ParentTableObjectID = inds.TableObjectID and fks.ParentColumnName = inds.ColumnName
            where inds.IndexColumnID is null
            order by a.TableRowCount desc, a.TableSchema, a.TableName, fks.ForeignKeyName, inds.IndexName
        ";
        var tableWithMissingIndices = _dbContext.Database.SqlQueryRaw<TableWithMissingIndex>(sqlToRun).ToList().OrderBy(x => x.TableSchema).ThenBy(x => x.TableName).ThenBy(x => x.ForeignKeyName);
        //Assert.AreEqual(0, tableWithMissingIndices.Count, "There are tables with more than 10000 rows that have missing indices.  Run above query to determine which indices are missing.");

        var tablesAsString = System.Text.Json.JsonSerializer.Serialize(tableWithMissingIndices, AssemblySteps.DefaultJsonSerializerOptions);
        Console.WriteLine(tablesAsString);
        await Verifier.Verify(tablesAsString);
    }

    public class TableWithMissingIndex
    {
        public string TableSchema { get; set; }
        public string TableName { get; set; }
        [JsonIgnore]
        public long TableRowCount { get; set; }
        public string ForeignKeyName { get; set; }
        public string ParentColumnName { get; set; }
        public string IndexName { get; set; }
        public string IndexToCreate { get; set; }
    }
}