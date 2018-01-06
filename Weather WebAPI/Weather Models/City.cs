using System;
using System.Collections.Generic;
using System.Text;

namespace Weather_Models
{
    public class City
    {
        public int id { get; set; }
        public string name { get; set; }
        public Coordinates coord { get; set; }
        public string country { get; set; }
    }
}
