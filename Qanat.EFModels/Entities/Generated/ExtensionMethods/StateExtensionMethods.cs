//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[State]

using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities
{
    public static partial class StateExtensionMethods
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