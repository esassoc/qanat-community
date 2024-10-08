using System.Diagnostics;

namespace Qanat.Common.Util;

public static class UnitConversionHelper
{
    private const decimal MillimetersToFeetConversionFactor = 304.8m;

    public static decimal ConvertMillimetersToAcreFeet(decimal millimeters, decimal acres)
    {
        return millimeters / MillimetersToFeetConversionFactor * acres;
    }

    public static decimal ConvertAcreFeetToMillimeters(decimal acreFeet, decimal acres)
    {
        return (acreFeet / acres) * MillimetersToFeetConversionFactor;
    }

    public static decimal ConvertInchesToAcreFeet(decimal inches, decimal acres)
    {
        return inches / 12 * acres;
    }

    public static decimal ConvertAcreFeetToInches(decimal acreFeet, decimal acres)
    {
        return (acreFeet / acres) * 12;
    }
}