using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace OpenWeatherMapAPI.Models
{
    public class Current_Weather
    {
        public Coordinates coord { get; set; }
        public Weather weather { get; set; }

        // base is a restricted word in c#
        // have to override this property name and create our own
        // for Newtonsoft to parse it
        [JsonProperty(PropertyName = "base")]
        public string Base {get;set;}

        public Main main { get; set; }
        public int visibility { get; set; }
        public Wind wind { get; set; }
        public Clouds clouds { get; set; }
        public int dt { get; set; }
        public Sys sys { get; set; }
        public int id { get; set; }
        public string name { get; set; }
        public int cod { get; set; }
    }
}
