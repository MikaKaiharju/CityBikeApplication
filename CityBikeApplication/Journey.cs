using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityBikeApplication
{
    public class Journey
    {
        public string id;
        public string departureTime;
        public string returnTime;
        public string departureStationId;
        public string departureStationName;
        public string returnStationId;
        public string returnStationName;
        public int coveredDistance; // in kilometres
        public int duration; // in minutes
    }
}
