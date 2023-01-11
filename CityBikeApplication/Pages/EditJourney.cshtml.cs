using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CityBikeApplication.Pages
{
    public class EditJourneyModel : PageModel
    {

        public string errorMessage = "";

        public Journey oldJourney;

        public void OnGet()
        {
            oldJourney = DataHandler.Instance.GetJourney(Request.Query["id"]);
        }

        public void GetOldJourney()
        {
            oldJourney = DataHandler.Instance.GetJourney(Request.Query["id"]);
        }

        public void OnPost()
        {
            Journey newJourney = new Journey();
            newJourney.id = Request.Form["id"];

            string departureTime = Request.Form["departureTime"];
            try
            {
                DateTime dateTime = DateTime.ParseExact(departureTime, "HH.mm.ss dd.MM.yyyy", CultureInfo.InvariantCulture);
                departureTime = dateTime.ToString("yyyy-MM-dd") + "T" + dateTime.ToString("HH:mm:ss");
                newJourney.departureTime = departureTime;
            }
            catch(Exception e)
            {
                errorMessage = "Departure Time needs to be in the form of \"HH.mm.ss dd.MM.yyyy\"";
                return;
            }

            string returnTime = Request.Form["returnTime"];
            try
            {
                DateTime dateTime = DateTime.ParseExact(returnTime, "HH.mm.ss dd.MM.yyyy", CultureInfo.InvariantCulture);
                returnTime = dateTime.ToString("yyyy-MM-dd") + "T" + dateTime.ToString("HH:mm:ss");
                newJourney.returnTime = returnTime;
            }
            catch (Exception e)
            {
                errorMessage = "Return Time needs to be in the form of \"HH.mm.ss dd.MM.yyyy\"";
                return;
            }

            newJourney.departureStationId = Request.Form["departureStationId"];
            newJourney.departureStationName = Request.Form["departureStationName"];
            newJourney.returnStationId = Request.Form["returnStationId"];
            newJourney.returnStationName = Request.Form["returnStationName"];

            string coveredDistanceString = Request.Form["coveredDistance"];
            try
            {
                int coveredDistance = int.Parse(coveredDistanceString);
                if(coveredDistance < 0)
                {
                    errorMessage = "Covered Distance needs to integer that is >= 0";
                    return;
                }
                else
                {
                    newJourney.coveredDistance = coveredDistance;
                }
            }
            catch(Exception e)
            {
                errorMessage = "Covered Distance needs to integer that is >= 0";
                return;
            }

            string durationString = Request.Form["duration"];
            try
            {
                int duration = int.Parse(durationString);
                if (duration < 0)
                {
                    errorMessage = "Duration needs to integer that is >= 0";
                    return;
                }
                else
                {
                    newJourney.duration = duration;
                }
            }
            catch (Exception e)
            {
                errorMessage = "Duration needs to integer that is >= 0";
                return;
            }

            oldJourney = newJourney;
            DataHandler.Instance.ReplaceJourney(newJourney);

            Response.Redirect("JourneyList");

        }

        private void p(string s)
        {
            System.Diagnostics.Debug.WriteLine(s);
        }
    }
}
