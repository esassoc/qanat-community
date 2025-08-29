using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities
{
    public static class StateExtensionMethods
    {
        public static StateSimpleDto AsSimpleDto(this State state)
        {
            var dto = new StateSimpleDto()
            {
                StateID = state.StateID,
                StateName = state.StateName,
                StatePostalCode = state.StatePostalCode
            };
            return dto;
        }
    }
}