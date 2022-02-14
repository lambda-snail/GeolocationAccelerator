using Newtonsoft.Json;

namespace Accelerator.GeoLocation.Models;

[JsonArray(AllowNullItems = false)]
public class CoordinatePair
{
    /// <summary>
    /// Index of longitude in tuples and lists with two elements.
    /// </summary>
    public const int LongitudeIndex = 0;

    /// <summary>
    /// Index of latitude in tuples and lists with two elements.
    /// </summary>
    public const int LatitudeIndex = 1;

    public double Longitude { get; set; }
    public double Latitude { get; set; }
}