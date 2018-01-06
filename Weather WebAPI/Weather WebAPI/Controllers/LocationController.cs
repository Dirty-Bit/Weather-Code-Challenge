﻿using Local_Database;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Weather_Models;
using Weather_WebAPI.Models;

namespace Weather_WebAPI.Controllers
{
    public class LocationController : ApiController
    {
        // instantiate the connection
        static Connection conn = new Connection();

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
            // query the db to get the right location
            Location location = conn.GetLocation(id);
            
            // make sure that we have this location
            if (location == null)
                return NotFound();

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
            // query the db to remove the location
            API_Response_Object response = new API_Response_Object(!conn.RemoveLocation(id));

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
            // query the db to add the new location
            API_Response_Object response = new API_Response_Object(conn.AddLocation(location));

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
            // query the database to make sure we have the location
            Location location = conn.GetLocation(id);

            // make sure that we have this location
            if (location == null)
                return NotFound();

            // we have a location, we can retrieve its weather
            Current_and_Forecasted_Weather weather = await OpenWeatherMapAPI.Requests.Weather.CurrentAndForecast(id);

            // return ok
            return Ok(weather);
        }
    }
}
