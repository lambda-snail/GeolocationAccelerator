using System;
using System.IO;
using System.Net;
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

namespace Accelerator.GeoLocation
{
    public class GetGeoRegion
    {
        private readonly ILogger<GetGeoRegion> _logger;
        private readonly ICosmosDbRegionService _service;

        public GetGeoRegion(ILogger<GetGeoRegion> log, ICosmosDbRegionService service)
        {
            _logger = log;
            _service = service;
        }

        [FunctionName("GetGeoRegion")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "region" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = true, Type = typeof(string), Description = "The dynamics id of the region object.")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/json", bodyType: typeof(string), Description = "The OK response")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.NotFound, contentType: "text/json", bodyType: typeof(string), Description = "When the resource does not exist in the database.")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "georegions/{id}")] HttpRequest req,
            string id)
        {
            try
            {
                GeoQueryResponse<GeoRegionModel> response = await _service.GetItem(id);

                if(response.Success)
                {
                    // TODO: return GeoRegionViewModel, add automappR
                    return new OkObjectResult(response.Item);
                }
            }
            catch(Exception e)
            {
                _logger.LogError(e.Message);
                return new ObjectResult(HttpStatusCode.InternalServerError);
            }

            return new NotFoundResult();
        }
    }
}

