using System;

namespace Qanat.Models.DataTransferObjects;

public partial class OpenETSyncDto
{
    public int OpenETSyncID { get; set; }
    public GeographyDto Geography { get; set; }
    public OpenETDataTypeSimpleDto OpenETDataType { get; set; }
    public int Year { get; set; }
    public int Month { get; set; }

    public DateTime? LastSyncDate { get; set; }
    public OpenETSyncResultTypeSimpleDto LastSyncStatus { get; set; }
    public DateTime? LastSuccessfulSyncDate { get; set; }
    public string LastSyncMessage { get; set; }
    public bool HasInProgressSync { get; set; }
    
    public DateTime? LastRasterCalculationDate { get; set; }
    public OpenETRasterCalculationResultTypeSimpleDto LastRasterCalculationStatus { get; set; }
    public DateTime? LastSuccessfulCalculationDate { get; set; }
    public string LastRasterCalculationMessage { get; set; }    
    public string FileResourceGUID { get; set; }
    public string FileResourceOriginalName { get; set; }
    public string FileResourceOriginalFileExtension { get; set; }

    public DateTime? FinalizeDate { get; set; }
}