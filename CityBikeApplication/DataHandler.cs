﻿
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using System.Globalization;
using System.Linq;

namespace CityBikeApplication
{
    public class DataHandler
    {
        private static DataHandler instance;

        // see if datahandler has completed all tasks
        public bool ready = false;

        // store sort order to invert order
        string currentJourneySortOrder = "";
        // is current sort order ascending
        bool ascendingJourneyOrder = true;

        // store sort order to invert order
        string currentStationSortOrder = "";
        // is current sort order ascending
        bool ascendingStationOrder = true;

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

                    // parse stationIds
                    int departureStationId;
                    try { departureStationId = int.Parse(values[2]); } catch(Exception e) { continue; }
                    int returnStationId;
                    try { returnStationId = int.Parse(values[4]); } catch (Exception e) { continue; }

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
                    journey.departureStationId = departureStationId;
                    journey.departureStationName = values[3];
                    journey.returnStationId = returnStationId;
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

                    // parse station id into int
                    int id;
                    try { id = int.Parse(values[1]); } catch(Exception e) { continue; }

                    // parse coordinate strings to double (skip if can't be read for some reason)
                    double x;
                    try { x = double.Parse(values[11], CultureInfo.InvariantCulture); } catch (Exception e) { continue; }
                    double y;
                    try { y = double.Parse(values[12], CultureInfo.InvariantCulture); } catch (Exception e) { continue; }

                    // parse station capacity into int
                    int kapasiteetti;
                    try { kapasiteetti = int.Parse(values[10]); } catch(Exception e) { continue; }

                    // since we got this far line of data should be validated
                    // create new station class class and populate it
                    Station station = new Station();
                    station.id = id;
                    station.nimi = values[2];
                    station.namn = values[3];
                    station.name = values[4];
                    station.osoite = values[5];
                    station.address = values[6];
                    station.kaupunki = values[7];
                    station.stad = values[8];
                    station.operaattori = values[9];
                    station.kapasiteetti = kapasiteetti;
                    station.x = ("" + x).Replace(",", "."); // store in dot form
                    station.y = ("" + y).Replace(",", ".");

                    // add station to station list
                    importedStations.Add(station);
                }
            }

            return importedStations;

        }

        public Station GetStation(int id)
        {
            foreach(Station station in stations)
            {
                if (station.id == id)
                {
                    return station;
                }
            }
            return null;
        }

        public Journey GetJourney(string id)
        {
            foreach(Journey journey in journeys)
            {
                if (journey.id.Equals(id))
                {
                    return journey;
                }
            }
            return null;
        }

        public void ReplaceStation(int oldStationId, Station newStation)
        {
            Station oldStation = GetStation(oldStationId);
            int index = stations.IndexOf(oldStation);
            stations.RemoveAt(index);
            stations.Insert(index, newStation);
        }

        public void ReplaceJourney(string oldJourneyId, Journey newJourney)
        {
            Journey oldJourney = GetJourney(oldJourneyId);
            int index = journeys.IndexOf(oldJourney);
            journeys.RemoveAt(index);
            journeys.Insert(index, newJourney);
        }

        public void CreateNewJourney(Journey newJourney)
        {
            journeys.Add(newJourney);
            SortJourneys(currentJourneySortOrder, false);
        }

        public void CreateNewStation(Station newStation)
        {
            stations.Add(newStation);
            SortStations(currentStationSortOrder, false);
        }

        public void DeleteStation(int id)
        {
            stations.Remove(GetStation(id));
        }

        public void DeleteJourney(string id)
        {
            journeys.Remove(GetJourney(id));
        }

        

        public void SortStations(string sortOrder)
        {
            // use normal inversion rules (pressing twice inverts order)
            SortStations(sortOrder, true);
        }

        public void SortStations(string sortOrder, bool useInversion)
        {
            // use of IQueryable to ease sorting
            IQueryable<Station> sortedStations = from s in stations.AsQueryable<Station>() select s;

            if (useInversion)
            {
                // if header is pressed twice sorting inverts
                ascendingStationOrder = currentStationSortOrder.Equals(sortOrder) ? !ascendingStationOrder : true;
            }

            // store latest sortOrder
            currentStationSortOrder = sortOrder;

            switch(sortOrder, ascendingStationOrder)
            {
                case ("id", true):
                    sortedStations = sortedStations.OrderBy(s => s.id);
                    break;
                case ("id", false):
                    sortedStations = sortedStations.OrderByDescending(s => s.id);
                    break;
                case ("name", true):
                    sortedStations = sortedStations.OrderBy(s => s.name);
                    break;
                case ("name", false):
                    sortedStations = sortedStations.OrderByDescending(s => s.name);
                    break;
                case ("osoite", true):
                    sortedStations = sortedStations.OrderBy(s => s.osoite);
                    break;
                case ("osoite", false):
                    sortedStations = sortedStations.OrderByDescending(s => s.osoite);
                    break;
                case ("kaupunki", true):
                    sortedStations = sortedStations.OrderBy(s => s.kaupunki);
                    break;
                case ("kaupunki", false):
                    sortedStations = sortedStations.OrderByDescending(s => s.kaupunki);
                    break;
                case ("operaattori", true):
                    sortedStations = sortedStations.OrderBy(s => s.operaattori);
                    break;
                case ("operaattori", false):
                    sortedStations = sortedStations.OrderByDescending(s => s.operaattori);
                    break;
                case ("kapasiteetti", true):
                    sortedStations = sortedStations.OrderBy(s => s.kapasiteetti);
                    break;
                case ("kapasiteetti", false):
                    sortedStations = sortedStations.OrderByDescending(s => s.kapasiteetti);
                    break;
                default:
                    sortedStations = sortedStations.OrderBy(s => s.id);
                    break;
            }

            // convert IQueryable back to list
            stations = sortedStations.ToList<Station>();
        }

        public void SortJourneys(string sortOrder)
        {
            // use normal inversion rules (pressing twice inverts order)
            SortJourneys(sortOrder, true);
        }

        public void SortJourneys(string sortOrder, bool useInversion)
        {
            // use of IQueryable to ease sorting
            IQueryable<Journey> sortedJourneys = from j in journeys.AsQueryable<Journey>() select j;

            if (useInversion)
            {
                // if header is pressed twice sorting inverts
                ascendingJourneyOrder = currentJourneySortOrder.Equals(sortOrder) ? !ascendingJourneyOrder : true;
            }

            // store latest sortOrder
            currentJourneySortOrder = sortOrder;

            switch (sortOrder, ascendingJourneyOrder)
            {
                case ("departureTime", true):
                    sortedJourneys = sortedJourneys.OrderBy(j => j.departureTime);
                    break;
                case ("departureTime", false):
                    sortedJourneys = sortedJourneys.OrderByDescending(j => j.departureTime);
                    break;
                case ("returnTime", true):
                    sortedJourneys = sortedJourneys.OrderBy(j => j.returnTime);
                    break;
                case ("returnTime", false):
                    sortedJourneys = sortedJourneys.OrderByDescending(j => j.returnTime);
                    break;
                case ("departureStation", true):
                    sortedJourneys = sortedJourneys.OrderBy(j => j.departureStationName);
                    break;
                case ("departureStation", false):
                    sortedJourneys = sortedJourneys.OrderByDescending(j => j.departureStationName);
                    break;
                case ("returnStation", true):
                    sortedJourneys = sortedJourneys.OrderBy(j => j.returnStationName);
                    break;
                case ("returnStation", false):
                    sortedJourneys = sortedJourneys.OrderByDescending(j => j.returnStationName);
                    break;
                case ("coveredDistance", true):
                    sortedJourneys = sortedJourneys.OrderBy(j => j.coveredDistance);
                    break;
                case ("coveredDistance", false):
                    sortedJourneys = sortedJourneys.OrderByDescending(j => j.coveredDistance);
                    break;
                case ("duration", true):
                    sortedJourneys = sortedJourneys.OrderBy(j => j.duration);
                    break;
                case ("duration", false):
                    sortedJourneys = sortedJourneys.OrderByDescending(j => j.duration);
                    break;
                default:
                    // as default sort by first column
                    sortedJourneys = sortedJourneys.OrderBy(j => j.departureTime);
                    break;
            }

            // convert IQueryable back to list
            journeys = sortedJourneys.ToList<Journey>();
        }

        private void p(string s)
        {
            System.Diagnostics.Debug.WriteLine(s);
        }


    }


}
