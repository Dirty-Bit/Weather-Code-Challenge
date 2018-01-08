using Local_Database;
using Local_Redis;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Weather_Models;
using Weather_WebAPI.Models;

namespace Weather_WebAPI.Controllers
{
    public class LocationController : ApiController
    {
        // instantiate redis
        static Redis_Connection redis = new Redis_Connection();

        // instantiate the connection
        static Connection conn = new Connection();

        /// <summary>
        /// get a location from redis (preferrably) or from the database
        /// </summary>
        /// <param name="id">the Id of the location, as defined by the OpenWeatherMap API</param>
        /// <returns>Location, if found by either redis or SQL. null if it does not exist</returns>
        private Location _GetLocation(int id)
        {
            // initialize the location
            Location location = null;

            // check redis to see if this exists
            string location_json = redis.get(string.Format("loc_{0}", id));

            // did we get something back?
            // check the json to see if it is null or not
            if (location_json != null)
            {
                // we had the location 
                // parse it into the location object
                location = JsonConvert.DeserializeObject<Location>(location_json);
            }
            else
            {
                // query the db to get the right location
                location = conn.GetLocation(id);
            }

            // send back what we've got
            return location;
        }

        /// <summary>
        /// default route that will show on start-up
        /// </summary>
        /// <returns></returns>
        [Route("")]
        public HttpResponseMessage Get()
        {
            // get the readme text, no need to fail here
            string readme = "";
            try
            {
                // the readme.md is handled as a linked asset and should be 
                // copied into the /bin folder when it is newer
                using (StreamReader r = new StreamReader(string.Format("{0}\\README.md", HttpRuntime.BinDirectory)))
                    readme = r.ReadToEnd();
            }
            catch (Exception e)
            { }

            // return with something when the start-up occurs
            // added a dependency CommonMark.NET
            // to display this data
            // CommonMark.NET will take the .md file and convert to html
            HttpResponseMessage response = new HttpResponseMessage();
            response.Content = new StringContent(string.Format("<!DOCTYPE HTML><html><head><title>Weather API Challenge</title></head><body>Hello world!<br /><br />This code challenge can be found on <a href=\"https://github.com/Dirty-Bit/Weather-Code-Challenge\" target=\"blank\">GitHub here</a>.<br />A decently interpreted version of the markdown file is below (it's not a front-end test).<br /><br />{0}</body></html>", CommonMark.CommonMarkConverter.Convert(readme)));
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
            return response;
        }

        /// <summary>
        /// GET /api/locations
        /// </summary>
        /// <returns>JSON object containing all locations</returns>
        public IEnumerable<Location> GetAllLocations()
        {
            // return all
            return conn.GetAllLocations();
        }

        /// <summary>
        /// GET /api/locations/{id}
        /// </summary>
        /// <param name="id">the ID of the location to get</param>
        /// <returns>JSON of the requested location</returns>
        public IHttpActionResult GetLocation(int id)
        {
            // get the location from redis or SQL
            Location location = _GetLocation(id);

            // make sure that we have this location
            // we may have checked this above
            // but could still have a parse error from Newtonsoft
            if (location == null)
                return NotFound();

            // save this in redis
            // we can keep it for a while since we don't expect the values to change
            // either add this to redis or reset the time to live
            redis.set(string.Format("loc_{0}", id), JsonConvert.SerializeObject(location), 60);

            // return success
            return Ok(location);
        }

        /// <summary>
        /// DELETE /api/locations/id
        /// </summary>
        /// <param name="id">the ID of the location to delete</param>
        /// <returns>JSON of the removed location</returns>
        [HttpDelete]
        public IHttpActionResult DeleteLocation(int id)
        {
            // delete this from redis, if it existed
            redis.delete(string.Format("loc_{0}", id));

            // query the db to remove the location
            API_Response_Object response = new API_Response_Object(conn.RemoveLocation(id));

            // return the result
            return Ok(response);
        }

        /// <summary>
        /// POST /api/locations
        /// </summary>
        /// <param name="location">a location that was newly added</param>
        /// <returns>JSON of the added location</returns>
        public IHttpActionResult PostLocation(Location location)
        {
            // attempt to add this location to the database
            // hold the result
            bool was_successful = conn.AddLocation(location);

            // check to see if this was successful before
            // adding an entry into redis
            redis.set(string.Format("loc_{0}", location.Id), JsonConvert.SerializeObject(location), 60);

            // query the db to add the new location
            API_Response_Object response = new API_Response_Object(was_successful);

            // return the result
            return Ok(response);
        }

        /// <summary>
        /// Get the weather for a particular location
        /// </summary>
        /// <param name="id">the ID of the location to get the weather of</param>
        /// <returns>the current weather and the 3 hour forcasted weather</returns>
        [Route("api/locations/{id}/weather")]
        public async Task<IHttpActionResult> GetWeather(int id)
        {
            // initialize a weather object for a response
            Current_and_Forecasted_Weather weather = null;

            // check to see if we have the weather in our redis cache
            string current_json = redis.get(string.Format("current_{0}", id));
            string forecast_json = redis.get(string.Format("forecast_{0}", id));

            // check to see if we have weather json
            // we need both to be in cache
            if (current_json != null && forecast_json != null)
            {
                // parse the redis response object to return
                weather = new Current_and_Forecasted_Weather(current_json, forecast_json);

                // note: we do not want to update redis if this came from redis
                // since we are expecting the weather to change at some point
                // and do not want to return the same result forever if continuously
                // refreshed with requests being made less than 5 min apart
            }
            else
            {
                // query redis or the database to make sure we have the location
                Location location = _GetLocation(id);

                // make sure that we have this location
                if (location == null)
                    return NotFound();

                // we have a location, we can retrieve its weather
                weather = await OpenWeatherMapAPI.Requests.Weather.CurrentAndForecast(id, redis);

                // cache combined weather object in redis for 5 min
                redis.set(string.Format("weather_{0}", id), JsonConvert.SerializeObject(weather));
            }                       

            // return ok
            return Ok(weather);
        }
    }
}
