using Accelerator.GeoLocation.Contracts;
using Accelerator.GeoLocation.Services;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;

[assembly: FunctionsStartup(typeof(Accelerator.GeoLocation.Startup))]

namespace Accelerator.GeoLocation
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            string connectionString = Environment.GetEnvironmentVariable("CosmosDbConnectionString"); ;
            builder.Services.AddSingleton(
                new CosmosClientBuilder(connectionString)
                    .WithSerializerOptions(new CosmosSerializationOptions
                    {
                        PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
                    })
                .Build());

            builder.Services.AddSingleton<ICosmosDbLocationService, CosmosDbLocationService>();

        }
    }
}