using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace CityBikeApplication
{
    public class Journey
    {
        public string Id { get; set; }
        public string DepartureTime { get; set; }
        public string ReturnTime { get; set; }
        public int DepartureStationId { get; set; }
        public string DepartureStationName { get; set; }
        public int ReturnStationId { get; set; }
        public string ReturnStationName { get; set; }
        public int CoveredDistance { get; set; } // in kilometres
        public int Duration { get; set; } // in minutes

        public string GetUnderstandableDepartureTime()
        {
            return GetUnderstandableTime(DepartureTime);
        }

        public string GetUnderstandableReturnTime()
        {
            return GetUnderstandableTime(ReturnTime);
        }

        private string GetUnderstandableTime(string timeString)
        {
            // 2021-05-31T23:57:25
            // ->
            // 23.57.25 31.05.2021

            if (timeString == null || timeString == "")
            {
                DateTime dateTime1 = DateTime.Now;
                return dateTime1.ToString("HH.mm.ss dd.MM.yyyy");
            }

            timeString = timeString.Replace("T", " ").Replace(".",":");

            DateTime dateTime = DateTime.ParseExact(timeString, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);

            string readableDateTime = dateTime.ToString("HH.mm.ss dd.MM.yyyy");

            return readableDateTime;
            
        }

        private void p(string s)
        {
            System.Diagnostics.Debug.WriteLine(s);
        }

    }
   
}
