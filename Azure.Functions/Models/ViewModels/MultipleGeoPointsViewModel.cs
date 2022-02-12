
using System.Collections.Generic;

namespace Accelerator.GeoLocation.Models.ViewModels;
public class MultipleGeoPointsViewModel
{
    public string DynamicsId { get; set; }
    public List<CoordinatePair> Coordinates { get; set; }
}

public struct CoordinatePair
{
    public double Longitude { get; set; }
    public double Latitude { get; set; }
}