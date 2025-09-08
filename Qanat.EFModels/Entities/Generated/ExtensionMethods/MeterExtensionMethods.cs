//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[Meter]

using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities
{
    public static partial class MeterExtensionMethods
    {
        public static MeterSimpleDto AsSimpleDto(this Meter meter)
        {
            var dto = new MeterSimpleDto()
            {
                MeterID = meter.MeterID,
                SerialNumber = meter.SerialNumber,
                DeviceName = meter.DeviceName,
                Make = meter.Make,
                ModelNumber = meter.ModelNumber,
                GeographyID = meter.GeographyID,
                MeterStatusID = meter.MeterStatusID
            };
            return dto;
        }
    }
}