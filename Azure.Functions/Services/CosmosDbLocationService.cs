using Accelerator.GeoLocation.Contracts;
using Accelerator.GeoLocation.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Accelerator.GeoLocation.Services;

public class CosmosDbLocationService : ICosmosDbLocationService
{
    private Container _pointContainer;

    public CosmosDbLocationService(CosmosClient client)
    {
        _pointContainer = client.GetContainer(Environment.GetEnvironmentVariable("CosmosDbName"),
                            Environment.GetEnvironmentVariable("CosmosDbCollectionName"));
    }

    public async Task<ICosmosDbLocationService.LocationQueryResponse> CreatePoint(GeoPointModel point)
    {
        ItemResponse<GeoPointModel> response = await _pointContainer.CreateItemAsync(point);
        
        if((int)response.StatusCode == 200 || (int)response.StatusCode == 201)
        {
            return new ICosmosDbLocationService.LocationQueryResponse(response.Resource, true);
        }

        return new ICosmosDbLocationService.LocationQueryResponse(null, false);
    }

    public async Task<ICosmosDbLocationService.MultipleLocationQueryResponse> GetPoints(string dynamicsId)
    {
        FeedIterator<GeoPointModel> pointIterator =
            _pointContainer.GetItemLinqQueryable<GeoPointModel>(false)
                           .Where(p => p.DynamicsId == dynamicsId)
                           .ToFeedIterator();

        List<GeoPointModel> locations = new();
        while(pointIterator.HasMoreResults)
        {
            FeedResponse<GeoPointModel> response = await pointIterator.ReadNextAsync();
            foreach (GeoPointModel point in response)
            {
               locations.Add(point);
            }
        }
        
        if (locations.Count > 0)
        {
            return new ICosmosDbLocationService.MultipleLocationQueryResponse(locations, true);
        }

        return new ICosmosDbLocationService.MultipleLocationQueryResponse(null, false);
    }
}
