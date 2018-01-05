using OpenWeatherMapAPI.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OpenWeatherMapAPI.Requests
{
    public class Weather
    {
        /// <summary>
        /// internal function to check to see if we need to make the request
        /// to the OpenWeatherAPI. If we have the request cached in Redis, then
        /// there is no reason to make this request
        /// </summary>
        /// <param name="key">the Redis Cache key to check</param>
        /// <returns>string, JSON representation of the object if it exists. Null otherwise</returns>
        private string CheckCache(string key)
        {
            // TODO: implement redis
            return null;
        }

        /// <summary>
        /// Fire and forget function that will update our data sources
        /// without blocking UI for enduser
        /// </summary>
        /// <param name="key">the key to update in Redis</param>
        /// <param name="JSON">the JSON to add to Redis</param>
        private void UpdateDBAndCache(string key, string JSON)
        {
            try
            {
                // spin off a non-blocking task
                Task.Run(() =>
                {
                    // update Redis here

                    // update SQL

                }).ConfigureAwait(false);
            }
            catch (Exception e)
            {

            }
        }

        /// <summary>
        /// Make a request to the OpenWeatherAPI to get the current weather
        /// for the location of the ID specified
        /// </summary>
        /// <param name="id">OpenWeatherAPI definition for the ID specified</param>
        /// <returns>Current Weather object as JSON</returns>
        public async Task<string> Current(int id)
        {
            // build the key
            string key = string.Format("current_{0}", id);

            // check to see if we have the current weather for this location
            // within our Redis cache
            string response = CheckCache(key);

            // if we have it, return it
            // otherwise, make the request to the API
            if (!string.IsNullOrWhiteSpace(response))
                return response;

            // we need to make the request to the OpenWeatherAPI to get the data
            response = await REST.MakeRequest("GET", string.Format("current?id={0}", id));

            // spin off a task to update the databases while 
            // not locking up ui for the end user
            //  1. add the response to Redis with 5 minute expiration
            //  2. add/update SQL to hold the location
            UpdateDBAndCache(key, response);

            // return the response
            return response;
        }
        
        /// <summary>
        /// Make a request to the OpenWeatherAPI to get the three hour forecast
        /// for the location of the ID specified
        /// </summary>
        /// <param name="id">OpenWeatherAPI definition for the ID specified</param>
        /// <returns>Three Hour Weather object as JSON</returns>
        public async Task<string> ThreeHour(int id)
        {
            // build the key
            string key = string.Format("threehr_{0}", id);

            // check to see if we have the current weather for this location
            // within our Redis cache
            string response = CheckCache(key);

            // if we have it, return it
            // otherwise, make the request to the API
            if (!string.IsNullOrWhiteSpace(response))
                return response;

            // we need to make the request to the OpenWeatherAPI to get the data
            response = await REST.MakeRequest("GET", string.Format("forecast5?id={0}", id));

            // spin off a task to update the databases while 
            // not locking up ui for the end user
            //  1. add the response to Redis with 5 minute expiration
            //  2. add/update SQL to hold the location
            UpdateDBAndCache(key, response);

            // return the response
            return response;
        }
    }
}
