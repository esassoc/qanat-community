using System;
using System.Collections.Generic;

namespace Qanat.Models.DataTransferObjects;

public partial class WaterAccountDto
{
    public int WaterAccountID { get; set; }
    public GeographySimpleDto Geography { get; set; }
    public int WaterAccountNumber { get; set; }
    public string WaterAccountName { get; set; }
    public string Notes { get; set; }
    public DateTime? UpdateDate { get; set; }
    public string WaterAccountPIN { get; set; }
    public DateTime? WaterAccountPINLastUsed { get; set; }
    public DateTime CreateDate { get; set; }
    public string ContactName { get; set; }
    public string ContactAddress { get; set; }


    public List<WaterAccountUserMinimalDto> Users { get; set; }
    public List<ParcelDisplayDto> Parcels { get; set; }
    public string WaterAccountNameAndNumber { get; set; }
}

public class WaterAccountSearchSummaryDto
{
    public int TotalResults { get; set; }
    public List<WaterAccountSearchResultWithMatchedFieldsDto> WaterAccountSearchResults { get; set; }
}

public class WaterAccountSearchResultWithMatchedFieldsDto
{
    public WaterAccountSearchResultDto WaterAccount { get; set; }
    public Dictionary<WaterAccountSearchMatchEnum,bool> MatchedFields { get; set; }
}

public class WaterAccountSearchResultDto
{
    public int WaterAccountID { get; set; }
    public int WaterAccountNumber { get; set; }
    public string WaterAccountName { get; set; }
    public string Notes { get; set; }
    public DateTime? UpdateDate { get; set; }
    public string WaterAccountPIN { get; set; }
    public DateTime CreateDate { get; set; }
    public string ContactName { get; set; }
    public string ContactAddress { get; set; }
    public List<WaterAccountUserMinimalDto> Users { get; set; }
    public List<ParcelDisplayDto> Parcels { get; set; }
    public string WaterAccountNameAndNumber { get; set; }
}

public enum WaterAccountSearchMatchEnum
{
    WaterAccountName = 1,
    ContactAddress = 2,
    ContactName = 3,
    WaterAccountNumber = 4,
    APN = 5
}