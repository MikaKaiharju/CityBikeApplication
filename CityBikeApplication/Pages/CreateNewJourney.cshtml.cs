using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CityBikeApplication.Pages
{
    public class CreateNewJourneyModel : PageModel
    {
        
        public string errorMessage = "";

        // store what data user gave
        public Journey oldJourney;

        public void OnGet()
        {
            GetOldJourney();
        }

        public void GetOldJourney()
        {
            if (oldJourney == null) { oldJourney = new Journey(); }
        }

        public void OnPost()
        {
            errorMessage = "";
            
            Journey newJourney = new Journey();

            string departureTime = Request.Form["departureTime"];
            try
            {
                DateTime dateTime = DateTime.ParseExact(departureTime, "HH.mm.ss dd.MM.yyyy", CultureInfo.InvariantCulture);
                int comparison = DateTime.Compare(DateTime.Now, dateTime);

                if(comparison < 0)
                {
                    errorMessage = "Given date is in the future";
                }
                else
                {
                    departureTime = dateTime.ToString("yyyy-MM-dd") + "T" + dateTime.ToString("HH:mm:ss");
                    newJourney.departureTime = departureTime;
                }
            }
            catch (Exception e)
            {
                errorMessage = "Departure Time needs to be in the form of \"HH.mm.ss dd.MM.yyyy\"";
            }

            string returnTime = Request.Form["returnTime"];
            try
            {
                DateTime dateTime = DateTime.ParseExact(returnTime, "HH.mm.ss dd.MM.yyyy", CultureInfo.InvariantCulture);
                int comparison = DateTime.Compare(DateTime.Now, dateTime);
                if(comparison < 0)
                {
                    errorMessage = "Given date is in the future";
                }
                else
                {
                    returnTime = dateTime.ToString("yyyy-MM-dd") + "T" + dateTime.ToString("HH:mm:ss");
                    newJourney.returnTime = returnTime;
                }
            }
            catch (Exception e)
            {
                errorMessage = "Return Time needs to be in the form of \"HH.mm.ss dd.MM.yyyy\"";
            }

            newJourney.departureStationId = Request.Form["departureStationId"];
            newJourney.departureStationName = Request.Form["departureStationName"];
            newJourney.returnStationId = Request.Form["returnStationId"];
            newJourney.returnStationName = Request.Form["returnStationName"];

            string coveredDistanceString = Request.Form["coveredDistance"];
            try
            {
                int coveredDistance = int.Parse(coveredDistanceString);
                if (coveredDistance < 0)
                {
                    errorMessage = "Covered Distance needs to integer that is >= 0";
                }
                else
                {
                    newJourney.coveredDistance = coveredDistance;
                }
            }
            catch (Exception e)
            {
                errorMessage = "Covered Distance needs to integer that is >= 0";
            }

            string durationString = Request.Form["duration"];
            try
            {
                int duration = int.Parse(durationString);
                if (duration < 0)
                {
                    errorMessage = "Duration needs to integer that is >= 0";
                }
                else
                {
                    newJourney.duration = duration;
                }
            }
            catch (Exception e)
            {
                errorMessage = "Duration needs to integer that is >= 0";
            }

            oldJourney = newJourney;

            // if there wasn't problems with validation
            if (errorMessage.Equals(""))
            {
                newJourney.id = Guid.NewGuid().ToString();
                DataHandler.Instance.CreateNewJourney(newJourney);

                Response.Redirect("JourneyList");
            }

            

        }

    }
}
