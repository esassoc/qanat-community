using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Qanat.Models.DataTransferObjects;

public class RechargeScenarioDto
{
    [Required]
    [DisplayName("Scenario Run Name")]
    public string ScenarioRunName { get; set; }

    public List<ScenarioRechargeSiteDto> ScenarioRechargeSites { get; set; }
    public List<ScenarioObservationPointDto> ScenarioObservationPoints { get; set; }
    public int ModelID { get; set; }
}