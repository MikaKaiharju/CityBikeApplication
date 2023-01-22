using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace CityBikeApplication.Pages
{
    public class StationInfoModel : PageModel
    {

        // store station info
        public Station Station { get; set; }
        
        public void OnGet()
        {

        }

        public void GetStation()
        {
            Station = DataHandler.Instance.GetStation(int.Parse(Request.Query["id"]));
        }

        public int TotalNumberOfJourneysFrom { get; set; } = 0;
        public int TotalNumberOfJourneysTo { get; set; } = 0;
        public double AverageJourneyDistaceFrom { get; set; } = 0;
        public double AverageJourneyDistanceTo { get; set; } = 0;
        public Dictionary<Station, int> MostPopularReturnStations { get; set; } = new Dictionary<Station, int>();
        public Dictionary<Station, int> MostPopularDepartureStations { get; set; } = new Dictionary<Station, int>();

        public void GetAdditionalInfo()
        {
            GetStation();

            int journeyDistanceFrom = 0;
            int journeyDistanceTo = 0;
            Dictionary<int, int> returnStations = new Dictionary<int, int>();
            Dictionary<int, int> departureStations = new Dictionary<int, int>();

            List<Journey> journeys = DataHandler.Instance.Journeys;

            foreach(Journey journey in journeys)
            {
                if(journey.DepartureStationId == Station.Id)
                {
                    TotalNumberOfJourneysFrom++;
                    journeyDistanceFrom += journey.CoveredDistance;
                    if (returnStations.ContainsKey(journey.ReturnStationId))
                    {
                        returnStations.TryGetValue(journey.ReturnStationId, out int value);
                        value++;
                        returnStations.Remove(journey.ReturnStationId);
                        returnStations.Add(journey.ReturnStationId, value);
                    }
                    else
                    {
                        returnStations.Add(journey.ReturnStationId, 1);
                    }
                }

                if(journey.ReturnStationId == Station.Id)
                {
                    TotalNumberOfJourneysTo++;
                    journeyDistanceTo += journey.CoveredDistance;
                    if (departureStations.ContainsKey(journey.DepartureStationId))
                    {
                        departureStations.TryGetValue(journey.DepartureStationId, out int value);
                        value++;
                        departureStations.Remove(journey.DepartureStationId);
                        departureStations.Add(journey.DepartureStationId, value);
                    }
                    else
                    {
                        departureStations.Add(journey.DepartureStationId, 1);
                    }
                }
            }

            AverageJourneyDistaceFrom = (double)journeyDistanceFrom / (double)TotalNumberOfJourneysFrom;
            AverageJourneyDistanceTo = (double)journeyDistanceTo / (double)TotalNumberOfJourneysTo;

            var sortedReturnStations = from entry in returnStations orderby entry.Value descending select entry;
            var sortedDepartureStations = from entry in departureStations orderby entry.Value descending select entry;

            foreach(var item in sortedReturnStations.Take(5))
            {
                MostPopularReturnStations.Add(DataHandler.Instance.GetStation(item.Key), item.Value);
            }
            foreach (var item in sortedDepartureStations.Take(5))
            {
                MostPopularDepartureStations.Add(DataHandler.Instance.GetStation(item.Key), item.Value);
            }
        }

        public void OnPost()
        {
            GetStation();

            var queryParams = new Dictionary<string, string>()
            {
                {"id", "" + Station.Id },
                {"cameFromStationInfo", "true" }
            };
            if (Request.Query["cameFromStationList"].Equals("true"))
            {
                queryParams.Add("cameFromStationList", "true");
            }
            Response.Redirect(QueryHelpers.AddQueryString("EditStation", queryParams));
        }

    }
}
