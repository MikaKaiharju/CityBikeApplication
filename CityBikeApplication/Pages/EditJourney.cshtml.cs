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
    public class EditJourneyModel : PageModel
    {
        // if data couldn't be validated show user what went wrong
        public List<string> errorMessages = new List<string>();

        // store what data user gave
        public Journey oldJourney;

        public void OnGet()
        {
            GetOldJourney();
        }

        public void GetOldJourney()
        {
            oldJourney = DataHandler.Instance.GetJourney(Request.Query["id"]);
        }

        public void OnPost()
        {
            // remove previous errorMessages
            errorMessages.Clear();
            
            Journey newJourney = new Journey();
            newJourney.id = Request.Form["id"];

            string departureTime = Sanitize(Request.Form["departureTime"]);
            string returnTime = Sanitize(Request.Form["returnTime"]);
            newJourney.departureStationId = Sanitize(Request.Form["departureStationId"]);
            newJourney.departureStationName = Sanitize(Request.Form["departureStationName"]);
            newJourney.returnStationId = Sanitize(Request.Form["returnStationId"]);
            newJourney.returnStationName = Sanitize(Request.Form["returnStationName"]);
            string coveredDistanceString = Sanitize(Request.Form["coveredDistance"]);
            string durationString = Sanitize(Request.Form["duration"]);

            DateTime departureDateTime = DateTime.MinValue;
            try
            {
                departureDateTime = DateTime.ParseExact(departureTime, "HH.mm.ss dd.MM.yyyy", CultureInfo.InvariantCulture);
                int comparison = DateTime.Compare(DateTime.Now, departureDateTime);

                if (comparison < 0)
                {
                    errorMessages.Add("Given departure time is in the future");
                }
                else
                {
                    departureTime = departureDateTime.ToString("yyyy-MM-dd") + "T" + departureDateTime.ToString("HH:mm:ss");
                    newJourney.departureTime = departureTime;
                }
            }
            catch(Exception e)
            {
                newJourney.departureTime = departureTime;
                errorMessages.Add("Departure Time needs to be in the form of \"HH.mm.ss dd.MM.yyyy\"");
                return;
            }

            try
            {
                DateTime dateTime = DateTime.ParseExact(returnTime, "HH.mm.ss dd.MM.yyyy", CultureInfo.InvariantCulture);
                int comparison = DateTime.Compare(DateTime.Now, dateTime);
                if (comparison < 0)
                {
                    errorMessages.Add("Given return time is in the future");
                }
                else
                {
                    returnTime = dateTime.ToString("yyyy-MM-dd") + "T" + dateTime.ToString("HH:mm:ss");
                    newJourney.returnTime = returnTime;

                    // check if return time is earlier than departure time
                    if(departureDateTime != DateTime.MinValue && DateTime.Compare(dateTime, departureDateTime) < 0)
                    {
                        errorMessages.Add("Return time is earlier than departure time");
                    }
                }
            }
            catch (Exception e)
            {
                newJourney.returnTime = returnTime;
                errorMessages.Add("Return Time needs to be in the form of \"HH.mm.ss dd.MM.yyyy\"");
                return;
            }

            try
            {
                int coveredDistance = int.Parse(coveredDistanceString);
                if(coveredDistance < 0)
                {
                    errorMessages.Add("Covered Distance needs to be integer that is >= 0");
                    return;
                }
                else
                {
                    newJourney.coveredDistance = coveredDistance;
                }
            }
            catch(Exception e)
            {
                errorMessages.Add("Covered Distance needs to be integer that is >= 0");
                return;
            }

            try
            {
                int duration = int.Parse(durationString);
                if (duration < 0)
                {
                    errorMessages.Add("Duration needs to be integer that is >= 0");
                    return;
                }
                else
                {
                    newJourney.duration = duration;
                }
            }
            catch (Exception e)
            {
                errorMessages.Add("Duration needs to be integer that is >= 0");
                return;
            }

            // if there were errors remember what data was given
            oldJourney = newJourney;

            if (errorMessages.Count == 0)
            {
                DataHandler.Instance.ReplaceJourney(newJourney);
                Response.Redirect("JourneyList");
            }
        }

        private string Sanitize(string str)
        {
            // remove special characters
            return Regex.Replace(str, "[^a-öA-Ö0-9_ .,()]", "", RegexOptions.Compiled);
        }

        private void p(string s)
        {
            System.Diagnostics.Debug.WriteLine(s);
        }
    }
}
