using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Accelerator.GeoLocation.Contracts;
using Accelerator.GeoLocation.Models;
using Accelerator.GeoLocation.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

namespace Accelerator.GeoLocation
{
    public class GetGeoLocation
    {
        private readonly ILogger<GetGeoLocation> _logger;
        private readonly ICosmosDbLocationService _cosmosService;

        public GetGeoLocation(ILogger<GetGeoLocation> log, ICosmosDbLocationService cosmosService)
        {
            _logger = log;
            _cosmosService = cosmosService;
        }

        [FunctionName("GetGeoLocation")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "location" }, Description = "Retreive a location from the database.")]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = true, Type = typeof(string), Description = "The dynamics id (Guid) of the location")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/json", bodyType: typeof(SingleGeoPointViewModel), Description = "The location definition")]
        public async Task<IActionResult> Run(
                [HttpTrigger(AuthorizationLevel.Function, "get", Route = "geolocations/{id}")] HttpRequest req,
                string id
            )
        {
            try
            {
                GeoQueryResponse<GeoPointModel> response = await _cosmosService.GetItem(id);
                if (response.Success)
                {
                    SingleGeoPointViewModel points = new SingleGeoPointViewModel { 
                                                            Id = id,
                                                            Longitude = response.Item.LocationDefinition.Position.Longitude,
                                                            Latitude = response.Item.LocationDefinition.Position.Latitude
                                                      };

                    return new OkObjectResult(points);
                }
            }
            catch(Exception e)
            {
                _logger.LogError(e.Message);
                _logger.LogError(e.StackTrace);
                return new ObjectResult(HttpStatusCode.InternalServerError);
            }

            // If we get here, then no error was thrown and no results were found
            return new NotFoundObjectResult(id);
        }
    }


}

