using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CityBikeApplication.Pages
{
    public class CreateNewStationModel : PageModel
    {
        public List<string> ErrorMessages = new List<string>();

        // station that user is editing
        // if there were errors, information given by the user is stored in oldStation
        public Station OldStation;

        public void OnGet()
        {
            
        }

        public void OnPost()
        {
            // if user is creating new station while creating a new journey
            bool cameFromNewJourney = Request.Query["cameFromNewJourney"].Equals("true");

            // if user is creating new station while editing a station
            bool cameFromEditJourney = Request.Query["cameFromEditJourney"].Equals("true");

            // remove previous errorMessages
            ErrorMessages.Clear();

            Station newStation = new Station();
            string idString = Sanitize(Request.Form["id"]);
            newStation.Nimi = Sanitize(Request.Form["nimi"]);
            newStation.Namn = Sanitize(Request.Form["namn"]);
            newStation.Name = Sanitize(Request.Form["name"]);
            newStation.Osoite = Sanitize(Request.Form["osoite"]);
            newStation.Address = Sanitize(Request.Form["address"]);
            newStation.Kaupunki = Sanitize(Request.Form["kaupunki"]);
            newStation.Stad = Sanitize(Request.Form["stad"]);
            newStation.Operaattori = Sanitize(Request.Form["operaattori"]);
            string kapasiteettiString = Sanitize(Request.Form["kapasiteetti"]);
            string xString = Sanitize(Request.Form["x"]);
            string yString = Sanitize(Request.Form["y"]);

            if (idString.Length > 0)
            {
                int id;
                if (int.TryParse(idString, out id))
                {
                    if (id <= 0)
                    {
                        ErrorMessages.Add("Id needs to be integer that is > 0");
                    }
                    else
                    {
                        // check if there is already a station with this new id and its not the old
                        Station station = DataHandler.Instance.GetStation(id);
                        if (station != null)
                        {
                            ErrorMessages.Add("There is already a station with this id: id=" + station.Id + " name=" + station.Name);
                        }
                    }
                    newStation.Id = id;
                }
                else
                {
                    ErrorMessages.Add("Id needs to be integer that is > 0");
                }
            }
            else
            {
                ErrorMessages.Add("Id is required and needs to be integer that is > 0");
            }

            if (kapasiteettiString.Length > 0)
            {
                int kapasiteetti;
                if (int.TryParse(kapasiteettiString, out kapasiteetti))
                {
                    if (kapasiteetti < 0)
                    {
                        newStation.Kapasiteetti = kapasiteetti;
                        ErrorMessages.Add("Capacity needs to be >= 0");
                    }

                    newStation.Kapasiteetti = kapasiteetti;
                }
                else
                {
                    ErrorMessages.Add("Capacity needs to be integer that is >= 0");
                    newStation.Kapasiteetti = 0;
                }
            }
            else
            {
                newStation.Kapasiteetti = 0;
            }

            // double parse needs number to be in comma form
            xString = xString.Replace(".", ",");
            yString = yString.Replace(".", ",");

            if (xString.Length > 0)
            {
                double x;
                if(double.TryParse(xString, out x))
                {
                    // store in dot form
                    newStation.X = ("" + double.Parse(xString)).Replace(",", "."); // longitude
                }
                else
                {
                    ErrorMessages.Add("Longitude must be a decimal number with either . or , as a separator");
                    // show what was previously
                    newStation.X = Request.Form["x"];
                }
            }
            else
            {
                newStation.X = "";
            }

            if (yString.Length > 0)
            {
                double y;
                if(double.TryParse(yString, out y))
                {
                    // store in dot form
                    newStation.Y = ("" + y).Replace(",", "."); // latitude
                }
                else
                {
                    ErrorMessages.Add("Latitude must be a decimal number with either . or , as a separator");
                    // show what was previously
                    newStation.Y = Request.Form["y"];
                }
            }
            else
            {
                newStation.Y = "";
            }

            // if there were errors remember what data was given
            OldStation = newStation;

            if (ErrorMessages.Count == 0)
            {
                DataHandler.Instance.CreateNewStation(newStation);

                if (cameFromEditJourney)
                {
                    //TODO: id of the journey that user was editing should be added to the query string
                    //          in the mean while redirect to journeylist

                    //string queryString = "";
                    //Response.Redirect("EditJourney" + queryString);

                    Response.Redirect("JourneyList");
                }
                else if (cameFromNewJourney)
                {
                    //TODO: id and other information of the new journey that user started creating before new station
                    //          should be attached as query information
                    
                    //string queryString = Request.QueryString.ToString();
                    //Response.Redirect("CreateNewJourney?" + queryString);

                    Response.Redirect("JourneyList");
                }
                else
                {
                    Response.Redirect("StationList");
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

        private void p(string s)
        {
            System.Diagnostics.Debug.WriteLine(s);
        }
    }
}

