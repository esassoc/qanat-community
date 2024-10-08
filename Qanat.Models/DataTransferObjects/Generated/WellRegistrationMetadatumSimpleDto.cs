//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[WellRegistrationMetadatum]

namespace Qanat.Models.DataTransferObjects
{
    public partial class WellRegistrationMetadatumSimpleDto
    {
        public int WellRegistrationMetadatumID { get; set; }
        public int WellRegistrationID { get; set; }
        public string WellName { get; set; }
        public string StateWellNumber { get; set; }
        public string StateWellCompletionNumber { get; set; }
        public string CountyWellPermit { get; set; }
        public DateOnly? DateDrilled { get; set; }
        public int? WellDepth { get; set; }
        public int? CasingDiameter { get; set; }
        public int? TopOfPerforations { get; set; }
        public int? BottomOfPerforations { get; set; }
        public string ManufacturerOfWaterMeter { get; set; }
        public string SerialNumberOfWaterMeter { get; set; }
        public string ElectricMeterNumber { get; set; }
        public decimal? PumpDischargeDiameter { get; set; }
        public decimal? MotorHorsePower { get; set; }
        public int? FuelTypeID { get; set; }
        public string FuelOther { get; set; }
        public int? MaximumFlow { get; set; }
        public bool? IsEstimatedMax { get; set; }
        public int? TypicalPumpFlow { get; set; }
        public bool? IsEstimatedTypical { get; set; }
        public string PumpTestBy { get; set; }
        public DateOnly? PumpTestDatePerformed { get; set; }
        public string PumpManufacturer { get; set; }
        public int? PumpYield { get; set; }
        public int? PumpStaticLevel { get; set; }
        public int? PumpingLevel { get; set; }
    }
}