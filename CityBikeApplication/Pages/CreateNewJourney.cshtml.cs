using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace CityBikeApplication.Pages
{
    public class CreateNewJourneyModel : PageModel
    {
        
        // if data couldn't be validated show user what went wrong
        public List<string> ErrorMessages { get; private set; } = new List<string>();

        // store what data user gave
        public Journey OldJourney { get; private set; }

        // user can select how many journeys are shown per page
        public List<Station> Choices { get; private set; } = new List<Station>();

        public void OnGet()
        {
            GetOldJourney();
        }

        public void GetOldJourney()
        {
            if (Request.Query["fromNewStation"].Equals("true"))
            {
                OldJourney = new Journey();
                OldJourney.DepartureTime = DateTime.Parse(Request.Query["dt"].ToString().Replace(".", ":"));
                OldJourney.ReturnTime = DateTime.Parse(Request.Query["rt"].ToString().Replace(".", ":"));
                OldJourney.DepartureStationId = int.Parse(Request.Query["ds"]);
                OldJourney.ReturnStationId = int.Parse(Request.Query["rs"]);
                OldJourney.CoveredDistance = int.Parse(Request.Query["cd"]);
                OldJourney.Duration = int.Parse(Request.Query["d"]);
            }
            else
            {
                OldJourney = new Journey();
            }
        }

        public List<Station> GetChoices()
        {
            DataHandler.Instance.SortStations(DataHandler.SortOrder.Id, false);
            return Choices = DataHandler.Instance.Stations;
        }

        public void OnPost()
        {
            // clear any previous error messages
            ErrorMessages.Clear();
            
            Journey newJourney = new Journey();

            DateTime departureTime = DateTime.Parse(Request.Form["departureTime"].ToString().Replace(".", ":"));
            newJourney.DepartureTime = departureTime;

            if (DateTime.Compare(DateTime.Now, departureTime) < 0)
            {
                ErrorMessages.Add("Given departure time is in the future");
            }

            DateTime returnTime = DateTime.Parse(Request.Form["returnTime"].ToString().Replace(".", ":"));
            newJourney.ReturnTime = returnTime;

            if (DateTime.Compare(DateTime.Now, returnTime) < 0)
            {
                ErrorMessages.Add("Given return time is in the future");
            }

            // check if return time is earlier than departure time
            if (DateTime.Compare(returnTime, departureTime) < 0)
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
            if (newJourney.ReturnStationId > 0)
            {
                newJourney.ReturnStationName = DataHandler.Instance.GetStation(newJourney.ReturnStationId).Name;
            }
            else
            {
                newJourney.ReturnStationName = "";
            }

            string coveredDistanceString = Sanitize(Request.Form["coveredDistance"]);
            string durationString = Sanitize(Request.Form["duration"]);

            if (coveredDistanceString.Length > 0)
            {
                if (int.TryParse(coveredDistanceString, out int coveredDistance))
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
                    newJourney.CoveredDistance = 0;
                }
            }
            else
            {
                newJourney.CoveredDistance = 0;
            }

            if (durationString.Length > 0)
            {
                if (int.TryParse(durationString, out int duration))
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
                    newJourney.Duration = 0;
                }
            }
            else
            {
                newJourney.Duration = 0;
            }

            // if there were errors remember what data was given
            OldJourney = newJourney;

            // if there wasn't problems with validation
            if (ErrorMessages.Count == 0)
            {
                // store info from CreateNewJourney-form to query
                var queryParams = new Dictionary<string, string>()
                {
                    { "fromNewJourney", "true" },
                    { "dt", departureTime.ToString("yyyy-MM-ddTHH:mm:ss") },
                    { "rt", returnTime.ToString("yyyy-MM-ddTHH:mm:ss") },
                    { "ds", "" + OldJourney.DepartureStationId },
                    { "rs", "" + OldJourney.ReturnStationId },
                    { "cd", "" + OldJourney.CoveredDistance },
                    { "d", "" + OldJourney.Duration }
                };

                if (Request.Form["newdeparturestation"].ToString().Equals("true"))
                {
                    queryParams.Add("departurestation", "true");
                    Response.Redirect(QueryHelpers.AddQueryString("CreateNewStation", queryParams));
                }
                else if (Request.Form["newreturnstation"].ToString().Equals("true"))
                {
                    queryParams.Add("returnstation", "true");
                    Response.Redirect(QueryHelpers.AddQueryString("CreateNewStation", queryParams));
                }
                else
                {
                    newJourney.Id = Guid.NewGuid().ToString();
                    DataHandler.Instance.CreateNewJourney(newJourney);
                    Response.Redirect("JourneyList");
                }
            }
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
