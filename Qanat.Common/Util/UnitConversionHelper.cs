namespace Qanat.Common.Util;

public static class UnitConversionHelper
{
    private const decimal MillimetersToFeetConversionFactor = 304.8m;
    private const decimal GallonsToAcreFeetConversionFactor = 325851m;
    private const decimal GallonsToCubicInchesConversionFactor = 231m;
    private const decimal AcreToSquareFootConversionFactor = 43560m;
    private const decimal SquareFootToSquareInchesConversionFactor = 144m;

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

    public static decimal ConvertAcreFeetToGallons(decimal acreFeet)
    {
        return acreFeet * GallonsToAcreFeetConversionFactor;
    }

    public static decimal ConvertGallonsToAcreFeet(decimal gallons)
    {
        return gallons / GallonsToAcreFeetConversionFactor;
    }

    public static decimal ConvertGallonsToInches(decimal gallons, decimal acres)
    {
        var cubicInches = gallons * GallonsToCubicInchesConversionFactor;
        var squareInches = acres * AcreToSquareFootConversionFactor * SquareFootToSquareInchesConversionFactor;

        var inchesOfDepth = cubicInches / squareInches;
        return inchesOfDepth;
    }
}