using Microsoft.Azure.Cosmos.Spatial;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Accelerator.GeoLocation.Models;

/// <summary>
/// Represents a region on the surface of the earth. Each GeoRegionModel corresponds to an entry in the CRM, and is synced one-way.
/// Thus changes in the CRM will be reflected in the corresponding entry in the Cosmos Db.
/// </summary>
public class GeoRegionModel
{
    /// <summary>
    /// The Id in the Cosmos Db and Dynamics.
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// The geometry of the region.
    /// </summary>
    public Polygon AreaDefinition { get; set; }

    public GeoRegionModel(string id, List<CoordinatePair> coordinates)
    {
        if(string.IsNullOrWhiteSpace(id))
        {
            throw new ArgumentNullException("Error: Parameter cannot be null or empty: " + nameof(id));
        }

        if(coordinates.Count == 0)
        {
            throw new ArgumentException("Error: No coordinates specified.");
        }
        
        // TODO: Benchmark this!
        IList<Position> positions = coordinates.Select<CoordinatePair, Position>(c => new Position(c.Longitude, c.Latitude)).ToList();

        AreaDefinition = new Polygon(positions);
    }
}
