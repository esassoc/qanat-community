//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[Scenario]

using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities
{
    public static partial class ScenarioExtensionMethods
    {
        public static ScenarioSimpleDto AsSimpleDto(this Scenario scenario)
        {
            var dto = new ScenarioSimpleDto()
            {
                ScenarioID = scenario.ScenarioID,
                ScenarioName = scenario.ScenarioName,
                ScenarioShortName = scenario.ScenarioShortName,
                ScenarioDescription = scenario.ScenarioDescription,
                ScenarioImage = scenario.ScenarioImage
            };
            return dto;
        }
    }
}