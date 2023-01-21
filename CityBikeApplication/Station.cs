using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace CityBikeApplication
{
    public class Station
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Operator { get; set; }
        public int Capacity { get; set; }
        public string X { get; set; }
        public string Y { get; set; }

        public string GetLocation()
        {
            return "https://www.google.com/maps/search/?api=1&query=" + Y + "," + X;
        }


    }
}
