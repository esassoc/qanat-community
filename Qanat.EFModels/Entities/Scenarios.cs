using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities;

public static class Scenarios
{
    public static List<ErrorMessage> ValidateAddAWellScenarioRun(AddAWellScenarioDto addAWellScenarioDto)
    {
        var results = new List<ErrorMessage>();
        if (addAWellScenarioDto.ScenarioPumpingWells == null || addAWellScenarioDto.ScenarioPumpingWells.Count == 0)
        {
            results.Add(new ErrorMessage(){Type = "Pumping Wells", Message = "Please provide at least one pumping well."});
        }

        // make sure all the names are unique for both observation points and wells
        if (addAWellScenarioDto.ScenarioPumpingWells != null && addAWellScenarioDto.ScenarioPumpingWells.Any())
        {
            var names = addAWellScenarioDto.ScenarioPumpingWells.Select(x => x.PumpingWellName).ToList();
            if (addAWellScenarioDto.ScenarioObservationPoints != null && addAWellScenarioDto.ScenarioObservationPoints.Any())
            {
                names.AddRange(addAWellScenarioDto.ScenarioObservationPoints.Select(x => x.ObservationPointName));
            }

            if (names.Distinct().Count() < names.Count)
            {
                results.Add(new ErrorMessage() { Type = "Labels", Message = "The labels for pumping wells and observation points must be unique." });
            }
        }

        return results;
    }

    public static List<ErrorMessage> ValidateRechargeScenarioRun(RechargeScenarioDto rechargeScenarioDto)
    {
        var results = new List<ErrorMessage>();
        if (rechargeScenarioDto.ScenarioRechargeSites == null || rechargeScenarioDto.ScenarioRechargeSites.Count == 0)
        {
            results.Add(new ErrorMessage() { Type = "Recharge Site", Message = "Please provide at least one recharge site." });

        }

        // make sure all the names are unique for both observation points and wells
        if (rechargeScenarioDto.ScenarioRechargeSites != null && rechargeScenarioDto.ScenarioRechargeSites.Any())
        {
            var names = rechargeScenarioDto.ScenarioRechargeSites.Select(x => x.RechargeSiteName).ToList();

            if (rechargeScenarioDto.ScenarioObservationPoints != null &&
                rechargeScenarioDto.ScenarioObservationPoints.Any())
            {
                names.AddRange(rechargeScenarioDto.ScenarioObservationPoints.Select(x => x.ObservationPointName));
            }

            if (names.Distinct().Count() < names.Count)
            {
                results.Add(new ErrorMessage() { Type = "Labels", Message = "The labels for recharge sites and observation points must be unique." });
            }
        }

        return results;
    }
}