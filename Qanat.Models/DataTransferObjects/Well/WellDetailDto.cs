using Qanat.Models.DataTransferObjects.Geography;
using Schemoto.InstanceNamespace;

namespace Qanat.Models.DataTransferObjects;

public class WellDetailDto
{
    public int WellID { get; set; }
    public string WellName { get; set; }
    public WellTypeMinimalDto WellType { get; set; }
    public WellStatusSimpleDto WellStatus { get; set; }
    public ParcelMinimalDto Parcel { get; set; }
    public List<ParcelDisplayDto> IrrigatedParcels { get; set; }
    public WellRegistrationMinimalDto WellRegistration { get; set; }
    public GeographyDisplayDto Geography { get; set; }
    public MeterGridDto Meter { get; set; }
    public string StateWCRNumber { get; set; }
    public string CountyWellPermitNumber { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public DateOnly? DateDrilled { get; set; }
    public int? WellDepth { get; set; }
    public bool MetersEnabled { get; set; }
    public string Notes { get; set; }
    public int? CasingDiameter { get; set; }
    public int? TopOfPerforations { get; set; }
    public int? BottomOfPerforations { get; set; }
    public string ElectricMeterNumber { get; set; }
}

public class WellInstanceDto
{
    public int WellID { get; set; }
    public string WellName { get; set; }
    public Instance SchemotoInstance { get; set; }
}