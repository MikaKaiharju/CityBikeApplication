
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using System.Globalization;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using System.Threading;

namespace CityBikeApplication
{
    public class DataHandler
    {
        private static DataHandler _instance;

        // see if datahandler has completed all tasks
        public bool Ready { get; private set; } = false;

        // store sort order to invert order
        public SortOrder CurrentJourneySortOrder { get; private set; }
        // is current sort order ascending
        public bool AscendingJourneyOrder { get; private set; } = true;

        // store sort order to invert order
        public SortOrder CurrentStationSortOrder { get; private set; }
        // is current sort order ascending
        public bool AscendingStationOrder { get; private set; } = true;

        public enum SortOrder
        {
            Null,
            Id,
            Name,
            Address,
            City,
            Operator,
            Capacity,
            DepartureTime,
            ReturnTime,
            DepartureStation,
            ReturnStation,
            CoveredDistance,
            Duration
        }

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
                return (_instance != null ? _instance : _instance = new DataHandler());
            }
        }

        public bool UntilFinished()
        {
            while (!Ready)
            {
                Thread.Sleep(100);
            }
            return true;
        }

        // for debugging limit amount of lines to be read per data set
        int _limit = int.MaxValue;

        // imported data
        public List<Journey> Journeys { get; set; } = new List<Journey>();
        public List<Station> Stations { get; set; } = new List<Station>();

        // journey data urls
        string _path1 = "https://dev.hsl.fi/citybikes/od-trips-2021/2021-05.csv";
        string _path2 = "https://dev.hsl.fi/citybikes/od-trips-2021/2021-06.csv";
        string _path3 = "https://dev.hsl.fi/citybikes/od-trips-2021/2021-07.csv";

        // station data urls
        string _path4 = "https://opendata.arcgis.com/datasets/726277c507ef4914b0aec3cbcfcbfafc_0.csv";

        private async void ImportDataSets()
        {         
            // create async tasks for reading data
            List<Task> tasks = new List<Task>();
            tasks.Add(GetJourneyListAsync(_path1));
            tasks.Add(GetJourneyListAsync(_path2));
            tasks.Add(GetJourneyListAsync(_path3));
            tasks.Add(GetStationListAsync(_path4));

            await Task.WhenAll(tasks);
            foreach (Task task in tasks)
            {
                if (task.GetType().Equals(typeof(Task<List<Journey>>)))
                {
                    Journeys.AddRange(((Task<List<Journey>>)task).Result);
                }
                if (task.GetType().Equals(typeof(Task<List<Station>>)))
                {
                    Stations.AddRange(((Task<List<Station>>)task).Result);
                }
            }

            Ready = true;
        }

        private Task<List<Journey>> GetJourneyListAsync(string path)
        {
            return Task.Run(() => ImportJourneyData(path));
        }
        private Task<List<Station>> GetStationListAsync(string path)
        {
            return Task.Run(() => ImportStationData(path));
        }

        public List<Journey> ImportJourneyData(string path)
        {
            //  import journey data
            List<Journey> importedJourneys = new List<Journey>();

            // for debugging how many lines have been read
            int currentIteration = 0;

            StreamReader reader;

            // check if path is local or internet
            if (new Uri(path).IsFile)
            {
                // read data from local file
                reader = new StreamReader(path);
            }
            else
            {
                // read data from url
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(path);
                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                reader = new StreamReader(httpWebResponse.GetResponseStream());
            }

            using (reader)
            {
                while (!reader.EndOfStream && currentIteration++ < _limit)
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

                    // skip if there is not sufficient amount of values
                    if (values.Length != 8) { continue; }

                    // parse dateTimes
                    if (!DateTime.TryParse(values[0], out DateTime departureTime)) { continue; }
                    if (!DateTime.TryParse(values[1], out DateTime returnTime)) { continue; }

                    // parse stationIds
                    if (int.TryParse(values[2], out int departureStationId)) { } else { continue; }
                    if (int.TryParse(values[4], out int returnStationId)) { } else { continue; }

                    // parse limiting factor strings to int (skip if can't be read for some reason)
                    if (int.TryParse(values[6], out int coveredDistanceInMetres)) { } else { continue; }
                    if (int.TryParse(values[7], out int journeyDurationInSeconds)) { } else { continue; }

                    // skip if journeys covered distance is less than 10 metres or duration is less than 10 seconds
                    if (coveredDistanceInMetres < 10 || journeyDurationInSeconds < 10) { continue; }

                    // convert metres to kilometres
                    int coveredDistanceInKilometres = (int)Math.Round((double)coveredDistanceInMetres / 1000d);

                    // convert seconds to minutes
                    int journeyDurationInMinutes = (int)Math.Round((double)journeyDurationInSeconds / 60d);

                    // since we got this far line of data should be validated
                    // create new journey class class and populate it
                    Journey journey = new Journey();
                    journey.Id = Guid.NewGuid().ToString();
                    journey.DepartureTime = departureTime;
                    journey.ReturnTime = returnTime;
                    journey.DepartureStationId = departureStationId;
                    journey.DepartureStationName = values[3];
                    journey.ReturnStationId = returnStationId;
                    journey.ReturnStationName = values[5];
                    journey.CoveredDistance = coveredDistanceInKilometres;
                    journey.Duration = journeyDurationInMinutes;

                    // add journey to journeys list
                    importedJourneys.Add(journey);
                }
            }

            return importedJourneys;
        }

        public List<Station> ImportStationData(string path)
        {
            List<Station> importedStations = new List<Station>();

            // for debugging how many lines have been read
            int currentIteration = 0;

            StreamReader reader;

            // check if path is local or internet
            if (new Uri(path).IsFile)
            {
                // read data from local file
                reader = new StreamReader(path);
            }
            else
            {
                // read data from url
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(path);
                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                reader = new StreamReader(httpWebResponse.GetResponseStream());
            }

            using (reader)
            {
                while (!reader.EndOfStream && currentIteration++ < _limit)
                {
                    string line = reader.ReadLine();
                    string[] values = line.Split(",");

                    // 0 == FID
                    // 1 == id
                    // 2 == name in Finnish
                    // 3 == name in Swedish
                    // 4 == name in English
                    // 5 == address in Finnish
                    // 6 == address in Swedish
                    // 7 == city in Finnish
                    // 8 == city in Swedish
                    // 9 == operator
                    // 10 == capacity
                    // 11 == x
                    // 12 == y

                    // skip if there is not sufficient amount of values
                    if (values.Length != 13) { continue; }

                    // parse station id into int
                    if (int.TryParse(values[1], out int id)) { } else { continue; }

                    // parse coordinate strings to double (skip if can't be read for some reason)
                    if (double.TryParse(values[11], NumberStyles.Any, CultureInfo.InvariantCulture, out double x)) { } else { continue; }
                    if (double.TryParse(values[12], NumberStyles.Any, CultureInfo.InvariantCulture, out double y)) { } else { continue; }

                    // parse station capacity into int
                    if (int.TryParse(values[10], out int capacity)) { } else { continue; }

                    // name: primary english, secondary finnish, tertiary swedish
                    string name = !values[4].Equals("") ? values[4] : !values[2].Equals("") ? values[2] : values[3];

                    // address: primary finnish, secondary swedish
                    string address = !values[5].Equals("") ? values[5] : values[6];

                    // city: primary finnish, secondary swedish
                    string city = !values[7].Equals("") ? values[7] : values[8];

                    // since we got this far line of data should be validated
                    // create new station class class and populate it
                    Station station = new Station();
                    station.Id = id;
                    station.Name = name;
                    station.Address = address;
                    station.City = city;
                    station.Operator = values[9];
                    station.Capacity = capacity;
                    station.X = ("" + x).Replace(",", "."); // store in dot form
                    station.Y = ("" + y).Replace(",", ".");

                    // add station to station list
                    importedStations.Add(station);
                }
            }

            return importedStations;

        }

        public Station GetStation(int id)
        {
            return Stations.FirstOrDefault(s => s.Id == id);
        }

        public Journey GetJourney(string id)
        {
            return Journeys.FirstOrDefault(j => j.Id.Equals(id));
        }

        public void ReplaceStation(int oldStationId, Station newStation)
        {
            Station oldStation = GetStation(oldStationId);
            int index = Stations.IndexOf(oldStation);
            Stations.RemoveAt(index);
            Stations.Insert(index, newStation);
        }

        public void ReplaceJourney(string oldJourneyId, Journey newJourney)
        {
            Journey oldJourney = GetJourney(oldJourneyId);
            int index = Journeys.IndexOf(oldJourney);
            Journeys.RemoveAt(index);
            Journeys.Insert(index, newJourney);
        }

        public void CreateNewJourney(Journey newJourney)
        {
            Journeys.Add(newJourney);
            SortJourneys(CurrentJourneySortOrder, false);
        }

        public void CreateNewStation(Station newStation)
        {
            Stations.Add(newStation);
            SortStations(CurrentStationSortOrder, false);
        }

        public void DeleteStation(int id)
        {
            Stations.Remove(GetStation(id));
        }

        public void DeleteJourney(string id)
        {
            Journeys.Remove(GetJourney(id));
        }

        public void SortStations(SortOrder sortOrder)
        {
            // use normal inversion rules (pressing twice inverts order)
            SortStations(sortOrder, true);
        }

        public void SortStations(SortOrder sortOrder, bool useInversion)
        {
            // use of IQueryable to ease sorting
            IQueryable<Station> sortedStations = from s in Stations.AsQueryable<Station>() select s;

            if (useInversion)
            {
                // if header is pressed twice sorting inverts
                AscendingStationOrder = CurrentStationSortOrder.Equals(sortOrder) ? !AscendingStationOrder : true;
            }
            else
            {
                AscendingStationOrder = true;
            }

            // store latest sortOrder
            CurrentStationSortOrder = sortOrder;

            switch(sortOrder, AscendingStationOrder)
            {
                case (SortOrder.Id, true):
                    sortedStations = sortedStations.OrderBy(s => s.Id);
                    break;
                case (SortOrder.Id, false):
                    sortedStations = sortedStations.OrderByDescending(s => s.Id);
                    break;
                case (SortOrder.Name, true):
                    sortedStations = sortedStations.OrderBy(s => s.Name);
                    break;
                case (SortOrder.Name, false):
                    sortedStations = sortedStations.OrderByDescending(s => s.Name);
                    break;
                case (SortOrder.Address, true):
                    sortedStations = sortedStations.OrderBy(s => s.Address);
                    break;
                case (SortOrder.Address, false):
                    sortedStations = sortedStations.OrderByDescending(s => s.Address);
                    break;
                case (SortOrder.City, true):
                    sortedStations = sortedStations.OrderBy(s => s.City);
                    break;
                case (SortOrder.City, false):
                    sortedStations = sortedStations.OrderByDescending(s => s.City);
                    break;
                case (SortOrder.Operator, true):
                    sortedStations = sortedStations.OrderBy(s => s.Operator);
                    break;
                case (SortOrder.Operator, false):
                    sortedStations = sortedStations.OrderByDescending(s => s.Operator);
                    break;
                case (SortOrder.Capacity, true):
                    sortedStations = sortedStations.OrderBy(s => s.Capacity);
                    break;
                case (SortOrder.Capacity, false):
                    sortedStations = sortedStations.OrderByDescending(s => s.Capacity);
                    break;
                default:
                    sortedStations = sortedStations.OrderBy(s => s.Id);
                    break;
            }

            // convert IQueryable back to list
            Stations = sortedStations.ToList<Station>();
        }

        public void SortJourneys(SortOrder sortOrder)
        {
            // use normal inversion rules (pressing twice inverts order)
            SortJourneys(sortOrder, true);
        }

        public void SortJourneys(SortOrder sortOrder, bool useInversion)
        {
            // use of IQueryable to ease sorting
            IQueryable<Journey> sortedJourneys = from j in Journeys.AsQueryable<Journey>() select j;

            if (useInversion)
            {
                // if header is pressed twice sorting inverts
                AscendingJourneyOrder = CurrentJourneySortOrder.Equals(sortOrder) ? !AscendingJourneyOrder : true;
            }

            // store latest sortOrder
            CurrentJourneySortOrder = sortOrder;

            switch (sortOrder, AscendingJourneyOrder)
            {
                case (SortOrder.DepartureTime, true):
                    sortedJourneys = sortedJourneys.OrderBy(j => j.DepartureTime);
                    break;
                case (SortOrder.DepartureTime, false):
                    sortedJourneys = sortedJourneys.OrderByDescending(j => j.DepartureTime);
                    break;
                case (SortOrder.ReturnTime, true):
                    sortedJourneys = sortedJourneys.OrderBy(j => j.ReturnTime);
                    break;
                case (SortOrder.ReturnTime, false):
                    sortedJourneys = sortedJourneys.OrderByDescending(j => j.ReturnTime);
                    break;
                case (SortOrder.DepartureStation, true):
                    sortedJourneys = sortedJourneys.OrderBy(j => j.DepartureStationId);
                    break;
                case (SortOrder.DepartureStation, false):
                    sortedJourneys = sortedJourneys.OrderByDescending(j => j.DepartureStationId);
                    break;
                case (SortOrder.ReturnStation, true):
                    sortedJourneys = sortedJourneys.OrderBy(j => j.ReturnStationId);
                    break;
                case (SortOrder.ReturnStation, false):
                    sortedJourneys = sortedJourneys.OrderByDescending(j => j.ReturnStationId);
                    break;
                case (SortOrder.CoveredDistance, true):
                    sortedJourneys = sortedJourneys.OrderBy(j => j.CoveredDistance);
                    break;
                case (SortOrder.CoveredDistance, false):
                    sortedJourneys = sortedJourneys.OrderByDescending(j => j.CoveredDistance);
                    break;
                case (SortOrder.Duration, true):
                    sortedJourneys = sortedJourneys.OrderBy(j => j.Duration);
                    break;
                case (SortOrder.Duration, false):
                    sortedJourneys = sortedJourneys.OrderByDescending(j => j.Duration);
                    break;
                default:
                    // as default sort by first column
                    sortedJourneys = sortedJourneys.OrderBy(j => j.DepartureTime);
                    break;
            }

            // convert IQueryable back to list
            Journeys = sortedJourneys.ToList<Journey>();
        }
    }
}
