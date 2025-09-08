using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Qanat.Models.DataTransferObjects;

public class GeographyConfigurationDto
{
    public int GeographyID { get; set; }
    public string GeographySlug { get; set; }
    public bool WellRegistryEnabled { get; set; }
    public bool LandingPageEnabled { get; set; }
}