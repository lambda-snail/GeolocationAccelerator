
using System.Collections.Generic;

namespace Accelerator.GeoLocation.Models.ViewModels;
public class MultipleGeoPointsViewModel
{
    public string Id { get; set; }
    public List<CoordinatePair> Coordinates { get; set; }
}
