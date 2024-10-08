using Qanat.Common.GeoSpatial;
using System.Collections.Generic;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using Newtonsoft.Json;

namespace Qanat.Models.DataTransferObjects;

public class GetParcelsWithGeoJSONRequest
{
    public List<int> ParcelIDs { get; set; }
}

public class ParcelWithGeoJSONDto
{
    public int ParcelID { get; set; }
    public string ParcelNumber { get; set; }
    public double ParcelArea { get; set; }
    public int? WaterAccountID { get; set; }
    public string WaterAccountNameAndNumber { get; set; }
    public string WaterAccountOwnerName { get; set; }
    public string GeoJSON { get; set; }
}