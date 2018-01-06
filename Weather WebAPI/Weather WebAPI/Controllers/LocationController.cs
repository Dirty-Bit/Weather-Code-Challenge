using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Weather_Models;

namespace Weather_WebAPI.Controllers
{
    public class LocationController : ApiController
    {
        static List<Location> locations = new List<Location>()
        {
            new Location { Id = 5308655, Country = "US", Name = "Phoenix", State = "AZ", Zipcode = "85044" },
            new Location { Id = 5419384, Country = "US", Name = "Denver", State = "CO", Zipcode = "80014" }
        };

        /// <summary>
        /// GET /api/locations
        /// </summary>
        /// <returns>JSON object containing all locations</returns>
        public IEnumerable<Location> GetAllLocations()
        {
            // return all
            return locations;
        }

        /// <summary>
        /// GET /api/locations/{id}
        /// </summary>
        /// <param name="id">the ID of the location to get</param>
        /// <returns>JSON of the requested location</returns>
        public IHttpActionResult GetLocation(int id)
        {
            // use LINQ to find the right location
            Location location = locations.FirstOrDefault((x) => x.Id == id);
            
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
            // use LINQ to find the right location
            Location location = locations.FirstOrDefault((x) => x.Id == id);

            // make sure that we have this location
            if (location == null)
                return NotFound();

            // remove the location from the list
            locations = locations.Where(x => x.Id != id).ToList();

            // return success
            return Ok(location);
        }

        /// <summary>
        /// POST /api/locations
        /// </summary>
        /// <param name="location">a location that was newly added</param>
        /// <returns>JSON of the added location</returns>
        public IHttpActionResult PostLocation(Location location)
        {
            // check to see if this is valid
            if (!ModelState.IsValid)
                return BadRequest("Invalid location");

            // make sure we don't already have this location
            // if we do, we can say success because it's already there
            if (locations.Any(x => x.Id == location.Id))
                return Ok(location);

            // we did not have this location already
            // add it now
            locations.Add(location);

            // return success
            return Ok(location);
        }

        /// <summary>
        /// Get the weather for a particular location
        /// </summary>
        /// <param name="id">the ID of the location to get the weather of</param>
        /// <returns>the current weather and the 3 hour forcasted weather</returns>
        [Route("api/locations/{id}/weather")]
        public async Task<IHttpActionResult> GetWeather(int id)
        {
            // use LINQ to find the right location
            Location location = locations.FirstOrDefault((x) => x.Id == id);

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
