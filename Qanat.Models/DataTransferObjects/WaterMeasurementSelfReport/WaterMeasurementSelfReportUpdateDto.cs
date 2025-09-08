using System.ComponentModel.DataAnnotations;

namespace Qanat.Models.DataTransferObjects;

public class WaterMeasurementSelfReportUpdateDto
{
    [Required, MinLength(1)]
    public List<WaterMeasurementSelfReportLineItemUpdateDto> LineItems { get; set; }

}

public class WaterMeasurementSelfReportLineItemUpdateDto
{
    [Required]
    public int ParcelID { get; set; }

    [Required]
    public int IrrigationMethodID { get; set; }

    public decimal? JanuaryOverrideValueInAcreFeet { get; set; }
    public decimal? FebruaryOverrideValueInAcreFeet { get; set; }
    public decimal? MarchOverrideValueInAcreFeet { get; set; }
    public decimal? AprilOverrideValueInAcreFeet { get; set; }
    public decimal? MayOverrideValueInAcreFeet { get; set; }
    public decimal? JuneOverrideValueInAcreFeet { get; set; }
    public decimal? JulyOverrideValueInAcreFeet { get; set; }
    public decimal? AugustOverrideValueInAcreFeet { get; set; }
    public decimal? SeptemberOverrideValueInAcreFeet { get; set; }
    public decimal? OctoberOverrideValueInAcreFeet { get; set; }
    public decimal? NovemberOverrideValueInAcreFeet { get; set; }
    public decimal? DecemberOverrideValueInAcreFeet { get; set; }

    public bool HasAnyOverrideValue => JanuaryOverrideValueInAcreFeet.HasValue
                                    || FebruaryOverrideValueInAcreFeet.HasValue
                                    || MarchOverrideValueInAcreFeet.HasValue
                                    || AprilOverrideValueInAcreFeet.HasValue
                                    || MayOverrideValueInAcreFeet.HasValue
                                    || JuneOverrideValueInAcreFeet.HasValue
                                    || JulyOverrideValueInAcreFeet.HasValue
                                    || AugustOverrideValueInAcreFeet.HasValue
                                    || SeptemberOverrideValueInAcreFeet.HasValue
                                    || OctoberOverrideValueInAcreFeet.HasValue
                                    || NovemberOverrideValueInAcreFeet.HasValue
                                    || DecemberOverrideValueInAcreFeet.HasValue;
}