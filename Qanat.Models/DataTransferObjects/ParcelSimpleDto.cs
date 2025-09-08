namespace Qanat.Models.DataTransferObjects
{
    public class ParcelSimpleDto
    {
        public int ParcelID { get; set; }
        public int GeographyID { get; set; }
        public int? WaterAccountID { get; set; }
        public string ParcelNumber { get; set; }
        public double ParcelArea { get; set; }
        public int ParcelStatusID { get; set; }
        public string OwnerAddress { get; set; }
        public string OwnerName { get; set; }
    }
}