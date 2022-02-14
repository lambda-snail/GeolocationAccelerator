using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Accelerator.GeoLocation.Contracts;
using Accelerator.GeoLocation.Models;
using Accelerator.GeoLocation.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos.Spatial;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

namespace Accelerator.GeoLocation;

public class CreateGeoLocation
{
    private readonly ILogger<CreateGeoLocation> _logger;
    private readonly ICosmosDbLocationService _cosmosService;

    public CreateGeoLocation(ILogger<CreateGeoLocation> log, ICosmosDbLocationService cosmosService)
    {
        _logger = log;
        _cosmosService = cosmosService;
    }

    [FunctionName("CreateGeoLocation")]
    [OpenApiOperation(operationId: "Run", tags: new[] { "location" }, Description = "Creates a new location in the database.")]
    [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
    [OpenApiRequestBody(contentType: "text/json", bodyType: typeof(SingleGeoPointViewModel), Description = "The CRM data to insert into the database.")]
    //[OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = true, Type = typeof(GeoPointModel), Description = "The Geo Location to send to the Db")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/json", bodyType: typeof(SingleGeoPointViewModel), Description = "The created location")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.InternalServerError, contentType: "text/json", bodyType: typeof(string), Description = "If something went wrong")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "put", Route = "geolocations")] HttpRequest req
        )
    {
        try
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            SingleGeoPointViewModel requestData = JsonConvert.DeserializeObject<SingleGeoPointViewModel>(requestBody);

            GeoPointModel point = new GeoPointModel(
                    requestData.Id,
                    requestData.Longitude,
                    requestData.Latitude
                );

            ICosmosDbLocationService.LocationQueryResponse response = await _cosmosService.UpsertPoint(point);

            if (response.Success)
            {
                return new OkObjectResult(requestData);
            }
            else
            {
                return new ObjectResult(HttpStatusCode.InternalServerError);
            }
        }
        catch(Exception e)
        {
            return new ObjectResult(HttpStatusCode.InternalServerError);
        }
    }
}
