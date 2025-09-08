namespace Qanat.Models.DataTransferObjects;

public class WellRegistrationBasicInfoFormDto
{
    
    public string WellName { get; set; }
    public string StateWellNumber { get; set; }
    public string StateWellCompletionNumber { get; set; }
    public string CountyWellPermit { get; set; }
    public DateOnly? DateDrilled { get; set; }
    public List<WaterUseTypesUsed> WaterUseTypes { get; set; }
    public ReferenceWellSimpleDto ReferenceWell { get; set; }

    public class WaterUseTypesUsed
    {
        public int WaterUseTypeID { get; set;}
        public bool Checked { get; set; }
        public string Description { get; set; }
    }

}