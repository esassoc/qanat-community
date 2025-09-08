using System.ComponentModel.DataAnnotations;

namespace Qanat.Models.DataTransferObjects;

public class WellRegistrationContactsUpsertDto
{
    public bool LandownerSameAsOwnerOperator { get; set; }
    public int LandownerWellRegistrationContactID { get; set; }
    public int LandownerWellRegistrationID { get; set; }
    public int LandownerWellRegistrationContactTypeID { get; set; }
    [Required(ErrorMessage = "Landowner contact name is a required field.")]
    public string LandownerContactName { get; set; }
    public string LandownerBusinessName { get; set; }
    [Required(ErrorMessage = "Landowner street address is a required field.")]
    public string LandownerStreetAddress { get; set; }
    [Required(ErrorMessage = "Landowner city is a required field.")]
    public string LandownerCity { get; set; }
    [Required(ErrorMessage = "Landowner state is a required field.")]
    public int? LandownerStateID { get; set; }
    [Required(ErrorMessage = "Landowner zip code is a required field.")]
    public string LandownerZipCode { get; set; }
    [Required(ErrorMessage = "Landowner phone is a required field.")]
    public string LandownerPhone { get; set; }
    [Required(ErrorMessage = "Landowner email is a required field.")]
    public string LandownerEmail { get; set; }

    public int OwnerOperatorWellRegistrationContactID { get; set; }
    public int OwnerOperatorWellRegistrationID { get; set; }
    public int OwnerOperatorWellRegistrationContactTypeID { get; set; }
    [Required(ErrorMessage = "Operator contact name is a required field.")]
    public string OwnerOperatorContactName { get; set; }
    public string OwnerOperatorBusinessName { get; set; }
    [Required(ErrorMessage = "Operator street address is a required field.")]
    public string OwnerOperatorStreetAddress { get; set; }
    [Required(ErrorMessage = "Operator city is a required field.")]
    public string OwnerOperatorCity { get; set; }
    [Required(ErrorMessage = "Operator state is a required field.")]
    public int? OwnerOperatorStateID { get; set; }
    [Required(ErrorMessage = "Operator zip code is a required field.")]
    public string OwnerOperatorZipCode { get; set; }
    [Required(ErrorMessage = "Operator phone is a required field.")]
    public string OwnerOperatorPhone { get; set; }
    [Required(ErrorMessage = "Operator email is a required field.")]
    public string OwnerOperatorEmail { get; set; }
}