using Microsoft.Azure.Cosmos.Spatial;
using System;

namespace Accelerator.GeoLocation.Models;

/// <summary>
/// Represents a region on the surface of the earth. Each GeoRegionModel corresponds to an entry in the CRM, and is synced one-way.
/// Thus changes in the CRM will be reflected in the corresponding entry in the Cosmos Db.
/// </summary>
public class GeoRegionModel
{
    /// <summary>
    /// The Id in the Cosmos Db.
    /// </summary>
    public Guid RegionId { get; set; }

    /// <summary>
    /// The Id of the corresponding row in the CRM.
    /// </summary>
    public Guid DynamicsId { get; set; }

    /// <summary>
    /// The geometry of the region.
    /// </summary>
    public Polygon AreaDefinition { get; set; }
}
