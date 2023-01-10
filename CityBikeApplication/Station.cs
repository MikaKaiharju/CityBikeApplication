using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace CityBikeApplication
{
    public class Station
    {
        public string id;
        public string nimi;
        public string namn;
        public string name;
        public string osoite;
        public string address;
        public string kaupunki;
        public string stad;
        public string operaattori;
        public string kapasiteetti;
        public double x;
        public double y;

        public string GetLocation()
        {
            var latitude = ("" + y).Replace(",", ".");
            var longitude = ("" + x).Replace(",", ".");

            return "https://www.google.com/maps/search/?api=1&query=" + latitude + "," + longitude;
        }

    }
}
