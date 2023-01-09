using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Net;

namespace CityBikeApplication.Pages
{
    public class IndexModel : PageModel
    {
        public class Journey
        {
            public int id;
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

        // id number that is incremented by 1 for every journey
        int runningNumberForJourneyId = 0;

        // journey data paths
        string path1 = "https://dev.hsl.fi/citybikes/od-trips-2021/2021-05.csv";
        string path2 = "https://dev.hsl.fi/citybikes/od-trips-2021/2021-06.csv";
        string path3 = "https://dev.hsl.fi/citybikes/od-trips-2021/2021-07.csv";

        // station data path
        string path4 = "https://opendata.arcgis.com/datasets/726277c507ef4914b0aec3cbcfcbfafc_0.csv";

        public void OnGet()
        {
            ImportJourneyData(path1);
        }

        public void ImportJourneyData(string path)
        {
            //  import journey data

            // for debugging limit amount of lines to be read
            int limit = 20;
            int currentIteration = 0;

            // read data from url
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(path);
            HttpWebResponse httpWebResponse = (HttpWebResponse) httpWebRequest.GetResponse();

            //TODO: do the line reading async

            using (StreamReader reader = new StreamReader(httpWebResponse.GetResponseStream()))
            {
                while (!reader.EndOfStream && currentIteration < limit)
                {
                    string line = reader.ReadLine();
                    string[] values = line.Split(",");

                    // 0 == departure time
                    // 1 == return time
                    // 2 == departure station id
                    // 3 == departure station name
                    // 4 == return station id
                    // 5 == return station name
                    // 6 == coveredDistance in metres
                    // 7 == journey duration is seconds

                    // parse limiting factor strings to int (skip if can't be read for some reason)
                    int coveredDistanceInMetres;
                    try { coveredDistanceInMetres = int.Parse(values[6]); } catch (Exception e) { continue; }
                    int journeyDurationInSeconds;
                    try { journeyDurationInSeconds = int.Parse(values[7]); } catch (Exception e) { continue; }

                    // skip if journeys covered distance is less than 10 metres or duration is less than 10 seconds
                    if (coveredDistanceInMetres < 10 || journeyDurationInSeconds < 10) { continue; }

                    // convert metres to kilometres
                    int coveredDistanceInKilometres = (int)Math.Round((double)coveredDistanceInMetres / 1000d);
                    // convert seconds to minutes
                    int journeyDurationInMinutes = (int)Math.Round((double)journeyDurationInSeconds / 60d);

                    //string coveredDistance = (coveredDistanceInKilometres < 1 ? "" + coveredDistanceInKilometres : "< 1");
                    //string duration = (journeyDurationInMinutes < 1 ? "" + journeyDurationInMinutes : "< 1");

                    // since we got this far line of data should be validated
                    // create new journey class class and populate it
                    Journey journey = new Journey();
                    journey.id = runningNumberForJourneyId;
                    journey.departureTime = values[0];
                    journey.returnTime = values[1];
                    journey.departureStationId = values[2];
                    journey.departureStationName = values[3];
                    journey.returnStationId = values[4];
                    journey.returnStationName = values[5];
                    journey.coveredDistance = coveredDistanceInKilometres;
                    journey.duration = journeyDurationInMinutes;

                    // add journey to journeys list
                    journeys.Add(journey);

                    // increment the running number for journey id
                    runningNumberForJourneyId++;


                    // increment debug limit by one
                    currentIteration++;

                    // show some information of read line
                    System.Diagnostics.Debug.WriteLine("journey" + journey.id + ", departureTime=" + journey.departureTime +
                        ", coveredDistance=" + journey.coveredDistance + ", duration=" + journey.duration);
                }
            }
        }
    }
}
