namespace Qanat.EFModels.Entities;

public partial class WaterMeasurementSelfReportLineItem
{
    public decimal? TotalAcreFeet
    {
        get
        {
            var total = JanuaryOverrideValueInAcreFeet.GetValueOrDefault(0)
                        + FebruaryOverrideValueInAcreFeet.GetValueOrDefault(0)
                        + MarchOverrideValueInAcreFeet.GetValueOrDefault(0)
                        + AprilOverrideValueInAcreFeet.GetValueOrDefault(0)
                        + MayOverrideValueInAcreFeet.GetValueOrDefault(0)
                        + JuneOverrideValueInAcreFeet.GetValueOrDefault(0)
                        + JulyOverrideValueInAcreFeet.GetValueOrDefault(0)
                        + AugustOverrideValueInAcreFeet.GetValueOrDefault(0)
                        + SeptemberOverrideValueInAcreFeet.GetValueOrDefault(0)
                        + OctoberOverrideValueInAcreFeet.GetValueOrDefault(0)
                        + NovemberOverrideValueInAcreFeet.GetValueOrDefault(0)
                        + DecemberOverrideValueInAcreFeet.GetValueOrDefault(0);

            return total;
        }
    }

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