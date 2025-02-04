using Qanat.Models.DataTransferObjects.Geography;

namespace Qanat.Models.DataTransferObjects;

public class FeeCalculatorInputOptionsDto
{
    //MK 10/23/2024 -- Geography is just included here for display convenience.
    public GeographyPublicDto Geography { get; set; }

    public List<int> FilterGridToWaterMeasurementTypeIDs => [ET_WaterMeasurementTypeID, Precip_WaterMeasurementTypeID];

    public List<WaterAccountMinimalDto> WaterAccounts { get; set; }
    public List<FeeCalculatorYearDto> Years { get; set; }
    public List<FeeStructureDto> FeeStructures { get; set; }
    public List<MLRPIncentiveDto> MLRPIncentives { get; set; }

    //MK 10/24/2024 -- Going to try prefilling the options and outputs so the page will have everything it needs to load in one go initially. Might need to undo this and make it a chained call. 
    public FeeCalculatorInputDto InitialInputs { get; set; }
    public FeeCalculatorOutputDto InitialOutput { get; set; }

    private static int ET_WaterMeasurementTypeID => 21; //21 ==LandIQ ET for ETSGSA, need to generalize this later.
    private static int Precip_WaterMeasurementTypeID => 22; //22 ==LandIQ Precip for ETSGSA, need to generalize this later.
}