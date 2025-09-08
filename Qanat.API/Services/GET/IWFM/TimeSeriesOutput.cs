using System;
using System.Collections.Generic;

namespace Qanat.API.Services.GET.IWFM;

public class TimeSeriesOutput
{
    public string Name { get; set; }
    public List<PivotedRunWellInput> PivotedRunWellInputs { get; set; }

    public class PivotedRunWellInput
    {
        public double Lat { get; set; }
        public double Lng { get; set; }
        public string Name { get; set; }
        public double? AverageValue { get; set; }
        public List<UserDataPointTimeStep> TimeSteps { get; set; }

    }
    public class UserDataPointTimeStep
    {
        public DateTime DateTime { get; set; }
        public double BaselineValueDifference { get; set; }
    }
}