using OpenWeatherMapAPI.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace OpenWeatherMapAPI.Requests
{
    public class Weather
    {
        // initialize the REST client to make requests
        // against the OpenWeatherMap API
        private static REST rest = new REST();

        /// <summary>
        /// Make a request to the OpenWeatherAPI to get the current weather
        /// for the location of the ID specified
        /// </summary>
        /// <param name="id">OpenWeatherAPI definition for the ID specified</param>
        /// <returns>Current Weather object as JSON</returns>
        public static async Task<string> Current(int id)
        {
            // build the key
            string key = string.Format("current_{0}", id);

            // check to see if we have the current weather for this location
            // within our Redis cache
            string response = Cache.Check(key);

            // if we have it, return it
            // otherwise, make the request to the API
            if (!string.IsNullOrWhiteSpace(response))
                return response;

            // we need to make the request to the OpenWeatherAPI to get the data
            response = await rest.MakeRequest("GET", string.Format("weather?id={0}", id));

            // spin off a task to update the databases while 
            // not locking up ui for the end user
            //  1. add the response to Redis with 5 minute expiration
            //  2. add/update SQL to hold the location
            Cache.UpdateDBAndCache(key, response);

            // return the response
            return response;
        }
        
        /// <summary>
        /// Make a request to the OpenWeatherAPI to get the three hour forecast
        /// for the location of the ID specified
        /// </summary>
        /// <param name="id">OpenWeatherAPI definition for the ID specified</param>
        /// <returns>Three Hour Weather object as JSON</returns>
        public static async Task<string> Forecast(int id)
        {
            // build the key
            string key = string.Format("forecast_{0}", id);

            // check to see if we have the current weather for this location
            // within our Redis cache
            string response = Cache.Check(key);

            // if we have it, return it
            // otherwise, make the request to the API
            if (!string.IsNullOrWhiteSpace(response))
                return response;

            // we need to make the request to the OpenWeatherAPI to get the data
            response = await rest.MakeRequest("GET", string.Format("forecast?id={0}", id));

            // spin off a task to update the databases while 
            // not locking up ui for the end user
            //  1. add the response to Redis with 5 minute expiration
            //  2. add/update SQL to hold the location
            Cache.UpdateDBAndCache(key, response);

            // return the response
            return response;
        }

        /// <summary>
        /// Make a request to get both the current weather and the forecasted weather
        /// for the location specified
        /// 
        /// This will make the request for each asyncrhonously and perform the action
        /// of building the appropriate response object in JSON
        /// </summary>
        /// <param name="id">the ID of the location to get the weather from</param>
        /// <returns>{ current: {current}, forecast: {forecast} }</returns>
        public static async Task<string> CurrentAndForecast(int id)
        {
            // start the tasks of retrieving the information
            // from either the cache or the API
            Task<string> currentWeatherTask = Current(id);
            Task<string> forecastedWeatherTask = Forecast(id);

            // wait until both tasks are completed
            await Task.WhenAll(currentWeatherTask, forecastedWeatherTask);

            // we have both, create the response JSON
            StringBuilder sb = new StringBuilder();
            sb.Append("{\"current\":");
            sb.Append(currentWeatherTask.Result);
            sb.Append(",\"forecast\":");
            sb.Append(forecastedWeatherTask.Result);
            sb.Append("}");

            return sb.ToString();
        }
    }
}
