using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace CityBikeApplication.Pages
{
    public class IndexModel : PageModel
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

        public class Station
        {
            public string id;
            public string stationName;
        }

        List<Journey> journeys = new List<Journey>();
        List<Station> stations = new List<Station>();

        public void OnGet()
        {
            
        }
    }
}
