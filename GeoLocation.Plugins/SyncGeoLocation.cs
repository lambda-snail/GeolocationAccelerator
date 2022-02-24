using System;
using Microsoft.Xrm.Sdk;

namespace GeoLocation.Plugins
{
    /// <summary>
    /// When a location is created or updated, we put it to the geolocation db in azure.
    /// </summary>
    /// <remarks>The plugin assumes that the Uri including authentication code is stored in the secure configuration. For updates, it is assumed that
    /// an image is registered with the name 'postimage' which contains the longitude and latitude.</remarks>
    /// 
    //[CrmPluginRegistration("Create", "niblo_geolocation",
    //StageEnum.PostOperation, ExecutionModeEnum.Asynchronous,
    //"niblo_longitude,niblo_latitude", "Create Step", 1, IsolationModeEnum.Sandbox,
    //Description = "Sync geolocations to the database when created or updated.",
    //SecureConfiguration = "Add to config file, add config file to gitignore")]
    public sealed class SyncGeoLocation : IPlugin
    {
        private string serviceUri;

        public SyncGeoLocation(string unsecureConfig, string secureUrl)
        {
            if (string.IsNullOrEmpty(secureUrl))
            {
                throw new InvalidPluginExecutionException("Service endpoint ID should be passed as config.");
            }

            serviceUri = secureUrl;
        }

        public void Execute(IServiceProvider serviceProvider)
        {
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            if (!TargetIsEntity(context))
            {
                return;
            }

            Entity location = context.InputParameters["Target"] as Entity;
            if(!HasLocationParameters(location))
            {
                return;
            }

            double longitude = 0d;
            double latitude = 0d;
            bool hasUpdated = false;
            if (context.MessageName == "Create")
            {
                longitude = location.GetAttributeValue<double>("niblo_longitude");
                latitude = location.GetAttributeValue<double>("niblo_latitude");
                hasUpdated = true;
            }
            else if (context.MessageName == "Update")
            {
                if(context.PostEntityImages.Contains("postimage"))
                {
                    longitude = context.PostEntityImages["postimage"].GetAttributeValue<double>("niblo_longitude");
                    latitude = context.PostEntityImages["postimage"].GetAttributeValue<double>("niblo_latitude");
                    hasUpdated = true;
                }
            }

            if(!hasUpdated)
            {
                return;
            }

            try
            {

                string message = GetSynchronizationMessage(location.Id, longitude, latitude);
                RemoteRequestUtils.SendMessage(serviceUri, message);
            }
            catch (Exception e)
            {
                tracingService.Trace("Exception: {0}", e.ToString());
                throw;
            }
        }

        private bool TargetIsEntity(IPluginExecutionContext context)
        {
            return context.InputParameters.Contains("Target") && (context.InputParameters["Target"] is Entity);
        }

        private bool HasLocationParameters(Entity e)
        {
            return e.Contains("niblo_longitude") && e.Contains("niblo_latitude");
        }

        private string GetSynchronizationMessage(Guid id, double longitude, double latitude)
        {
            return $@"
                {{
                    'id': {id},
                    'longitude': {longitude},
                    'latitude': {latitude}
                }}
                ";
        }
    }
}