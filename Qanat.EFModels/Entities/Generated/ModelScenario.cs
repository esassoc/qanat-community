using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Qanat.EFModels.Entities;

[Table("ModelScenario")]
public partial class ModelScenario
{
    [Key]
    public int ModelScenarioID { get; set; }

    public int ModelID { get; set; }

    public int ScenarioID { get; set; }

    public int GETScenarioID { get; set; }
}
