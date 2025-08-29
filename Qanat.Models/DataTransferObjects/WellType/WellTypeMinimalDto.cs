using Schemoto.SchemaNamespace;

namespace Qanat.Models.DataTransferObjects;

public class WellTypeMinimalDto 
{
    public int WellTypeID { get; set; }
    public string Name { get; set; }
    public bool HasSchomotoSchema { get; set; }
}

public class WellTypeDto
{
    public int WellTypeID { get; set; }
    public string Name { get; set; }
    public Schema SchemotoSchema { get; set; }
}