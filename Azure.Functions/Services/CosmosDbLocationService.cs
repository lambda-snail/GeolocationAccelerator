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

    // TODO: Inject logger as well
    public CosmosDbLocationService(CosmosClient client)
    {
        _pointContainer = client.GetContainer(Environment.GetEnvironmentVariable("CosmosDbName"),
                            Environment.GetEnvironmentVariable("CosmosDbCollectionName"));
    }

    public async Task<ICosmosDbLocationService.LocationQueryResponse> UpsertPoint(GeoPointModel point)
    {
        ItemResponse<GeoPointModel> response = await _pointContainer.UpsertItemAsync(point);
        
        if((int)response.StatusCode == 200 || (int)response.StatusCode == 201)
        {
            return new ICosmosDbLocationService.LocationQueryResponse(response.Resource, true);
        }

        return new ICosmosDbLocationService.LocationQueryResponse(null, false);
    }

    public async Task<ICosmosDbLocationService.LocationQueryResponse> GetPoint(string id)
    {
        GeoPointModel point = await _pointContainer.ReadItemAsync<GeoPointModel>(id, new PartitionKey(id));

        if (point != default(GeoPointModel))
        {
            return new ICosmosDbLocationService.LocationQueryResponse(point, true);
        }

        return new ICosmosDbLocationService.LocationQueryResponse(null, false);
    }
}
