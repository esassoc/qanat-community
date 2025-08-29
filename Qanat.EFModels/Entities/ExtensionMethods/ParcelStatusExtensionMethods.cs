using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities
{
    public static class ParcelStatusExtensionMethods
    {
        public static ParcelStatusSimpleDto AsSimpleDto(this ParcelStatus parcelStatus)
        {
            var dto = new ParcelStatusSimpleDto()
            {
                ParcelStatusID = parcelStatus.ParcelStatusID,
                ParcelStatusName = parcelStatus.ParcelStatusName,
                ParcelStatusDisplayName = parcelStatus.ParcelStatusDisplayName
            };
            return dto;
        }
    }
}