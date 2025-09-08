namespace Qanat.Models.DataTransferObjects;

public class HangfireBackgroundJobResultDto
{
    public string BackgroundJobID { get; set; }
    public string ContinuedWithJobID { get; set; }
}