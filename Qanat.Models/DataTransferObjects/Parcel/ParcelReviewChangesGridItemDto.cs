namespace Qanat.Models.DataTransferObjects;

public class ParcelChangesGridItemDto
{
    public int ParcelID { get; set; }
    public string ParcelNumber { get; set; }
    public int GeographyID { get; set; }
    public WaterAccountLinkDisplayDto WaterAccount { get; set; }
    public ParcelStatusSimpleDto ParcelStatus { get; set; }
    public bool IsReviewed { get; set; }
    public DateTime? ReviewDate { get; set; }
    public DateTime CurrentParcelHistoryUploadDate { get; set; }
    public DateTime? PreviousParcelHistoryUploadDate { get; set; }

    public List<ParcelFieldDiffDto> ParcelFieldDiffs { get; set; }
}

public class ParcelFieldDiffDto
{
    public string FieldName { get; set; }
    public string FieldShortName { get; set; }
    public string PreviousFieldValue { get; set; }
    public string CurrentFieldValue { get; set; }
}