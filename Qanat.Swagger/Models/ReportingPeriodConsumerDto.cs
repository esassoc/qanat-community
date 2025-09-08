using System;

namespace Qanat.Swagger.Models;

public class ReportingPeriodConsumerDto
{
    public int ReportingPeriodID { get; set; }
    public string Name { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}