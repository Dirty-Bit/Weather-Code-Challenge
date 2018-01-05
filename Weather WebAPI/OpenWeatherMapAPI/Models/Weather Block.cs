using System;
using System.Collections.Generic;
using System.Text;

namespace OpenWeatherMapAPI.Models
{
    public class Weather_Block
    {
        public int dt { get; set; }
        public Main main { get; set; }
        public Weather weather { get; set; }
        public Clouds clouds { get; set; }
        public Wind wind { get; set; }
        public Sys sys { get; set; }
        public string dt_txt { get; set; }
    }
}
