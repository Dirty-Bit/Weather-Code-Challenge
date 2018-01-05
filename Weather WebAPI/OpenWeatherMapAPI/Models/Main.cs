using System;
using System.Collections.Generic;
using System.Text;

namespace OpenWeatherMapAPI.Models
{
    public class Main
    {
        public double temp { get; set; }
        public int pressure { get; set; }
        public int humidity { get; set; }
        public double temp_min { get; set; }
        public double temp_max { get; set; }
        public double sea_level { get; set; }
        public double grnd_level { get; set; }
        public double temp_kf { get; set; }
    }
}
