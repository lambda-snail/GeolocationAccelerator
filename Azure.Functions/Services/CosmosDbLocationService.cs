using Accelerator.GeoLocation.Contracts;
using Accelerator.GeoLocation.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Accelerator.GeoLocation.Services;

public class CosmosDbLocationService : CosmosDbServiceBase<GeoPointModel>, ICosmosDbLocationService
{
    public CosmosDbLocationService(CosmosClient client) : base(client)
    { }
}
