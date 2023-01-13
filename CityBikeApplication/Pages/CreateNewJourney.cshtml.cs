using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CityBikeApplication.Pages
{
    public class CreateNewJourneyModel : PageModel
    {
        
        // if data couldn't be validated show user what went wrong
        public List<string> ErrorMessages = new List<string>();

        // store what data user gave
        public Journey OldJourney;

        public void OnGet()
        {
            GetOldJourney();
        }

        public void GetOldJourney()
        {
            if (OldJourney == null) { OldJourney = new Journey(); }
        }

        public void OnPost()
        {
            // clear any previous error messages
            ErrorMessages.Clear();
            
            Journey newJourney = new Journey();

            string departureTime = Sanitize(Request.Form["departureTime"]);
            string returnTime = Sanitize(Request.Form["returnTime"]);
            //string departureStationId = Sanitize(Request.Form["departureStationId"]);
            //string returnStationId = Sanitize(Request.Form["returnStationId"]);
            newJourney.DepartureStationName = Sanitize(Request.Form["departureStationName"]);
            newJourney.ReturnStationName = Sanitize(Request.Form["returnStationName"]);
            string coveredDistanceString = Sanitize(Request.Form["coveredDistance"]);
            string durationString = Sanitize(Request.Form["duration"]);

            try
            {
                DateTime dateTime = DateTime.ParseExact(departureTime, "HH.mm.ss dd.MM.yyyy", CultureInfo.InvariantCulture);
                int comparison = DateTime.Compare(DateTime.Now, dateTime);

                if(comparison < 0)
                {
                    ErrorMessages.Add("Given departure time is in the future");
                }
                else
                {
                    departureTime = dateTime.ToString("yyyy-MM-dd") + "T" + dateTime.ToString("HH:mm:ss");
                    newJourney.DepartureTime = departureTime;
                }
            }
            catch (Exception e)
            {
                ErrorMessages.Add("Departure Time needs to be in the form of \"HH.mm.ss dd.MM.yyyy\"");
            }

            try
            {
                DateTime dateTime = DateTime.ParseExact(returnTime, "HH.mm.ss dd.MM.yyyy", CultureInfo.InvariantCulture);
                int comparison = DateTime.Compare(DateTime.Now, dateTime);
                if(comparison < 0)
                {
                    ErrorMessages.Add("Given return time is in the future");
                }
                else
                {
                    returnTime = dateTime.ToString("yyyy-MM-dd") + "T" + dateTime.ToString("HH:mm:ss");
                    newJourney.ReturnTime = returnTime;
                }
            }
            catch (Exception e)
            {
                ErrorMessages.Add("Return Time needs to be in the form of \"HH.mm.ss dd.MM.yyyy\"");
            }

            

            try
            {
                int coveredDistance = int.Parse(coveredDistanceString);
                if (coveredDistance < 0)
                {
                    ErrorMessages.Add("Covered Distance needs to be integer that is >= 0");
                }
                else
                {
                    newJourney.CoveredDistance = coveredDistance;
                }
            }
            catch (Exception e)
            {
                ErrorMessages.Add("Covered Distance needs to be integer  that is >= 0");
            }

            try
            {
                int duration = int.Parse(durationString);
                if (duration < 0)
                {
                    ErrorMessages.Add("Duration needs to be integer that is >= 0");
                }
                else
                {
                    newJourney.Duration = duration;
                }
            }
            catch (Exception e)
            {
                ErrorMessages.Add("Duration needs to be integer that is >= 0");
            }

            // if there were errors remember what data was given
            OldJourney = newJourney;

            // if there wasn't problems with validation
            if (ErrorMessages.Count == 0)
            {
                newJourney.Id = Guid.NewGuid().ToString();
                DataHandler.Instance.CreateNewJourney(newJourney);

                Response.Redirect("JourneyList");
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
