using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace Qanat.EFModels.Entities;

[Table("WellRegistration")]
public partial class WellRegistration
{
    [Key]
    public int WellRegistrationID { get; set; }

    public int GeographyID { get; set; }

    public int? WellID { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string WellName { get; set; }

    [Column(TypeName = "geometry")]
    public Geometry LocationPoint { get; set; }

    [Column(TypeName = "geometry")]
    public Geometry LocationPoint4326 { get; set; }

    public int WellRegistrationStatusID { get; set; }

    public int? ParcelID { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string StateWCRNumber { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string CountyWellPermitNumber { get; set; }

    public DateOnly? DateDrilled { get; set; }

    public int? WellDepth { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? SubmitDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ApprovalDate { get; set; }

    public int? CreateUserID { get; set; }

    public Guid? CreateUserGuid { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string CreateUserEmail { get; set; }

    public int? ReferenceWellID { get; set; }

    public int? FairyshrimpWellID { get; set; }

    public bool ConfirmedWellLocation { get; set; }

    public bool SelectedIrrigatedParcels { get; set; }

    [ForeignKey("CreateUserID")]
    [InverseProperty("WellRegistrations")]
    public virtual User CreateUser { get; set; }

    [ForeignKey("GeographyID")]
    [InverseProperty("WellRegistrations")]
    public virtual Geography Geography { get; set; }

    [ForeignKey("ParcelID")]
    [InverseProperty("WellRegistrations")]
    public virtual Parcel Parcel { get; set; }

    [ForeignKey("ReferenceWellID")]
    [InverseProperty("WellRegistrations")]
    public virtual ReferenceWell ReferenceWell { get; set; }

    [ForeignKey("WellID")]
    [InverseProperty("WellRegistrations")]
    public virtual Well Well { get; set; }

    [InverseProperty("WellRegistration")]
    public virtual ICollection<WellRegistrationContact> WellRegistrationContacts { get; set; } = new List<WellRegistrationContact>();

    [InverseProperty("WellRegistration")]
    public virtual ICollection<WellRegistrationFileResource> WellRegistrationFileResources { get; set; } = new List<WellRegistrationFileResource>();

    [InverseProperty("WellRegistration")]
    public virtual ICollection<WellRegistrationIrrigatedParcel> WellRegistrationIrrigatedParcels { get; set; } = new List<WellRegistrationIrrigatedParcel>();

    [InverseProperty("WellRegistration")]
    public virtual WellRegistrationMetadatum WellRegistrationMetadatum { get; set; }

    [InverseProperty("WellRegistration")]
    public virtual ICollection<WellRegistrationWaterUse> WellRegistrationWaterUses { get; set; } = new List<WellRegistrationWaterUse>();
}
