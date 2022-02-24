using System;

namespace GeoLocation.Plugins
{
    public static class RemoteRequestUtils
    {
        /// <summary>
        /// Send a message to the specified endpoint, using authentication in the request header.
        /// </summary>
        public static void SendMessage(string functionUri, string message)
        {
            using (System.Net.Http.HttpClient client = new System.Net.Http.HttpClient())
            {
                client.BaseAddress = new Uri(functionUri);

                //System.Net.Http.HttpContent content = new System.Net.Http.ByteArrayContent(message);
                System.Net.Http.HttpContent content = new System.Net.Http.StringContent(message);
                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

                var response = client.PutAsync(functionUri, content).Result;
                if (response.IsSuccessStatusCode == false)
                {
                    throw new System.Net.WebException($"Error Posting to {functionUri} with code {response.StatusCode} and reason {response.ReasonPhrase}");
                }
            }
        }
    }
}
