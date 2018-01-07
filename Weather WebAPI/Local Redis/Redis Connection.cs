using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace Local_Redis
{
    public class Redis_Connection : IDisposable
    {
        private ConnectionMultiplexer redis;
        private IDatabase db;

        public Redis_Connection()
        {
            // create the connection
            redis = ConnectionMultiplexer.Connect("localhost");

            // get the database
            db = redis.GetDatabase();
        }

        /// <summary>
        /// Check to see if a key exists within the Redis cache
        /// </summary>
        /// <param name="key">the key to check within the redis cache</param>
        /// <returns>true if the key exists within the cache, false if it does not</returns>
        public bool exists(string key)
        {
            return db.KeyExists(key);
        }

        /// <summary>
        /// Set a value within the Redis Cache
        /// </summary>
        /// <param name="key">the key to store the value within</param>
        /// <param name="value">the value associated to the key</param>
        /// <param name="minutes">the number of minutes to keep the data in cache before expiration</param>
        /// <returns>flag: whether or not we successfully added the key</returns>
        public bool set(string key, string value, double minutes = 5)
        {
            return db.StringSet(key, value, TimeSpan.FromMinutes(minutes), When.Always, CommandFlags.None);
        }

        /// <summary>
        /// Get a value that is associated with a key from the Redis cache
        /// </summary>
        /// <param name="key">the key to attempt to get</param>
        /// <returns>null: if the key does not exist in the database, string if it does</returns>
        public string get(string key)
        {
            // get and return
            return db.StringGet(key);
        }

        /// <summary>
        /// Delete an entry from the Redis cache
        /// </summary>
        /// <param name="key">the entry to delete</param>
        /// <returns>true if the delete was successful, false if not</returns>
        public bool delete(string key)
        {
            return db.KeyDelete(key);
        }

        /// <summary>
        /// clean up redis connection when we are done with it
        /// </summary>
        public void Dispose()
        {
            // close the connections once they have returned
            // any current requests
            redis.Close(true);

            // dispose the connection
            redis.Dispose();

            // set to null
            redis = null;
        }
    }
}
