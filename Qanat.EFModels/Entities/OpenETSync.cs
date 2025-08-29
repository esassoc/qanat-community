namespace Qanat.EFModels.Entities;

public partial class OpenETSync
{
    public DateTime ReportedDate => new(Year, Month, 1);
}