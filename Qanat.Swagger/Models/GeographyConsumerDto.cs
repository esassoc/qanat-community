using Qanat.Models.DataTransferObjects;
using System;
using System.Collections.Generic;

namespace Qanat.Swagger.Models
{
    public class GeographyConsumerDto
    {
        public int GeographyID { get; set; }
        public string GeographyName { get; set; }
        public string GeographyDisplayName { get; set; }
    }

    public class ParcelConsumerDto
    {
        public int ParcelID { get; set; }
        public string ParcelNumber { get; set; }
        public double ParcelArea { get; set; }
        public string OwnerName { get; set; }
        public string OwnerAddress { get; set; }
        public int? WaterAccountID { get; set; }
        public int GeographyID { get; set; }
    }

    public class WellConsumerDto
    {
        public int WellID { get; set; }
        public string WellName { get; set; }
        public string WellStatus { get; set; }
        public string StateWCRNumber { get; set; }
        public string CountyPermitNumber { get; set; }
        public double? WellDepth { get; set; }
        public DateOnly? DateDrilled { get; set; }
        public int? ParcelID { get; set; }
        public List<int> IrrigatedParcelIDs { get; set; }
        public int GeographyID { get; set; }
    }

    public class ReportingPeriodConsumerDto
    {
        public int ReportingPeriodID { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    public class UsageLocationConsumerDto
    {
        public int UsageLocationID { get; set; }
        public string Name { get; set; }
        public double Area { get; set; }
        public string UsageLocationType { get; set; }
        public int? WaterAccountID { get; set; }
        public int? WaterAccountNumber { get; set; }
        public int ParcelID { get; set; }
        public List<ZoneConsumerDto> ParcelZones { get; set; }
        public int ReportingPeriodID { get; set; }
        public int GeographyID { get; set; }
    }

    public class ZoneConsumerDto
    {
        public int ZoneID { get; set; }
        public string ZoneName { get; set; }   
        public int ZoneGroupID { get; set; }
        public string ZoneGroupName { get; set; }
    }

    public class WaterMeasurementTypeConsumerDto
    {
        public int WaterMeasurementTypeID { get; set; }
        public string WaterMeasurementTypeName { get; set; }
        public string WaterMeasurementCategoryType { get; set; }
        public bool IsActive { get; set; }
        public int GeographyID { get; set; }
    }

    public class WaterMeasurementConsumerDto
    {
        public int WaterMeasurementID { get; set; }
        public int WaterMeasurementTypeID { get; set; }
        public int UsageLocationID { get; set; }
        public DateTime ReportingDate { get; set; }
        public decimal ReportedValueInFeet { get; set; }
        public decimal ReportedValueInAcreFeet { get; set; }
    }

    public class MeterReadingConsumerDto
    {
        public int MeterReadingID { get; set; }
        public string MeterSerialNumber { get; set; }
        public int WellID { get; set; }
        public DateTime ReadingDate { get; set; }
        public decimal PreviousReading { get; set; }
        public decimal CurrentReading { get; set; }
        public decimal VolumeInAcreFeet { get; set; }
        public string ReaderInitials { get; set; }
        public string Comment { get; set; }
    }
}
