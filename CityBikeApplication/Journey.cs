using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace CityBikeApplication
{
    public class Journey
    {
        public string Id { get; set; }
        
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-ddTHH:mm:ss}")]
        [DataType(DataType.DateTime)]
        public DateTime DepartureTime { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-ddTHH:mm:ss}")]
        [DataType(DataType.DateTime)]
        public DateTime ReturnTime { get; set; }
        public int DepartureStationId { get; set; }
        public string DepartureStationName { get; set; }
        public int ReturnStationId { get; set; }
        public string ReturnStationName { get; set; }
        public int CoveredDistance { get; set; } // in kilometres
        public int Duration { get; set; } // in minutes

    }
   
}
