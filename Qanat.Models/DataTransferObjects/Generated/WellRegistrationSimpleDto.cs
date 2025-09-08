//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[WellRegistration]

namespace Qanat.Models.DataTransferObjects
{
    public partial class WellRegistrationSimpleDto
    {
        public int WellRegistrationID { get; set; }
        public int GeographyID { get; set; }
        public int? WellID { get; set; }
        public string WellName { get; set; }
        public int WellRegistrationStatusID { get; set; }
        public int? ParcelID { get; set; }
        public string StateWCRNumber { get; set; }
        public string CountyWellPermitNumber { get; set; }
        public DateOnly? DateDrilled { get; set; }
        public int? WellDepth { get; set; }
        public DateTime? SubmitDate { get; set; }
        public DateTime? ApprovalDate { get; set; }
        public int? CreateUserID { get; set; }
        public Guid? CreateUserGuid { get; set; }
        public string CreateUserEmail { get; set; }
        public int? ReferenceWellID { get; set; }
        public int? FairyshrimpWellID { get; set; }
        public bool ConfirmedWellLocation { get; set; }
        public bool SelectedIrrigatedParcels { get; set; }
    }
}