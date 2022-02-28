using Accelerator.GeoLocation.Contracts;
using Accelerator.GeoLocation.Models;
using Accelerator.GeoLocation.Services;
using AutoMapper;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;

[assembly: FunctionsStartup(typeof(Accelerator.GeoLocation.Startup))]

namespace Accelerator.GeoLocation
{
    /// <summary>
    /// Configures services and dependency injection.
    /// </summary>
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            InitAutoMapper(builder);
            InitCosmosDbServices(builder);
        }

        private void InitAutoMapper(IFunctionsHostBuilder builder)
        {
            MapperConfiguration mapperConfig = new MapperConfiguration(mapperconfig =>
            {
                mapperconfig.AddProfile(new AutoMapperProfiles());
            });

            IMapper mapper = mapperConfig.CreateMapper();
            builder.Services.AddSingleton(mapper);
        }

        private void InitCosmosDbServices(IFunctionsHostBuilder builder)
        {
            string connectionString = Environment.GetEnvironmentVariable("CosmosDbConnectionString"); ;
            builder.Services.AddSingleton(
                new CosmosClientBuilder(connectionString)
                    .WithSerializerOptions(new CosmosSerializationOptions
                    {
                        PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
                    })
                .Build());

            builder.Services.AddSingleton(typeof(IGeoQueryable<GeoPointModel>), typeof(CosmosDbServiceBase<GeoPointModel>));
            builder.Services.AddSingleton(typeof(ICosmosDbLocationService), typeof(CosmosDbLocationService));
            builder.Services.AddSingleton(typeof(ICosmosDbRegionService), typeof(CosmosDbRegionService));
        }
    }
}