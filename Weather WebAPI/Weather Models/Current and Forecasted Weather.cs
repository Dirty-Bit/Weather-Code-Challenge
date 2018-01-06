using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weather_Models
{
    public class Current_and_Forecasted_Weather
    {
        /// <summary>
        /// build a response object for our weather API call from ASP.NET
        /// </summary>
        /// <param name="current">JSON response with the current weather</param>
        /// <param name="forecast">JSON response for the forecasted weather</param>
        public Current_and_Forecasted_Weather(string current, string forecast)
        {
            // parse API response into an object
            this.current = JsonConvert.DeserializeObject<Current_Weather>(current);
            this.forecast = JsonConvert.DeserializeObject<Forecast>(forecast);

            // remove some of the items from the forecast list
            // there are a lot of 3 hour forecasted weather blocks
            // that we don't need to respond with for our API
            this.forecast.list = this.forecast.list.Take(1).ToArray();
        }

        public Current_Weather current { get; set; }
        public Forecast forecast { get; set; }
    }
}
