using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace OpenWeatherMapAPI.Core
{
    public class REST
    {
        // base url
        private const string base_url = "http://api.openweathermap.org/data/2.5/";

        // instantiate a single shared instance of HttpClient
        // despite being IDisposable, disposing HttpClient will cause connection issues
        // if instantiated for each request
        // HttpClient is re-usable and thread safe
        private HttpClient client = new HttpClient();

        public REST()
        {
            // set the standard client data

            // convert the base url is above to a URI
            client.BaseAddress = new Uri(base_url);

            // set the content type
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            // set the user-agent
            client.DefaultRequestHeaders.Add("User-Agent", "Teddy Garland (https://teddygarland.com)");
        }
        
        /// <summary>
        /// Add the API key to the request
        /// all requests are GET requests (currently)
        /// </summary>
        /// <param name="path">the current path with querystring</param>
        /// <returns>the path with querystring and API key</returns>
        private string AddAPIkey(string path)
        {
            // check to see if we have query string parameters already
            // if so, the path would have a ? in it
            if (path.Contains("?"))
            {
                // we have other parameters
                // specify the key by appending an ampersand
                path += "&";
            }
            else
            {
                // we don't have any parameters yet, just add the API key
                path += "?";
            }

            // append the key value pair for the API key
            // and return
            return path + string.Format("APPID={0}", ConfigurationManager.AppSettings["key"]);
        }

        /// <summary>
        /// Make a request to the OpenWeatherMap API
        /// 
        /// This implementation of the API is GET only (for now)
        /// </summary>
        /// <param name="method">HTTP verb to use (e.g.: GET)</param>
        /// <param name="path">the path of the URI, with querystring if applicable</param>
        /// <returns>JSON response from the API server</returns>
        public async Task<string> MakeRequest(string method, string path)
        {


            // make the GET request and receive the response
            HttpResponseMessage response = client.GetAsync(AddAPIkey(path)).Result;

            // TODO: handle errors here
            // though it seems to respond with a body when errors are made
            //  { "cod": xxx } is the Status Code for the response
            //      401: unauthorized, with bad API key
            //      404: internal error, wrong resource requested

            // read the JSON
            return await response.Content.ReadAsStringAsync();
        }
    }
}
