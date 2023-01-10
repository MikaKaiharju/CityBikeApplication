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
        
        List<Journey> journeys = new List<Journey>();
        List<Station> stations = new List<Station>();

        // journey data paths
        //string path1 = "https://dev.hsl.fi/citybikes/od-trips-2021/2021-05.csv";
        //string path2 = "https://dev.hsl.fi/citybikes/od-trips-2021/2021-06.csv";
        //string path3 = "https://dev.hsl.fi/citybikes/od-trips-2021/2021-07.csv";

        string path1 = "C:\\Users\\Kaihiz\\Desktop\\DevAcademy\\2021-05.csv";
        string path2 = "C:\\Users\\Kaihiz\\Desktop\\DevAcademy\\2021-06.csv";
        string path3 = "C:\\Users\\Kaihiz\\Desktop\\DevAcademy\\2021-07.csv";

        // station data path
        string path4 = "https://opendata.arcgis.com/datasets/726277c507ef4914b0aec3cbcfcbfafc_0.csv";

        public async void OnGet()
        {

            
            // create timestamp to calculate how long import took
            UpdateProgress("Import started");
            DateTime dt1 = DateTime.Now;

            // create async tasks for reading data
            List<Task> tasks = new List<Task>();
            tasks.Add(GetListAsync(path1));
            tasks.Add(GetListAsync(path2));
            tasks.Add(GetListAsync(path3));

            await Task.WhenAll(tasks);
            foreach(Task task in tasks)
            {
                journeys.AddRange(((Task<List<Journey>>) task).Result);
            }

            // calculation of how long import took
            DateTime dt2 = DateTime.Now;
            TimeSpan ts = dt2.Subtract(dt1);
            UpdateProgress("Import finished in " + (int) ts.TotalSeconds + " seconds with journey count of " + journeys.Count);
        }

        public Task<List<Journey>> GetListAsync(string path)
        {
            return Task.Run(() => ImportJourneyData(path));
        }

        public List<Journey> ImportJourneyData(string path)
        {
            //  import journey data
            List<Journey> importedJourneys = new List<Journey>();

            // for debugging limit amount of lines to be read
            int limit = 50000;
            int currentIteration = 0;

            // read data from url
            //HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(path);
            //HttpWebResponse httpWebResponse = (HttpWebResponse) httpWebRequest.GetResponse();

            //using (StreamReader reader = new StreamReader(httpWebResponse.GetResponseStream()))
            using (StreamReader reader = new StreamReader(path))
            {
                while (!reader.EndOfStream && currentIteration++ < limit)
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

                    // since we got this far line of data should be validated
                    // create new journey class class and populate it
                    Journey journey = new Journey();
                    journey.departureTime = values[0];
                    journey.returnTime = values[1];
                    journey.departureStationId = values[2];
                    journey.departureStationName = values[3];
                    journey.returnStationId = values[4];
                    journey.returnStationName = values[5];
                    journey.coveredDistance = coveredDistanceInKilometres;
                    journey.duration = journeyDurationInMinutes;

                    // add journey to journeys list
                    importedJourneys.Add(journey);
                }
            }

            return importedJourneys;
        }

        public void UpdateProgress(string s)
        {
            System.Diagnostics.Debug.WriteLine(s);
        }

    }
}
