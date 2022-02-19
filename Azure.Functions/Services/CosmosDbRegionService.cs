using Accelerator.GeoLocation.Contracts;
using Accelerator.GeoLocation.Models;
using Microsoft.Azure.Cosmos;
using System;
using System.Net;
using System.Threading.Tasks;

using static Accelerator.GeoLocation.Contracts.ICosmosDbRegionService;

namespace Accelerator.GeoLocation.Services;

public class CosmosDbRegionService : CosmosDbServiceBase<GeoRegionModel>, ICosmosDbRegionService
{
    public CosmosDbRegionService(CosmosClient client) : base(client)
    { }
}
