using System;
using System.Collections.Generic;
using System.Text;

namespace Weather_Models
{
    public class Forecast
    {
        public string cod { get; set; }
        public double message { get; set; }
        public int cnt { get; set; }
        public Weather_Block[] list { get; set; }
        public City city { get; set; }
    }
}
