using System.Collections.Generic;

namespace Qanat.Models.DataTransferObjects;

public class ParcelBulkUpdateParcelStatusDto
{
    public List<int> ParcelIDs { get; set; }
    public int ParcelStatusID { get; set; }
    public int EndYear { get; set; }
}