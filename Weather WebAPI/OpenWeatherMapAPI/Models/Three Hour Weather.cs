using System;
using System.Collections.Generic;
using System.Text;

namespace OpenWeatherMapAPI.Models
{
    public class Three_Hour_Weather
    {
        public string cod { get; set; }
        public double message { get; set; }
        public int cnt { get; set; }
        public Weather_Block[] list { get; set; }
        public City city { get; set; }
    }
}
