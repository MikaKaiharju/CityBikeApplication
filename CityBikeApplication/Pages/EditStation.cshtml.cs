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
    public class EditStationModel : PageModel
    {

        public List<string> errorMessages = new List<string>();

        public Station oldStation;

        public void OnGet()
        {
            GetOldStation();
        }

        public void GetOldStation()
        {
            oldStation = DataHandler.Instance.GetStation(Request.Query["id"]);
        }

        public void OnPost()
        {
            // remove previous errorMessages
            errorMessages.Clear();

            Station newStation = new Station();
            newStation.id = Sanitize(Request.Form["id"]);
            newStation.nimi = Sanitize(Request.Form["nimi"]);
            newStation.namn = Sanitize(Request.Form["namn"]);
            newStation.name = Sanitize(Request.Form["name"]);
            newStation.osoite = Sanitize(Request.Form["osoite"]);
            newStation.address = Sanitize(Request.Form["address"]);
            newStation.kaupunki = Sanitize(Request.Form["kaupunki"]);
            newStation.stad = Sanitize(Request.Form["stad"]);
            newStation.operaattori = Sanitize(Request.Form["operaattori"]);
            string kapasiteettiString = Sanitize(Request.Form["kapasiteetti"]);
            string xString = Sanitize(Request.Form["x"]);
            string yString = Sanitize(Request.Form["y"]);

            if(kapasiteettiString.Length > 0)
            {
                try
                {
                    newStation.kapasiteetti = "" + int.Parse(kapasiteettiString);
                }
                catch(Exception e)
                {
                    newStation.kapasiteetti = kapasiteettiString;
                    errorMessages.Add("Capacity needs to be integer that is >= 0");
                }
            }
            else
            {
                newStation.kapasiteetti = "";
            }

            // double parse needs number to be in comma form
            xString = xString.Replace(".", ",");
            yString = yString.Replace(".", ",");

            if (xString.Length > 0)
            {
                try
                {
                    // store in dot form
                    newStation.x = ("" + double.Parse(xString)).Replace(",","."); // longitude
                }
                catch (Exception e)
                {
                    errorMessages.Add("Longitude must be a decimal number with either . or , as a separator");
                    return;
                }
            }
            else
            {
                newStation.x = "";
            }
            if(yString.Length > 0)
            {
                try
                {
                    // store in dot form
                    newStation.y = ("" + double.Parse(yString)).Replace(",", "."); // latitude
                }
                catch(Exception e)
                {
                    errorMessages.Add("Latitude must be a decimal number with either . or , as a separator");
                    return;
                }
            }
            else
            {
                newStation.y = "";
            }

            // if there were errors remember what data was given
            oldStation = newStation;

            if(errorMessages.Count == 0)
            {
                DataHandler.Instance.ReplaceStation(newStation);
                Response.Redirect("StationList");
            }
        }

        private string Sanitize(string str)
        {
            // remove special characters
            return Regex.Replace(str, "[^a-�A-�0-9_ .,()]", "", RegexOptions.Compiled);
        }
    }
}
