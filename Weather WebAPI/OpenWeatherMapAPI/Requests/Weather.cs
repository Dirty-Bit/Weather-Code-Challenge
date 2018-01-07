using OpenWeatherMapAPI.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using Weather_Models;
using Local_Redis;

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
        /// <param name="redis">The Redis connection to use</param>
        /// <returns>Current Weather object as JSON</returns>
        public static async Task<string> Current(int id, Redis_Connection redis)
        {
            // build the key
            string key = string.Format("current_{0}", id);

            // check to see if we have the current weather for this location
            // within our Redis cache
            string response = Cache.Check(key, redis);

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
            Cache.UpdateDBAndCache(key, response, redis);

            // return the response
            return response;
        }

        /// <summary>
        /// Make a request to the OpenWeatherAPI to get the three hour forecast
        /// for the location of the ID specified
        /// </summary>
        /// <param name="id">OpenWeatherAPI definition for the ID specified</param>
        /// <param name="redis">The Redis connection to use</param>
        /// <returns>Three Hour Weather object as JSON</returns>
        public static async Task<string> Forecast(int id, Redis_Connection redis)
        {
            // build the key
            string key = string.Format("forecast_{0}", id);

            // check to see if we have the current weather for this location
            // within our Redis cache
            string response = Cache.Check(key, redis);

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
            Cache.UpdateDBAndCache(key, response, redis);

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
        /// <param name="redis">The Redis connection to use</param>
        /// <returns>{ current: {current}, forecast: {forecast} }</returns>
        public static async Task<Current_and_Forecasted_Weather> CurrentAndForecast(int id, Redis_Connection redis)
        {
            // start the tasks of retrieving the information
            // from either the cache or the API
            Task<string> currentWeatherTask = Current(id, redis);
            Task<string> forecastedWeatherTask = Forecast(id, redis);

            // wait until both tasks are completed
            await Task.WhenAll(currentWeatherTask, forecastedWeatherTask);

            // create the object and return it
            return new Current_and_Forecasted_Weather(currentWeatherTask.Result, forecastedWeatherTask.Result);
        }
    }
}
