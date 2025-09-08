using System;

namespace Qanat.Swagger.Models;

public class WaterAccountConsumerDto
{
    public int WaterAccountID { get; set; }
    public int WaterAccountNumber { get; set; }
    public string WaterAccountName { get; set; }
    public string Notes { get; set; }
    public string WaterAccountPIN { get; set; }
    public DateTime? WaterAccountPINLastUsed { get; set; }
    public string WaterAccountContactName { get; set; }
    public string ContactEmail { get; set; }
    public string ContactPhoneNumber { get; set; }
    public string FullAddress { get; set; }
    public int GeographyID { get; set; }
}