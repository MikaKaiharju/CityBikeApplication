using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace CityBikeApplication
{
    public class Station
    {
        public int id;
        public string nimi;
        public string namn;
        public string name;
        public string osoite;
        public string address;
        public string kaupunki;
        public string stad;
        public string operaattori;
        public int kapasiteetti;
        public string x;
        public string y;

        public string GetLocation()
        {
            string latitude = y;
            string longitude = x;

            return "https://www.google.com/maps/search/?api=1&query=" + latitude + "," + longitude;
        }


    }
}
