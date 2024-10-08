using System;
using System.Security.Cryptography.X509Certificates;

namespace Qanat.EFModels.Entities;

public partial class OpenETSync
{
    public DateTime ReportedDate => new(Year, Month, 1);
}