using Accelerator.GeoLocation.Contracts;
using Accelerator.GeoLocation.Models;
using Microsoft.Azure.Cosmos;
using System;
using System.Net;
using System.Threading.Tasks;

using static Accelerator.GeoLocation.Contracts.ICosmosDbRegionService;

namespace Accelerator.GeoLocation.Services;

public class CosmosDbRegionService : ICosmosDbRegionService
{
    private Container _regionContainer;

    public CosmosDbRegionService(CosmosClient client)
    {
        _regionContainer = client.GetContainer(Environment.GetEnvironmentVariable("CosmosDbName"),
                            Environment.GetEnvironmentVariable("CosmosDbCollectionName"));
    }

    public async Task<RegionQueryResponse> UpsertRegion(GeoRegionModel region)
    {
        ItemResponse<GeoRegionModel> response = await _regionContainer.UpsertItemAsync(region, new PartitionKey(region.Id));

        if(response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.Created)
        {
            return new RegionQueryResponse(response.Resource, true);
        }

        return new RegionQueryResponse(null, false);
    }

    public async Task<RegionQueryResponse> GetRegion(string id)
    {
        ItemResponse<GeoRegionModel> response = await _regionContainer.ReadItemAsync<GeoRegionModel>(id, new PartitionKey(id));

        if (response.StatusCode == HttpStatusCode.OK)
        {
            return new RegionQueryResponse(response.Resource, true);
        }

        return new RegionQueryResponse(null, false);
    }
}
