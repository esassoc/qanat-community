using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Qanat.EFModels.Entities;

[Table("WaterMeasurementSelfReportFileResource")]
public partial class WaterMeasurementSelfReportFileResource
{
    [Key]
    public int WaterMeasurementSelfReportFileResourceID { get; set; }

    public int WaterMeasurementSelfReportID { get; set; }

    public int FileResourceID { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string FileDescription { get; set; }

    [ForeignKey("FileResourceID")]
    [InverseProperty("WaterMeasurementSelfReportFileResources")]
    public virtual FileResource FileResource { get; set; }

    [ForeignKey("WaterMeasurementSelfReportID")]
    [InverseProperty("WaterMeasurementSelfReportFileResources")]
    public virtual WaterMeasurementSelfReport WaterMeasurementSelfReport { get; set; }
}
