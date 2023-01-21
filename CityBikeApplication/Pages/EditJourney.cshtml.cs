using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CityBikeApplication.Pages
{
    public class EditJourneyModel : PageModel
    {
        // if data couldn't be validated show user what went wrong
        public List<string> ErrorMessages = new List<string>();

        // store what data user gave
        public Journey OldJourney { get; set; }

        // user can select how many journeys are shown per page
        public List<Station> Choices = new List<Station>();

        public void OnGet()
        {
            GetOldJourney();

            GetChoices();
        }

        public List<Station> GetChoices()
        {
            DataHandler.Instance.SortStations(DataHandler.SortOrder.Id, false);
            return Choices = DataHandler.Instance.Stations;
        }

        public void GetOldJourney()
        {
            if (Request.Query["fromNewStation"].Equals("true"))
            {
                OldJourney = new Journey();
                OldJourney.Id = Request.Query["journeyId"];
                OldJourney.DepartureTime = DateTime.Parse(Request.Query["dt"].ToString().Replace(".", ":"));
                OldJourney.ReturnTime = DateTime.Parse(Request.Query["rt"].ToString().Replace(".", ":"));
                OldJourney.DepartureStationId = int.Parse(Request.Query["ds"]);
                OldJourney.ReturnStationId = int.Parse(Request.Query["rs"]);
                OldJourney.CoveredDistance = int.Parse(Request.Query["cd"]);
                OldJourney.Duration = int.Parse(Request.Query["d"]);
            }
            else
            {
                OldJourney = DataHandler.Instance.GetJourney(Request.Query["id"]);
            }
        }

        public void OnPost()
        {
            // remove previous errorMessages
            ErrorMessages.Clear();

            GetOldJourney();

            Journey newJourney = new Journey();
            newJourney.Id = OldJourney.Id;

            DateTime dt = DateTime.Parse(Request.Form["departureTime"].ToString().Replace(".", ":"));
            newJourney.DepartureTime = dt;

            if (DateTime.Compare(DateTime.Now, dt) < 0)
            {
                ErrorMessages.Add("Given departure time is in the future");
            }

            DateTime rt = DateTime.Parse(Request.Form["returnTime"].ToString().Replace(".", ":"));
            newJourney.ReturnTime = rt;

            if (DateTime.Compare(DateTime.Now, rt) < 0)
            {
                ErrorMessages.Add("Given return time is in the future");
            }

            // check if return time is earlier than departure time
            if (DateTime.Compare(rt, dt) < 0)
            {
                ErrorMessages.Add("Return time is earlier than departure time");
            }

            newJourney.DepartureStationId = int.Parse(Request.Form["departureStationId"]);
            if (newJourney.DepartureStationId > 0)
            {
                newJourney.DepartureStationName = DataHandler.Instance.GetStation(newJourney.DepartureStationId).Name;
            }
            else
            {
                newJourney.ReturnStationName = "";
            }
            newJourney.ReturnStationId = int.Parse(Request.Form["returnStationId"]);
            if(newJourney.ReturnStationId > 0)
            {
                newJourney.ReturnStationName = DataHandler.Instance.GetStation(newJourney.ReturnStationId).Name;
            }
            else
            {
                newJourney.ReturnStationName = "";
            }

            string coveredDistanceString = Sanitize(Request.Form["coveredDistance"]);
            string durationString = Sanitize(Request.Form["duration"]);

            if(coveredDistanceString.Length > 0)
            {
                int coveredDistance;
                if(int.TryParse(coveredDistanceString, out coveredDistance))
                {
                    if (coveredDistance < 0)
                    {
                        ErrorMessages.Add("Covered Distance needs to be integer that is >= 0");
                    }
                    
                    newJourney.CoveredDistance = coveredDistance;
                }
                else
                {
                    ErrorMessages.Add("Covered Distance needs to be integer that is >= 0");
                    // show what was previously
                    newJourney.CoveredDistance = OldJourney.CoveredDistance;
                }
            }
            else
            {
                newJourney.CoveredDistance = 0;
            }

            if(durationString.Length > 0)
            {
                int duration;
                if(int.TryParse(durationString, out duration))
                {
                    if (duration < 0)
                    {
                        ErrorMessages.Add("Duration needs to be integer that is >= 0");
                    }

                    newJourney.Duration = duration;
                }
                else
                {
                    ErrorMessages.Add("Duration needs to be integer that is >= 0");
                    // show what was previously
                    newJourney.Duration = OldJourney.Duration;
                }
            }
            else
            {
                newJourney.Duration = 0;
            }

            // if there were errors remember what data was given
            OldJourney = newJourney;

            bool newDepartureStation = Request.Form["newdeparturestation"].ToString().Equals("true");
            bool newReturnStation = Request.Form["newreturnstation"].ToString().Equals("true");

            if (ErrorMessages.Count == 0)
            {
                if (newDepartureStation)
                {
                    // store info from CreateNewJourney-form to querystring
                    string queryString = "?fromEditJourney=true&" +
                        "departurestation=true&" + 
                        "journeyId=" + newJourney.Id + "&" +
                        "dt=" + dt.ToString("yyyy-MM-ddTHH:mm:ss") + "&" +
                        "rt=" + rt.ToString("yyyy-MM-ddTHH:mm:ss") + "&" +
                        "ds=" + OldJourney.DepartureStationId + "&" +
                        "rs=" + OldJourney.ReturnStationId + "&" +
                        "cd=" + OldJourney.CoveredDistance + "&" +
                        "d=" + OldJourney.Duration;
                    Response.Redirect("CreateNewStation" + queryString);
                }
                else if (newReturnStation)
                {
                    // store info from CreateNewJourney-form to querystring
                    string queryString = "?fromEditJourney=true&" +
                        "returnstation=true&" +
                        "journeyId=" + newJourney.Id + "&" +
                        "dt=" + dt.ToString("yyyy-MM-ddTHH:mm:ss") + "&" +
                        "rt=" + rt.ToString("yyyy-MM-ddTHH:mm:ss") + "&" +
                        "ds=" + OldJourney.DepartureStationId + "&" +
                        "rs=" + OldJourney.ReturnStationId + "&" +
                        "cd=" + OldJourney.CoveredDistance + "&" +
                        "d=" + OldJourney.Duration;
                    Response.Redirect("CreateNewStation" + queryString);
                }
                else
                {
                    DataHandler.Instance.ReplaceJourney(OldJourney.Id, newJourney);
                    Response.Redirect("JourneyList");
                }
            }
        }

        public void OnPostNewDepartureStation(string newStation)
        {
            string oldJourneyId = Request.Query["id"];
            GetOldJourney();
            Response.Redirect("CreateNewStation");
        }

        private string Sanitize(string str)
        {
            // remove special characters
            if(str != null)
            {
                return Regex.Replace(str, "[^a-öA-Ö0-9_ .,()]", "", RegexOptions.Compiled);
            }
            else { return ""; }
        }
    }
}
