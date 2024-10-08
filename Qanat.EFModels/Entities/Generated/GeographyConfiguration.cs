using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Qanat.EFModels.Entities;

[Table("GeographyConfiguration")]
public partial class GeographyConfiguration
{
    [Key]
    public int GeographyConfigurationID { get; set; }

    public bool WellRegistryEnabled { get; set; }

    public bool LandingPageEnabled { get; set; }

    public bool MetersEnabled { get; set; }

    public bool ZonePrecipMultipliersEnabled { get; set; }

    [InverseProperty("GeographyConfiguration")]
    public virtual Geography Geography { get; set; }
}
