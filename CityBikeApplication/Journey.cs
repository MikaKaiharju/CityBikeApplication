using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace CityBikeApplication
{
    public class Journey
    {
        public string id;
        public string departureTime = "";
        public string returnTime = "";
        public int departureStationId;
        public string departureStationName;
        public int returnStationId;
        public string returnStationName;
        public int coveredDistance; // in kilometres
        public int duration; // in minutes

        public string GetUnderstandableDepartureTime()
        {
            return GetUnderstandableTime(departureTime);
        }

        public string GetUnderstandableReturnTime()
        {
            return GetUnderstandableTime(returnTime);
        }

        private string GetUnderstandableTime(string timeString)
        {
            // 2021-05-31T23:57:25
            // ->
            // 23.57\n31.05.2021

            if (timeString.Equals(""))
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
