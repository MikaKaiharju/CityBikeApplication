
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
        private static DataHandler _instance;

        // see if datahandler has completed all tasks
        public bool Ready = false;

        // store sort order to invert order
        SortOrder _currentJourneySortOrder;
        // is current sort order ascending
        bool _ascendingJourneyOrder = true;

        // store sort order to invert order
        SortOrder _currentStationSortOrder;
        // is current sort order ascending
        bool _ascendingStationOrder = true;

        public enum SortOrder
        {
            Id,
            Name,
            Osoite,
            Kaupunki,
            Operaattori,
            Kapasiteetti,
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

        // for debugging limit amount of lines to be read per data set
        int _limit = 500;

        // imported data
        public List<Journey> Journeys = new List<Journey>();
        public List<Station> Stations = new List<Station>();

        // journey data urls
        //string _path1 = "https://dev.hsl.fi/citybikes/od-trips-2021/2021-05.csv";
        //string _path2 = "https://dev.hsl.fi/citybikes/od-trips-2021/2021-06.csv";
        //string _path3 = "https://dev.hsl.fi/citybikes/od-trips-2021/2021-07.csv";

        // if data is on local hard drive
        string _path1 = "C:\\Users\\Kaihiz\\Desktop\\DevAcademy\\2021-05.csv";
        string _path2 = "C:\\Users\\Kaihiz\\Desktop\\DevAcademy\\2021-06.csv";
        string _path3 = "C:\\Users\\Kaihiz\\Desktop\\DevAcademy\\2021-07.csv";

        // station data urls
        //string _path4 = "https://opendata.arcgis.com/datasets/726277c507ef4914b0aec3cbcfcbfafc_0.csv";

        // if data is on local hard drive
        string _path4 = "C:\\Users\\Kaihiz\\Desktop\\DevAcademy\\Helsingin_ja_Espoon_kaupunkipyB6rA4asemat_avoin.csv";

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
                    journey.Id = Guid.NewGuid().ToString();
                    journey.DepartureTime = values[0];
                    journey.ReturnTime = values[1];
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
                while (!reader.EndOfStream && currentIteration++ < _limit)
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
                    station.Id = id;
                    station.Nimi = values[2];
                    station.Namn = values[3];
                    station.Name = values[4];
                    station.Osoite = values[5];
                    station.Address = values[6];
                    station.Kaupunki = values[7];
                    station.Stad = values[8];
                    station.Operaattori = values[9];
                    station.Kapasiteetti = kapasiteetti;
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
            foreach(Station station in Stations)
            {
                if (station.Id == id)
                {
                    return station;
                }
            }
            return null;
        }

        public Journey GetJourney(string id)
        {
            foreach(Journey journey in Journeys)
            {
                if (journey.Id.Equals(id))
                {
                    return journey;
                }
            }
            return null;
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
            SortJourneys(_currentJourneySortOrder, false);
        }

        public void CreateNewStation(Station newStation)
        {
            Stations.Add(newStation);
            SortStations(_currentStationSortOrder, false);
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
                _ascendingStationOrder = _currentStationSortOrder.Equals(sortOrder) ? !_ascendingStationOrder : true;
            }

            // store latest sortOrder
            _currentStationSortOrder = sortOrder;

            switch(sortOrder, _ascendingStationOrder)
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
                case (SortOrder.Osoite, true):
                    sortedStations = sortedStations.OrderBy(s => s.Osoite);
                    break;
                case (SortOrder.Osoite, false):
                    sortedStations = sortedStations.OrderByDescending(s => s.Osoite);
                    break;
                case (SortOrder.Kaupunki, true):
                    sortedStations = sortedStations.OrderBy(s => s.Kaupunki);
                    break;
                case (SortOrder.Kaupunki, false):
                    sortedStations = sortedStations.OrderByDescending(s => s.Kaupunki);
                    break;
                case (SortOrder.Operaattori, true):
                    sortedStations = sortedStations.OrderBy(s => s.Operaattori);
                    break;
                case (SortOrder.Operaattori, false):
                    sortedStations = sortedStations.OrderByDescending(s => s.Operaattori);
                    break;
                case (SortOrder.Kapasiteetti, true):
                    sortedStations = sortedStations.OrderBy(s => s.Kapasiteetti);
                    break;
                case (SortOrder.Kapasiteetti, false):
                    sortedStations = sortedStations.OrderByDescending(s => s.Kapasiteetti);
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
                _ascendingJourneyOrder = _currentJourneySortOrder.Equals(sortOrder) ? !_ascendingJourneyOrder : true;
            }

            // store latest sortOrder
            _currentJourneySortOrder = sortOrder;

            switch (sortOrder, _ascendingJourneyOrder)
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
                    sortedJourneys = sortedJourneys.OrderBy(j => j.DepartureStationName);
                    break;
                case (SortOrder.DepartureStation, false):
                    sortedJourneys = sortedJourneys.OrderByDescending(j => j.DepartureStationName);
                    break;
                case (SortOrder.ReturnStation, true):
                    sortedJourneys = sortedJourneys.OrderBy(j => j.ReturnStationName);
                    break;
                case (SortOrder.ReturnStation, false):
                    sortedJourneys = sortedJourneys.OrderByDescending(j => j.ReturnStationName);
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

        private void p(string s)
        {
            System.Diagnostics.Debug.WriteLine(s);
        }


    }


}
