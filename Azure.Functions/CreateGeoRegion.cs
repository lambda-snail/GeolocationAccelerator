using System.IO;
using System.Net;
using System.Threading.Tasks;
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
    public class CreateGeoRegion
    {
        private readonly ILogger<CreateGeoRegion> _logger;

        public CreateGeoRegion(ILogger<CreateGeoRegion> log)
        {
            _logger = log;
        }

        [FunctionName("CreateGeoRegion")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "region" }, Description = "Sync a region (polygon) consisting of multiple coordinate pairs to the database.")]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "if the object was successfully synced with the database.")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.InternalServerError, contentType: "text/plain", bodyType: typeof(string), Description = "If something went wrong")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req)
        {
            
            // TODO Implement
            // Also need webhook
            // Service for regions
            // Models and viewmodels

            return new OkObjectResult(string.Empty);
        }
    }
}

