using Microsoft.Xrm.Sdk;
using System;
using System.Net;

namespace GeoLocation.Plugins
{
    /// <summary>
    /// When a location is created or updated, sync with the external database.
    /// 
    /// The url to the azure function should be stored in the secure configuration.
    /// </summary>
    public class SyncGeoLocation : IPlugin
    {
        private string _unsecureconfig;
        private string _azureFunctionUrl;

        public SyncGeoLocation(string unsecureconfig, string secureconfig)
        {
            _unsecureconfig = unsecureconfig;
            _azureFunctionUrl = secureconfig; // Url to azure function here
        }

        public void Execute(IServiceProvider serviceProvider)
        {
            ITracingService tracingService = serviceProvider.GetService(typeof(ITracingService)) as ITracingService;
            IPluginExecutionContext context = serviceProvider.GetService(typeof(IPluginExecutionContext)) as IPluginExecutionContext;

            if (context.InputParameters.Contains("Target") && (context.InputParameters["Target"] is Entity))
            {
                try
                {
                    IOrganizationServiceFactory serviceFactory = serviceProvider.GetService(typeof(IOrganizationServiceFactory)) as IOrganizationServiceFactory;
                    IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

                    Entity location = context.InputParameters["Target"] as Entity;

                    string postRequest = $@"
                            {{
                                'dynamicsId': '{location.Id}',
                                'locationDefinition': {{
                                    'longitude': {location["niblo_longitude"]},
                                    'latitude': {location["niblo_latitude"]}
                                    }}
                            }}";


                    HttpWebRequest request = WebRequest.Create(_azureFunctionUrl) as HttpWebRequest;
                    request.Method = "POST";
                    request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.36";
                    request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;

                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    
                    if(response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created)
                    {
                        tracingService.Trace($"Unable to sync location with id {location.Id} to the database: {response.StatusDescription}");
                    }
                }
                catch (Exception ex)
                {
                    tracingService.Trace(ex.Message);
                }
            }
        }
    }
}
