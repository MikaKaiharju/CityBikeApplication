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
        public string x;
        public string y;

        public string GetLocation()
        {
            // x and y are stored with comma but url query needs dots
            var latitude = ("" + y).Replace(",", ".");
            var longitude = ("" + x).Replace(",", ".");

            return "https://www.google.com/maps/search/?api=1&query=" + latitude + "," + longitude;
        }


    }
}
