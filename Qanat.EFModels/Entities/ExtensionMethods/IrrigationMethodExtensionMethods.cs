using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities
{
    public static class IrrigationMethodExtensionMethods
    {
        public static IrrigationMethodSimpleDto AsSimpleDto(this IrrigationMethod irrigationMethod)
        {
            var dto = new IrrigationMethodSimpleDto()
            {
                IrrigationMethodID = irrigationMethod.IrrigationMethodID,
                GeographyID = irrigationMethod.GeographyID,
                Name = irrigationMethod.Name,
                SystemType = irrigationMethod.SystemType,
                EfficiencyAsPercentage = irrigationMethod.EfficiencyAsPercentage,
                DisplayOrder = irrigationMethod.DisplayOrder
            };
            return dto;
        }
    }
}