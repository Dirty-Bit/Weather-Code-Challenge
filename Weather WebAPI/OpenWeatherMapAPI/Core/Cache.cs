using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OpenWeatherMapAPI.Core
{
    public class Cache
    {
        /// <summary>
        /// internal function to check to see if we need to make the request
        /// to the OpenWeatherAPI. If we have the request cached in Redis, then
        /// there is no reason to make this request
        /// </summary>
        /// <param name="key">the Redis Cache key to check</param>
        /// <returns>string, JSON representation of the object if it exists. Null otherwise</returns>
        public static string Check(string key)
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
        public static void UpdateDBAndCache(string key, string JSON)
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
    }
}
