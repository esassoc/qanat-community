//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ModelScenario]

using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities
{
    public static partial class ModelScenarioExtensionMethods
    {
        public static ModelScenarioSimpleDto AsSimpleDto(this ModelScenario modelScenario)
        {
            var dto = new ModelScenarioSimpleDto()
            {
                ModelScenarioID = modelScenario.ModelScenarioID,
                ModelID = modelScenario.ModelID,
                ScenarioID = modelScenario.ScenarioID,
                GETScenarioID = modelScenario.GETScenarioID
            };
            return dto;
        }
    }
}