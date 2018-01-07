using Local_Redis;
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
        /// <param name="redis">the connection to the redis server to use</param>
        /// <returns>string, JSON representation of the object if it exists. Null otherwise</returns>
        public static string Check(string key, Redis_Connection redis)
        {
            // check the redis cache for the value we need
            return redis.get(key);
        }

        /// <summary>
        /// Fire and forget function that will update our data sources
        /// without blocking UI for enduser
        /// </summary>
        /// <param name="key">the key to update in Redis</param>
        /// <param name="JSON">the JSON to add to Redis</param>
        /// <param name="redis">the redis connection to use</param>
        public static void UpdateDBAndCache(string key, string JSON, Redis_Connection redis)
        {
            try
            {
                // spin off a non-blocking task
                Task.Run(() =>
                {
                    // update Redis
                    redis.set(key, JSON);

                }).ConfigureAwait(false);
            }
            catch (Exception e)
            {

            }
        }
    }
}
