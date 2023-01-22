using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace CityBikeApplication.Pages
{
    public class CreateNewStationModel : PageModel
    {
        public List<string> ErrorMessages { get; private set; } = new List<string>();

        // station that user is editing
        // if there were errors, information given by the user is stored in oldStation
        public Station OldStation { get; set; }

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
                if (int.TryParse(idString, out int id))
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
                if (int.TryParse(capacityString, out int capacity))
                {
                    if (capacity < 0)
                    {
                        newStation.Capacity = capacity;
                        ErrorMessages.Add("Capacity needs to be >= 0");
                    }

                    newStation.Capacity = capacity;
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
                if(double.TryParse(xString, out double x))
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
                if(double.TryParse(yString, out double y))
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

                // store info given on station form and journey form
                var queryParams = new Dictionary<string, string>()
                {
                    { "fromNewStation", "true" },
                    { "dt", Request.Query["dt"] },
                    { "rt", Request.Query["rt"] },
                    { "cd", Request.Query["cd"] },
                    { "d", Request.Query["d"] }
                };

                if (Request.Query["departurestation"].Equals("true"))
                {
                    queryParams.Add("ds", "" + newStation.Id);
                    queryParams.Add("rs", Request.Query["rs"]);
                }
                else
                {
                    queryParams.Add("ds", Request.Query["ds"]);
                    queryParams.Add("rs", "" + newStation.Id);
                }

                // if user is creating new station while editing a station
                if (Request.Query["fromEditJourney"].Equals("true"))
                {
                    queryParams.Add("journeyId", Request.Query["journeyId"]);
                    Response.Redirect(QueryHelpers.AddQueryString("EditJourney", queryParams));
                }
                // if user is creating new station while creating a new journey
                else if (Request.Query["fromNewJourney"].Equals("true"))
                {
                    Response.Redirect(QueryHelpers.AddQueryString("CreateNewJourney", queryParams));
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

