using Accelerator.GeoLocation.Contracts;
using Accelerator.GeoLocation.Models;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Accelerator.GeoLocation.Services;

/// <summary>
/// A base class for cosmosDb services that holds generic crud methods. For query methods see specific service
/// implementations.
/// </summary>
/// <typeparam name="T">The type of object to CRUD.</typeparam>
public class CosmosDbServiceBase<T> : IGeoQueryable<T> where T : IModel
{
    private Container _container;

    // TODO: Inject logger as well
    public CosmosDbServiceBase(CosmosClient client)
    {
        _container = client.GetContainer(Environment.GetEnvironmentVariable("CosmosDbName"),
                            Environment.GetEnvironmentVariable("CosmosDbCollectionName"));
    }

    public async Task<GeoQueryResponse<T>> UpsertItem(T item)
    {
        ItemResponse<T> response = await _container.UpsertItemAsync<T>(item, new PartitionKey(item.Id));
       
        if (response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.Created)
        {
            return new GeoQueryResponse<T>(response.Resource, true);
        }

        return new GeoQueryResponse<T>(default, false);
    }

    public async Task<GeoQueryResponse<T>> GetItem(string id)
    {
        try
        {
            ItemResponse<T> response = await _container.ReadItemAsync<T>(id, new PartitionKey(id));

            if (response.StatusCode == HttpStatusCode.OK)
            {
                return new GeoQueryResponse<T>(response.Resource, true);
            }
        }
        catch(CosmosException e) when (e.StatusCode == HttpStatusCode.NotFound) { }
        catch(Exception e)
        {
            // TODO: add logging here
            throw;
        }

        return new GeoQueryResponse<T>(default, false);
    }
}
