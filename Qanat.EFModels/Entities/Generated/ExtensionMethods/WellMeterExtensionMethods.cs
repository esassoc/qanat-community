//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[WellMeter]

using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities
{
    public static partial class WellMeterExtensionMethods
    {
        public static WellMeterSimpleDto AsSimpleDto(this WellMeter wellMeter)
        {
            var dto = new WellMeterSimpleDto()
            {
                WellMeterID = wellMeter.WellMeterID,
                WellID = wellMeter.WellID,
                MeterID = wellMeter.MeterID,
                StartDate = wellMeter.StartDate,
                EndDate = wellMeter.EndDate
            };
            return dto;
        }
    }
}