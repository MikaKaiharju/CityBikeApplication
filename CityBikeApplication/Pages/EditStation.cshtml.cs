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
    public class EditStationModel : PageModel
    {

        public List<string> ErrorMessages { get; set; } = new List<string>();

        // station that user is editing
        // if there were errors, information given by the user is stored in oldStation
        public Station OldStation { get; set; }

        public void OnGet()
        {
            GetOldStation();
        }

        public void GetOldStation()
        {
            OldStation = DataHandler.Instance.GetStation(int.Parse(Request.Query["id"]));
        }

        public void OnPost()
        {
            // id of the station that user started editing because we need to replace old with new
            int oldStationId = int.Parse(Request.Query["id"]);
            GetOldStation();

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

            if(idString.Length > 0)
            {
                if(int.TryParse(idString, out int id))
                {
                    if (id < 0)
                    {
                        ErrorMessages.Add("Id needs to be integer that is > 0");
                    }
                    else
                    {
                        // check if there is already a station with this new id and its not the old
                        Station station = DataHandler.Instance.GetStation(id);
                        if (station != null && id != oldStationId)
                        {
                            ErrorMessages.Add("There is already a station with this id: id=" + station.Id + " name=" + station.Name);
                        }
                    }

                    newStation.Id = id;
                }
                else
                {
                    ErrorMessages.Add("Id needs to be integer that is > 0");
                    newStation.Id = OldStation.Id;
                }
            }
            else
            {
                ErrorMessages.Add("Id is required and needs to be integer that is > 0");
            }

            if(capacityString.Length > 0)
            {
                if(int.TryParse(capacityString, out int capacity))
                {
                    if (capacity < 0)
                    {
                        ErrorMessages.Add("Capacity needs to be >= 0");
                    }
                    
                    newStation.Capacity = capacity;
                }
                else
                {
                    ErrorMessages.Add("Capacity needs to be integer that is >= 0");
                    newStation.Capacity = OldStation.Capacity;
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
                    newStation.X = ("" + x).Replace(",", "."); // longitude
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

            if(yString.Length > 0)
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

            // if user pressed edit station in the station info
            bool cameFromStationInfo = Request.Query["cameFromStationInfo"].Equals("true");
            bool cameFromStationList = Request.Query["cameFromStationList"].Equals("true");

            if (ErrorMessages.Count == 0)
            {
                DataHandler.Instance.ReplaceStation(oldStationId, newStation);

                if (cameFromStationInfo)
                {
                    if (cameFromStationList)
                    {
                        Response.Redirect(QueryHelpers.AddQueryString("StationInfo", new Dictionary<string, string>() { { "id", "" + newStation.Id }, { "cameFromStationList", "true" } }));
                    }
                    else
                    {
                        Response.Redirect(QueryHelpers.AddQueryString("StationInfo", new Dictionary<string, string>() { { "id", "" + newStation.Id } }));
                    }
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
