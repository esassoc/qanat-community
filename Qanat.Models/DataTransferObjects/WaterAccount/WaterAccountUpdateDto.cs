using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Qanat.Models.DataTransferObjects;

public class WaterAccountUpdateDto
{
    [MaxLength(255)]
    [DefaultValue("")]
    public string WaterAccountName { get; set; }

    public string Notes { get; set; }
}