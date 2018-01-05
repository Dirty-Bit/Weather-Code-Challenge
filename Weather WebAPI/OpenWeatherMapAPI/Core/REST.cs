using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace OpenWeatherMapAPI.Core
{
    public static class REST
    {
        // base url
        private const string base_url = "http://api.openweathermap.org/data/2.5/";

        // instantiate a single shared instance of HttpClient
        // despite being IDisposable, disposing HttpClient will cause connection issues
        // if instantiated for each request
        // HttpClient is re-usable and thread safe
        private static HttpClient client = new HttpClient();
        
        /// <summary>
        /// Make a request to the OpenWeatherMap API
        /// 
        /// This implementation of the API is GET only (for now)
        /// </summary>
        /// <param name="method">HTTP verb to use (e.g.: GET)</param>
        /// <param name="path">the path of the URI, with querystring if applicable</param>
        /// <returns>JSON response from the API server</returns>
        public static async Task<string> MakeRequest(string method, string path)
        {
            // set the standard client data

            // convert the base url is above to a URI
            client.BaseAddress = new Uri(base_url);

            // set the content type
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            // set the user-agent
            client.DefaultRequestHeaders.Add("User-Agent", "Teddy Garland (https://teddygarland.com)");

            // make the GET request and receive the response
            HttpResponseMessage response = client.GetAsync(path).Result;

            // TODO: handle errors here

            // read the JSON
            return await response.Content.ReadAsStringAsync();
        }
    }
}
