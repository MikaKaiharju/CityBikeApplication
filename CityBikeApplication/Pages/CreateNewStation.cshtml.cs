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
            // remove previous errorMessages
            ErrorMessages.Clear();

            Station newStation = new Station();
            string idString = Sanitize(Request.Form["id"]);
            newStation.Name = Sanitize(Request.Form["name"]);
            newStation.Address = Sanitize(Request.Form["address"]);
            newStation.City = Sanitize(Request.Form["city"]);
            newStation.Operator = Sanitize(Request.Form["operator"]);
            string capacityString = Sanitize(Request.Form["capacity"]);
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

            if (capacityString.Length > 0)
            {
                int kapasiteetti;
                if (int.TryParse(capacityString, out kapasiteetti))
                {
                    if (kapasiteetti < 0)
                    {
                        newStation.Capacity = kapasiteetti;
                        ErrorMessages.Add("Capacity needs to be >= 0");
                    }

                    newStation.Capacity = kapasiteetti;
                }
                else
                {
                    ErrorMessages.Add("Capacity needs to be integer that is >= 0");
                    newStation.Capacity = 0;
                }
            }
            else
            {
                newStation.Capacity = 0;
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

            // if user is creating new station while creating a new journey
            bool cameFromNewJourney = Request.Query["fromNewJourney"].Equals("true");

            // if user is creating new station while editing a station
            bool cameFromEditJourney = Request.Query["fromEditJourney"].Equals("true");

            if (ErrorMessages.Count == 0)
            {
                DataHandler.Instance.CreateNewStation(newStation);

                if (cameFromEditJourney)
                {
                    // store info given on station form and journey form
                    string queryString = "?fromNewStation=true&journeyId=" + Request.Query["journeyId"] + "&";
                    bool departureStation = Request.Query["departurestation"].Equals("true");
                    if (departureStation)
                    {
                        queryString += "ds=" + newStation.Id + "&" +
                            "rs=" + Request.Query["rs"] + "&";
                    }
                    else
                    {
                        queryString += "ds=" + Request.Query["ds"] + "&" + 
                            "rs=" + newStation.Id + "&"; 
                    }
                    queryString += 
                        "dt=" + Request.Query["dt"] + "&" +
                        "rt=" + Request.Query["rt"] + "&" +
                        "cd=" + Request.Query["cd"] + "&" +
                        "d=" + Request.Query["d"];

                    Response.Redirect("EditJourney" + queryString);
                }
                else if (cameFromNewJourney)
                {
                    // store info given on station form and journey form
                    string queryString = "?fromNewStation=true&";
                    bool departureStation = Request.Query["departurestation"].Equals("true");
                    if (departureStation)
                    {
                        queryString += "ds=" + newStation.Id + "&" +
                            "rs=" + Request.Query["rs"] + "&";
                    }
                    else
                    {
                        queryString += "ds=" + Request.Query["ds"] + "&" +
                            "rs=" + newStation.Id + "&";
                    }
                    queryString += "dt=" + Request.Query["dt"] + "&" +
                        "rt=" + Request.Query["rt"] + "&" +
                        "cd=" + Request.Query["cd"] + "&" +
                        "d=" + Request.Query["d"];

                    Response.Redirect("CreateNewJourney" + queryString);
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
    }
}

