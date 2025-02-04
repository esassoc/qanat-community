using Qanat.Models.DataTransferObjects.Geography;

namespace Qanat.Models.DataTransferObjects;

public class WellPopupDto
{
    public int WellID { get; set; }
    public string WellName { get; set; }
    public ParcelSimpleDto Parcel { get; set; }
    public GeographyDisplayDto Geography { get; set; }
    public int? WellRegistrationID { get; set; }
}