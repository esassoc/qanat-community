using System.Collections.Generic;

namespace Qanat.API.Services.GET;

public class UserDataJsonFile
{
    public string Name { get; set; }
    public List<PivotedRunWellInput> PivotedRunWellInputs { get; set; }

    public class PivotedRunWellInput
    {
        public double Lat { get; set; }
        public double Lng { get; set; }
        public string Name { get; set; }
        public double? AverageValue { get; set; }

    }
}