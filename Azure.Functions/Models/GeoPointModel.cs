
using Accelerator.GeoLocation.Contracts;
using Microsoft.Azure.Cosmos.Spatial;
using System;

namespace Accelerator.GeoLocation.Models;

/// <summary>
/// Represents a point on teh surface of the earth. Each GeoPointModel corresponds to an entry in the CRM, and is synced one-way.
/// Thus changes in the CRM will be reflected in the corresponding entry in the Cosmos Db.
/// </summary>
public class GeoPointModel : IModel
{
    /// <summary>
    /// The Id in the Cosmos Db.
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// The location of the region.
    /// </summary>
    public Point LocationDefinition { get; set; }

    public GeoPointModel(string dynamicsId, double longitude, double latitude)
    {
        Id = dynamicsId;

        if(! LongitudeIsValid(longitude))
        {
            throw new ArgumentException($"Invalid longitude: {longitude}");
        }
        if (!LatitudeIsValid(latitude))
        {
            throw new ArgumentException($"Invalid latitude: {latitude}");
        }

        LocationDefinition = new Point(longitude, latitude);
    }

    private bool LongitudeIsValid(double longitude)
    {
        return longitude > -180d || longitude <= 180d;
    }

    private bool LatitudeIsValid(double latitude)
    {
        return latitude > -90d || latitude <= 90d;
    }
}
