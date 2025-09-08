using System;

namespace Qanat.Models.DataTransferObjects;

public class WellRegistrySupportingInfoDto
{

    public int? WellDepth { get; set; }
    public int? CasingDiameter { get; set; }
    public int? TopOfPerforations { get; set; }
    public int? BottomOfPerforations { get; set; }
    public string ManufacturerOfWaterMeter { get; set; }
    public string SerialNumberOfWaterMeter { get; set; }
    public string ElectricMeterNumber { get; set; }

    // pump details
    public decimal? PumpDischargeDiameter { get; set; }
    public decimal? MotorHorsePower { get; set; }

    public int? FuelTypeID { get; set; }
    public string FuelOther { get; set; }
    public int? MaximumFlow { get; set; }
    public bool? IsEstimatedMax { get; set; }
    public int? TypicalPumpFlow { get; set; }
    public bool? IsEstimatedTypical { get; set; }

    // pump test
    public string PumpTestBy { get; set; }
    public DateOnly? PumpTestDatePerformed { get; set; }
    public string PumpManufacturer { get; set; }
    public int? PumpYield { get; set; }
    public int? PumpStaticLevel { get; set; }
    public int? PumpingLevel { get; set; }

}