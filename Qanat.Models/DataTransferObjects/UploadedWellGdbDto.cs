using System;

namespace Qanat.Models.DataTransferObjects;

public class UploadedWellGdbDto
{
    public int UploadedWellGdbID { get; set; }
    public UserDto User { get; set; }
    public GeographySimpleDto Geography { get; set; }
    public string CanonicalName { get; set; }
    public DateTime UploadDate { get; set; }
    public DateTime? EffectiveDate { get; set; }
    public bool Finalized { get; set; }
    public int SRID { get; set; }
}