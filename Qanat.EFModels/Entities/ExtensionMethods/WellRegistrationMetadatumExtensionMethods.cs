using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities
{
    public static class WellRegistrationMetadatumExtensionMethods
    {
        public static WellRegistrationMetadatumSimpleDto AsSimpleDto(this WellRegistrationMetadatum wellRegistrationMetadatum)
        {
            var dto = new WellRegistrationMetadatumSimpleDto()
            {
                WellRegistrationMetadatumID = wellRegistrationMetadatum.WellRegistrationMetadatumID,
                WellRegistrationID = wellRegistrationMetadatum.WellRegistrationID,
                WellName = wellRegistrationMetadatum.WellName,
                StateWellNumber = wellRegistrationMetadatum.StateWellNumber,
                StateWellCompletionNumber = wellRegistrationMetadatum.StateWellCompletionNumber,
                CountyWellPermit = wellRegistrationMetadatum.CountyWellPermit,
                DateDrilled = wellRegistrationMetadatum.DateDrilled,
                WellDepth = wellRegistrationMetadatum.WellDepth,
                CasingDiameter = wellRegistrationMetadatum.CasingDiameter,
                TopOfPerforations = wellRegistrationMetadatum.TopOfPerforations,
                BottomOfPerforations = wellRegistrationMetadatum.BottomOfPerforations,
                ManufacturerOfWaterMeter = wellRegistrationMetadatum.ManufacturerOfWaterMeter,
                SerialNumberOfWaterMeter = wellRegistrationMetadatum.SerialNumberOfWaterMeter,
                ElectricMeterNumber = wellRegistrationMetadatum.ElectricMeterNumber,
                PumpDischargeDiameter = wellRegistrationMetadatum.PumpDischargeDiameter,
                MotorHorsePower = wellRegistrationMetadatum.MotorHorsePower,
                FuelTypeID = wellRegistrationMetadatum.FuelTypeID,
                FuelOther = wellRegistrationMetadatum.FuelOther,
                MaximumFlow = wellRegistrationMetadatum.MaximumFlow,
                IsEstimatedMax = wellRegistrationMetadatum.IsEstimatedMax,
                TypicalPumpFlow = wellRegistrationMetadatum.TypicalPumpFlow,
                IsEstimatedTypical = wellRegistrationMetadatum.IsEstimatedTypical,
                PumpTestBy = wellRegistrationMetadatum.PumpTestBy,
                PumpTestDatePerformed = wellRegistrationMetadatum.PumpTestDatePerformed,
                PumpManufacturer = wellRegistrationMetadatum.PumpManufacturer,
                PumpYield = wellRegistrationMetadatum.PumpYield,
                PumpStaticLevel = wellRegistrationMetadatum.PumpStaticLevel,
                PumpingLevel = wellRegistrationMetadatum.PumpingLevel
            };
            return dto;
        }
    }
}