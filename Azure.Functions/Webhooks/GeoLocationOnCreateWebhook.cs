using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Accelerator.GeoLocation.Contracts;
using Accelerator.GeoLocation.Models;
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

namespace Accelerator.GeoLocation.Webhooks
{
    public class GeoLocationOnCreateWebhook
    {
        private readonly ILogger<GeoLocationOnCreateWebhook> _logger;
        private readonly ICosmosDbLocationService _cosmosService;

        public GeoLocationOnCreateWebhook(ILogger<GeoLocationOnCreateWebhook> log, ICosmosDbLocationService cosmosService)
        {
            _logger = log;
            _cosmosService = cosmosService;
        }

        [FunctionName("GeoLocationOnCreateWebhook")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "webhooks" }, Description = "Webhook that syncs data from Dynamics to the geolocation database.")]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The received webhook is aknowledged with a 200.")]

        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            JObject jsonObj = JObject.Parse(requestBody);
            var inputParameters = jsonObj.GetValue("InputParameters").First();
            var target = inputParameters.First().Next;
            var __type = target.First().First; // {"__type": "Entity:http://schemas.microsoft.com/xrm/2011/Contracts"}
            var attributeListJson = __type.Next.First();

            IEnumerable<Attribute> attributes = attributeListJson.Select(x => x.ToObject<Attribute>()).ToList();

            Attribute id = attributes.Where(attribute => attribute.Key == "niblo_geolocationid").FirstOrDefault();
            Attribute longitude = attributes.Where(attribute => attribute.Key == "niblo_longitude").FirstOrDefault();
            Attribute latitude = attributes.Where(attribute => attribute.Key == "niblo_latitude").FirstOrDefault();

            try
            {
                string dynamicsId = id.Value as string;
                double longitudeDouble = double.Parse(longitude.Value as string, CultureInfo.InvariantCulture);
                double latitudeDouble = double.Parse(latitude.Value as string, CultureInfo.InvariantCulture);

                GeoPointModel point = new GeoPointModel(
                    dynamicsId,
                    longitudeDouble,
                    latitudeDouble
                );

                ICosmosDbLocationService.LocationQueryResponse response = await _cosmosService.CreatePoint(point);

                if(! response.Success)
                {
                    _logger.LogInformation($"Unable to sync data to the database: lon: {longitudeDouble}, lat: {latitudeDouble}, id: {dynamicsId}");
                }
            }
            catch(Exception e)
            {
                _logger.LogError(e.Message);
            }

            return new OkObjectResult(string.Empty);
        }
    }

    // TODO: Move to shared space
    public class Attribute
    {
        [JsonPropertyName("key")]
        public string Key { get; set; }

        [JsonPropertyName("value")]
        public object Value { get; set; }
    }
}

