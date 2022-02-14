using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Configurations;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;

public class OpenApiConfigurationOptions : DefaultOpenApiConfigurationOptions
{
    public override OpenApiInfo Info { get; set; } = new OpenApiInfo()
    {
        Version = "0.2.0",
        Title = "GeoLocation Accelerator API",
        Description = "Provides a simple API for making geospatial queries in Dynamics365.",
        //TermsOfService = new Uri("https://github.com/Azure/azure-functions-openapi-extension"),
        Contact = new OpenApiContact()
        {
            Name = "Niclas Blomberg",
            Url = new Uri("https://github.com/lambda-snail/GeolocationAccelerator"),
        },
        License = new OpenApiLicense()
        {
            Name = "MIT",
            Url = new Uri("http://opensource.org/licenses/MIT"),
        }
    };
}