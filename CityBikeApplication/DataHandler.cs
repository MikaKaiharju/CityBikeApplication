
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using System.Globalization;

namespace CityBikeApplication
{
    public class DataHandler
    {
        private static DataHandler instance;

        // see if datahandler has completed all tasks
        public bool ready = false;

        // import data sets only once
        private DataHandler()
        {
            ImportDataSets();
        }

        // get instance of datahandler
        public static DataHandler Instance
        {
            get
            {
                return (instance != null ? instance : instance = new DataHandler());
            }
        }

        // for debugging limit amount of lines to be read per data set
        int limit = 500;

        // imported data
        public List<Journey> journeys = new List<Journey>();
        public List<Station> stations = new List<Station>();

        // journey data urls
        //string path1 = "https://dev.hsl.fi/citybikes/od-trips-2021/2021-05.csv";
        //string path2 = "https://dev.hsl.fi/citybikes/od-trips-2021/2021-06.csv";
        //string path3 = "https://dev.hsl.fi/citybikes/od-trips-2021/2021-07.csv";

        // if data is on local hard drive
        string path1 = "C:\\Users\\Kaihiz\\Desktop\\DevAcademy\\2021-05.csv";
        string path2 = "C:\\Users\\Kaihiz\\Desktop\\DevAcademy\\2021-06.csv";
        string path3 = "C:\\Users\\Kaihiz\\Desktop\\DevAcademy\\2021-07.csv";

        // station data urls
        //string path4 = "https://opendata.arcgis.com/datasets/726277c507ef4914b0aec3cbcfcbfafc_0.csv";

        // if data is on local hard drive
        string path4 = "C:\\Users\\Kaihiz\\Desktop\\DevAcademy\\Helsingin_ja_Espoon_kaupunkipyB6rA4asemat_avoin.csv";

        private async void ImportDataSets()
        {
            // create timestamp to calculate how long import took
            UpdateProgress("Import started");
            DateTime dt1 = DateTime.Now;

            // create async tasks for reading data
            List<Task> tasks = new List<Task>();
            tasks.Add(GetJourneyListAsync(path1));
            tasks.Add(GetJourneyListAsync(path2));
            tasks.Add(GetJourneyListAsync(path3));
            tasks.Add(GetStationListAsync(path4));

            await Task.WhenAll(tasks);
            foreach (Task task in tasks)
            {
                if (task.GetType().Equals(typeof(Task<List<Journey>>)))
                {
                    journeys.AddRange(((Task<List<Journey>>)task).Result);
                }
                if (task.GetType().Equals(typeof(Task<List<Station>>)))
                {
                    stations.AddRange(((Task<List<Station>>)task).Result);
                }
            }

            // calculation of how long import took
            DateTime dt2 = DateTime.Now;
            TimeSpan ts = dt2.Subtract(dt1);
            UpdateProgress("Import finished in " + (int)ts.TotalSeconds + " seconds with journey count of " + journeys.Count +
                " and station count of " + stations.Count);

            ready = true;
        }

        private Task<List<Journey>> GetJourneyListAsync(string path)
        {
            return Task.Run(() => ImportJourneyData(path));
        }
        private Task<List<Station>> GetStationListAsync(string path)
        {
            return Task.Run(() => ImportStationData(path));
        }


        private List<Journey> ImportJourneyData(string path)
        {
            //  import journey data
            List<Journey> importedJourneys = new List<Journey>();

            // for debugging how many lines have been read
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
                    journey.id = Guid.NewGuid().ToString();
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

        private List<Station> ImportStationData(string path)
        {
            List<Station> importedStations = new List<Station>();

            // for debugging how many lines have been read
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

                    // 0 == FID
                    // 1 == id
                    // 2 == nimi
                    // 3 == namn
                    // 4 == name
                    // 5 == osoite
                    // 6 == address
                    // 7 == kaupunki
                    // 8 == stad
                    // 9 == operaattori
                    // 10 == kapasiteetti
                    // 11 == x
                    // 12 == y

                    // parse coordinate strings to double (skip if can't be read for some reason)
                    double x;
                    try { x = double.Parse(values[11], CultureInfo.InvariantCulture); } catch (Exception e) { UpdateProgress("values[11]=" + values[11] + "e1=" + e.ToString()); continue; }
                    double y;
                    try { y = double.Parse(values[12], CultureInfo.InvariantCulture); } catch (Exception e) { UpdateProgress("values[12]=" + values[12] + "e2=" + e.ToString()); continue; }

                    // since we got this far line of data should be validated
                    // create new station class class and populate it
                    Station station = new Station();
                    station.id = values[1];
                    station.nimi = values[2];
                    station.namn = values[3];
                    station.name = values[4];
                    station.osoite = values[5];
                    station.address = values[6];
                    station.kaupunki = values[7];
                    station.stad = values[8];
                    station.operaattori = values[9];
                    station.kapasiteetti = values[10];
                    station.x = x;
                    station.y = y;

                    // add station to station list
                    importedStations.Add(station);
                }
            }

            return importedStations;

        }


        private void UpdateProgress(string s)
        {
            System.Diagnostics.Debug.WriteLine(s);
        }

    }


}
