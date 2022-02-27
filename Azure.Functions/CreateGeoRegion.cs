using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
using Newtonsoft.Json.Linq;

namespace Accelerator.GeoLocation
{
    public class CreateGeoRegion
    {
        private readonly ILogger<CreateGeoRegion> _logger;
        private readonly ICosmosDbRegionService _service;

        public CreateGeoRegion(ILogger<CreateGeoRegion> log, ICosmosDbRegionService service)
        {
            _logger = log;
            _service = service;
        }

        [FunctionName("CreateGeoRegion")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "region" }, Description = "Sync a region (polygon) consisting of multiple coordinate pairs to the database.")]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiRequestBody(contentType: "text/json", bodyType: typeof(SingleGeoPointViewModel), Description = "The CRM data to insert into the database.")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/json", bodyType: typeof(GeoRegionViewModel), Description = "if the object was successfully synced with the database.")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.InternalServerError, contentType: "text/json", bodyType: typeof(string), Description = "If something went wrong")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = "georegions")] HttpRequest req)
        {
            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                JObject intermediate = JsonConvert.DeserializeObject<JObject>(requestBody);

                string id = ParseId(intermediate);
                List<CoordinatePair> coordinateList = ParseCoordinateList(intermediate);

                GeoRegionModel region = new(id, coordinateList);
                GeoQueryResponse<GeoRegionModel> response = await _service.UpsertItem(region);
                if(response.Success)
                {
                    return new OkObjectResult(region);
                }
                else
                {
                    return new BadRequestResult();
                }
            }
            catch (ArgumentException e)
            {
                _logError(e);
                return new BadRequestResult();
            }
            catch (Exception e)
            {
                _logError(e);
                return new ObjectResult(HttpStatusCode.InternalServerError);
            }
        }
        private void _logError(Exception e)
        {
            _logger.LogError(e.Message);
            _logger.LogError(e.StackTrace);
        }

        private string ParseId(JObject jObject)
        {
            return jObject.Value<string>("id");
        }

        private List<CoordinatePair> ParseCoordinateList(JObject jObject)
        {
            JArray jsonCoordinates = jObject.Value<JArray>("coordinates");
            List<List<double>> doubleCoordinates = jsonCoordinates.ToObject<List<List<double>>>();
            return doubleCoordinates.Select(clist => new CoordinatePair
                   {
                       Longitude = clist[CoordinatePair.LongitudeIndex],
                       Latitude = clist[CoordinatePair.LatitudeIndex]
                   })
                   .ToList();
        }
    }
}

